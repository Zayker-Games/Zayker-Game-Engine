using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace ZEngine.Rendering
{
    /// <summary>
    /// Object send to the render que. 
    /// </summary>
    public class RenderRequest
    {
        public VertexArrayObject vao;
        public Material material;

        public Math.Vector positionInWorldspace;
        public Math.Vector eulerAnglesInWorldspace;
        public Math.Vector scaleInWorldspace;

        public RenderRequest()
        {
            this.positionInWorldspace = new Math.Vector(0f, 0f, 0f);
            this.eulerAnglesInWorldspace = new Math.Vector(0f, 0f, 0f);
            this.scaleInWorldspace = new Math.Vector(1f, 1f, 1f);
        }

        public RenderRequest(VertexArrayObject vao, Material material, Math.Vector positionInWorldspace, Math.Vector eulerAnglesInWorldspace, Math.Vector scaleInWorldspace)
        {
            this.vao = vao;
            this.material = material;

            this.positionInWorldspace = positionInWorldspace;
            this.eulerAnglesInWorldspace = eulerAnglesInWorldspace;
            this.scaleInWorldspace = scaleInWorldspace;
        }

        public bool IsValid()
        {
            if (vao == null)
                return false;

            if (material == null)
                return false;

            return true;
        }
    }
}
