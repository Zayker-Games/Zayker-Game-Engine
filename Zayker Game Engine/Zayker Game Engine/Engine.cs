using System;
using System.Collections.Generic;
using System.Drawing;

namespace ZEngine
{
    class Engine
    {
        public static string coreDirectory;

        public static SaveData data;

        public delegate void Update(double deltaTime);
        public static event Update OnUpdate;

        private static void Main(string[] args)
        {
            Console.WriteLine("Engine starting...");

            // Load engine data. If there is no data, create default data.
            data = Data.DataHandler.Load<SaveData>(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "savedata.json"));
            if (data == null)
                data = new SaveData();

            // Get core directory
            if (false && System.IO.Directory.GetCurrentDirectory().Contains("netcoreapp3.1"))
            {
                // If we are building the engine, use the working directory
                coreDirectory = System.IO.Directory.GetCurrentDirectory().Substring(0, System.IO.Directory.GetCurrentDirectory().LastIndexOf(@"bin\")) + @"Core\";
            }
            else
            {
                // If we are not, use the current directory (Not tested!)
                coreDirectory = System.IO.Directory.GetCurrentDirectory() + @"\Engine\Core\";
            }

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

            // Create main window
            Rendering.Window mainWindow = Rendering.RendererCore.CreateWindow();

            // Create everything needed to render the island mesh
            Rendering.VertexArrayObject testingVao = Rendering.Primitives.Plane(mainWindow.Gl);
            Rendering.Texture textingTexture = new Rendering.Texture(mainWindow.Gl, System.IO.Path.Combine(Core.ModuleSystem.GetModuleById("renderer_core").GetDirectory(), "BuiltInTextures/uvTest.png"));
            Rendering.Material textingMaterial = new Rendering.Material(mainWindow.GetShader("standard_lit"), textingTexture);
            textingMaterial.transparent = true;

            Rendering.RenderRequest islandRenderRequest = (new Rendering.RenderRequest(
                testingVao,
                textingMaterial,
                new System.Numerics.Vector3(0f, 0f, 0f),
                new System.Numerics.Vector3(0f, 0f, 0f),
                new System.Numerics.Vector3(0.05f, 0.05f, 0.05f)
                ));

            // Create the gui instance for the engines main window and add ui
            Debugging.DebuggerGuiInstance engineGuiInstance = Debugging.Debugger.GetDebuggerGuiInstance(mainWindow);
            engineGuiInstance.AddContainer(new Debugging.FpsViewer());
            engineGuiInstance.AddContainer(new Core.ProjectSystemUi());

            // DeltaTime Stopwatch
            System.Diagnostics.Stopwatch deltaTimeStopwatch = new System.Diagnostics.Stopwatch();
            deltaTimeStopwatch.Start();

            Console.WriteLine("Engine initialized. Entering main loop...");

            while (true)
            {
                // Exit the program loop if the main window was closed
                if (mainWindow == null || mainWindow.window.IsClosing)
                    break;

                if (Core.ProjectSystem.currentProjectSettings.name != "")
                    mainWindow.window.Title = "Z-Engine - " + Core.ProjectSystem.currentProjectSettings.name;
                else
                    mainWindow.window.Title = "Z-Engine - No Project Loaded";

                // Animate and render the island object
                islandRenderRequest.positionInWorldspace.Y = MathF.Sin((float)mainWindow.window.Time) * 0.1f;
                islandRenderRequest.scaleInWorldspace = new System.Numerics.Vector3(1f) * (1f + (MathF.Sin((float)mainWindow.window.Time) * 0.1f));
                islandRenderRequest.eulerAnglesInWorldspace.Y += 0.5f;

                mainWindow.AddToRenderQue(islandRenderRequest);

                // Calculate the delta time
                double dt = deltaTimeStopwatch.Elapsed.TotalSeconds;
                deltaTimeStopwatch.Restart();

                // Invoke the update event
                OnUpdate.Invoke(dt);
            }

            // Save engine data to a file, so we can load it when the engine is reopened
            Data.DataHandler.Save(data, System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "savedata.json"));
        }

        /// <summary>
        /// All user specific data of the engine like settings.
        /// </summary>
        public class SaveData
        {
            public List<string> recentlyLoadedProjects = new List<string>();
        }
    }
}
