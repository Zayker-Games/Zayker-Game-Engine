using System;
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
            Rendering.VertexArrayObject islandVao = Rendering.ModelLoader.LoadObjFile(mainWindow.Gl, System.IO.Path.Combine(Core.ModuleSystem.GetModuleById("renderer_core").GetDirectory(), "BuildInMeshes/EngineMascot.obj"));
            Rendering.Texture islandTexture = new Rendering.Texture(mainWindow.Gl, System.IO.Path.Combine(Core.ModuleSystem.GetModuleById("renderer_core").GetDirectory(), "BuiltInTextures/EngineMascotPalette.png"));
            Rendering.Material islandMaterial = new Rendering.Material(mainWindow.GetShader("standard_lit"), islandTexture);
            Rendering.RenderRequest islandRenderRequest = new Rendering.RenderRequest(
                islandVao, 
                islandMaterial, 
                new System.Numerics.Vector3(0f, 0f, 0f),
                new System.Numerics.Vector3(0f, 0f, 0f),
                new System.Numerics.Vector3(1f, 1f, 1f)
                );

            // Create everything needed to render a screenspace quad
            Rendering.VertexArrayObject screenSpaceQuadVao = Rendering.Primitives.Plane(mainWindow.Gl);
            Rendering.Texture uvTestTexture = new Rendering.Texture(mainWindow.Gl, renderer.GetDirectory() + @"BuiltInTextures/uvTest.png");
            Rendering.Material screenSpaceMaterial = new Rendering.Material(mainWindow.GetShader("screenspace"), uvTestTexture);
            Rendering.RenderRequest screenSpaceRenderRequest = new Rendering.RenderRequest(
                screenSpaceQuadVao,
                screenSpaceMaterial,
                new System.Numerics.Vector3(0f, 0f, 0f),
                new System.Numerics.Vector3(0f, 0f, 0f),
                new System.Numerics.Vector3(1f, 1f, 1f)
                );

            Debugging.Debugger.AddDebuggingUiEntity(new Debugging.Container(mainWindow));

            Console.WriteLine("Engine initialized. Entering main loop...");

            // DeltaTime Stopwatch
            System.Diagnostics.Stopwatch deltaTimeStopwatch = new System.Diagnostics.Stopwatch();
            deltaTimeStopwatch.Start();

            while (true)
            {
                // Animate the island object
                islandRenderRequest.positionInWorldspace.Y = MathF.Sin((float)mainWindow.window.Time) * 0.1f;
                islandRenderRequest.scaleInWorldspace = new System.Numerics.Vector3(1f) * (1f + (MathF.Sin((float)mainWindow.window.Time) * 0.1f));
                islandRenderRequest.eulerAnglesInWorldspace.Y += 0.5f;

                // Scale and position the screenspace object to always take up 50% of the screen width and be alligned to the top right corner
                screenSpaceRenderRequest.scaleInWorldspace = new System.Numerics.Vector3(1f, (float)mainWindow.window.Size.X / (float)mainWindow.window.Size.Y, 1f);
                screenSpaceRenderRequest.positionInWorldspace.X = 1f - ((screenSpaceRenderRequest.scaleInWorldspace.X) / 2f);
                screenSpaceRenderRequest.positionInWorldspace.Y = 1f - ((screenSpaceRenderRequest.scaleInWorldspace.Y) / 2f);

                if (mainWindow != null)
                {
                    if (mainWindow.window.IsClosing)
                    {
                        mainWindow = null;
                    }
                    else
                    {
                        mainWindow.AddToRenderQue(islandRenderRequest);
                        mainWindow.AddToRenderQue(screenSpaceRenderRequest);
                    }
                }

                double dt = deltaTimeStopwatch.Elapsed.TotalSeconds;
                deltaTimeStopwatch.Restart();

                OnUpdate.Invoke(dt);

            }

            islandRenderRequest.vao.Dispose();
        }
    }
}
