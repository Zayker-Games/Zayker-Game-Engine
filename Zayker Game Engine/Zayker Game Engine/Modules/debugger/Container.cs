using System;
using System.Collections.Generic;
using System.Text;

namespace ZEngine.Debugging
{
    public class Container : UIEntity
    {
        public Container(Rendering.Window window)
        {
            this.window = window;

            // Create the render request
            renderRequest = new Rendering.RenderRequest(
                Rendering.Primitives.Plane(window.Gl),
                new Rendering.Material(
                    window.GetShader("screenspace"), 
                    new Rendering.Texture(
                        window.Gl, 
                        Core.ModuleSystem.GetModuleById("renderer_core").GetDirectory() + "BuiltInTextures/white.png")
                    ), 
                new System.Numerics.Vector3(), 
                new System.Numerics.Vector3(), 
                new System.Numerics.Vector3(1f, 1f, 1f));
        }

        public override void Draw()
        {
            base.Draw();

            // Update positioning

            // Draw the object onto the screen
            window.AddToRenderQue(renderRequest);
        }
    }
}
