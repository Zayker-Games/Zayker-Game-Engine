using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.DependencyModel;
using Basic.Reference.Assemblies;

namespace ZEngine.Core
{
    /// <summary>
    /// The build-system handles comping the users project into an executable (and other formats in later versions).
    /// </summary>
    class BuildSystem
    {
        public static void BuildFolder(string projectPath)
        {
            var assemblyPath = projectPath + "/Build/" + "Build.exe";
            if (!System.IO.Directory.Exists(projectPath + "/Build/"))
                System.IO.Directory.CreateDirectory(projectPath + "/Build/");
            Compiler compiler = new Compiler(projectPath);

            // Build C# Code
            Console.WriteLine("Compiling C# code...");
            EmitResult r = compiler.Emit(assemblyPath);
            foreach (Diagnostic d in r.Diagnostics)
            {
                Console.WriteLine(d);
            }
            Console.WriteLine(r.Success ? "Copiled C# code successfully!" : "Failed to compile C# code!");

            // Copy asset folder

            // Copy Modules Files (Shaders, etc.)

            // Copy Module System
        }

        public class Compiler
        {
            private CSharpCompilation _compilation;

            public Compiler(string projectPath)
            {
                string[] sourceCodePaths = System.IO.Directory.GetFiles(projectPath, "*.cs", SearchOption.AllDirectories);
                Console.WriteLine("Found " + sourceCodePaths.Length + " files to compile.");
                var syntaxTrees = ParseSyntaxTrees(sourceCodePaths);
                _compilation = CSharpCompilation.Create("Build.exe", syntaxTrees, GetMetadataReference(), GetCompilationOptions());
            }

            

            private static void DirectoryCopy(string sourcePath, string destPath)
            {
                // Get the subdirectories for the specified directory.
                DirectoryInfo dir = new DirectoryInfo(sourcePath);

                if (!dir.Exists)
                {
                    throw new DirectoryNotFoundException(
                        "Source directory does not exist or could not be found: "
                        + sourcePath);
                }

                DirectoryInfo[] dirs = dir.GetDirectories();

                // If the destination directory doesn't exist, create it.       
                Directory.CreateDirectory(destPath);

                // Get the files in the directory and copy them to the new location.
                FileInfo[] files = dir.GetFiles();
                foreach (FileInfo file in files)
                {
                    string tempPath = Path.Combine(destPath, file.Name);
                    file.CopyTo(tempPath, true);
                }

                // If copying subdirectories, copy them and their contents to new location.
                foreach (DirectoryInfo subdir in dirs)
                {
                    string tempPath = Path.Combine(destPath, subdir.Name);
                    DirectoryCopy(subdir.FullName, tempPath);
                }

            }


            public EmitResult Emit(string filePath)
            {
                return _compilation.Emit(filePath);
            }

            private IEnumerable<SyntaxTree> ParseSyntaxTrees(string[] sourceCodePaths)
            {
                return sourceCodePaths.Select(source => CSharpSyntaxTree.ParseText(File.ReadAllText(source)));
            }

            public unsafe IEnumerable<MetadataReference> GetMetadataReference()
            {
                List<PortableExecutableReference> referenceAssemblies = new List<PortableExecutableReference>();

                // Add references for netcoreapp31
                referenceAssemblies.AddRange(ReferenceAssemblies.Get(ReferenceAssemblyKind.NetStandard20).ToList());
                
                // Try getting the version of System.Runtime that is in memory
                PortableExecutableReference coreRef;
                if (System.Reflection.Metadata.AssemblyExtensions.TryGetRawMetadata(typeof(System.Object).Assembly, out var blob, out var length))
                {
                    var md = ModuleMetadata.CreateFromMetadata((IntPtr)blob, length);
                    coreRef = AssemblyMetadata.Create(md).GetReference();
                    //referenceAssemblies.Add(coreRef);
                }
                // Try adding System.Runtime
                //referenceAssemblies.Add(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));
                //referenceAssemblies.Add(MetadataReference.CreateFromFile(Path.Combine(@"C:\Program Files\dotnet\sdk\NuGetFallbackFolder\microsoft.netcore.app\2.1.0\ref\netcoreapp2.1", "System.Runtime.dll")));
                //referenceAssemblies.Add(MetadataReference.CreateFromFile(typeof(System.Console).Assembly.Location));
                // Add silk references
                referenceAssemblies.Add(MetadataReference.CreateFromFile(@"D:\C# Projects\Zayker-Game-Engine\Zayker Game Engine\Zayker Game Engine\bin\Debug\netcoreapp3.1\Silk.NET.Core.dll"));
                referenceAssemblies.Add(MetadataReference.CreateFromFile(@"D:\C# Projects\Zayker-Game-Engine\Zayker Game Engine\Zayker Game Engine\bin\Debug\netcoreapp3.1\Silk.NET.Maths.dll"));
                referenceAssemblies.Add(MetadataReference.CreateFromFile(@"D:\C# Projects\Zayker-Game-Engine\Zayker Game Engine\Zayker Game Engine\bin\Debug\netcoreapp3.1\Silk.NET.OpenGL.dll"));
                referenceAssemblies.Add(MetadataReference.CreateFromFile(@"D:\C# Projects\Zayker-Game-Engine\Zayker Game Engine\Zayker Game Engine\bin\Debug\netcoreapp3.1\Silk.NET.Windowing.Common.dll"));

                foreach (PortableExecutableReference p in referenceAssemblies)
                {
                    Console.WriteLine("   " + p.Display + " - " + p.FilePath);
                }
                Console.WriteLine("");

                // Return all references we added
                return referenceAssemblies;
            }

            private CSharpCompilationOptions GetCompilationOptions()
            {
                return new CSharpCompilationOptions(OutputKind.ConsoleApplication)
                        .WithOverflowChecks(true)
                        .WithOptimizationLevel(OptimizationLevel.Release).WithAssemblyIdentityComparer(AssemblyIdentityComparer.Default);
            }
        }
    }
}
