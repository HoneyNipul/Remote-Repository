//////////////////////////////////////////////////////////////////////
// App.xaml.cs - Application start                                   //
// ver 1.0                                                           //
// Honey Shah, Ms Computer Science                                   //
// Source :Jim Fawcett,                                              //
// CSE687-OnLine Object Oriented Design, Spring 2018                 //
///////////////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * This package provides a WPF-based GUI for Project3HelpWPF demo.  It's 
 * responsibilities are to:
 * -Calls the constructor for the main window.
  
 * Required Files:
 * ---------------
 * App.xaml, App.xaml.cs
 * Translater.dll
 * 
 * Maintenance History:
 * --------------------
 *
 * ver 1.0:   12 April 2018
 *  - First release
 *  
 */

// Translater has to be statically linked with CommLibWrapper
// - loader can't find Translater.dll dependent CommLibWrapper.dll

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp1
{
  
  public partial class App : Application
  {
        void App_Startup(object sender, StartupEventArgs e)
        {
            
            
            MainWindow client1 = new MainWindow("8082");
            client1.Show();
            
            MainWindow client2 = new MainWindow("8085");
            client2.Show();
        }


    }
}
