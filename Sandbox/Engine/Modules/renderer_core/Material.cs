using System;
using System.Collections.Generic;
using System.Text;

namespace ZEngine.Rendering
{
    public class Material
    {
        public Shader shader;
        public Texture texture_diffuse;
        public bool transparent = false;

        public Material (Shader shader, Texture texture_diffuse)
        {
            this.shader = shader;
            this.texture_diffuse = texture_diffuse;
        }

        /// <summary>
        /// Tell OpenGl to use the shader of this material and set general parameters.
        /// </summary>
        public void Use()
        {
            shader.Use();

            //Bind diffuse texture and and set the uTexture0 to use texture0.
            texture_diffuse.Bind(Silk.NET.OpenGL.TextureUnit.Texture0);
            shader.SetUniform("uTexture0", 0);
        }
    }
}
