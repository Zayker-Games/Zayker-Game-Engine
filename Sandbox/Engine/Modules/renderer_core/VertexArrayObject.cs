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

        public unsafe void Draw(Shader shader, Camera camera, Vector3 positionInWorldspace, Vector3 eulerAnglesInWorldspace, Vector3 scaleInWorldspace)
        {
            //Bind the geometry and shader.
            _gl.BindVertexArray(_handle); // We already bound this ealier

            shader.Use();

            // Generate transform matrices
            var model = Matrix4x4.CreateTranslation(positionInWorldspace) *
                       (Matrix4x4.CreateRotationZ(Core.Math.DegreesToRadians(eulerAnglesInWorldspace.Z)) * 
                        Matrix4x4.CreateRotationY(Core.Math.DegreesToRadians(eulerAnglesInWorldspace.Y)) * 
                        Matrix4x4.CreateRotationX(Core.Math.DegreesToRadians(eulerAnglesInWorldspace.X)) *
                        Matrix4x4.CreateScale(scaleInWorldspace)
                        );
            var view = Matrix4x4.CreateLookAt(camera.position, camera.position + camera.forwards, camera.up);
            var projection = Matrix4x4.CreatePerspectiveFieldOfView(Core.Math.DegreesToRadians(camera.fov), camera.aspectRatio, 0.1f, 100.0f);

            // Set matrices for transform
            shader.SetUniform("uModel", model);
            shader.SetUniform("uView", view);
            shader.SetUniform("uProjection", projection);

            // Set uvs
            if(uvData.Length > 0) { 
                uint uvBuffer;
                _gl.GenBuffers(1, &uvBuffer);
                _gl.BindBuffer(BufferTargetARB.ArrayBuffer, uvBuffer);
                fixed (void* d = uvData)
                {
                    _gl.BufferData(BufferTargetARB.ArrayBuffer, (uint)(sizeof(float) * uvData.Length), d, GLEnum.StaticDraw);
                }

                _gl.BindBuffer(BufferTargetARB.ArrayBuffer, uvBuffer);
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
        /// <summary>
        /// Load a mesh from a .obj file at a given path. Returns a VertexArrayObject containing this meshs data.
        /// </summary>
        public static VertexArrayObject LoadObjFile(GL gl, string path)
        {
            // List conaining every line of the given obj file.
            List<string> lines = new List<string>();

            // Save each line of the obj file into a list for further iterating. 
            System.IO.StreamReader file = new System.IO.StreamReader(path);
            string line;
            while ((line = file.ReadLine()) != null)
                lines.Add(line);
            file.Close();

            // Define variables for the data we will send to the VAO at the end.
            List<float> uvData = new List<float>();
            List<float> vertices = new List<float>();
            List<uint> indices = new List<uint>();


            // Load raw uv coordinates in the order they are present in the .obj file
            // This is important, because the .obj file referes to these in the faces, 
            // while we need them to be in a list.
            List<Vector2> rawObjuvData = new List<Vector2>();
            foreach (string l in lines.Where(x => x.Substring(0, 2) == "vt"))
            {
                string[] formated = l.Substring(3).Split(" ");
                rawObjuvData.Add(new Vector2(float.Parse(formated[0], CultureInfo.InvariantCulture), float.Parse(formated[1], CultureInfo.InvariantCulture)));
            }

            // Load raw vertex positions. The raw vertex positions are the vertices as they are present in the obj file.
            // We need this list for easier and faster access, when we create our own vertex list.
            List<float> rawObjVertices = new List<float>();
            foreach (string l in lines.Where(x => x.Substring(0, 2) == "v "))
            {
                string[] formated = l.Substring(2).Split(" ");
                rawObjVertices.Add(float.Parse(formated[0], CultureInfo.InvariantCulture));
                rawObjVertices.Add(float.Parse(formated[1], CultureInfo.InvariantCulture));
                rawObjVertices.Add(float.Parse(formated[2], CultureInfo.InvariantCulture));
            }


            // Process the data to fit the format in which we hand it to the VertexArrayObject.
            // One main functionality of this is removing shared verices:
            //      Imagine a cube. In an obj file, each corer is one verice. This vertice is shared by multiple triangles.
            //      This means, that one verice can have multiple normals and uvs, which is not possible with this current implementaion.
            //      Therefor this code creates a new verices list, where these shared vertices are cloned, so that each only has 1 uv and normal.
            uint vertexIndex = 0;
            foreach (string l in lines.Where(x => x.Substring(0, 2) == "f "))
            {
                // Split the string into its three sections (one for each vert of the current face/triangle)
                string[] formated = l.Substring(2).Split(" ");

                if (formated.Length != 3)
                    Console.WriteLine("Warning! The renderer only supports triangles at this point! Triangulate your mesh!");

                // Iterate through the pairs of three, which are a triangle. 
                // Then save the indices and uv data in the respective array
                for (int i = 0; i < 3; i++)
                {
                    // Add the refered to vertex to the new list
                    uint rawIndice = (uint.Parse(formated[i].Split("/")[0])-1)*3;
                    vertices.AddRange(new float[3]{ rawObjVertices[(int)rawIndice], rawObjVertices[(int)rawIndice + 1], rawObjVertices[(int)rawIndice + 2] });
                    
                    // Add the index refering to this new vertice
                    indices.Add(vertexIndex);

                    // Add UV Data
                    uvData.Add(rawObjuvData[int.Parse(formated[i].Split("/")[1]) - 1].X);
                    uvData.Add(rawObjuvData[int.Parse(formated[i].Split("/")[1]) - 1].Y);

                    vertexIndex++;
                }
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
            return ModelLoader.LoadObjFile(gl, System.IO.Path.Combine(Core.ModuleSystem.GetModuleById("renderer_core").GetDirectory(), "BuildInMeshes/cube.obj"));
        }
    }
}
