using System.Collections.Generic;
using System.Text;

/// <summary>
/// Placeholder class, that will be replaced by user content when running the game. 
/// </summary>
public static class Game
{
    static ZEngine.Rendering.Window window;
    static ZEngine.Debugging.GuiInstance debugger;

    static ZEngine.Rendering.Texture texture;
    static ZEngine.Rendering.Material material;

    static ZEngine.ECS.Entity entity;

    public static void Start()
    {
        // Create the games main window and get a refernece to the debugging Gui for that window
        window = ZEngine.Rendering.RenderingModule.CreateWindow("Sandbox");
        debugger = ZEngine.Debugging.DebuggingModule.GetDebuggerGuiInstance(window);

        // Load the texture and model. Then combine them into a render request, using a new material and the default shader. 
        string rendererCoreDirectory = ZEngine.Core.ModuleSystem.GetModule<ZEngine.Rendering.RenderingModule>().GetDirectory();
        texture = new ZEngine.Rendering.Texture(window.Gl, rendererCoreDirectory + @"BuiltInTextures/EngineMascotPalette.png");
        material = new ZEngine.Rendering.Material(window.GetBuiltinShader(ZEngine.Rendering.Window.BuiltInShaders.lit), texture);

        // Get a reference to the ECS module
        ZEngine.ECS.ECSModule ecsModule = ZEngine.Core.ModuleSystem.GetModule<ZEngine.ECS.ECSModule>();

        // Create stat viewer
        ZEngine.Debugging.StatsContainer stats = new ZEngine.Debugging.StatsContainer(debugger);
        stats.opened = true;
        debugger.AddContainer(stats);

        // Create ecs inspector
        ZEngine.Debugging.EcsInspector ecsInspector = new ZEngine.Debugging.EcsInspector(debugger);
        ecsInspector.opened = true;
        debugger.AddContainer(ecsInspector);

        // Create an entity
        entity = ecsModule.AddEntity();

        // Add a transform to this entity
        entity.AddComponent<ZEngine.ECS.Components.Transform>().scale = new ZEngine.Math.Vector(3f, 3f, 3f);


        // Add a MeshRenderer to the entity and set its properties
        ZEngine.ECS.Components.MeshRenderer renderer = entity.AddComponent<ZEngine.ECS.Components.MeshRenderer>();
        renderer.SetTargetWindow(window);
        renderer.SetVao(ZEngine.Rendering.ModelLoader.LoadObjFile(window.Gl, rendererCoreDirectory + @"BuildInMeshes/EngineMascot.obj"));
        renderer.SetMaterial(material);
        renderer.SetTexture(texture);

        entity.AddComponent<ZEngine.Physics.RigidBody>();

        // Setup camera
        ZEngine.Rendering.Camera camera = window.camera;
        camera.position.x = 0.0f;
        camera.position.y = 1.0f;
        camera.position.z = 0.0f;
        camera.forwards = new ZEngine.Math.Vector(0f, -0.5f, -1f).normalized;
        camera.fov = 45f;
    }

    public static void Update(double deltaTime)
    {
        // Move player with WASD
        float zInput = 0f;
        if (ZEngine.Input.InputModule.IsKeyDown(Silk.NET.Input.Key.W))
            zInput = -1f;
        else if (ZEngine.Input.InputModule.IsKeyDown(Silk.NET.Input.Key.S))
            zInput = 1f;

        float xInput = 0f;
        if (ZEngine.Input.InputModule.IsKeyDown(Silk.NET.Input.Key.A))
            xInput = -1f;
        else if (ZEngine.Input.InputModule.IsKeyDown(Silk.NET.Input.Key.D))
            xInput = 1f;

        float speed = 0.1f;
        window.camera.position += window.camera.forwards * zInput * speed;

        // Rotate camera
        float mouseX = ZEngine.Input.InputModule.GetMousePos().X / window.window.Size.X;
        mouseX *= 360f;
        float mouseY = -0.5f + (ZEngine.Input.InputModule.GetMousePos().Y / window.window.Size.Y);
        mouseY *= 180f;

        window.camera.forwards = ZEngine.Math.Quaternion.FromEulerAngles(0, -mouseX, -mouseY) * ZEngine.Math.Vector.Forwards;


    }
}