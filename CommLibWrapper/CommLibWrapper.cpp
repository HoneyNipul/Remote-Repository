/////////////////////////////////////////////////////////////////////
// CommLibWrapper.cpp - Comm object factory                        //
// ver 1.0                                                         //
//                                                                 //
// Source :Jim Fawcett,                                            //
// CSE687-OnLine Object Oriented Design, Spring 2018               //
/////////////////////////////////////////////////////////////////////
#define IN_DLL

#include "CommLibWrapper.h"
#include "../CppCommWithFileXfer/MsgPassingComm/Comm.h"  // definition of create

using namespace MsgPassingCommunication;

DLL_DECL IComm* CommFactory::create(const std::string& machineAddress, size_t port)
{
  std::cout << "\n  using CommFactory to invoke IComm::create";
  return IComm::create(machineAddress, port);
}


