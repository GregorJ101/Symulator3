// This is the main DLL file.

#include "stdafx.h"

#include "GetMscVersionLib.h"

namespace GetMscVersionLib
{
    System::Int32 CGetMscVersionLib::GetCompilerVersionStatic ()
    {
        //Console::WriteLine ("In CGetMscVersionLib::GetCompilerVersionStatic ()");
        return _MSC_VER;
    }

    System::Int32 CGetMscVersionLib::GetCompilerFullVersionStatic ()
    {
        //Console::WriteLine ("In CGetMscVersionLib::GetCompilerFullVersionStatic ()");
        return _MSC_FULL_VER;
    }

    System::Int32 CGetMscVersionLib::GetCompilerVersion ()
    {
        //Console::WriteLine ("In CGetMscVersionLib::GetCompilerVersion ()");
        return _MSC_VER;
    }

    System::Int32 CGetMscVersionLib::GetCompilerFullVersion ()
    {
        //Console::WriteLine ("In CGetMscVersionLib::GetCompilerFullVersion ()");
        return _MSC_FULL_VER;
    }
}
