#pragma once
/////////////////////////////////////////////////////////////////////
// checkIn.h - Implements the checkIn functionality in checkIn     //
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
* - checkIn implements implements all the check-in functionality of a package
* The package  provides functions for checkin files from local to Repository.
*
*findIfFileIsThereInDatabase(DbCore<PayLoad>& db, std::string fileName):Functionality of checking a file
*ParentSearch(DbCore<PayLoad>& db, Keys fileName,std::string):
*fileTransfer(std::string fileName):Functionality of Appending .1 to the file when checked In first time
*findMaxAndCopy(std::string fileName):Functionality of searching a Parent of file
*
* Required Files:
* ---------------
* Filsystem.h, Dbcore.h
* PayLoad.h , Query.h
* versioning,h
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
#include<vector>
#include "../FileSystem-Windows/FileSystemDemo/FileSystem.h"
#include "../DateTime/DateTime.h"
#include "../DbCore/DbCore.h"
#include "../PayLoad/PayLoad.h"
#include "../Query/Query.h"
#include"../version/versioning.h"

using namespace FileSystem;

namespace NoSqlDb
{

	class checkInFile
	{
	public:
		DbCore<PayLoad>findIfFileIsThereInDatabase(DbCore<PayLoad>& db, std::string fileName,std::string desc,std::string status,std::string author,std::string child);
		DbCore<PayLoad>ParentSearch(DbCore<PayLoad>& db, Keys fileName,std::string);
		DbCore<PayLoad> changeCheckInStatus(DbCore<PayLoad>& db, std::string file);
		std::vector<std::string> breakingUpTheString(std::string something);
		bool fileTransfer(std::string fileName);
		bool findMaxAndCopy(std::string fileName);
	};

	//----< Functionality of Appending .1 to the file when checked In first time >--------------------
	bool checkInFile::fileTransfer(std::string fileName)
	{
		std::stringstream ss;
		std::size_t found = fileName.find_first_of(":");
		std::string x = fileName.substr(0, found);
		std::string src = "../SendFiles/"+ fileName.substr(found + 2);
		std::cout << "\n Src = " << src;
		std::string y = "../Storage/" + x + "/" + fileName.substr(found + 2);
		std::cout << "\n DEST = " << y;
		bool test = Directory::exists("../Storage/" + x);
		if (test == 0) { Directory::create("../Storage/" + x); }
		std::string Dest =y.append(".1");
		bool value = File::copy(src, Dest);
		return value;
	}

	bool checkInFile::findMaxAndCopy(std::string fileName)
	{
		std::size_t found = fileName.find_first_of(":");
		std::string x = fileName.substr(0, found);
		std::string dest = "../Storage/" + x + "/" + fileName.substr(found + 2);
		std::string y = "../SendFiles/"  + fileName.substr(found + 2);
		
		found = y.find_last_of(".");
		std::string src = y.substr(0, found);
		bool value = File::copy(src, dest);
		return value;
	}
	std::vector<std::string> checkInFile::breakingUpTheString(std::string justString)
	{
		std::vector<std::string> myvector;
		std::string delimiter = "-";

		size_t pos = 0;
		std::string token;
		while ((pos = justString.find(delimiter)) != std::string::npos) {
			token = justString.substr(0, pos);
			myvector.push_back(token);
			std::cout << token << std::endl;
			justString.erase(0, pos + delimiter.length());
		}
		std::cout << justString << std::endl;
		myvector.push_back(justString);
		return myvector;

	}

	//----< Functionality of checking a file >--------------------
	DbCore<PayLoad> checkInFile::findIfFileIsThereInDatabase(DbCore<PayLoad>& db, std::string fileName,std::string desc,std::string category,std::string author,std::string child)
	{	Query<PayLoad> q(db);
		Conditions<PayLoad> c;
		std::string demo = fileName; c.keyM(fileName); q.select(c).show();
		if (q.keys().empty())
		{  bool value = fileTransfer(fileName);
			if (value)
			{	DbElement<PayLoad> elem = makeElement<PayLoad>(author,desc);PayLoad pl;
				std::size_t found = fileName.find_first_of(":");
				std::string pValue = fileName.substr(0, found); ;
				pl.value("../Storage/" + pValue);
				std::vector<std::string> cat = breakingUpTheString(category);
				pl.categories() = cat;
				pl.truthValue("open");
				elem.payLoad(pl);		
				db[fileName.append(".1")] = elem;
				if(child.length() !=0) db[fileName].children() = breakingUpTheString(child);
			}
			else
			{std::cout << "cannot copy file, File not present";}}
		else
		{   int max = 0;
			std::string fileNameWithVersion;std::string keyFoundMax;versioning v1;
			keyFoundMax = v1.findMaxOfFile(db, fileName, q);
			DbElement<PayLoad> elem,elem1;
			elem = db[keyFoundMax];		
			if (desc.length() != 0) elem.descrip(desc);
			if (author.length()!= 0) elem.name(author);
			if(category.length() != 0) elem.payLoad().categories() = breakingUpTheString(category);
			db[keyFoundMax] = elem;
			if (child.length() != 0) db[keyFoundMax].children() = breakingUpTheString(child);		
			if (elem.payLoad().truthValue()=="open")
			{  std::size_t found = keyFoundMax.find_first_of(":");
				std::string x = keyFoundMax.substr(found + 2);
				std::cout << "Checking status found to be Open appending to the Existing Version : " << x;
				bool value = findMaxAndCopy(keyFoundMax);	
			}
			else /*if(elem.payLoad().truthValue() == "close")*/
			{ versioning v1,v2;
				std::string path =v1.versionIncrement(keyFoundMax);
				bool value = findMaxAndCopy(path);
				std::string oldPath = v2.versionDecrement(path);
				Keys keys{ oldPath };
				db = ParentSearch(db, keys, path);
				elem.name("Honey");
				elem.payLoad().truthValue("open");
				db[path] = elem;
			}
			std::cout << '\n';}
		return db;
	}

	//----< Functionality of searching a Parent of file >--------------------
	DbCore<PayLoad> checkInFile::ParentSearch(DbCore<PayLoad>& db, Keys fileName,std::string newPath)
	{
		Query<PayLoad> q1(db);
		Conditions<PayLoad> conds0;
		conds0.children(fileName);
		q1.select(conds0);
		for (unsigned i = 0; i < q1.keys().size(); i++)
		{
			std::string key = q1.keys().at(i);
			db[key].children().push_back(newPath);
		}
		return db;
	}

	DbCore<PayLoad> checkInFile::changeCheckInStatus(DbCore<PayLoad>& db, std::string file)
	{
		Query<PayLoad> q(db);
		Conditions<PayLoad> c;
		std::string demo = file;
		c.keyM(file);
		q.select(c);
		int max = 0, count = 0;
		std::string fileNameWithVersion, keyFoundMax, fileTo1, key;
		for (unsigned i = 0; i < q.keys().size(); i++)
		{
			key = q.keys().at(i);
			std::size_t found = key.find_last_of("/.");
			fileTo1 = key.substr(found + 1);
			int file = atoi(fileTo1.c_str());
			DbElement<PayLoad> elem1;
			if (file > max)
			{
				max = file;
				keyFoundMax = q.keys().at(i);
			}
		}
		DbElement<PayLoad> elem, childElem;
		elem = db[keyFoundMax];
		Children children = elem.children();
		if (children.size() > 0)
		{
			for (auto key : children)
			{
				childElem = db[key];
				if (childElem.payLoad().truthValue() == "close" || childElem.payLoad().truthValue() == "checkInPending")
				{
					count++;
				}
			}
		}
		if (count == children.size())
		{
			elem.payLoad().truthValue("close");
			std::cout << "\n changing the CheckIn Status to \"Close\" for : " << file;
			db[keyFoundMax] = elem;
		}
		else
		{
			elem.payLoad().truthValue("checkInPending");
			std::cout << "\n Changing the CheckIn Status to \"CheckInPending\" \n";
			db[keyFoundMax] = elem;
		}
		return db;
	}
}

	