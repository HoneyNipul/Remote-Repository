#pragma once
/////////////////////////////////////////////////////////////////////
// version.cpp - Implements all the function of versioning class   //
//                 in versioning.h                                 //
// ver 1.0                                                         //
// Author Name : Honey Shah                                        //
// Source: Dr. Jim Fawcett                                         //
// CSE687 - Object Oriented Design, Spring 2018                    //
/////////////////////////////////////////////////////////////////////
/*
* Package Operations:
* -------------------
*  findMaxOfFile():   Find the latest version of a file 
*  versionIncrement(): Increment the version Number 
*  versionDecrement(): Decrement the version Number 
*
*
*
* Required Files:
* ---------------
* Filsystem.h, Dbcore.h
* PayLoad.h,checkIn.h, checkOut.h,
* browse.h,Dbcore.h

* Maintenance History:
* --------------------
* ver 1.0 : 04 March 2018
* - first release
*/
#include <iomanip>
#include <utility>
#include "../FileSystem-Windows/FileSystemDemo/FileSystem.h"
#include "../DateTime/DateTime.h"
#include "../DbCore/DbCore.h"
#include "../PayLoad/PayLoad.h"
#include "../Query/Query.h"



namespace NoSqlDb
{
	class versioning {
	public:
		std::string  findMaxOfFile(DbCore<PayLoad>& db, std::string fileName, Query<PayLoad>& q);
		std::string versionIncrement( std::string fileName);
		std::string versionDecrement(std::string fileName);

	};
	//----< Decrement the version Number  >--------------------
	std::string versioning::versionDecrement( std::string fileName)
	{
		std::size_t found = fileName.find_last_of("/.");
		std::string path = fileName.substr(0, found);
		std::string num = fileName.substr(found + 1);
		int file = atoi(num.c_str());
		file = file - 1;
		num = std::to_string(file);
		path.append(".");
		path.append(num);
		return path;
	}

	//----< Increment the version Number  >--------------------
	std::string versioning::versionIncrement(std::string fileName)
	{
		std::size_t found = fileName.find_last_of("/.");
		std::string path = fileName.substr(0, found);
		std::string num = fileName.substr(found + 1);
		int file = atoi(num.c_str());
		file = file + 1;
		num = std::to_string(file);
		path.append(".");
		path.append(num);
		return path;
	}
	//----< Find the latest version of a file >--------------------
	std::string versioning::findMaxOfFile(DbCore<PayLoad>& db, std::string, Query<PayLoad>& q)
	{
		int max = 0;
		std::string keyFoundMax = "";
		for (unsigned i = 0; i < q.keys().size(); i++)
		{

			DbElement<PayLoad> elem;
			std::string key = q.keys().at(i);
			elem = db[q.keys().at(i)];
			std::size_t found = key.find_last_of("/.");
			std::string fileTo1 = key.substr(found + 1);
			std::cout << "\n \n";
			int file = atoi(fileTo1.c_str());
			DbElement<PayLoad> elem1;
			if (file > max)
			{
				max = file;
				keyFoundMax = q.keys().at(i);
			}
		}
		return keyFoundMax;
	}


}
