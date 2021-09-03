using Silk.NET.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Text;

namespace ZEngine.Rendering
{
    public class Texture
    {
        private GL _gl;
        private uint _handle;

        public unsafe Texture(GL gl, string path)
        {
            this._gl = gl;

            // Load the image using ImageSharp from the given path
            Image<Rgba32> image = (Image<Rgba32>)Image.Load(path);
            image.Mutate(x => x.Flip(FlipMode.Vertical));

            // 
            fixed (void* data = &System.Runtime.InteropServices.MemoryMarshal.GetReference(image.GetPixelRowSpan(0)))
            {
                //Loading the actual image.
                Load(data, (uint)image.Width, (uint)image.Height);
            }
        }

        private unsafe void Load(void* data, uint width, uint height)
        {
            //Generating the opengl handle;
            _handle = _gl.GenTexture();
            Bind();

            //Setting the data of a texture.
            _gl.TexImage2D(TextureTarget.Texture2D, 0, (int)InternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, data);
            //Setting some texture perameters so the texture behaves as expected.
            _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.Repeat);
            _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.Repeat);
            _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Linear);
            _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Linear);

            //Generating mipmaps.
            _gl.GenerateMipmap(TextureTarget.Texture2D);
        }

        public void Bind(TextureUnit textureSlot = TextureUnit.Texture0)
        {
            //When we bind a texture we can choose which textureslot we can bind it to.
            _gl.ActiveTexture(textureSlot);
            _gl.BindTexture(TextureTarget.Texture2D, _handle);
        }

        public void Dispose()
        {
            //In order to dispose we need to delete the opengl handle for the texure.
            _gl.DeleteTexture(_handle);
        }
    }
}
