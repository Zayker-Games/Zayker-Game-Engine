using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// Placeholder class, that will be replaced by user content when running the game. 
/// </summary>
public static class Game
{
    static ZEngine.Rendering.Window window;

    static ZEngine.Rendering.Texture texture;
    static ZEngine.Rendering.Material material;
    static ZEngine.Rendering.RenderRequest renderRequest;

    public static void Start()
    {

        window = ZEngine.Rendering.RendererCore.CreateWindow();

        // Load the texture and model. Then combine them into a render request, using a material and the default shader. 
        string rendererCoreDirectory = ZEngine.Core.ModuleSystem.GetModuleById("renderer_core").GetDirectory();
        texture = new ZEngine.Rendering.Texture(window.Gl, rendererCoreDirectory + @"BuiltInTextures/EngineMascotPalette.png");
        material = new ZEngine.Rendering.Material(window.GetShader("default"), texture);
        renderRequest = new ZEngine.Rendering.RenderRequest(
            ZEngine.Rendering.ModelLoader.LoadObjFile(window.Gl, rendererCoreDirectory + @"BuildInMeshes/EngineMascot.obj"),
            material, 
            new System.Numerics.Vector3(0f, 0f, 0f),
            new System.Numerics.Vector3(0f, 0f, 0f),
            new System.Numerics.Vector3(1f, 1f, 1f)
            );
    }

    public static void Update(float deltaTime)
    {
        // Rotate the renderRequest object
        renderRequest.eulerAnglesInWorldspace.Y += deltaTime * 5f;

        // Send the request we created to the renderer of our window every frame
        window.AddToRenderQue(renderRequest);
    }
}