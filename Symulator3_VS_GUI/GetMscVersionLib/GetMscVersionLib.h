// GetMscVersionLib.h

#pragma once

using namespace System;

namespace GetMscVersionLib
{
	public ref class CGetMscVersionLib
	{
    public:
        static System::Int32 GetCompilerVersionStatic ();
        static System::Int32 GetCompilerFullVersionStatic ();
        System::Int32 GetCompilerVersion ();
        System::Int32 GetCompilerFullVersion ();
	};
}
