/////////////////////////////////////////////////////////////////////////
// ServerPrototype.cpp - Console App that processes incoming messages  //
// ver 1.0                                                             //
// Honey Shah, Ms Computer Science                                     //
// Source :Jim Fawcett,                                                //
// CSE687-OnLine Object Oriented Design, Spring 2018                   //
/////////////////////////////////////////////////////////////////////////

#include "ServerPrototype.h"
#include "../FileSystem-Windows/FileSystemDemo/FileSystem.h"
//#include "../FileSystemDemo/FileSystem.h"
#include <chrono>

namespace MsgPassComm = MsgPassingCommunication;

using namespace Repository;
using namespace FileSystem;

using Msg = MsgPassingCommunication::Message;

// Function to get files from server 
Files Server::getFiles(const Repository::SearchPath& path)
{
  return Directory::getFiles(path);
}

//Function to get Directories from server
Dirs Server::getDirs(const Repository::SearchPath& path)
{
  return Directory::getDirectories(path);
}

// Function to show the content of the message
template<typename T>
void show(const T& t, const std::string& msg)
{
  std::cout << "\n  " << msg.c_str();
  for (auto item : t)
  {
    std::cout << "\n    " << item.c_str();
  }
}


std::function<Msg(Msg)> echo = [](Msg msg) {
  Msg reply = msg;
  reply.to(msg.from());
  reply.from(msg.to());
  reply.command(msg.command());
  return reply;
  
};

//Processing GetFiles to get the files for server
std::function<Msg(Msg)> getFiles = [](Msg msg) {
  Msg reply;
  reply.to(msg.from());
  reply.from(msg.to());
  reply.command("getFiles");
  std::string path = msg.value("path");
  if (path != "")
  {
    std::string searchPath = storageRoot;
    if (path != ".")
      searchPath = searchPath + "\\" + path;
    Files files = Server::getFiles(searchPath);
    size_t count = 0;
    for (auto item : files)
    {
      std::string countStr = Utilities::Converter<size_t>::toString(++count);
      reply.attribute("file" + countStr, item);
    }
  }
  else
  {
    std::cout << "\n  getFiles message did not define a path attribute";
  }
  return reply;
};

std::function<Msg(Msg)> getFiles1 = [](Msg msg) {
	Msg reply;
	reply.name(msg.name());
	reply.to(msg.from());
	reply.from(msg.to());
	reply.command("getFiles1");
	std::string path = msg.value("path1");
	if (path != "")
	{
		std::string searchPath = storageRoot;
		if (path != ".")
			searchPath = searchPath + "\\" + path;
		Files files = Server::getFiles(searchPath);
		size_t count = 0;
		for (auto item : files)
		{
			std::string countStr = Utilities::Converter<size_t>::toString(++count);
			reply.attribute("file" + countStr, item);
		}
	}
	else
	{
		std::cout << "\n  getFiles message did not define a path attribute";
	}
	return reply;
};

std::function<Msg(Msg)> search = [](Msg msg) {
	std::string n;
	Msg reply;
	reply.name(msg.name());
	reply.to(msg.from());
	reply.from(msg.to());
	reply.command("listOfFiles");
	std::string sName = msg.value("BrowseByName");
	std::string sCat = msg.value("BrowseByCat");
	std::string sChild = msg.value("BrowseByDependency");
	std::string sVersion = msg.value("BrowseByVersion");
	std::string sAuthor = msg.value("BrowseByname");
	std::string sDesc = msg.value("BrowseByDesc");
	DbCore<PayLoad> db, db1;
	DbProvider dbp;
	db = dbp.db();
	Query<PayLoad> q1(db), q2(db), q4(db);
	Conditions<PayLoad> conds1, conds2;
	conds1.keyM(sName);
	if (sChild != "")
	{	conds2.keyM(sChild);
		q4.select(conds2);
		Keys keys = q4.keys();
		conds1.children(keys);
	}
	conds1.version(sVersion);
	conds1.name(sAuthor);
	conds1.description(sDesc);
	std::cout << " \n ----------------The result of the search is------------\n";
	q1.select(conds1).show();
	if (sCat != "")
	{   std::string category = msg.value("Category");
		auto hasCategory = [&sCat](DbElement<PayLoad>& elem) {
			return (elem.payLoad()).hasCategory(sCat);
		};
		q2.select(hasCategory);
		std::cout << " \n ----------------The result of the search is------------\n";
		q1.query_or(q2).show();
	}
	std::string searchResult = "";
	if (!q1.keys().empty())
	{
		for (std::string key : q1.keys())
		{
			searchResult = key + " " + searchResult;
		}
	}	
	reply.attribute("FileList", searchResult);	
	return reply;
};

//Processing sendFile to Transfer the file for popUp
std::function<Msg(Msg)> sendFile = [](Msg msg) {
	Msg reply;
	reply.to(msg.from());
	reply.from(msg.to());
	reply.command("sendFile");
	std::string path = msg.value("path");
	std::cout << "\n The path is :" << path;
	std::size_t found = path.find_last_of("/\\");
	std::string filePath = path.substr(0, found);
	std::string fileName =path.substr(found + 1);
	std::cout << "\n FilePath:" << filePath;
	std::cout << "\n FileName : "<< fileName;
    found = fileName.find_last_of(".");
	std::string fold = path.substr(0, found);
	std::string extension = path.substr(found + 1);

	std::string src = fileName + "/" + filePath;
	std::string dest = "../SendFiles/" + fold + "/" + fileName;
	//File f1;
	
	reply.file(fileName);
	reply.attribute("path", filePath);
	reply.attribute("savePath", "../../../../SaveFiles");
	//checkInFile c1;



	
	return reply;
};

//Processing GetFiles to get the Directory for server
std::function<Msg(Msg)> getDirs = [](Msg msg) {
  Msg reply;
  reply.to(msg.from());
  reply.from(msg.to());
  reply.command("getDirs");
  std::string path = msg.value("path");
  if (path != "")
  {
    std::string searchPath = storageRoot;
    if (path != ".")
      searchPath = searchPath + "\\" + path;
    Files dirs = Server::getDirs(searchPath);
    size_t count = 0;
    for (auto item : dirs)
    {
      if (item != ".." && item != ".")
      {
        std::string countStr = Utilities::Converter<size_t>::toString(++count);
        reply.attribute("dir" + countStr, item);
      }
    }
  }
  else
  {
    std::cout << "\n  getDirs message did not define a path attribute";
  }
  return reply;
};
std::function<Msg(Msg)> getDirs1 = [](Msg msg) {
	Msg reply;
	reply.name(msg.name());
	reply.to(msg.from());
	reply.from(msg.to());
	reply.command("getDirs1");
	std::string path = msg.value("path1");
	if (path != "")
	{
		std::string searchPath = storageRoot;
		if (path != ".")
			searchPath = searchPath + "\\" + path;
		Files dirs = Server::getDirs(searchPath);
		size_t count = 0;
		for (auto item : dirs)
		{
			if (item != ".." && item != ".")
			{
				std::string countStr = Utilities::Converter<size_t>::toString(++count);
				reply.attribute("dir" + countStr, item);
			}
		}
	}
	else
	{
		std::cout << "\n  getDirs message did not define a path attribute";
	}
	return reply;
};

//Processing checkInRequest for sending Msg to client checking ready
std::function<Msg(Msg)> checkInRequest = [](Msg msg) {
	//std::cout << " \n  Requirement #1 : checking Test Case for File \"Test.cpp\" in \"RemoteRepo\"  \n";
	Msg reply;
	reply.to(msg.from());
	reply.from(msg.to());
	reply.command("checkInReady");
	reply.attribute("path", msg.value("path"));
	reply.attribute("Description", msg.value("Description"));
	reply.attribute("category", msg.value("category"));
	reply.attribute("author", msg.value("author"));
	reply.attribute("child", msg.value("child"));
	std::string path = msg.value("path");
	if (path != "")
	{
		std::string searchPath = storageRoot1;
		if (path != ".")
			searchPath = searchPath + "\\" + path;
		reply.attribute("Ready_for_checking", msg.name());
	}
	else
	{
		std::cout << "\n  Files couldnot checkIn";
	}
	return reply;
};



//Processing checkInFile for sending Msg to client checking Done
std::function<Msg(Msg)> checkinFile = [](Msg msg) {

	Msg reply;
	reply.to(msg.from());
	reply.from(msg.to());
	reply.command("checkInDone");
	std::string path = msg.value("path");
	std::string name = msg.value("name");
	reply.attribute("Description", msg.value("Description"));
	reply.attribute("category", msg.value("category"));
	reply.attribute("author", msg.value("author"));
	reply.attribute("child", msg.value("child"));

	std::size_t found = path.find_last_of("/\\");
	std::string filePath = path.substr(0, found);
	std::string fileName = path.substr(found + 1);


//	std::string path = msg.value("path");
	if (path != "")
	{
		std::string searchPath = storageRoot1;
		if (path != ".")
			searchPath = searchPath + "\\" + path;
		reply.attribute("Done", msg.name());
		reply.attribute("path", filePath);
		
	}
	else
	{
		std::cout << "\n  Files couldnot checkIn";
	}
	return reply;
};

//Processing checkoutRequest for sending Msg to client for file transfer 
std::function<Msg(Msg)> checkoutRequest = [](Msg msg) {

	Msg reply;
	reply.to(msg.from());
	reply.from(msg.to());
	reply.command("fileTransfer");
	std::string name = msg.value("name");
	
	std::string path = msg.value("path");
	std::size_t found = path.find_last_of("/\\");
	std::string filePath = path.substr(0, found);
	std::string fileName = path.substr(found + 1);

	if (path != "")
	{
		std::string searchPath = storageRoot;
		if (path != ".")
			searchPath = searchPath + "\\" + path;
		reply.attribute("Ready_for_checkOut", msg.name());
		reply.file(name);
		reply.attribute("path",filePath);
		reply.attribute("savePath", "../../../../SaveFiles");



	}
	else
	{
		std::cout << "\n  Files couldnot checkIn";
	}
	return reply;
};

//Processing getMetaData for getting meta data
std::function<Msg(Msg)> getMetaData = [](Msg msg) {
	std::string Category1;
	std::string child1,category;
	Msg reply;
	reply.name(msg.name());
	reply.to(msg.from());
	reply.from(msg.to());
	reply.command("sending_metaData");
	std::string fileName = msg.value("fileNamespace");
	DbProvider dbp;
	DbCore<PayLoad> db = dbp.db();
	DbElement<PayLoad> elem;
	elem = db[fileName];
	std::string status= "Status: "+ elem.payLoad().truthValue();
	for (auto cat : elem.payLoad().categories())
	{
		category = cat + " " + category;
	}
	std::string pathMeta = "Path: " + msg.value("path1");
	std::string path = msg.value("path1");
	std::string author = "Author: " + elem.name();
	std::string Description = "Description:" + elem.descrip();
	std::string DateTime = elem.dateTime();
	Children children = elem.children();
	if (children.size() > 0)
	{
		for (auto key : children)
		{
			child1 = " " + key;
		}
	}
	std::string child = "Childrens: " + child1;
	if (path != "")
	{
		std::string searchPath = storageRoot;
		if (path != ".") searchPath = searchPath + "\\" + path;
		reply.attribute("pathMeta", pathMeta);
		reply.attribute("Description", Description);
		reply.attribute("DateTime", DateTime);
		reply.attribute("author", author);
		reply.attribute("childrens", child);
		reply.attribute("status", status);
		reply.attribute("category", category);	
	}
	else
	{
		std::cout << "\n  Files couldnot checkIn";
	}	
	return reply;
};

//Processing connectFile for connecting to server
std::function<Msg(Msg)> connectFile = [](Msg msg) {
	Msg reply;
	reply.name(msg.name());
	reply.to(msg.from());
	reply.from(msg.to());
	reply.command("connection_Sucessful");
	reply.attribute("Connection Sucessful", msg.name());
	return reply;
};
std::function<Msg(Msg)> sendWithoutParent = [](Msg msg) {
	Msg reply;
	reply.name(msg.name());
	reply.to(msg.from());
	reply.from(msg.to());
	reply.command("FileWithoutParents");
	std::string keyList;
	DbProvider dbp;
	DbCore<PayLoad> db = dbp.db();
	Conditions<PayLoad>  conds0;
	Query<PayLoad> q1(db);
	for (auto key : db.keys())
	{
		Keys keys{ key };
		conds0.children(keys);
		q1.select(conds0);
		if (q1.keys().empty())
		{
			keyList = keyList + " " + key;
			
		}
	}

	std::cout << "\n Key is :" << keyList;
	reply.attribute("Filelist", keyList);
	return reply;
};

std::function<Msg(Msg)> changeStatus = [](Msg msg) {
	Msg reply;
	reply.name(msg.value("name"));
	reply.to(msg.from());
	reply.from(msg.to());
	reply.command("statusChanged");
	std::string fileNamePlusNamespace = msg.value("fileNamespace");
	reply.attribute("fileNamespace", fileNamePlusNamespace);
	DbProvider dbp;
	DbCore<PayLoad> db = dbp.db();
	checkInFile c;
	db=c.changeCheckInStatus(db,fileNamePlusNamespace);
	dbp.db() = db;
	showDb(db);
	PayLoad p;
	p.showDb(db);
	return reply;
};
void show()
{
	std::cout << "\n  Requirement Test Case's Implementation";
	std::cout << "\n ==========================";
	std::cout << "\n";
	
	std::cout << " \n Test Case checkout File \"test.cpp.1\"";
	std::cout << "\n  Test Case for Checkout file \"time::time.cpp.1\"  having children \"time::time.h.1\"";

	std::cout << " \n Test Case for Checking \"sample.cpp\"";
	std::cout<<" \n Test Case for changing status for file \"time.cpp.1\"(checking pending) having child \"time.h.1\" with status open \n";
	std::cout << "\n Test Case for Checking \"test.cpp\"(succesfull closed) having child \"test.h\" having Status of child is closed \n";
	std::cout << "--------<Test Case for changing status for file \"sample.h.1\">--------\n";

	std::cout << "\n Test Case for Query Functionality showing key containing \"test\" and files with version \".1\" \n";
	std::cout << "\n Test Case for Query showing Author name containing \"jim\" and description \"Prof\" \n";

    
	std::cout << " \n Test Case for viewing metadata for \"test.cpp\">-----\n";
	std::cout << " \n Requirement  View File \"test.h\"";
	std::cout << " \n Requirement  Connect File ";
	std::cout << "\n Server has two clients. Client 1= \"8082\" and Client 2 = \"8085\"";
	std::cout << "\n Test Case for \"checkout\" is implemented from \"client 1\"";
	std::cout << "\n Test case for \"checkIN\" , \"View file\", \"view meta Data\"";
	std::cout<<" implemented from client with port : \"8085\" \n";



}
int main()
{
  std::cout << "\n  Testing Server Prototype";
  std::cout << "\n ==========================";
  std::cout << "\n";
  Server server(serverEndPoint, "ServerPrototype");
  server.start();
  std::cout << "\n  testing message processing";
  std::cout << "\n ----------------------------";
  server.addMsgProc("echo", echo);
  server.addMsgProc("getFiles", getFiles);
  server.addMsgProc("getFiles1", getFiles1);
  server.addMsgProc("getDirs", getDirs);
  server.addMsgProc("getDirs1", getDirs1);
  server.addMsgProc("serverQuit", echo);
  server.addMsgProc("checkInRequest", checkInRequest);
  server.addMsgProc("checkoutRequest", checkoutRequest);
  server.addMsgProc("sendFile", sendFile);
  server.addMsgProc("checkinFile", checkinFile);
  server.addMsgProc("getMetaData", getMetaData);
  server.addMsgProc("connectFile", connectFile); 
  server.addMsgProc("search", search);
  server.addMsgProc("changeStatus", changeStatus);
  server.addMsgProc("sendWithoutParent", sendWithoutParent);
  server.processMessages();  
  show();
  createDatabase();
 Msg msg(serverEndPoint, serverEndPoint);  // send to self
 /*msg.name("msgToSelf");
  msg.command("echo");
  msg.attribute("verbose", "show me");
  server.postMessage(msg);
  std::this_thread::sleep_for(std::chrono::milliseconds(1000));
  msg.command("getFiles");
  msg.remove("verbose");
  msg.attributes()["path"] = storageRoot;
  server.postMessage(msg);
  std::this_thread::sleep_for(std::chrono::milliseconds(1000));
  msg.command("getDirs");
  msg.attributes()["path"] = storageRoot;
  server.postMessage(msg);
  std::this_thread::sleep_for(std::chrono::milliseconds(1000));*/
  std::cout << "\n  press enter to exit";
  std::cin.get();
  std::cout << "\n";
  msg.command("serverQuit");
  server.postMessage(msg);
  server.stop();
  return 0;
}

