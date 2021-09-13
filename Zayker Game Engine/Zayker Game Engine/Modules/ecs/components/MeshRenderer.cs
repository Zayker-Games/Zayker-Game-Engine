using System;
using System.Collections.Generic;
using System.Text;

namespace ZEngine.ECS.Components
{
    class MeshRenderer : Component
    {
        private Rendering.Window targetWindow;

        private Rendering.RenderRequest renderRequest;

        public override void _init()
        {
            base._init();

            renderRequest = new Rendering.RenderRequest();
        }

        public override void Update(double deltaTime)
        {
            base.Update(deltaTime);

            Transform t = entity.GetComponent<Transform>();

            if(t != null)
            {
                renderRequest.positionInWorldspace = t.position;
                renderRequest.eulerAnglesInWorldspace = t.localEulerAngles;
                renderRequest.scaleInWorldspace = t.scale;
            }

            targetWindow.AddToRenderQue(renderRequest);
        }

        public void SetVao(Rendering.VertexArrayObject vao)
        {
            this.renderRequest.vao = vao;
        }

        public void SetMaterial(Rendering.Material material)
        {
            renderRequest.material = material;
        }

        public void SetTexture(Rendering.Texture texture)
        {
            renderRequest.material.texture_diffuse = texture;
        }

        public void SetTargetWindow(Rendering.Window window)
        {
            targetWindow = window;
        }

        public override void DrawInspector()
        {
            ImGuiNET.ImGui.LabelText("Window: ", targetWindow.window.Title);
            ImGuiNET.ImGui.LabelText("Material: ", renderRequest.material.ToString());
            ImGuiNET.ImGui.LabelText("Texture: ", renderRequest.material.texture_diffuse.path);
        }
    }
}
