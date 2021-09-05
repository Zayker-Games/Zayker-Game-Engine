using System;
using System.Collections.Generic;
using System.Drawing;

namespace ZEngine
{
    class Engine
    {
        /// <summary>
        /// Directory to the "Modules"-Folder. This is inside the engine folder for the editor and inside the project folder for the game. 
        /// </summary>
        //public static string modulesDirectory;
        public static string coreDirectory;

        public delegate void Update(double deltaTime);
        public static event Update OnUpdate;

        private static void Main(string[] args)
        {
            // Get modulesDirectory
            if (false && System.IO.Directory.GetCurrentDirectory().Contains("netcoreapp3.1"))
            {
                // If we are building the engine, use the working directory
                //modulesDirectory = System.IO.Directory.GetCurrentDirectory().Substring(0, System.IO.Directory.GetCurrentDirectory().LastIndexOf(@"bin\")) + @"Modules\";
                coreDirectory = System.IO.Directory.GetCurrentDirectory().Substring(0, System.IO.Directory.GetCurrentDirectory().LastIndexOf(@"bin\")) + @"Core\";
            }
            else
            {
                // If we are not, use the current directory (Not tested!)
                //modulesDirectory = System.IO.Directory.GetCurrentDirectory() + @"\Modules\";
                coreDirectory = System.IO.Directory.GetCurrentDirectory() + @"\Engine\Core\";
            }

            Console.WriteLine("Engine starting...");

            // Add event for update loop
            OnUpdate += Core.ModuleSystem.Update;

            // Enable modules
            Core.ModuleSystem.Initialize();
            Core.ModuleSystem.EnableModule("input");
            Core.ModuleSystem.EnableModule("renderer_core");
            Core.ModuleSystem.EnableModule("ecs");
            Core.ModuleSystem.EnableModule("debugger");

            // Get reference to renderer module
            Rendering.RendererCore renderer = (Rendering.RendererCore)(Core.ModuleSystem.GetModuleById("renderer_core"));

            // Create testing window
            Rendering.Window mainWindow = Rendering.RendererCore.CreateWindow();

            // Test saving/loading system
            Core.ProjectSystem.LoadProject(@"D:\C# Projects\Zayker-Game-Engine\Sandbox\");

            Core.ProjectSystem.ImportCoreToProject();
            Core.ProjectSystem.ReimportAllModulesToProject();

            // Test input module
            Input.Input.OnKeyDown += delegate (Silk.NET.Input.IKeyboard arg1, Silk.NET.Input.Key arg2, int arg3) { Console.WriteLine("↓" + arg2); };
            Input.Input.OnKeyUp += delegate (Silk.NET.Input.IKeyboard arg1, Silk.NET.Input.Key arg2, int arg3) { Console.WriteLine("↑" + arg2); };

            // Create everything needed to render the island mesh
            //Rendering.VertexArrayObject islandVao = Rendering.ModelLoader.LoadObjFile(mainWindow.Gl, System.IO.Path.Combine(Core.ModuleSystem.GetModuleById("renderer_core").GetDirectory(), "BuildInMeshes/EngineMascot.obj"));
            Rendering.VertexArrayObject islandVao = Rendering.Primitives.Plane(mainWindow.Gl);
            Rendering.Texture islandTexture = new Rendering.Texture(mainWindow.Gl, System.IO.Path.Combine(Core.ModuleSystem.GetModuleById("renderer_core").GetDirectory(), "BuiltInTextures/uvTest.png"));
            Rendering.Material islandMaterial = new Rendering.Material(mainWindow.GetShader("standard_lit"), islandTexture);
            islandMaterial.transparent = true;

            Rendering.RenderRequest islandRenderRequest = (new Rendering.RenderRequest(
                islandVao,
                islandMaterial,
                new System.Numerics.Vector3(0f, 0f, 0f),
                new System.Numerics.Vector3(0f, 0f, 0f),
                new System.Numerics.Vector3(0.05f, 0.05f, 0.05f)
                ));

            Debugging.DebuggerGuiInstance debuggerGuiInstance = Debugging.Debugger.GetDebuggerGuiInstance(mainWindow);
            debuggerGuiInstance.AddContainer(new Debugging.FpsViewer());

            Console.WriteLine("Engine initialized. Entering main loop...");

            // DeltaTime Stopwatch
            System.Diagnostics.Stopwatch deltaTimeStopwatch = new System.Diagnostics.Stopwatch();
            deltaTimeStopwatch.Start();

            while (true)
            {
                // This check should not be needed. I will move some of this into the window class later. 
                if (mainWindow != null)
                {
                    if (mainWindow.window.IsClosing)
                    {
                        mainWindow = null;
                    }
                    else
                    {
                        // Animate the island object
                        islandRenderRequest.positionInWorldspace.X = 0;
                        islandRenderRequest.positionInWorldspace.Z = 0;
                        islandRenderRequest.positionInWorldspace.Y = MathF.Sin((float)mainWindow.window.Time) * 0.1f;
                        islandRenderRequest.scaleInWorldspace = new System.Numerics.Vector3(1f) * (1f + (MathF.Sin((float)mainWindow.window.Time) * 0.1f));
                        islandRenderRequest.eulerAnglesInWorldspace.Y += 0.5f;

                        mainWindow.AddToRenderQue(islandRenderRequest);
                    }
                }

                // Calculate the delta time
                double dt = deltaTimeStopwatch.Elapsed.TotalSeconds;
                deltaTimeStopwatch.Restart();

                // Invoke the update event
                OnUpdate.Invoke(dt);
            }
        }
    }
}
