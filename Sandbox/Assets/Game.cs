using System;

public static class Game
{
    public static void Start()
    {
        Console.WriteLine("Start");

        ZEngine.Rendering.RendererCore renderCore = (ZEngine.Rendering.RendererCore)ZEngine.Core.ModuleSystem.GetModuleById("renderer_core");

        renderCore.CreateWindow();
    }

    public static void Update(float deltaTime)
    {
        Console.WriteLine("Update");
    }
}