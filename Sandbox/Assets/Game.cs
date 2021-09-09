using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// Placeholder class, that will be replaced by user content when running the game. 
/// </summary>
public static class Game
{
    static ZEngine.Rendering.Window window;
    static ZEngine.Debugging.DebuggerGuiInstance debugger;

    static ZEngine.Rendering.Texture texture;
    static ZEngine.Rendering.Material material;

    static ZEngine.ECS.Entity entity;

    public static void Start()
    {
        window = ZEngine.Rendering.RendererCore.CreateWindow("Sandbox");
        debugger = ZEngine.Debugging.Debugger.GetDebuggerGuiInstance(window);

        // Load the texture and model. Then combine them into a render request, using a material and the default shader. 
        string rendererCoreDirectory = ZEngine.Core.ModuleSystem.GetModule<ZEngine.Rendering.RendererCore>().GetDirectory();
        texture = new ZEngine.Rendering.Texture(window.Gl, rendererCoreDirectory + @"BuiltInTextures/EngineMascotPalette.png");
        material = new ZEngine.Rendering.Material(window.GetBuiltinShader(ZEngine.Rendering.Window.BuiltInShaders.lit), texture);
        
        // Get ecs module reference
        ZEngine.ECS.EntityComponentSystem ecs = (ZEngine.ECS.EntityComponentSystem)ZEngine.Core.ModuleSystem.GetModuleById("ecs");

        // Register Debuggers
        ZEngine.Debugging.StatsContainer stats = new ZEngine.Debugging.StatsContainer(debugger);
        stats.opened = true;
        debugger.AddContainer(stats);
        ZEngine.Debugging.EcsInspector ecsInspector = new ZEngine.Debugging.EcsInspector(debugger);
        ecsInspector.opened = true;
        debugger.AddContainer(ecsInspector);

        // Create an entity
        entity = ecs.AddEntity();

        entity.AddComponent<ZEngine.ECS.Components.Transform>();

        // Add a MeshRenderer and set its properties
        ZEngine.ECS.Components.MeshRenderer renderer = entity.AddComponent<ZEngine.ECS.Components.MeshRenderer>();
        renderer.SetTargetWindow(window);
        renderer.SetVao(ZEngine.Rendering.ModelLoader.LoadObjFile(window.Gl, rendererCoreDirectory + @"BuildInMeshes/EngineMascot.obj"));
        renderer.SetMaterial(material);
        renderer.SetTexture(texture);
    }

    public static void Update(double deltaTime)
    {

        ZEngine.ECS.Components.Transform transform = entity.GetComponent<ZEngine.ECS.Components.Transform>();

        transform.rotation.Y = (float)window.window.Time * 36f;
        transform.position.Y = MathF.Sin((float)window.window.Time) * 0.25f;
    }
}