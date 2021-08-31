using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace ZEngine.Core
{
    static class ProjectSystem
    {
        public static string currentProjectPath = "";

        public static ProjectSettings currentProjectSettings;

        public static void LoadProject(string projectPath)
        {
            currentProjectPath = projectPath;

            // Read project.meta
            ProjectSettings projectSettings = JsonConvert.DeserializeObject<ProjectSettings>(System.IO.File.ReadAllText(projectPath + "project.meta"));
            
            currentProjectSettings = projectSettings;
        }

        public static void SaveProject()
        {
            if (!String.IsNullOrEmpty(currentProjectPath))
                SaveProject(currentProjectPath);
        }

        public static void SaveProject(string projectPath)
        {
            currentProjectPath = projectPath;

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

        // Copies module to target directory (UNDOES ALL CHANGES TO SOURCE!)
        public static void ReimportAllModulesToProject()
        {
            if (Directory.Exists(GetProjectModulesPath()))
                Directory.Delete(GetProjectModulesPath(), true);
            if (!Directory.Exists(GetProjectModulesPath()))
                Directory.CreateDirectory(GetProjectModulesPath());

            foreach (string moduleId in currentProjectSettings.includedModules)
            {
                ImportModuleToProject(moduleId);
            }
        }

        public static void ImportCoreToProject()
        {
            if (Directory.Exists(Path.Combine(GetProjectEngineSourcePath(), "Module System")))
                Directory.Delete(Path.Combine(GetProjectEngineSourcePath(), "Module System"), true);
            if (!Directory.Exists(Path.Combine(GetProjectEngineSourcePath(), "Module System")))
                Directory.CreateDirectory(Path.Combine(GetProjectEngineSourcePath(), "Module System"));

            DirectoryCopy(Path.Combine(Engine.coreDirectory, "Module System"), Path.Combine(GetProjectEngineSourcePath(), "Module System"));

            File.Copy(Path.Combine(Engine.coreDirectory, "EngineRuntime.cs"), Path.Combine(GetProjectEngineSourcePath(), "EngineRuntime.cs"), true);
        }

        static void ImportModuleToProject(string moduleId)
        {
            
            DirectoryCopy(ModuleSystem.GetModuleById(moduleId).GetDirectory(), Path.Combine(GetProjectModulesPath(), moduleId));
        }

        public static void CloseProject()
        {
            currentProjectPath = "";
        }

        private static string GetProjectEngineSourcePath()
        {
            return Path.Combine(currentProjectPath, "Engine");
        }

        private static string GetProjectModulesPath()
        {
            return Path.Combine(GetProjectEngineSourcePath(), "Modules");
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
    }

    [Serializable]
    public struct ProjectSettings
    {
        public string name;
        public List<string> includedModules;
    }
}
