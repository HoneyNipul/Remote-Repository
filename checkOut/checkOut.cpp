/////////////////////////////////////////////////////////////////////
// checkOut.cpp - Implements all the function of checkOut class    //
//                 in checkOut.h                                   //
// ver 1.0                                                         //
// Author Name : Honey Shah                                        //
// Source: Dr. Jim Fawcett                                         //
// CSE687 - Object Oriented Design, Spring 2018                    //
/////////////////////////////////////////////////////////////////////


#include "checkOut.h"
#include "../Utilities/StringUtilities/StringUtilities.h"

using namespace NoSqlDb;
using namespace FileSystem;

#define TEST_checkOut
#ifdef TEST_checkOut
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
bool createDatabase()
{
	Utilities::title("making demo database");
	Utilities::putline();
	DbCore<PayLoad> db;
	DbProvider dbp;
	dbp.db() = db;
	DbElement<PayLoad> elem = makeElement<PayLoad>("Honey", "Header");
	elem.dateTime(DateTime().now());
	PayLoad pl, p2, p3, p4;
	pl.value("../repository/test");
	pl.categories().push_back("Header c++");
	pl.truthValue("open");
	elem.payLoad(pl);
	db["test::Test.h.1"] = elem;
	DbElement<PayLoad> elem1 = makeElement<PayLoad>("Honey", "C++File");
	elem1.dateTime(DateTime().now());
	p2.value("../repository/test");
	p2.categories().push_back("C++");
	p2.truthValue("open");
	elem1.payLoad(p2);
	db["test::Test.cpp.1"] = elem;
	makeElement<PayLoad>("Honey", "Header");
	elem.dateTime(DateTime().now());
	p3.value("../repository/time");
	p3.categories().push_back("Header c++");
	p3.truthValue("close");
	elem.payLoad(p3);
	db["time::time.h.1"] = elem;
	elem = makeElement<PayLoad>("Honey", "C++File");
	elem.dateTime(DateTime().now());
	p4.value("../repository/time");
	p4.categories().push_back("C++");
	p4.truthValue("close");
	elem.payLoad(p4);
	db["time::time.cpp.1"] = elem;
	dbp.db() = db;
	putLine();
	showDb(db);
	PayLoad p;
	p.showDb(db);
	return true;
}
int main()
{
	Utilities::Title("TestApplication - demonstrates that deployment strategy works");
	Utilities::putline();
	createDatabase();
	DbProvider dbp;
	DbCore<PayLoad> db = dbp.db();
	checkOutFile c;
	c.copyToLocal(db, "Test::test.h.1");

    return 0;
}
#endif
