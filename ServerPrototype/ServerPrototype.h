#pragma once
///////////////////////////////////////////////////////////////////////
// ServerPrototype.h - Console App that processes incoming messages  //
// ver 1.0                                                           //
// Honey Shah, Ms Computer Science                                   //
// Source :Jim Fawcett,                                              //
// CSE687-OnLine Object Oriented Design, Spring 2018                 //
///////////////////////////////////////////////////////////////////////
/*
*  Package Operations:
* ---------------------
*  Package contains one class, Server, that contains a Message-Passing Communication
*  facility. It processes each message by invoking an installed callable object
*  defined by the message's command key.
*
*  Message handling runs on a child thread, so the Server main thread is free to do
*  any necessary background processing (none, so far).
*
*  Required Files:
* -----------------
*  ServerPrototype.h, ServerPrototype.cpp
*  Comm.h, Comm.cpp, IComm.h
*  Message.h, Message.cpp
*  FileSystem.h, FileSystem.cpp
*  Utilities.h
*
*  Maintenance History:
* ----------------------
*  ver 1.0 : 3/27/2018
*  - first release
*/
#include <vector>
#include <string>
#include <unordered_map>
#include <functional>
#include <thread>
#include "../CppCommWithFileXfer/Message/Message.h"
#include "../CppCommWithFileXfer/MsgPassingComm/Comm.h"
#include <windows.h>
#include <tchar.h>
#include "../PayLoad/PayLoad.h"
#include"../checkIn/checkIn.h"
#include "../checkOut/checkOut.h"
#include "../Query/Query.h"
#include "../Utilities/StringUtilities/StringUtilities.h"

using namespace NoSqlDb;
//using namespace FileSystem;


namespace Repository
{
	class DbProvider
	{
	public:
		DbCore<PayLoad>& db() { return db_; }
	private:
		static DbCore<PayLoad> db_;
	};
	DbCore<PayLoad> DbProvider::db_;

	auto putLine = [](size_t n = 1, std::ostream& out = std::cout)
	{
		Utilities::putline(n, out);
	};

  using File = std::string;
  using Files = std::vector<File>;
  using Dir = std::string;
  using Dirs = std::vector<Dir>;
  using SearchPath = std::string;
  using Key = std::string;
  using Msg = MsgPassingCommunication::Message;
  using ServerProc = std::function<Msg(Msg)>;
  using MsgDispatcher = std::unordered_map<Key,ServerProc>;
  
  const SearchPath storageRoot = "../Storage";  // root for all server file storage
  const SearchPath storageRoot1 = "../RemoteRepo";
  const MsgPassingCommunication::EndPoint serverEndPoint("localhost", 8080);  // listening endpoint
  const MsgPassingCommunication::EndPoint clientEndPoint("localhost", 8082);  // listening endpoint

  class Server
  {
  public:
    Server(MsgPassingCommunication::EndPoint ep, const std::string& name);
    void start();
    void stop();
    void addMsgProc(Key key, ServerProc proc);
    void processMessages();
    void postMessage(MsgPassingCommunication::Message msg);
    MsgPassingCommunication::Message getMessage();
    static Dirs getDirs(const SearchPath& path = storageRoot);
    static Files getFiles(const SearchPath& path = storageRoot);
	void CheckOut_Done(Msg msg);
	void checkin_Successful(Msg msg);
  private:
    MsgPassingCommunication::Comm comm_;
    MsgDispatcher dispatcher_;
    std::thread msgProcThrd_;
  };
  //----< initialize server endpoint and give server a name >----------

  inline Server::Server(MsgPassingCommunication::EndPoint ep, const std::string& name)
    : comm_(ep, name) {}

  //----< start server's instance of Comm >----------------------------

  inline void Server::start()
  {
    comm_.start();
  }
  //----< stop Comm instance >-----------------------------------------

  inline void Server::stop()
  {
    if(msgProcThrd_.joinable())
      msgProcThrd_.join();
    comm_.stop();
  }
  //----< pass message to Comm for sending >---------------------------

  inline void Server::postMessage(MsgPassingCommunication::Message msg)
  {
    comm_.postMessage(msg);
  }
  //----< get message from Comm >--------------------------------------

  inline MsgPassingCommunication::Message Server::getMessage()
  {
    Msg msg = comm_.getMessage();
    return msg;
  }
  //----< add ServerProc callable object to server's dispatcher >------

  inline void Server::addMsgProc(Key key, ServerProc proc)
  {
    dispatcher_[key] = proc;
  }
  inline void Server::CheckOut_Done(Msg msg)
  {
	  DbProvider dbp;
	  DbCore<PayLoad> db = dbp.db();
	  std::string nameFile = msg.name();
	  std::string path = msg.value("path");
	  std::size_t found = path.find_last_of("/\\");
	  std::string filePath = path.substr(0, found);
	  std::string nameSpace = path.substr(found + 1);
	  checkOutFile c;
	  c.copyToLocal(db, nameSpace + "::" + nameFile);
	  dbp.db() = db;
	  std::cout << "\n -------------- Name:" << nameSpace + "::" + nameFile;
	  DbElement<PayLoad> elem;
	  elem = db[nameSpace + "::" + nameFile];
	  Children children = elem.children();
	  if (children.size() > 0)
	  {
		  std::cout << "\n inside child";
		  for (auto key : children)
		  {
			  std::size_t found = key.find_first_of(":");
			  std::string x = key.substr(0, found);
			  std::string filePassingNAme = key.substr(found + 2);
			  std::cout << "\n " << x;
			  std::cout << "\n " << filePassingNAme;
			  Msg reply;
			  reply.to(msg.from());
			  reply.from(msg.to());
			  reply.command("fileTransfer");
			  //std::string name = msg.value("name");
			  reply.attribute("Ready_for_checkOut", filePassingNAme);
			  reply.file(filePassingNAme);
			  reply.attribute("path", "../Storage/" + x);
			  reply.attribute("savePath", "../../../../SaveFiles");
			  postMessage(reply);
		  }
	  }
	  //showDb(db);
	  msg.show();
  }
  inline void Server::checkin_Successful(Msg msg)
  {
	  msg.show();
	  std::string path = msg.value("path");
	  std::string nameFile = msg.name();
	  std::size_t found = path.find_last_of("/\\");
	  std::string filePath = path.substr(0, found);
	  std::string nameSpace = path.substr(found + 1);
	  std::string desc = msg.value("Description");
	  std::string category = msg.value("category");
	  std::string author = msg.value("author");
	  std::string child = msg.value("child");
	  DbProvider dbp;
	  DbCore<PayLoad> db = dbp.db();
	  checkInFile c1;
	  db = c1.findIfFileIsThereInDatabase(db, nameSpace + "::" + nameFile, desc, category, author, child);
	  dbp.db() = db;
	  showDb(db);
	  PayLoad p;
	  p.showDb(db);
  }
  //----< start processing messages on child thread >------------------

  inline void Server::processMessages()
  {
	auto proc = [&]()
    {
      if (dispatcher_.size() == 0)
      {
        std::cout << "\n  no server procs to call";
        return;
      }
      while (true)
      {
        Msg msg = getMessage();
		std::cout << "\n";
        std::cout << "\n  received message: " << msg.command() << " from " << msg.from().toString();
        if (msg.containsKey("verbose"))
        {
          std::cout << "\n";
          msg.show();
        }
        if (msg.command() == "serverQuit")
          break;
		if (msg.command() == "CheckOut_Done")
		{
			CheckOut_Done(msg);
			
		}
		else if (msg.command() == "checkin_Successful")
		{
			checkin_Successful(msg);
			
		}	
		else {
			Msg reply = dispatcher_[msg.command()](msg);
			if (msg.to().port != msg.from().port)  // avoid infinite message loop
			{
		    	postMessage(reply);
				msg.show();
				std::cout << "\n  Sending message: " << reply.command() << " from " << msg.to().toString();
				reply.show();
			}	
			else
				std::cout << "\n  server attempting to post to self";
		}
      }
      std::cout << "\n  server message processing thread is shutting down";
    };
    std::thread t(proc);
    std::cout << "\n  starting server thread to process messages";
    msgProcThrd_ = std::move(t);
  }

  bool createDatabase()
  {   Utilities::title("making demo database");
	  DbCore<PayLoad> db;
	  DbProvider dbp;
	  dbp.db() = db;
	  DbElement<PayLoad> elem = makeElement<PayLoad>("jim fawcett", "Professor's file");
	  elem.dateTime(DateTime().now());
	  PayLoad pl, p2, p3, p4,p5;
	  pl.value("../Storage/test");
	  pl.categories().push_back("Header");
	  pl.truthValue("close");
	  elem.payLoad(pl);
	  db["test::test.h.1"] = elem;
	  DbElement<PayLoad> elem1 = makeElement<PayLoad>("Honey-test.cpp.1", "C++File");
	  elem1.dateTime(DateTime().now());
	  p2.value("../Storage/test");
	  p2.categories().push_back("C++");
	  p2.truthValue("close");
	  elem1.payLoad(p2);
	  db["test::test.cpp.1"] = elem1;
	  db["test::test.cpp.1"].children().push_back("test::test.h.1");
	  DbElement<PayLoad> elem2= makeElement<PayLoad>("Honey-time.h.1", "Header");
	  elem.dateTime(DateTime().now());
	  p3.value("../Storage/time");
	  p3.categories().push_back("java");
	  p3.truthValue("open");
	  elem2.payLoad(p3);
	  db["time::time.h.1"] = elem2;
	  DbElement<PayLoad> elem3 = makeElement<PayLoad>("Honey-time.cpp.1", "C++File");
	  elem.dateTime(DateTime().now());
	  p4.value("../Storage/time");
	  p4.categories().push_back("c#");
	  p4.truthValue("open");
	  elem3.payLoad(p4);
	  db["time::time.cpp.1"] = elem3;
	  db["time::time.cpp.1"].children().push_back("time::time.h.1");
	  DbElement<PayLoad> elem4 = makeElement<PayLoad>("Sample", "header");
	  elem4.dateTime(DateTime().now());
	  p5.value("../Storage/sample");
	  p5.categories().push_back("");
	  p5.truthValue("open");
	  elem4.payLoad(p5);
	  db["sample::sample.h.1"] = elem4;
	  dbp.db() = db;
	  putLine();
	  showDb(db);
	  PayLoad p;
	  p.showDb(db);
	  return true;
  }
}