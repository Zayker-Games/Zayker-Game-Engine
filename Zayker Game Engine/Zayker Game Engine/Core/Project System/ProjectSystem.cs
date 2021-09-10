using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ImGuiNET;
using Newtonsoft.Json;

namespace ZEngine.Core
{
    static class ProjectSystem
    {
        public static string currentProjectPath = "";

        public static ProjectSettings currentProjectSettings;

        public static void LoadProject(string projectPath)
        {
            // Return if the given path does not exits
            if ((Path.GetExtension(projectPath) == "" && !Directory.Exists(projectPath)) ||
                (Path.GetExtension(projectPath) != "" && !File.Exists(projectPath)))
            {
                Debugging.Console.WriteToMain("Failed to load project!", "Directory or file does not exist!", Debugging.Console.LogLevel.error);
                return;
            }

            // Return if the path leads to a folder that does not conain a project.meta file
            if (Path.GetExtension(projectPath) == "" && !Directory.GetFiles(projectPath).Where(s => s.Contains("project.meta")).Any())
            {
                Debugging.Console.WriteToMain("Failed to load project!", "Directory is not a project!", Debugging.Console.LogLevel.error);
                return;
            }

            // Return is the path leads to a file but it's not a project.meta file
            if (Path.GetExtension(projectPath) != "" && Path.GetFileName(projectPath) != "project.meta")
            {
                Debugging.Console.WriteToMain("Failed to load project!", "Path does not lead to a project.meta file!", Debugging.Console.LogLevel.error);
                return;
            }

            // Set the current working project directory
            currentProjectPath = projectPath;

            // Save the path to the recently loaded list
            if(!Engine.data.recentlyLoadedProjects.Contains(projectPath))
                Engine.data.recentlyLoadedProjects.Add(projectPath);

            // Limit the "Open Recent" list to a maximum of 5 entries
            while (Engine.data.recentlyLoadedProjects.Count > 5)
                Engine.data.recentlyLoadedProjects.RemoveAt(0);

            // Read project.meta
            string projectMetaPath;
            if (Path.GetExtension(projectPath) == "")
                projectMetaPath = Path.Combine(projectPath, "project.meta");
            else
                projectMetaPath = projectPath;

            ProjectSettings projectSettings = Data.DataModule.Load<ProjectSettings>(projectMetaPath);
            
            currentProjectSettings = projectSettings;

            Debugging.Console.WriteToMain("Successfully loaded project: " + currentProjectSettings.name, "");
        }

        public static void SaveProject()
        {
            if (!String.IsNullOrEmpty(currentProjectPath))
                SaveProject(currentProjectPath);
        }

        public static void SaveProject(string projectPath)
        {
            currentProjectPath = projectPath;

            Data.DataModule.Save(currentProjectSettings, Path.Combine(projectPath, "project.meta"));
        }

        // Copies module to target directory (UNDOES ALL CHANGES TO SOURCE!)
        public static void ReimportAllModulesToProject()
        {
            if(String.IsNullOrEmpty(currentProjectSettings.name))
            {
                Debugging.Console.WriteToMain("Cant import modules if no project is loaded!", "", Debugging.Console.LogLevel.error);
                return;
            }

            Debugging.Console.WriteToMain("Reimporting all included modules to project...", "");

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
            if (String.IsNullOrEmpty(currentProjectSettings.name))
            {
                Debugging.Console.WriteToMain("Cant import core if no project is loaded!", "", Debugging.Console.LogLevel.error);
                return;
            }

            Debugging.Console.WriteToMain("Importing Engine-Core to project...", "");

            if (Directory.Exists(Path.Combine(GetProjectEngineSourcePath(), "Module System")))
                Directory.Delete(Path.Combine(GetProjectEngineSourcePath(), "Module System"), true);
            if (!Directory.Exists(Path.Combine(GetProjectEngineSourcePath(), "Module System")))
                Directory.CreateDirectory(Path.Combine(GetProjectEngineSourcePath(), "Module System"));

            DirectoryCopy(Path.Combine(Engine.coreDirectory, "Module System"), Path.Combine(GetProjectEngineSourcePath(), "Module System"));

            File.Copy(Path.Combine(Engine.coreDirectory, "EngineRuntime.cs"), Path.Combine(GetProjectEngineSourcePath(), "EngineRuntime.cs"), true);
            File.Copy(Path.Combine(Engine.coreDirectory, "Math.cs"), Path.Combine(GetProjectEngineSourcePath(), "Math.cs"), true);
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
