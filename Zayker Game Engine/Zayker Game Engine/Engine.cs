using ImGuiNET;
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

        public static Debugging.DebuggerGuiInstance engineGuiInstance;

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

            // Enable all modules
            Core.ModuleSystem.Initialize();
            foreach (Core.Module module in Core.ModuleSystem.modules)
            {
                Core.ModuleSystem.EnableModule(module.id);
            }

            // Get reference to renderer module
            Rendering.RendererCore renderer = (Rendering.RendererCore)(Core.ModuleSystem.GetModuleById("renderer_core"));

            // Create main window
            Rendering.Window mainWindow = Rendering.RendererCore.CreateWindow();

            // Create everything needed to render the island mesh
            Rendering.VertexArrayObject testingVao = Rendering.Primitives.Plane(mainWindow.Gl);
            Rendering.Texture textingTexture = new Rendering.Texture(mainWindow.Gl, System.IO.Path.Combine(Core.ModuleSystem.GetModuleById("renderer_core").GetDirectory(), "BuiltInTextures/uvTest.png"));
            Rendering.Material textingMaterial = new Rendering.Material(mainWindow.GetBuiltinShader(Rendering.Window.BuiltInShaders.lit), textingTexture);
            textingMaterial.transparent = true;

            Rendering.RenderRequest islandRenderRequest = (new Rendering.RenderRequest(
                testingVao,
                textingMaterial,
                new System.Numerics.Vector3(0f, 0f, 0f),
                new System.Numerics.Vector3(0f, 0f, 0f),
                new System.Numerics.Vector3(0.05f, 0.05f, 0.05f)
                ));

            // Create the gui instance for the engines main window and add ui
            engineGuiInstance = Debugging.Debugger.GetDebuggerGuiInstance(mainWindow);
            engineGuiInstance.AddContainer(new Debugging.StatsContainer(engineGuiInstance));
            engineGuiInstance.AddContainer(new ProjectSystemUi(engineGuiInstance));
            engineGuiInstance.AddContainer(new ModuleSystemUi(engineGuiInstance));

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

    // To be renamed!
    public class ProjectSystemUi : Debugging.Container
    {
        public ProjectSystemUi(Debugging.DebuggerGuiInstance debugger)
        {
            base.Init(debugger);
            name = "Project System";
        }

        private bool showProjectLoadScreen = false;
        private string pathToLoadInput = "";
        public override void Update(float dt)
        {
            // Draw top bar
            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenu("Project"))
                {
                    if (ImGui.MenuItem("Open"))
                    {
                        showProjectLoadScreen = true;
                    }
                    if (ImGui.BeginMenu("Open Recent"))
                    {
                        foreach (string path in Engine.data.recentlyLoadedProjects)
                        {
                            if (ImGui.MenuItem(path))
                            {
                                Core.ProjectSystem.LoadProject(path);
                            }
                        }
                        ImGui.EndMenu();
                    }
                    if (ImGui.MenuItem("Save"))
                    {
                        Core.ProjectSystem.SaveProject();
                    }
                    if (ImGui.MenuItem("Reimport Everything", !String.IsNullOrEmpty(Core.ProjectSystem.currentProjectSettings.name)))
                    {
                        Core.ProjectSystem.ImportCoreToProject();
                        Core.ProjectSystem.ReimportAllModulesToProject();
                    }


                    ImGui.EndMenu();
                }

                if(ImGui.BeginMenu("Tools"))
                {
                    foreach (Debugging.Container c in Engine.engineGuiInstance.GetContainers())
                    {
                        if (ImGui.MenuItem(c.name + " - " + c.id))
                        {
                            c.opened = !c.opened;
                        }
                    }

                    ImGui.EndMenu();
                }

                ImGui.EndMainMenuBar();
            }

            if (showProjectLoadScreen)
            {
                ImGui.Begin("Load##" + id, ImGuiWindowFlags.AlwaysAutoResize);

                ImGui.InputText("Path", ref pathToLoadInput, 500);
                if (ImGui.Button("Open Project"))
                {
                    if (pathToLoadInput != "")
                    {
                        Core.ProjectSystem.LoadProject(pathToLoadInput);
                        showProjectLoadScreen = false;
                        pathToLoadInput = "";
                    }
                }

                ImGui.End();
            }
        }
    }

    public class ModuleSystemUi : Debugging.Container
    {
        public ModuleSystemUi(Debugging.DebuggerGuiInstance debugger)
        {
            base.Init(debugger);
            name = "Module Manager";
        }

        public override void Update(float dt)
        {
            ImGui.SetNextWindowSizeConstraints(new System.Numerics.Vector2(250, 300), new System.Numerics.Vector2(500, 1000));

            if (opened)
            {
                ImGui.Begin("Module Manager##" + id, ref opened);

                ImGui.TextWrapped("Set which modules to include into the current project. ");

                if (!String.IsNullOrEmpty(Core.ProjectSystem.currentProjectSettings.name))
                {
                    ImGui.BeginChild("scrolling");
                    foreach (Core.Module module in Core.ModuleSystem.modules)
                    {
                        bool projectAlreadyIncluded = Core.ProjectSystem.currentProjectSettings.includedModules.Contains(module.id);
                        if (ImGui.Button("[" + (projectAlreadyIncluded ? "X" : " ") + "] " + module.id))
                        {
                            if (!projectAlreadyIncluded)
                                Core.ProjectSystem.currentProjectSettings.includedModules.Add(module.id);
                            else
                                Core.ProjectSystem.currentProjectSettings.includedModules.Remove(module.id);
                        }
                    }
                    ImGui.EndChild();
                }
                else
                {
                    ImGui.Separator();
                    ImGui.Text("NO PROJECT LOADED!");
                    ImGui.Separator();
                }
                ImGui.End();
            }
        }
    }
}
