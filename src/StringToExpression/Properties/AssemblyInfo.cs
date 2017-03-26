using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
 [assembly: AssemblyConfiguration("Release")]
#endif

[assembly: AssemblyTitle("StringToExpression")]
[assembly: AssemblyDescription(@"StringToExpression supports the compiling a domain specific string into a .NET expression.

Out of the box configuration is provided for parsing arithmetic expressions and for parsing OData filter strings. Although can be configured to parse string of any format")]
[assembly: AssemblyCompany("Codecutout")]
[assembly: AssemblyProduct("StringToExpression")]
[assembly: AssemblyCopyright("Copyright ©  2017")]

[assembly: ComVisible(false)]
[assembly: Guid("b1b0a26f-04e2-40e5-b25c-12764e50befc")]

[assembly: AssemblyVersion("0.1.0")]
[assembly: AssemblyFileVersion("0.1.0")]
[assembly: InternalsVisibleTo("StringToExpression.Test")]

