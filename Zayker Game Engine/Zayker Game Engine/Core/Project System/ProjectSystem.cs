﻿using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Zayker_Game_Engine.Core.Project_System
{
    static class ProjectSystem
    {
        public static string currentProjectPath = "";

        public static void LoadProject(string projectPath)
        {
            currentProjectPath = projectPath;

            // Read project.meta
            ProjectSettings projectSettings = JsonConvert.DeserializeObject<ProjectSettings>(System.IO.File.ReadAllText(projectPath + "project.meta"));

            // Apply the gathered information
            foreach (string moduleId in projectSettings.includedModules)
            {
                ModuleSystem.EnableModule(moduleId);
            }
        }

        public static void SaveProject()
        {
            if (!String.IsNullOrEmpty(currentProjectPath))
                SaveProject(currentProjectPath);
        }

        public static void SaveProject(string projectPath)
        {
            // Create Settings
            ProjectSettings projectSettings = new ProjectSettings();
            projectSettings.includedModules = new List<string>();

            // Gather information
            foreach (Module module in ModuleSystem.modules)
            {
                if (module.isEnabled)
                {
                    Console.WriteLine(module.id);
                    projectSettings.includedModules.Add(module.id);
                }
            }

            string jsonString = JsonConvert.SerializeObject(projectSettings);
            System.IO.File.WriteAllText(projectPath + "project.meta", jsonString);
        }

        public static void CloseProject()
        {
            currentProjectPath = "";
        }
    }

    [Serializable]
    public struct ProjectSettings
    {
        public string name;
        public List<string> includedModules;
    }
}
