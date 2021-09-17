#region Using directives

using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;

#endregion

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle ("Symulator3GUI")]
[assembly: AssemblyProduct ("Symulator/3 EmulatorEngine")]
[assembly: AssemblyDescription ("Symulator/3 WPF user interface")]
#if _x64
    #if DEBUG
        [assembly: AssemblyConfiguration ("Debug x64")]
    #else
        [assembly: AssemblyConfiguration ("Release x64")]
    #endif
#elif _x86
    #if DEBUG
        [assembly: AssemblyConfiguration ("Debug x86")]
    #else
        [assembly: AssemblyConfiguration ("Release x86")]
    #endif
#elif Win32
    #if DEBUG
        [assembly: AssemblyConfiguration ("Debug Win32")]
    #else
        [assembly: AssemblyConfiguration ("Release Win32")]
    #endif
#else
    #if DEBUG
        [assembly: AssemblyConfiguration ("Debug")]
    #else
        [assembly: AssemblyConfiguration ("Release")]
    #endif
#endif
[assembly: AssemblyCompany ("Sacred Cat Software")]
[assembly: AssemblyCopyright ("Copyright © Sacred Cat Software 2021")]
[assembly: AssemblyTrademark ("SCatSoft")]
[assembly: AssemblyCulture ("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible (false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid ("FC87DA96-768C-4F70-8EF3-AEB80D9E3E4D")]

//In order to begin building localizable applications, set 
//<UICulture>CultureYouAreCodingWith</UICulture> in your .csproj file
//inside a <PropertyGroup>.  For example, if you are using US english
//in your source files, set the <UICulture> to en-US.  Then uncomment
//the NeutralResourceLanguage attribute below.  Update the "en-US" in
//the line below to match the UICulture setting in the project file.

//[assembly: NeutralResourcesLanguage("en-US", UltimateResourceFallbackLocation.Satellite)]


//[assembly: ThemeInfo(
//    ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
//    //(used if a resource is not found in the page, 
//    // or application resource dictionaries)
//    ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
//    //(used if a resource is not found in the page, 
//    // app, or any theme specific resource dictionaries)
//)]


// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Revision and Build Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion ("0.1.2.22")]
[assembly: AssemblyFileVersion ("0.1.1.21")]
