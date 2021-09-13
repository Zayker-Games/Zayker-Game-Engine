using ImGuiNET;
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

        public static Debugging.GuiInstance engineGuiInstance;

        private static void Main(string[] args)
        {
            Debugging.Console.WriteToMain("Engine starting...", "");

            // Load engine data. If there is no data, create default data.
            data = Data.DataModule.Load<SaveData>(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "savedata.json"));
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
            Rendering.RenderingModule renderer = Core.ModuleSystem.GetModule<Rendering.RenderingModule>();

            // Create main window
            Rendering.Window mainWindow = Rendering.RenderingModule.CreateWindow();

            // Create everything needed to render the island mesh
            Rendering.VertexArrayObject testingVao = Rendering.ModelLoader.LoadObjFile(mainWindow.Gl, System.IO.Path.Combine(Core.ModuleSystem.GetModule<Rendering.RenderingModule>().GetDirectory(), "BuiltInMeshes/axisTest.obj"));
            Rendering.Texture textingTexture = new Rendering.Texture(mainWindow.Gl, System.IO.Path.Combine(Core.ModuleSystem.GetModule<Rendering.RenderingModule>().GetDirectory(), "BuiltInTextures/EngineMascotPalette.png"));
            Rendering.Material textingMaterial = new Rendering.Material(mainWindow.GetBuiltinShader(Rendering.Window.BuiltInShaders.lit), textingTexture);
            textingMaterial.transparent = true;

            Rendering.RenderRequest islandRenderRequest = (new Rendering.RenderRequest(
                testingVao,
                textingMaterial,
                new Math.Vector(0f, 0f, 0f),
                new Math.Vector(0f, 0f, 0f),
                new Math.Vector(1f, 1f, 1f)
                ));

            // Create the gui instance for the engines main window and add ui
            engineGuiInstance = Debugging.DebuggingModule.GetDebuggerGuiInstance(mainWindow);
            engineGuiInstance.AddContainer(new Debugging.StatsContainer(engineGuiInstance));
            engineGuiInstance.AddContainer(new EngineUi(engineGuiInstance, 0));
            engineGuiInstance.AddContainer(new ModuleSystemUi(engineGuiInstance));

            // Create a console and set it to be the main console
            Debugging.Console console = new Debugging.Console(engineGuiInstance, 0);
            engineGuiInstance.AddContainer(console);
            Debugging.Console.main = console;

            // DeltaTime Stopwatch
            System.Diagnostics.Stopwatch deltaTimeStopwatch = new System.Diagnostics.Stopwatch();
            deltaTimeStopwatch.Start();

            ImGui.LoadIniSettingsFromDisk("engineGuiLayout.ini");

            Debugging.Console.WriteToMain("Engine initialized.", "Entering main loop...");

            Math.Vector v = new Math.Vector(0f, 0f, 1f);
            Math.Quaternion q = Math.Quaternion.FromEulerAngles(0f, 90f, 0f);
            Math.Vector rv = q * v;

            while (true)
            {
                // Exit the program loop if the main window was closed
                if (mainWindow == null || mainWindow.window.IsClosing)
                    break;

                if (Core.ProjectSystem.currentProjectSettings.name != "")
                    mainWindow.window.Title = "Z-Engine - " + Core.ProjectSystem.currentProjectSettings.name;
                else
                    mainWindow.window.Title = "Z-Engine - No Project Loaded";

                mainWindow.camera.position = new Math.Vector(0.1f, 0.1f, 3f);
                mainWindow.camera.up = Math.Vector.Up;
                mainWindow.camera.forwards = Math.Quaternion.FromEulerAngles(0, (float)mainWindow.window.Time * 25f, 0f) * Math.Vector.Forwards;
                //mainWindow.camera.position.x = System.MathF.Sin((float)mainWindow.window.Time);

                mainWindow.AddToRenderQue(islandRenderRequest);

                // Calculate the delta time
                double dt = deltaTimeStopwatch.Elapsed.TotalSeconds;
                deltaTimeStopwatch.Restart();

                // Invoke the update event
                OnUpdate.Invoke(dt);
            }

            // Save engine data to a file, so we can load it when the engine is reopened
            Data.DataModule.Save(data, System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "savedata.json"));
            ImGui.SaveIniSettingsToDisk("engineGuiLayout.ini");
        }

        /// <summary>
        /// All user specific data of the engine like settings.
        /// </summary>
        public class SaveData
        {
            public List<string> recentlyLoadedProjects = new List<string>();
        }
    }

    /// <summary>
    /// Container handeling most of the engines ui.
    /// </summary>
    public class EngineUi : Debugging.Container
    {
        public EngineUi(Debugging.GuiInstance debugger, int id = int.MinValue)
        {
            if (id == int.MinValue)
                base.Init(debugger);
            else
                base.Init(debugger, id);

            name = "Engine";
        }

        private bool showProjectLoadScreen = false;
        private string pathToLoadInput = "";
        
        uint dockspace_id = ImGui.GetID("MyDockspace");

        public override void Update(float dt)
        {
            // Enable docking
            ImGui.GetIO().ConfigFlags = ImGuiConfigFlags.DockingEnable | ImGuiConfigFlags.NavEnableKeyboard;

            // Main Dockspace of the engines ui
            ImGuiViewportPtr viewport = ImGui.GetMainViewport();
            ImGui.SetNextWindowPos(viewport.GetWorkPos());
            ImGui.SetNextWindowContentSize(viewport.GetWorkSize());
            ImGui.SetNextWindowViewport(viewport.ID);
            ImGui.SetNextWindowBgAlpha(0f);

            ImGuiWindowFlags window_flags = ImGuiWindowFlags.NoDocking;
            window_flags |= ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove;
            window_flags |= ImGuiWindowFlags.NoBringToFrontOnFocus | ImGuiWindowFlags.NoNavFocus;

            // Set StyleVars for dockspace, create the dockspace window and then pop the 4 set style vars
            ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0.0f);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.0f);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new System.Numerics.Vector2(0f, 0f));
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new System.Numerics.Vector2(0.0f, 0.0f));
            ImGui.Begin("Main Dockspac##" + id, window_flags);
            ImGui.PopStyleVar(4);

            // Create the actuall dockspace
            ImGuiDockNodeFlags dockspace_flags = ImGuiDockNodeFlags.PassthruCentralNode;
            ImGui.DockSpace(dockspace_id, new System.Numerics.Vector2(0.0f, 0.0f), dockspace_flags);

            ImGui.End();

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
                    if (ImGui.MenuItem("Reimport Everything", !System.String.IsNullOrEmpty(Core.ProjectSystem.currentProjectSettings.name)))
                    {
                        Core.ProjectSystem.ImportCoreToProject();
                        Core.ProjectSystem.ReimportAllModulesToProject();
                    }


                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("Tools"))
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

            // Shows the loading screen to load a projec ty path
            if (showProjectLoadScreen)
            {
                ImGui.SetNextWindowSizeConstraints(new System.Numerics.Vector2(400, 50), new System.Numerics.Vector2(1000, 100));
                ImGui.Begin("Load##" + id, ref showProjectLoadScreen, ImGuiWindowFlags.AlwaysAutoResize);

                ImGui.SetNextItemWidth(ImGui.GetWindowContentRegionWidth() * 0.75f);
                ImGui.InputText("##LoadProjectPathInput" + id, ref pathToLoadInput, 500);
                ImGui.SameLine();
                ImGui.SetNextItemWidth(ImGui.GetWindowContentRegionWidth() * 0.25f);
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
        public ModuleSystemUi(Debugging.GuiInstance debugger)
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

                if (!System.String.IsNullOrEmpty(Core.ProjectSystem.currentProjectSettings.name))
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
