using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Zayker_Game_Engine.Core.Project_System
{
    static class ProjectSystem
    {
        public static void LoadProject(string projectPath)
        {
            // Read project.meta
            ProjectSettings projectSettings = JsonSerializer.Deserialize<ProjectSettings>(projectPath + "project.meta");

            // Apply the gathered information
            foreach (string moduleId in projectSettings.includedModules)
            {
                Core.EngineModules.EngineModuleSystem.EnableModule(moduleId);
            }
        }

        public static void SaveProject(string projectPath)
        {
            ProjectSettings projectSettings = new ProjectSettings();

            // Apply the gathered information
            foreach (EngineModules.EngineModule module in EngineModules.EngineModuleSystem.modules)
            {
                if (module.isEnabled)
                    projectSettings.includedModules.Add(module.id);
            }
            string jsonString = JsonSerializer.Serialize(projectSettings);
            Console.WriteLine(jsonString);
            System.IO.File.WriteAllText(projectPath + "project.meta", jsonString);
        }

        public static void CloseProject()
        {

        }
    }

    class ProjectSettings
    {
        public string name = "Unnamed Project";
        public List<string> includedModules = new List<string>();
    }
}
