﻿using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("EntityFramework.MappingAPI")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("EntityFramework.MappingAPI")]
[assembly: AssemblyCopyright("Copyright ©  2014")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("79ca2e96-9fed-4351-975c-6009c381c3a8")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
#if EF4
[assembly: AssemblyVersion("4.1.0.2")]
[assembly: AssemblyFileVersion("4.1.0.2")]
#endif

#if EF5
[assembly: AssemblyVersion("5.0.0.2")]
[assembly: AssemblyFileVersion("5.0.0.2")]
#endif

#if EF6
[assembly: AssemblyVersion("6.0.0.2")]
[assembly: AssemblyFileVersion("6.0.0.2")]
#endif
