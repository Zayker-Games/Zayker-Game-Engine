﻿using System;
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
                Console.WriteLine("Failed to load project! Directory or file does not exist!");
                return;
            }

            // Return if the path leads to a folder that does not conain a project.meta file
            if (Path.GetExtension(projectPath) == "" && !Directory.GetFiles(projectPath).Where(s => s.Contains("project.meta")).Any())
            {
                Console.WriteLine("Failed to load project! Directory is not a project!");
                return;
            }

            // Return is the path leads to a file but it's not a project.meta file
            if (Path.GetExtension(projectPath) != "" && Path.GetFileName(projectPath) != "project.meta")
            {
                Console.WriteLine("Failed to load project! Path does not lead to a project.meta file!");
                return;
            }

            currentProjectPath = projectPath;

            // Read project.meta
            string projectMetaPath;
            if (Path.GetExtension(projectPath) == "")
                projectMetaPath = Path.Combine(projectPath, "project.meta");
            else
                projectMetaPath = projectPath;
            ProjectSettings projectSettings = JsonConvert.DeserializeObject<ProjectSettings>(File.ReadAllText(projectMetaPath));
            
            currentProjectSettings = projectSettings;

            Console.WriteLine("Successfully loaded project: " + currentProjectSettings.name);
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

    public class ProjectSystemUi : Debugging.Container
    {
        public ProjectSystemUi()
        {
            base.Init();
        }

        public override void Update(float dt)
        {
            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenu("File"))
                {
                    if (ImGui.MenuItem("Load...")) {
                        Console.WriteLine("Please enter the full path to your project.meta file:");
                        ProjectSystem.LoadProject(Console.ReadLine()); 
                    }
                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("Testing"))
                {
                    if (ImGui.MenuItem("Load...")) { }
                    ImGui.EndMenu();
                }

                ImGui.EndMainMenuBar();
            }
        }
    }
}
