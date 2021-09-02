using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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

    public static class ModelLoader
    {
        public static VertexArrayObject LoadObjFile(GL gl, string path)
        {
            List<string> lines = new List<string>();
            string line;

            // Read the file line by line and save each into the lines list
            System.IO.StreamReader file = new System.IO.StreamReader(path);
            while ((line = file.ReadLine()) != null)
            {
                Console.WriteLine(line);
                lines.Add(line);
            }
            file.Close();
            
            List<float> vertices = new List<float>();
            List<uint> indices = new List<uint>();
            List<float> uvData = new List<float>();
            List<float> rawObjuvData = new List<float>();

            // Load raw uv coordinates in the order they are present in the .obj file
            // This is important, because the .obj file referes to these in the faces, 
            // while I need them to be in a list, simmilar to the indicess.
            foreach (string l in lines.Where(x => x.Substring(0, 2) == "vt"))
            {
                string[] formated = l.Substring(3).Split(" ");
                rawObjuvData.Add(float.Parse(formated[0], CultureInfo.InvariantCulture));
                rawObjuvData.Add(float.Parse(formated[1], CultureInfo.InvariantCulture));
                Console.WriteLine(l);
                Console.WriteLine(float.Parse(formated[0], CultureInfo.InvariantCulture));
                Console.WriteLine(float.Parse(formated[1], CultureInfo.InvariantCulture));
            }

            // Load vertex positions
            foreach (string l in lines.Where(x => x.Substring(0, 2) == "v "))
            {
                string[] formated = l.Substring(2).Split(" ");
                vertices.Add(float.Parse(formated[0], CultureInfo.InvariantCulture));
                vertices.Add(float.Parse(formated[1], CultureInfo.InvariantCulture));
                vertices.Add(float.Parse(formated[2], CultureInfo.InvariantCulture));
            }

            // Load indices and uvs
            foreach (string l in lines.Where(x => x.Substring(0, 2) == "f "))
            {
                string[] formated = l.Substring(2).Split(" ");

                if (formated.Length != 3)
                    Console.WriteLine("Warning! The renderer only supports triangles at this point! Triangulate your mesh!");

                uint indice;

                // Indices
                indice = uint.Parse(formated[0].Split("/")[0]) - 1;
                indices.Add(indice);
                indices.Add(uint.Parse(formated[1].Split("/")[0]) - 1);
                indices.Add(uint.Parse(formated[2].Split("/")[0]) - 1);


                // UVs
                int rawUvDataXIndex = (int.Parse(formated[0].Split("/")[1]) - 1) * 2;
                uvData.Add(rawObjuvData[rawUvDataXIndex]);      // x
                uvData.Add(rawObjuvData[rawUvDataXIndex + 1]);  // y
                Console.WriteLine(uint.Parse(formated[0].Split("/")[0]) - 1 + " -> " + rawObjuvData[rawUvDataXIndex] + ", " + rawObjuvData[rawUvDataXIndex + 1]);

                rawUvDataXIndex = (int.Parse(formated[1].Split("/")[1]) - 1) * 2;
                uvData.Add(rawObjuvData[rawUvDataXIndex]);      // x
                uvData.Add(rawObjuvData[rawUvDataXIndex + 1]);  // y
                Console.WriteLine(uint.Parse(formated[1].Split("/")[0]) - 1 + " -> " + rawObjuvData[rawUvDataXIndex] + ", " + rawObjuvData[rawUvDataXIndex + 1]);

                rawUvDataXIndex = (int.Parse(formated[2].Split("/")[1]) - 1) * 2;
                uvData.Add(rawObjuvData[rawUvDataXIndex]);      // x
                uvData.Add(rawObjuvData[rawUvDataXIndex + 1]);  // y
                Console.WriteLine(uint.Parse(formated[2].Split("/")[0]) - 1 + " -> " + rawObjuvData[rawUvDataXIndex] + ", " + rawObjuvData[rawUvDataXIndex + 1]);

            }

            foreach (float f in indices)
            {
                Console.WriteLine("i: " + f);
            }
            foreach (float f in uvData)
            {
                Console.WriteLine("uv: " + f);
            }

            return new VertexArrayObject(gl, vertices.ToArray(), indices.ToArray(), uvData.ToArray());
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
