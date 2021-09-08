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

        public Vector3 positionInWorldspace;
        public Vector3 eulerAnglesInWorldspace;
        public Vector3 scaleInWorldspace;

        public RenderRequest()
        {
            this.positionInWorldspace = new Vector3(0f);
            this.eulerAnglesInWorldspace = new Vector3(0f);
            this.scaleInWorldspace = new Vector3(1f, 1f, 1f);
        }

        public RenderRequest(VertexArrayObject vao, Material material, Vector3 positionInWorldspace, Vector3 eulerAnglesInWorldspace, Vector3 scaleInWorldspace)
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
