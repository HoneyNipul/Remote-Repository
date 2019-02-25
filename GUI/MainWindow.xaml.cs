///////////////////////////////////////////////////////////////////////
// MainWindow.xaml.cs - GUI for Project3HelpWPF                      //
// ver 1.2                                                          //
// Honey Shah, Ms Computer Science                                    //
// Source :Jim Fawcett,                                              //
// CSE687-OnLine Object Oriented Design, Spring 2018                 //
///////////////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * This package provides a WPF-based GUI for Project3HelpWPF demo.  It's 
 * responsibilities are to:
 * - Provide a display of directory contents of a remote ServerPrototype.
 * - It provides a subdirectory list and a filelist for the selected directory.
 * - You can navigate into subdirectories by double-clicking on subdirectory
 *   or the parent directory, indicated by the name "..".
 *   
 * Required Files:
 * ---------------
 * Mainwindow.xaml, MainWindow.xaml.cs
 * Translater.dll
 * 
 * Maintenance History:
 * --------------------
 * ver 1.2    01 May 2018
 *   - third release
 *   
 * ver 1.1:   12 April 2018
 *  - second release
 *  
 * ver 1.0 : 30 Mar 2018
 * - first release
 * - Several early prototypes were discussed in class. Those are all superceded
 *   by this package.
 */

// Translater has to be statically linked with CommLibWrapper
// - loader can't find Translater.dll dependent CommLibWrapper.dll
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using MsgPassingCommunication;

namespace WpfApp1
{
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();
      Console.Title = "GUI CONSOLE";
    }

        public MainWindow(string port)
        {
            
            InitializeComponent();
            Console.Title = "GUI CONSOLE";
            port_ = port;
        }



        string port_;
        List<CodePopUpWindow> popups = new List<CodePopUpWindow>();
    private Stack<string> pathStack_ = new Stack<string>();
    private Stack<string> pathStack1_ = new Stack<string>();
        private Stack<string> pathStack4_ = new Stack<string>();
        private Stack<string> pathStack2_ = new Stack<string>();
        private Stack<string> checkIn = new Stack<string>();
        private List<string> cat = new List<string>();
        private List<string> addChild = new List<string>();
        private Translater translater;
    private CsEndPoint endPoint_;
    private Thread rcvThrd = null;
    private Dictionary<string, Action<CsMessage>> dispatcher_ 
      = new Dictionary<string, Action<CsMessage>>();

    //----< process incoming messages on child thread >----------------

    private void processMessages()
    {
      ThreadStart thrdProc = () => {
        while (true)
        {
          CsMessage msg = translater.getMessage();
             // msg.show();
          
             if(msg.attributes.Count() == 0)
              {
                  continue;
              }
              string msgId = msg.value("command");
              if (dispatcher_.ContainsKey(msgId))
                  dispatcher_[msgId].Invoke(msg);
              
        }
      };

      rcvThrd = new Thread(thrdProc);
      rcvThrd.IsBackground = true;
      rcvThrd.Start();
    }
    //----< function dispatched by child thread to main thread >-------

    private void clearDirs()
    {
      DirList.Items.Clear();
    }
        private void clearDirs1()
        {
            DirList1.Items.Clear();
        }
        private void clearDirs2()
        {
            DirList2.Items.Clear();
        }
        private void clearDirs4()
        {
            DirList4.Items.Clear();
        }
        //----< function dispatched by child thread to main thread >-------

        private void addDir(string dir)
    {
      DirList.Items.Add(dir);
    }
        private void addDir1(string dir)
        {
            DirList1.Items.Add(dir);
        }
        private void addDir2(string dir)
        {
            DirList2.Items.Add(dir);
        }
        private void addDir4(string dir)
        {
            DirList4.Items.Add(dir);
        }
        //----< function dispatched by child thread to main thread >-------

        private void insertParent()
    {
      DirList.Items.Insert(0, "..");
    }
        private void insertParent1()
        {
            DirList1.Items.Insert(0, "..");
        }
        private void insertParent2()
        {
            DirList2.Items.Insert(0, "..");
        }
        private void insertParent4()
        {
            DirList4.Items.Insert(0, "..");
        }

        //----< function dispatched by child thread to main thread >-------

    private void clearFiles()
    {
      FileList.Items.Clear();
    }
        private void clearFiles1()
        {
            FileList1.Items.Clear();
        }
        private void clearFiles2()
        {
            FileList2.Items.Clear();
        }
        private void clearFiles4()
        {
            FileList4.Items.Clear();
        }
        //----< function dispatched by child thread to main thread >-------

    private void addFile(string file)
    {
      FileList.Items.Add(file);
    }
        private void addFile1(string file)
        {
            FileList1.Items.Add(file);
        }
        private void addFile2(string file)
        {
            FileList2.Items.Add(file);
        }
        private void addFile4(string file)
        {
            FileList4.Items.Add(file);
        }


        //----< add client processing for message with key >---------------

        private void addClientProc(string key, Action<CsMessage> clientProc)
    {
      dispatcher_[key] = clientProc;
    }
    //----< load getDirs processing into dispatcher dictionary >-------

    private void DispatcherLoadGetDirs()
    {
      Action<CsMessage> getDirs = (CsMessage rcvMsg) =>
      {
        Action clrDirs = () =>
        {
          clearDirs();
            
        };
        Dispatcher.Invoke(clrDirs, new Object[] { });
        var enumer = rcvMsg.attributes.GetEnumerator();
        while (enumer.MoveNext())
        {
          string key = enumer.Current.Key;
          if (key.Contains("dir"))
          {
            Action<string> doDir = (string dir) =>
            {
              addDir(dir);
              
            };
            Dispatcher.Invoke(doDir, new Object[] { enumer.Current.Value });
          }
        }
        Action insertUp = () =>
        {
          insertParent();
            
        };
        Dispatcher.Invoke(insertUp, new Object[] { });
      };
      addClientProc("getDirs", getDirs);
    }
        //----< load Sending_metadata processing into dispatcher dictionary >-------
        private void DispatcherLoadMetaData()
        {
            Action<CsMessage> sending_metaData = (CsMessage rcvMsg) =>
            {
                Console.WriteLine("---------<Showing METADATA>-----");
                rcvMsg.show();
                var enumer = rcvMsg.attributes.GetEnumerator();
                while (enumer.MoveNext())
                {
                    string key = enumer.Current.Key;
                    
                        Action<string> dometa = (string MetaData) =>
                        {   
                            Meta.Items.Add(MetaData);

                        };
                        Dispatcher.Invoke(dometa, new Object[] { enumer.Current.Value });
                    
                }
                
            };
            addClientProc("sending_metaData", sending_metaData);
        }
        //----< load sendFile processing into dispatcher dictionary >-------
        private void DispatcherSendFile()
        {
            Action<CsMessage> sendFile = (CsMessage rcvMsg) =>
            {

                var enumer = rcvMsg.attributes.GetEnumerator();
                while (enumer.MoveNext())
                {
                    string key = enumer.Current.Key;
                    if (key.Contains("file"))
                    {
                        Action<string> sender_file = (string dir) =>
                        {
                           
                            showFile(dir);

                        };
                        Dispatcher.Invoke(sender_file, new Object[] { enumer.Current.Value });
                        
                    }
                }
                
            };
            
            addClientProc("sendFile", sendFile);
        }
        //----< Code for POPUP >-------
        private void showFile(string fileName)
        {
            CodePopUpWindow popUp = new CodePopUpWindow();

            popUp.Show();
            popups.Add(popUp);
            popUp.codeView.Blocks.Clear();
            Paragraph p = new Paragraph();
            statusBarText.Text = "The File Selected for popUp = " + fileName;
            string path = "../../../../SaveFiles" + "/" + fileName ;
            Paragraph paragraph = new Paragraph();
            string fileText = "";
            try
            {
                fileText = System.IO.File.ReadAllText(path);
            }
            finally
            {
                paragraph.Inlines.Add(new Run(fileText));
            }

            // add code text to code view
            popUp.codeView.Blocks.Clear();
            popUp.codeView.Blocks.Add(paragraph);

        }
        //----< load getDirs1 processing into dispatcher dictionary >-------
        private void DispatcherLoadGetDirs1()
        {
            Action<CsMessage> getDirs1 = (CsMessage rcvMsg) =>
            {
                Action clrDirs1 = () =>
                {
                    if (rcvMsg.value("name") == "Browse")
                        clearDirs2();
                    else if (rcvMsg.value("name") == "ViewMeta")
                        clearDirs4();
                };
                Dispatcher.Invoke(clrDirs1, new Object[] { });
                var enumer = rcvMsg.attributes.GetEnumerator();
                while (enumer.MoveNext())
                {
                    string key = enumer.Current.Key;
                    if (key.Contains("dir"))
                    {
                        Action<string> doDir1 = (string dir) =>
                        {
                            if (rcvMsg.value("name") == "Browse")
                                addDir2(dir);
                            else if (rcvMsg.value("name") == "ViewMeta")
                                addDir4(dir);

                        };
                        Dispatcher.Invoke(doDir1, new Object[] { enumer.Current.Value });
                    }
                }
                Action insertUp1 = () =>
                {
                    if (rcvMsg.value("name") == "Browse")
                        insertParent2();
                    else if (rcvMsg.value("name") == "ViewMeta")
                        insertParent4();

                };
                Dispatcher.Invoke(insertUp1, new Object[] { });
            };
            addClientProc("getDirs1", getDirs1);
        }
        //----< load getFiles processing into dispatcher dictionary >------

    private void DispatcherLoadGetFiles()
    {
      Action<CsMessage> getFiles = (CsMessage rcvMsg) =>
      {
        Action clrFiles = () =>
        {
          clearFiles();
           
        };
        Dispatcher.Invoke(clrFiles, new Object[] { });
        var enumer = rcvMsg.attributes.GetEnumerator();
        while (enumer.MoveNext())
        {
          string key = enumer.Current.Key;
          if (key.Contains("file"))
          {
            Action<string> doFile = (string file) =>
            {
              addFile(file);
                
            };
            Dispatcher.Invoke(doFile, new Object[] { enumer.Current.Value });
          }
        }
      };
      addClientProc("getFiles", getFiles);
    }
        //----< load getFiles1 processing into dispatcher dictionary >------
        private void DispatcherLoadGetFiles1()
        {
            Action<CsMessage> getFiles1 = (CsMessage rcvMsg) =>
            {
                Action clrFiles = () =>
                {
                    if (rcvMsg.value("name") == "Browse")
                        clearFiles2();
                    else if (rcvMsg.value("name") == "ViewMeta")
                        clearFiles4();

                    
                };
                Dispatcher.Invoke(clrFiles, new Object[] { });
                var enumer = rcvMsg.attributes.GetEnumerator();
                while (enumer.MoveNext())
                {
                    string key = enumer.Current.Key;
                    if (key.Contains("file"))
                    {
                        Action<string> doFile = (string file) =>
                        {
                            if (rcvMsg.value("name") == "Browse")
                                addFile2(file);
                            else if (rcvMsg.value("name") == "ViewMeta")
                                addFile4(file);

                            //addFile2(file);
                        };
                        Dispatcher.Invoke(doFile, new Object[] { enumer.Current.Value });
                    }
                }
            };
            addClientProc("getFiles1", getFiles1);
        }

        //----< load CheckInReady and CheckInDone processing into dispatcher dictionary >-------
        private void DispatcherCheckin()
        {
            Action<CsMessage> checkInReady = (CsMessage rcvMsg) =>
            {

                Console.Write("-------<Check-In-Ready>----");
                rcvMsg.show();
                CsEndPoint serverEndPoint = new CsEndPoint();
                serverEndPoint.machineAddress = "localhost";
                serverEndPoint.port = 8080;
                CsMessage msg = new CsMessage();
                String c = rcvMsg.value("Ready_for_checking");
                String desc = rcvMsg.value("Description");
                String category = rcvMsg.value("category");
                String author = rcvMsg.value("author");
                String path = rcvMsg.value("path");
                String child = rcvMsg.value("child");
                msg.add("name", c);
                msg.add("to", CsEndPoint.toString(serverEndPoint));
                msg.add("from", CsEndPoint.toString(endPoint_));
                msg.remove("command");
                msg.add("command", "checkinFile");
                msg.add("path",path);
                msg.add("category", category);
                msg.add("Description", desc);
                msg.add("author", author);
                msg.add("child", child);
                translater.postMessage(msg);
                

                
                msg.show();
               
            };
            Action<CsMessage> checkInDone = (CsMessage rcvMsg) =>
            {
                Console.Write("-------<Check-In-Done>----");
                rcvMsg.show();
                CsEndPoint serverEndPoint = new CsEndPoint();
                serverEndPoint.machineAddress = "localhost";
                serverEndPoint.port = 8080;
                CsMessage msg = new CsMessage();
                String c = rcvMsg.value("Done"); 
                String path = rcvMsg.value("path");       
                String desc = rcvMsg.value("Description");
                String category = rcvMsg.value("category");
                String author = rcvMsg.value("author");
                String child = rcvMsg.value("child");
                msg.add("name", c);
                msg.add("to", CsEndPoint.toString(serverEndPoint));
                msg.add("from", CsEndPoint.toString(endPoint_));
                msg.remove("command");
                msg.add("command", "checkin_Successful");
                msg.add("category", category);
                msg.add("Description", desc);
                msg.add("author", author);
                msg.add("path",path);
                msg.add("file", c);
                msg.add("child", child);
                msg.add("savePath", "../SendFiles");
                Action enableTab = () =>
                {
                    statusBarText.Text = "Checking done for file: " + c;
                };

                Dispatcher.Invoke(enableTab, new Object[] { });
                translater.postMessage(msg);
                
            };
            addClientProc("checkInReady", checkInReady);
            addClientProc("checkInDone", checkInDone);
        }

        //----< load fileTransfer processing into dispatcher dictionary >-------
        private void DispatcherCheckOut()
        {
            Action<CsMessage> fileTransfer = (CsMessage rcvMsg) =>
            {
                Console.Write("---<Check Out Done.>----");
                rcvMsg.show();
                CsEndPoint serverEndPoint = new CsEndPoint();
                serverEndPoint.machineAddress = "localhost";
                serverEndPoint.port = 8080;
                CsMessage msg = new CsMessage();
               string c= rcvMsg.value("Ready_for_checkOut");
                string path = rcvMsg.value("path");
                msg.add("name", c);
                msg.add("to", CsEndPoint.toString(serverEndPoint));
                msg.add("from", CsEndPoint.toString(endPoint_));
                msg.remove("command");
                msg.add("command", "CheckOut_Done");
                msg.add("path", path);
                Action enableTab = () =>
                {
                    statusBarText.Text = "CheckOut  done for file: " + c;
                };

                Dispatcher.Invoke(enableTab, new Object[] { });
                translater.postMessage(msg);
            };
            addClientProc("fileTransfer", fileTransfer);
        }

        //----< load connection_sucessful processing into dispatcher dictionary >-------
        private void DispatcherConnect()
        {
            Action<CsMessage> connection_Sucessful = (CsMessage rcvMsg) =>
            {

                Action enableTab = () =>
                {
                    
                    statusBarText.Text = "Server Connected";
                };

                Dispatcher.Invoke(enableTab, new Object[] { });

            };
            addClientProc("connection_Sucessful", connection_Sucessful);
        }

        //----< Load  listOfFiles for Query result for given filters in Query Tab >-------
        private void DispatcherListOfFiles()
        {
            Action<CsMessage> listOfFiles = (CsMessage rcvMsg) =>
            {

                Console.Write("---<Msg Recieved for Query>----");
                rcvMsg.show();
                Action addSearch = () =>
                {
                    string str = rcvMsg.value("FileList");
                    findSearch(str);

                };

                Dispatcher.Invoke(addSearch, new Object[] { });
                


            };
            addClientProc("listOfFiles", listOfFiles);
        }
        void findSearch(string str)
        {
            string[] ssize = str.Split(new char[0]);
            foreach(string n in ssize)
            {
                searchList.Items.Add(n);
            }

        }

        //----< load DispatcherStatus for processing change of status  >-------
        private void DispatcherStatus()
        {
            Action<CsMessage> statusChanged = (CsMessage rcvMsg) =>
            {

                Console.Write("---------<Status changed>-------");

            };
            addClientProc("statusChanged", statusChanged);
        }

        //----< load DispatcherFilesWithoutParent for getting list of files without Parent >-------
        private void DispatcherFilesWithoutParent()
        {
            Action<CsMessage> FileWithoutParents = (CsMessage rcvMsg) =>
            {
                Console.Write("---<Msg Recieved for Search file without parents>----");
                rcvMsg.show();
                Action addSearch = () =>
                {
                    string str = rcvMsg.value("Filelist");
                    findParent(str);

                };
                Dispatcher.Invoke(addSearch, new Object[] { });

            };
            addClientProc("FileWithoutParents", FileWithoutParents);
        }
        void findParent(string str)
        {
            string[] ssize = str.Split(new char[0]);
            foreach (string n in ssize)
            {
                searchParent.Items.Add(n);
            }

        }
        //----< load all dispatcher processing >---------------------------
    private void loadDispatcher()
    {
      DispatcherLoadGetDirs();
      DispatcherLoadGetFiles();
      DispatcherCheckin();
      DispatcherCheckOut();
      DispatcherSendFile();
      DispatcherLoadGetDirs1();
      DispatcherLoadGetFiles1();
      DispatcherLoadMetaData();
      DispatcherConnect();
      DispatcherListOfFiles();
      DispatcherStatus();
      DispatcherFilesWithoutParent();
    }

        //----< start Comm, fill window display with dirs and files >------
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      
      endPoint_ = new CsEndPoint();
      endPoint_.machineAddress = "localhost";
      endPoint_.port = Int32.Parse(port_);
              
      translater = new Translater();
      translater.listen(endPoint_);
      
      processMessages();
     
      loadDispatcher();

      CsEndPoint serverEndPoint = new CsEndPoint();
      serverEndPoint.machineAddress = "localhost";
      serverEndPoint.port = 8080;
      PathTextBlock.Text = "Storage";
      pathStack_.Push("../Storage");

      PathTextBlock2.Text = "Storage";
      pathStack1_.Push("../Storage");

      PathTextBlock1.Text = "RemoteRepo";
      pathStack2_.Push("../../../../ RemoteRepo");

      PathTextBlock4.Text = "Storage";
      pathStack4_.Push("../Storage");

            connectServer();

            CsMessage msg = new CsMessage();
      msg.add("to", CsEndPoint.toString(serverEndPoint));
      msg.add("from", CsEndPoint.toString(endPoint_));
      msg.add("command", "getDirs");
      msg.add("path", "../Storage");
      translater.postMessage(msg);

      msg.remove("command");
      msg.add("command", "getFiles");
      translater.postMessage(msg);
           
            if(endPoint_.port == 8085)
            {
                test();
            }
            else
            {
                test2();
            }
          
    }
        //----< Test Case Function call for port = 8085 >-------
        void test()
        {
            checkInFromGUI();
            pendingStatus();
            checkInTime();
            changeStatusToClose();
            queryFromGUI1();
            queryFromGUI();  
            metaFromGui();

           FileList_MouseDoubleClickBrowseGui();


        }
        //----< test case function call for port = 8082 >-------
        void test2()
        {

            checkOutFromGUI();
            checkOutFromGUIDependency();
            searchWithoutParentsGUI();


        }

        //----< strip off name of first part of path >---------------------
        private string removeFirstDir(string path)
    {
      string modifiedPath = path;
      int pos = path.IndexOf("/");
      modifiedPath = path.Substring(pos + 1, path.Length - pos - 1);
      return modifiedPath;
    }

    //----< respond to mouse double-click on dir name for checkOut >----------------

    private void DirList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      
      string selectedDir = (string)DirList.SelectedItem;
            statusBarText.Text = "The Selected Directory = " + selectedDir;
           

            string path;
                if (selectedDir == "..")
                {
                    if (pathStack_.Count > 1)  // don't pop off "Storage"
                        pathStack_.Pop();
                    else
                        return;
                }
                else
                {
                    path = pathStack_.Peek() + "/" + selectedDir;
                    pathStack_.Push(path);
                }
                
                PathTextBlock.Text = removeFirstDir(pathStack_.Peek());
                CsEndPoint serverEndPoint = new CsEndPoint();
                serverEndPoint.machineAddress = "localhost";
                serverEndPoint.port = 8080;
                CsMessage msg = new CsMessage();
                msg.add("to", CsEndPoint.toString(serverEndPoint));
                msg.add("from", CsEndPoint.toString(endPoint_));
                msg.add("command", "getDirs");
                msg.add("path", pathStack_.Peek());
                translater.postMessage(msg);
                msg.remove("command");
                msg.add("command", "getFiles");
                translater.postMessage(msg);
    }
        //----< respond to mouse double-click on dir name for Browse >----------------
        private void DirList_MouseDoubleClick1(object sender, MouseButtonEventArgs e)
        {
            // build path for selected dir
            string selectedDir = (string)DirList2.SelectedItem;
            

            string path;
            if (selectedDir == "..")
            {
                if (pathStack1_.Count > 1)  // don't pop off "Storage"
                    pathStack1_.Pop();
                else
                    return;
            }
            else
            {
                path = pathStack1_.Peek() + "/" + selectedDir;
                pathStack1_.Push(path);
            }
            // display path in Dir TextBlcok
            PathTextBlock2.Text = removeFirstDir(pathStack1_.Peek());

            // build message to get dirs and post it
            CsEndPoint serverEndPoint = new CsEndPoint();
            serverEndPoint.machineAddress = "localhost";
            serverEndPoint.port = 8080;
            CsMessage msg1 = new CsMessage();
            msg1.add("name", "Browse");
            msg1.add("to", CsEndPoint.toString(serverEndPoint));
            msg1.add("from", CsEndPoint.toString(endPoint_));
            msg1.add("command", "getDirs1");
            msg1.add("path1", pathStack1_.Peek());
            translater.postMessage(msg1);

            // build message to get files and post it
            msg1.remove("command");
            msg1.add("command", "getFiles1");
            translater.postMessage(msg1);
        }

        //----< respond to mouse double-click on dir name for ViewMetaData >----------------
        private void DirList_MouseDoubleClick2(object sender, MouseButtonEventArgs e)
        {
           
            string selectedDir = (string)DirList4.SelectedItem;


            string path;
            if (selectedDir == "..")
            {

                Meta.Items.Clear();
                if (pathStack4_.Count > 1)  // don't pop off "Storage"
                    pathStack4_.Pop();
                else
                    return;
            }
            else
            {
                path = pathStack4_.Peek() + "/" + selectedDir;
                pathStack4_.Push(path);
            }
            // display path in Dir TextBlcok
            PathTextBlock4.Text = removeFirstDir(pathStack4_.Peek());

            // build message to get dirs and post it
           CsEndPoint serverEndPoint = new CsEndPoint();
            serverEndPoint.machineAddress = "localhost";
            serverEndPoint.port = 8080;
            CsMessage msg1 = new CsMessage();
            msg1.add("name", "ViewMeta");
            msg1.add("to", CsEndPoint.toString(serverEndPoint));
            msg1.add("from", CsEndPoint.toString(endPoint_));
            msg1.add("command", "getDirs1");
            msg1.add("path1", pathStack4_.Peek());
            translater.postMessage(msg1);

            // build message to get files and post it
            msg1.remove("command");
            msg1.add("command", "getFiles1");
            translater.postMessage(msg1);
        }

        //----< respond to mouse double-click on file name for ViewMeta Data >----------------
        private void Meta_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Meta.Items.Clear();
            string selectedFile = (string)FileList4.SelectedItem;
            statusBarText.Text = "Meta Data For:  " + selectedFile;
            CsEndPoint serverEndPoint = new CsEndPoint();
            serverEndPoint.machineAddress = "localhost";
            serverEndPoint.port = 8080;
            string[] words = selectedFile.Split('.');
            string path = "../Storage" + "/" + words[0] + "/" + selectedFile;
            CsMessage msg1 = new CsMessage();
            msg1.add("name", selectedFile);
            msg1.add("to", CsEndPoint.toString(serverEndPoint));
            msg1.add("from", CsEndPoint.toString(endPoint_));
            msg1.add("command", "getMetaData");
            msg1.add("path1", path);
            msg1.add("fileNamespace", words[0] + "::" + selectedFile);
            translater.postMessage(msg1);

            
        }

        //----< Test Case For Viewing Meta Data >----------------
        private void metaFromGui()
        {
            Meta.Items.Clear();
            Console.WriteLine("-------<Test Case for viewing metadata for \"test.cpp\">-----");
            string selectedFile = "test.cpp.1";
            CsEndPoint serverEndPoint = new CsEndPoint();
            serverEndPoint.machineAddress = "localhost";
            serverEndPoint.port = 8080;
            string[] words = selectedFile.Split('.');
            string path = "../Storage" + "/" + words[0] + "/" + selectedFile;
            CsMessage msg1 = new CsMessage();
            msg1.add("name", selectedFile);
            msg1.add("to", CsEndPoint.toString(serverEndPoint));
            msg1.add("from", CsEndPoint.toString(endPoint_));
            msg1.add("command", "getMetaData");
            msg1.add("path1", path);
            msg1.add("fileNamespace", words[0] + "::" + selectedFile);
            translater.postMessage(msg1);
            msg1.show();
        }

        //----< respond to mouse double-click on dir name for checkIn >----------------
        private void DirListCheckIn_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            
            string selectedDir = (string)DirList1.SelectedItem;
            statusBarText.Text =  "The Selected Directory = " +selectedDir;
            
             
             string[] array1 = Directory.GetFiles("../../../../RemoteRepo\\"+selectedDir);
            
            clearFiles1();
            foreach (string name in array1)
            {
                
                string[] words = name.Split('\\');
                addFile1(words[2]);

            }    
        }

        //----< Test Case For Browse and Opening a particular file - POPUP >----------------
        private void FileList_MouseDoubleClickBrowseGui()
        {
            
            string path = "../Storage/test/test.h.1";
            CsEndPoint serverEndPoint = new CsEndPoint();
            serverEndPoint.machineAddress = "localhost";
            serverEndPoint.port = 8080;
            CsMessage msg = new CsMessage();
            msg.add("name", "PopUp");
            msg.add("to", CsEndPoint.toString(serverEndPoint));
            msg.add("from", CsEndPoint.toString(endPoint_));
            msg.add("command", "sendFile");
            msg.add("path", path);
            translater.postMessage(msg);
        }

        //----< respond to mouse double-click on file name for Browse and POPUP >----------------
        private void FileList_MouseDoubleClickBrowse(object sender, MouseButtonEventArgs e)
        {
            string selectedFile = (string)FileList2.SelectedItem;
            statusBarText.Text = selectedFile;
            string path = " ";
            if (selectedFile == "..")
            {
                if (pathStack1_.Count > 1)  // don't pop off "Storage"
                    pathStack1_.Pop();
                else
                    return;
            }
            else
            {
                path = pathStack1_.Peek() + "/" + selectedFile;
                pathStack1_.Push(path);
            }
           
            PathTextBlock2.Text = removeFirstDir(pathStack1_.Peek());

            CsEndPoint serverEndPoint = new CsEndPoint();
            serverEndPoint.machineAddress = "localhost";
            serverEndPoint.port = 8080;
            CsMessage msg = new CsMessage();
            msg.add("name", "PopUp");
            msg.add("to", CsEndPoint.toString(serverEndPoint));
            msg.add("from", CsEndPoint.toString(endPoint_));
            msg.add("command", "sendFile");
            msg.add("path", path);
            translater.postMessage(msg);


        }

        //----< Test Case For checking a particular file >----------------
        private void  checkInFromGUI()
        {
            Console.Write("---------<Test Case for Checking \"sample.cpp\">--------");
            CsEndPoint serverEndPoint = new CsEndPoint();
            serverEndPoint.machineAddress = "localhost";
            serverEndPoint.port = 8080;
            CsMessage msg = new CsMessage();
            msg.add("name", "sample.cpp");
            msg.add("to", CsEndPoint.toString(serverEndPoint));
            msg.add("from", CsEndPoint.toString(endPoint_));
            msg.add("command", "checkInRequest");
            msg.add("path", "../../../../RemoteRepo/sample/sample.cpp");
            msg.add("Description", "First time checking");
            msg.add("category", "python");
            msg.add("author", "Sayali Naval");
            msg.add("child", "sample::sample.h.1");

            translater.postMessage(msg);
            msg.show();

        }

        //---------<Test Case for changing status for file \"time.cpp.1\" having child \"time.h.1\" with status open>--------
        private void pendingStatus()
        {
            Console.Write("---------<Test Case for changing status for file \"time.cpp.1\" having child \"time.h.1\" with status open>--------");
            CsEndPoint serverEndPoint = new CsEndPoint();
            serverEndPoint.machineAddress = "localhost";
            serverEndPoint.port = 8080;
            CsMessage msg2 = new CsMessage();
            msg2.add("name", "time.cpp.1");
            msg2.add("to", CsEndPoint.toString(serverEndPoint));
            msg2.add("from", CsEndPoint.toString(endPoint_));
            msg2.add("command", "changeStatus");
            msg2.add("path", "../../../../RemoteRepo/time/time.cpp");
            msg2.add("fileNamespace", "time::time.cpp.1");
            msg2.show();
            translater.postMessage(msg2);

        }

        

        //----< Test Case For changing the status of particular file  >----------------
        private void changeStatusToClose()
        {
            Console.Write("---------<Test Case for changing status for file \"sample.h.1\">--------");
            CsEndPoint serverEndPoint = new CsEndPoint();
            serverEndPoint.machineAddress = "localhost";
            serverEndPoint.port = 8080;
            CsMessage msg2 = new CsMessage();
            msg2.add("to", CsEndPoint.toString(serverEndPoint));
            msg2.add("from", CsEndPoint.toString(endPoint_));
            msg2.add("command", "changeStatus");
            msg2.add("path", "../../../../RemoteRepo/test/test.h");
            msg2.add("fileNamespace", "test::test.h.1");
            msg2.show();
            translater.postMessage(msg2);
        }

        //---------<Test Case for Checking \"test.cpp\" having child \"test.h\" having Status of child is closed >----
        private void checkInTime()
        {
            Console.Write("---------<Test Case for Checking \"test.cpp\" having child \"test.h\" >----");
            Console.Write("----------<Status of child is closed>--------");
            CsEndPoint serverEndPoint = new CsEndPoint();
            serverEndPoint.machineAddress = "localhost";
            serverEndPoint.port = 8080;
            CsMessage msg = new CsMessage();
            msg.add("name", "test.cpp");
            msg.add("to", CsEndPoint.toString(serverEndPoint));
            msg.add("from", CsEndPoint.toString(endPoint_));
            msg.add("command", "checkInRequest");
            msg.add("path", "../../../../RemoteRepo/test/test.cpp");
            msg.add("Description", "First time checking");
            msg.add("category", "python");
            msg.add("author", "Sayali Naval");
            msg.add("child", "");

            translater.postMessage(msg);
            msg.show();

        }
        //-------------------------------------------------------------
        //----< Sending the CheckIn request for the selected file >----------------
        private void CheckingWithFileAuto()
        {
            
            foreach (string SelectedItems in FileList1.SelectedItems)
            {
                string something = string.Join("-", cat);
                string child = string.Join("-", addChild);

                Console.Write(something);
                string selectedFile = " ";
                 selectedFile = (string)SelectedItems;
                statusBarText.Text = "Selected File = " + selectedFile;
                string desc,author;           
                desc = Tdesc.Text;
                author = Tauthor.Text;
                Tdesc.Clear();
                Tauthor.Clear();
                Tcategory.Clear();
                TChild.Clear();

                string path;
                string[] words = selectedFile.Split('.');
                path = "../../../../RemoteRepo" + "/" + words[0] + "/" + selectedFile;
                Console.Write(path);
                CsEndPoint serverEndPoint = new CsEndPoint();
                serverEndPoint.machineAddress = "localhost";
                serverEndPoint.port = 8080;
                CsMessage msg = new CsMessage();
                msg.add("name", selectedFile);
                msg.add("to", CsEndPoint.toString(serverEndPoint));
                msg.add("from", CsEndPoint.toString(endPoint_));
                msg.add("command", "checkInRequest");
                msg.add("path", path);
                msg.add("Description", desc);
                msg.add("category", something);
                msg.add("author", author);
                msg.add("child", child);
                translater.postMessage(msg);

            }
            cat.Clear();
        }

        //----< Sending the checkout request for the selected file >----------------
        private void CheckoutWithFileAuto()
        {
            
            foreach (string SelectedItems in FileList.SelectedItems)
            {
                string selectedFile = " ";
                selectedFile = (string)SelectedItems;

                string path;
                if (selectedFile == "..")
                {
                    if (pathStack_.Count > 1)  // don't pop off "Storage"
                        pathStack_.Pop();
                    else
                        return;
                }
                else
                {
                    string[] words = selectedFile.Split('.');
                    path = "../Storage" + "/" + words[0] + "/" + selectedFile;
                    pathStack_.Push(path);
                }
                PathTextBlock.Text = " ";

                PathTextBlock.Text = removeFirstDir(pathStack_.Peek());

                CsEndPoint serverEndPoint = new CsEndPoint();
                serverEndPoint.machineAddress = "localhost";
                serverEndPoint.port = 8080;
                CsMessage msg = new CsMessage();
                msg.add("name", selectedFile);
                msg.add("to", CsEndPoint.toString(serverEndPoint));
                msg.add("from", CsEndPoint.toString(endPoint_));
                msg.add("command", "checkoutRequest");
                msg.add("path", pathStack_.Peek());
                translater.postMessage(msg);
                
            }
        }

        //----< Test case : Sending the checkout request for the particular  file >----------------
        private void checkOutFromGUI()
        {

            Console.Write("-------------<Checking out \"test.cpp.1\">----------");
            string selectedFile = "test.cpp.1";
            Console.WriteLine("Test Case for Checkout file \"test::test.cpp.1\"");
            CsEndPoint serverEndPoint = new CsEndPoint();
            serverEndPoint.machineAddress = "localhost";
            serverEndPoint.port = 8080;
            endPoint_.port = 8082;
            CsMessage msg = new CsMessage();
            msg.add("name", selectedFile);
            msg.add("to", CsEndPoint.toString(serverEndPoint));
            msg.add("from", CsEndPoint.toString(endPoint_));
            msg.add("command", "checkoutRequest");
            msg.add("path", "../Storage/test/test.cpp.1");
            translater.postMessage(msg);
            Console.WriteLine("Sending Msg");
            msg.show();

        }

        //----< Test case : Sending the checkout request for the particular  file >----------------
        private void checkOutFromGUIDependency()
        {

            string selectedFile = "time.cpp.1";
            Console.WriteLine("Test Case for Checkout file \"time::time.cpp.1\"  having children \"time::time.h.1\"");
            CsEndPoint serverEndPoint = new CsEndPoint();
            serverEndPoint.machineAddress = "localhost";
            serverEndPoint.port = 8080;
            endPoint_.port = 8082;
            CsMessage msg = new CsMessage();
            msg.add("name", selectedFile);
            msg.add("to", CsEndPoint.toString(serverEndPoint));
            msg.add("from", CsEndPoint.toString(endPoint_));
            msg.add("command", "checkoutRequest");
            msg.add("path", "../Storage/time/time.cpp.1");
            translater.postMessage(msg);
            //Console.WriteLine("Sending Msg");
            msg.show();

        }


        //--------------<Test Case for Query showing key containing \"test\" and files with version \".1\">-------------
        private void queryFromGUI()
        {
                Console.WriteLine("Test Case for Query Functionality showing key containing \"test\" and files with version \".1\" ");
                CsEndPoint serverEndPoint = new CsEndPoint();
                serverEndPoint.machineAddress = "localhost";
                serverEndPoint.port = 8080;
                CsMessage msg2 = new CsMessage();
                msg2.add("to", CsEndPoint.toString(serverEndPoint));
                msg2.add("from", CsEndPoint.toString(endPoint_));
                msg2.add("command", "search");
                msg2.add("BrowseByName", "test");
                msg2.add("BrowseByCat", "");
                msg2.add("BrowseByVersion", "1");
                msg2.add("BrowseByDependency", "");
                msg2.add("BrowseByname", "");
                msg2.add("BrowseByDesc", "");
                translater.postMessage(msg2);
                msg2.show();

          
        }

        //--------------<Test Case for Query showing Author name containing \"jim\" and description \"Prof\"">-------------
        private void queryFromGUI1()
        {
            Console.WriteLine("Test Case for Query showing Author name containing \"jim\" and description \"Prof\" ");
            CsEndPoint serverEndPoint = new CsEndPoint();
            serverEndPoint.machineAddress = "localhost";
            serverEndPoint.port = 8080;
            CsMessage msg2 = new CsMessage();
            msg2.add("to", CsEndPoint.toString(serverEndPoint));
            msg2.add("from", CsEndPoint.toString(endPoint_));
            msg2.add("command", "search");
            msg2.add("BrowseByName", "");
            msg2.add("BrowseByCat", "");
            msg2.add("BrowseByVersion", "");
            msg2.add("BrowseByDependency", "");
            msg2.add("BrowseByname", "jim");
            msg2.add("BrowseByDesc", "Prof");
            translater.postMessage(msg2);
            msg2.show();

        }
        //--------------<Test Case for Files without Parent>-------------
        private void searchWithoutParentsGUI()
        {
            Console.Write("--------<Searching File for No Parents>-------");
            CsEndPoint serverEndPoint = new CsEndPoint();
            serverEndPoint.machineAddress = "localhost";
            serverEndPoint.port = 8080;
            CsMessage msg2 = new CsMessage();
            msg2.add("to", CsEndPoint.toString(serverEndPoint));
            msg2.add("from", CsEndPoint.toString(endPoint_));
            msg2.add("command", "sendWithoutParent");
            translater.postMessage(msg2);
            msg2.show();

        }

        //----< Change of events when a particular tab is selected. >----------------
        private void tabControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            string selectedFirst= (string)FileList.SelectedItem;
            statusBarText.Text = selectedFirst;
            int tabItem = ((sender as TabControl)).SelectedIndex;
            CsEndPoint serverEndPoint = new CsEndPoint();
            serverEndPoint.machineAddress = "localhost";
            serverEndPoint.port = 8080;
            if (e.Source is TabControl) // This is a soultion of those problem.
            {
                switch (tabItem)
                {       case 0: PathTextBlock.Text = "Storage";
                        pathStack_.Push("../Storage");
                        break;
                    case 1:
                        clearFiles1();
                        clearDirs1();
                        insertParent1();
                        string[] dirs = Directory.GetDirectories("../../../../RemoteRepo");
                        foreach (string dir in dirs)
                        {   string[] words = dir.Split('\\');
                            addDir1(words[1]);    
                        }
                        break;
                    case 2: PathTextBlock.Text = "../Storage";
                        break;
                    case 3:
                        CsMessage msg1 = new CsMessage();
                        msg1.add("name", "Browse");
                        msg1.add("to", CsEndPoint.toString(serverEndPoint));
                        msg1.add("from", CsEndPoint.toString(endPoint_));
                        msg1.add("command", "getDirs1");
                        msg1.add("path1", "../Storage");
                        translater.postMessage(msg1);
                        msg1.remove("command");
                        msg1.add("command", "getFiles1");
                        translater.postMessage(msg1);
                        break;
                    case 4: break;
                    case 5:      
                        CsMessage msg2 = new CsMessage();
                        msg2.add("name", "ViewMeta");
                        msg2.add("to", CsEndPoint.toString(serverEndPoint));
                        msg2.add("from", CsEndPoint.toString(endPoint_));
                        msg2.add("command", "getDirs1");
                        msg2.add("path1", "../Storage");
                        translater.postMessage(msg2);
                        break;
                }
            }          
        }

        //----< Test case: Connect Server >----------------
        private void connectServer()
        {
           
            CsEndPoint serverEndPoint = new CsEndPoint();
            serverEndPoint.machineAddress = "localhost";
            serverEndPoint.port = 8080;
            CsMessage msg2 = new CsMessage();
            msg2.add("name", "connectMsg");
            msg2.add("to", CsEndPoint.toString(serverEndPoint));
            msg2.add("from", CsEndPoint.toString(endPoint_));
            msg2.add("command", "connectFile");
            translater.postMessage(msg2);
        }

    void test1()
    {
      MouseButtonEventArgs e = new MouseButtonEventArgs(null, 0, 0);
      DirList.SelectedIndex = 1;
      DirList_MouseDoubleClick(this, e);
    }
        //----< Button Click Event Check Out  >----------------
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string selectedFirst = (string)FileList.SelectedItem;
            CheckoutWithFileAuto();
        }

        //----< Button Click Event CheckIn >----------------
        private void Button_Click1(object sender, RoutedEventArgs e)
        {
          
            CheckingWithFileAuto();
        }

        //----< Button Click Event for connecting to server >----------------
        private void Button_Click_connect(object sender, RoutedEventArgs e)
        {
            checkin.IsEnabled = true;
            checkOut.IsEnabled = true;
            browse.IsEnabled = true;
            viewFile.IsEnabled = true;
            viewMetaData.IsEnabled = true;
            connection.IsEnabled = false;
            CsEndPoint serverEndPoint = new CsEndPoint();
            serverEndPoint.machineAddress = "localhost";
            serverEndPoint.port = 8080;
            CsMessage msg2 = new CsMessage();
            msg2.add("name", "connectMsg");
            msg2.add("to", CsEndPoint.toString(serverEndPoint));
            msg2.add("from", CsEndPoint.toString(endPoint_));
            msg2.add("command", "connectFile");
            translater.postMessage(msg2);


        }

        //----------------<Button Click event for Search>-------------------------------
        private void Button_Click_Search(object sender, RoutedEventArgs e)
        {
            string sName,sCat,sVersion,sChild, sAuthor,sDesc;
            sName = BrowseByName.Text;
            sCat = BrowseByCat.Text;
            sVersion = BrowseByVersion.Text;
            sChild = BrowseByDependency.Text;
            sAuthor = BrowseByname.Text;
            sDesc= BrowseByDesc.Text;

            if (sCat.Length != 0 || sName.Length != 0 || sVersion.Length != 0 || sChild.Length != 0 || sAuthor.Length!=0 || sDesc.Length!= 0)
            {
                CsEndPoint serverEndPoint = new CsEndPoint();
                serverEndPoint.machineAddress = "localhost";
                serverEndPoint.port = 8080;
                CsMessage msg2 = new CsMessage();
                msg2.add("to", CsEndPoint.toString(serverEndPoint));
                msg2.add("from", CsEndPoint.toString(endPoint_));
                msg2.add("command", "search");
                msg2.add("BrowseByName", sName);
                msg2.add("BrowseByCat", sCat);
                msg2.add("BrowseByVersion", sVersion);
                msg2.add("BrowseByDependency", sChild);
                msg2.add("BrowseByname", sAuthor);
                msg2.add("BrowseByDesc", sDesc);
                translater.postMessage(msg2);
                
            }
            else
            {
                Console.Write("SEARCH ENGINE PARAMETERS EMPTY");

            }
            BrowseByname.Clear();
            BrowseByName.Clear();
            BrowseByCat.Clear();
            BrowseByDependency.Clear();
            BrowseByVersion.Clear();
            BrowseByDesc.Clear();
            searchList.Items.Clear();

        }

        //---------<Button click event for changing the status for a file>----------------------
        private void buttonClick_ChangeTheStatus1(object sender, RoutedEventArgs e)
        {
            string selectedFirst = (string)FileList.SelectedItem;
            string[] words = selectedFirst.Split('.');
            String path = "../../../../RemoteRepo" + "/" + words[0] + "/" + selectedFirst;

            CsEndPoint serverEndPoint = new CsEndPoint();
            serverEndPoint.machineAddress = "localhost";
            serverEndPoint.port = 8080;
            CsMessage msg2 = new CsMessage();
            msg2.add("name", selectedFirst);
            msg2.add("to", CsEndPoint.toString(serverEndPoint));
            msg2.add("from", CsEndPoint.toString(endPoint_));
            msg2.add("command", "changeStatus");
            msg2.add("path", path);
            msg2.add("fileNamespace", words[0] + "::" + selectedFirst);
            msg2.show();

            translater.postMessage(msg2);

        }
        //----< Button Click Event Adding category >----------------
        private void buttonClick_cat(object sender, RoutedEventArgs e)
        {
            String category = Tcategory.Text;
            Tcategory.Clear();
            cat.Add(category);

        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        //----< Button Click Event getting files without Parent >----------------
        private void  ButtonClick_WithoutParent(object sender, RoutedEventArgs e)
        {
            CsEndPoint serverEndPoint = new CsEndPoint();
            serverEndPoint.machineAddress = "localhost";
            serverEndPoint.port = 8080;
            CsMessage msg2 = new CsMessage();
            msg2.add("to", CsEndPoint.toString(serverEndPoint));
            msg2.add("from", CsEndPoint.toString(endPoint_));
            msg2.add("command", "sendWithoutParent");
            translater.postMessage(msg2);
        }

        private void Tcategory_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        //----< Button Click Event for Adding child >----------------
        private void ButtonClick_AddChild(object sender, RoutedEventArgs e)
        {
            String child = TChild.Text;
            TChild.Clear();
            addChild.Add(child);
        }

    
    }
}
