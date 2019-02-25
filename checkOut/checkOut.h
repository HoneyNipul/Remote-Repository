#pragma once
/////////////////////////////////////////////////////////////////////
// checkOut.h - Implements the checkOut functionality in checkOut  //
//                                   class                         //
// ver 1.0                                                         //
// Author Name : Honey Shah                                        //
// Source: Dr. Jim Fawcett                                         //
// Jim Fawcett, CSE687 - Object Oriented Design, Spring 2018       //
/////////////////////////////////////////////////////////////////////
/*
* Package Operations:
* -------------------
* This package provides one classe:
* - checkout implements implements all the check-out functionality of a package
* The package  provides functions for checkout files from repository to local.


* Required Files:
* ---------------
* Filsystem.h, Dbcore.h
* PayLoad.h 
*
* Maintenance History:
* --------------------
* ver 1.0 : 04 March 2018
* - first release
*/
#include <iostream>
#include <string>
#include <sstream>
#include <sstream>
#include <iomanip>
#include <utility>
#include "../FileSystem-Windows/FileSystemDemo/FileSystem.h"
#include "../DateTime/DateTime.h"
//#include "../FileSystemDemo/FileSystem.h"
#include "../DbCore/DbCore.h"
#include "../PayLoad/PayLoad.h"
using namespace FileSystem;

namespace NoSqlDb
{
	class checkOutFile
	{
	public:
		void copyToLocal(DbCore<PayLoad>& db,std::string);
	};
	
	//----< Functionality of checkOut >--------------------
	void checkOutFile::copyToLocal(DbCore<PayLoad>& db,std::string fileName)
	{
		DbElement<PayLoad> elem;
		elem = db[fileName];
		std::size_t found = fileName.find_first_of(":");
		std::string x = fileName.substr(0, found);
		std::string filePassingNAme = fileName.substr(found + 2);
		std::string src = "../SaveFiles/" + fileName.substr(found + 2);
		std::string y = "../RemoteRepo/" + x + "/" + fileName.substr(found + 2);
		found = y.find_last_of(".");
		std::string dest = y.substr(0, found);
		bool test = Directory::exists("../RemoteRepo/" + x);
		if (test==0){ Directory::create("../RemoteRepo/" + x); }
		bool value = File::copy(src, dest);
		if (value ==1)
		{ std::cout << "\n CheckOut Done For file:" << filePassingNAme;}
		else
		{std::cout << "\n couldn't Perform Checkout for file" << filePassingNAme;}
		
	}

}
