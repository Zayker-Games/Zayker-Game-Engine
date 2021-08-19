using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.DependencyModel;
using Basic.Reference.Assemblies;

namespace Zayker_Game_Engine.Modules.Build_System
{
    /// <summary>
    /// The build-system handles comping the users project into an executable (and other formats in later versions).
    /// </summary>
    class BuildSystem
    {
        public static void BuildFolder(string projectPath)
        {
            var assemblyPath = projectPath + "Build.exe";
            Compiler compiler = new Compiler(projectPath);

            EmitResult r = compiler.Emit(assemblyPath);

            Console.WriteLine();
            foreach (Diagnostic d in r.Diagnostics)
            {
                Console.WriteLine(d);
            }
            Console.WriteLine();
            Console.WriteLine(r.Success ? "Build completed" : "Build Failed");
            Console.ReadLine();
        }

        public class Compiler
        {
            private CSharpCompilation _compilation;

            public Compiler(string projectPath)
            {
                string[] sourceCodePaths = System.IO.Directory.GetFiles(projectPath, "*.cs");
                // TODO: Add all the included module scripts to the sourceCodePaths
                Console.WriteLine("Found " + sourceCodePaths.Length + " files to compile.");
                var syntaxTrees = ParseSyntaxTrees(sourceCodePaths);
                _compilation = CSharpCompilation.Create("Build.exe", syntaxTrees, ReferenceAssemblies.NetStandard20, GetCompilationOptions());
            }

            public EmitResult Emit(string filePath)
            {
                return _compilation.Emit(filePath);
            }

            private IEnumerable<SyntaxTree> ParseSyntaxTrees(string[] sourceCodePaths)
            {
                return sourceCodePaths.Select(source => CSharpSyntaxTree.ParseText(File.ReadAllText(source)));
            }

            public IEnumerable<MetadataReference> GetMetadataReference()
            {
                MetadataReference[] a =
                    DependencyContext.Default.CompileLibraries
                    .SelectMany(cl => cl.ResolveReferencePaths())
                    .Select(asm => MetadataReference.CreateFromFile(asm))
                    .ToArray();
                var assemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location);
                var netstandard = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.GetName().Name == "netstandard").Single();
                MetadataReference[] b = new MetadataReference[] {
                    MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                    MetadataReference.CreateFromFile(netstandard.Location),
                    MetadataReference.CreateFromFile(typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo).Assembly.Location),
                };

                List<MetadataReference> o = a.ToList();
                o.AddRange(b.ToList());

                return new MetadataReference[] { 
                    MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(Console).Assembly.Location)
                };
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
