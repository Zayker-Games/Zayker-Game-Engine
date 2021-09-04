using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace ZEngine.Debugging
{
    public abstract class UIEntity
    {
        protected Rendering.RenderRequest renderRequest;

        protected Rendering.Window window;

        /// <summary>
        /// Anchor at which the object is alligned:
        ///   (0,0) -> Center
        ///   (1,1) -> Top Right
        /// </summary>
        private Vector2 anchor = new Vector2(0f);

        public virtual void Draw()
        {

        }

        protected bool IsMouseOver()
        {
            return Silk.NET.OpenGL.Extensions.ImGui;
        }
    }
}
