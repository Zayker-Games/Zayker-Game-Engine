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

        public delegate void Update(float deltaTime);
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
            Rendering.VertexArrayObject testVao = Rendering.ModelLoader.LoadObjFile(mainWindow.Gl, System.IO.Path.Combine(Core.ModuleSystem.GetModuleById("renderer_core").GetDirectory(), "BuildInMeshes/EngineMascot.obj"));
            Rendering.Texture testTexture = new Rendering.Texture(mainWindow.Gl, System.IO.Path.Combine(Core.ModuleSystem.GetModuleById("renderer_core").GetDirectory(), "BuiltInTextures/EngineMascotPalette.png"));
            Rendering.Material testMaterial = new Rendering.Material(mainWindow.GetShader("default"), testTexture);
            Rendering.RenderRequest testRenderRequest = new Rendering.RenderRequest(
                testVao, 
                testMaterial, 
                new System.Numerics.Vector3(0f, 0f, 0f),
                new System.Numerics.Vector3(0f, 0f, 0f),
                new System.Numerics.Vector3(1f, 1f, 1f)
                );

            Console.WriteLine("Engine initialized. Entering main loop...");
            while (true)
            {
                testRenderRequest.eulerAnglesInWorldspace.Y += 0.5f;

                if (mainWindow != null)
                {
                    if (mainWindow.window.IsClosing)
                        mainWindow = null;
                    else
                        mainWindow.AddToRenderQue(testRenderRequest);
                }

                OnUpdate.Invoke(0.1f); // TODO: Actuall dt

            }

            testRenderRequest.vao.Dispose();
        }
    }
}
