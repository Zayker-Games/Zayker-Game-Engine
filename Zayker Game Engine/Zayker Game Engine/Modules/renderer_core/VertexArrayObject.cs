using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace ZEngine.Rendering
{
    public class VertexArrayObject
    {
        public uint _handle;
        private GL _gl;

        uint vao;
        uint ebo;

        int indicesCound;

        public float[] uvData;

        public unsafe VertexArrayObject(GL gl, float[] vertices, uint[] indices, float[] uvData)
        {
            _gl = gl;

            indicesCound = indices.Length;
            this.uvData = uvData;

            //Creating the vertex array, storing all data. 

            _gl.CreateVertexArrays(1, out vao);
            _gl.BindVertexArray(vao);
            
            //Initializing a vertex buffer that holds the vertex data.
            uint vbo;
            _gl.CreateBuffers(1, out vbo);
            _gl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);
            fixed (void* v = &vertices[0])
            {
                _gl.BufferData(BufferTargetARB.ArrayBuffer, (uint)(vertices.Length * sizeof(uint)), v, BufferUsageARB.StaticDraw); //Setting buffer data.
            }

            //Initializing a element buffer that holds the index data.
            
            _gl.CreateBuffers(1, out ebo); //Creating the buffer.
            _gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, ebo); //Binding the buffer.
            fixed (void* i = &indices[0])
            {
                _gl.BufferData(BufferTargetARB.ElementArrayBuffer, (uint)(indices.Length * sizeof(uint)), i, BufferUsageARB.StaticDraw); //Setting buffer data.
            }

            //Tell opengl how to give the data to the shaders.
            _gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), null);
            _gl.EnableVertexAttribArray(0);



            //_handle = _gl.GenVertexArray();
            _handle = vao;
            Bind();
        }

        public unsafe void Draw(Shader shader, Camera camera)
        {
            //Bind the geometry and shader.
            _gl.BindVertexArray(_handle); // We already bound this ealier

            shader.Use();

            // Generate transform matrices
            var model = Matrix4x4.CreateRotationZ(Core.Math.DegreesToRadians(0)) * 
                        Matrix4x4.CreateRotationY(Core.Math.DegreesToRadians(0)) * 
                        Matrix4x4.CreateRotationX(Core.Math.DegreesToRadians(0));
            var view = Matrix4x4.CreateLookAt(camera.position, camera.position + camera.forwards, camera.up);
            var projection = Matrix4x4.CreatePerspectiveFieldOfView(Core.Math.DegreesToRadians(camera.fov), camera.aspectRatio, 0.1f, 100.0f);

            // Set matrices for transform
            shader.SetUniform("uModel", model);
            shader.SetUniform("uView", view);
            shader.SetUniform("uProjection", projection);

            // Set uvs
            if(uvData.Length > 0) { 
                uint colorbuffer;
                _gl.GenBuffers(1, &colorbuffer);
                _gl.BindBuffer(BufferTargetARB.ArrayBuffer, colorbuffer);
                fixed (void* d = uvData)
                {
                    _gl.BufferData(BufferTargetARB.ArrayBuffer, (uint)(sizeof(float) * uvData.Length), d, GLEnum.StaticDraw);
                }

                _gl.BindBuffer(BufferTargetARB.ArrayBuffer, colorbuffer);
                _gl.VertexAttribPointer(
                    1,                                // attribute. No particular reason for 1, but must match the layout in the shader.
                    2,                                // size
                    VertexAttribPointerType.Float,                         // type
                    false,                         // normalized?
                    0,                                // stride
                    (void*)0                          // array buffer offset
                );

                _gl.EnableVertexAttribArray(1);
            }

            //Draw the geometry.
            _gl.DrawElements(Silk.NET.OpenGL.GLEnum.Triangles, (uint)indicesCound, Silk.NET.OpenGL.GLEnum.UnsignedInt, (void*)0);
        }

        public void Bind()
        {
            _gl.BindVertexArray(_handle);
        }

        public void Dispose()
        {
            _gl.DeleteBuffer(vao);
            _gl.DeleteBuffer(ebo);
            _gl.DeleteVertexArray(_handle);
        }
    }

    public static class Primitives
    {
        public static VertexArrayObject Plane(GL gl)
        {
            float[] vertices =
            {
                //X    Y      Z
                 0.5f,  0.5f, 0.0f,
                 0.5f, -0.5f, 0.0f,
                -0.5f, -0.5f, 0.0f,
                -0.5f,  0.5f, 0.0f
            };

            //Index data, uploaded to the EBO.
            uint[] indices =
            {
                0, 1, 3,
                1, 2, 3
            };

            float[] uvData = {
                1.0f,  1.0f,
                1.0f,  0.0f,
                0.0f,  0.0f,
                0.0f,  1.0f
            };

            return new VertexArrayObject(gl, vertices, indices, uvData);
        }

        public static VertexArrayObject Cube(GL gl)
        {
            float[] vertices =
            {
                //X    Y      Z
                 0.5f,  0.5f, 0.0f,
                 0.5f, -0.5f, 0.0f,
                -0.5f, -0.5f, 0.0f,
                -0.5f,  0.5f, 0.0f
            };

            //Index data, uploaded to the EBO.
            uint[] indices =
            {
                0, 1, 3,
                1, 2, 3
            };

            return new VertexArrayObject(gl, vertices, indices, new float[] { });
        }
    }
}
