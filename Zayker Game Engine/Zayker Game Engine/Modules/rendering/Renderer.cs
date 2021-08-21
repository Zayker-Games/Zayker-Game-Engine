using System.Collections.Generic;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System;
using Silk.NET.Input;

namespace Zayker_Game_Engine.Rendering
{
    class RendererCore : Core.Module
    {
        /// <summary>
        /// List of all windows. 
        /// </summary>
        public List<Window> windows = new List<Window>();


        public RendererCore()
        {
            this.id = "renderer_core";
        }

        public override void OnEnable()
        {
            base.OnEnable();
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            foreach (Window window in windows)
            {
                window.window.DoUpdate();
                window.window.DoEvents();
                window.window.DoRender();
            }
        }

        public Window CreateWindow()
        {
            Window window = new Window();
            windows.Add(window);
            return window;
        }

        
    }

    public class Window
    {
        public IWindow window;
        private static GL Gl;
        Dictionary<string, uint> shaders = new Dictionary<string, uint>();

        private static uint VaoA;
        private static uint VaoB;

        //Vertex data, uploaded to the VBO.
        private static readonly float[] VerticesA =
        {
            //X    Y      Z
             0.5f,  0.9f, 0.0f,
             0.5f,  0.1f, 0.0f,
            -0.5f,  0.1f, 0.0f,
            -0.5f,  0.9f, 0.6f
        };
        private static readonly float[] VerticesB =
        {
            //X    Y      Z
             0.5f, -0.1f, 0.0f,
             0.5f, -0.9f, 0.0f,
            -0.5f, -0.9f, 0.0f,
            -0.5f, -0.1f, 0.6f
        };

        //Index data, uploaded to the EBO.
        private static readonly uint[] Indices =
        {
            0, 1, 3,
            1, 2, 3
        };


        public Window()
        {
            var options = WindowOptions.Default;
            options.Size = new Silk.NET.Maths.Vector2D<int>(500, 500);
            options.Title = "LearnOpenGL with Silk.NET";
            window = Silk.NET.Windowing.Window.Create(options);

            window.Load += OnLoad;
            window.Render += OnRender;
            window.Update += OnUpdate;
            window.Closing += OnClose;
            window.Initialize();
        }

        unsafe uint CreateVertexArrayObject(float[] vertices, uint[] indices)
        {

            //Creating the vertex array, storing all data. 
            uint vao;
            Gl.CreateVertexArrays(1, out vao);
            Gl.BindVertexArray(vao);

            //Initializing a vertex buffer that holds the vertex data.
            uint vbo;
            Gl.CreateBuffers(1, out vbo);
            Gl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);
            fixed (void* v = &vertices[0])
            {
                Gl.BufferData(BufferTargetARB.ArrayBuffer, (uint)(vertices.Length * sizeof(uint)), v, BufferUsageARB.StaticDraw); //Setting buffer data.
            }

            //Initializing a element buffer that holds the index data.
            uint ebo;
            Gl.CreateBuffers(1, out ebo); //Creating the buffer.
            Gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, ebo); //Binding the buffer.
            fixed (void* i = &indices[0])
            {
                Gl.BufferData(BufferTargetARB.ElementArrayBuffer, (uint)(indices.Length * sizeof(uint)), i, BufferUsageARB.StaticDraw); //Setting buffer data.
            }

            //Tell opengl how to give the data to the shaders.
            Gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), null);
            Gl.EnableVertexAttribArray(0);

            // vbo and ebo should be deleted when the program stops. I cant do that right now, because there are no references passed out. 
            // Gl.DeleteBuffer(vbo);
            // Gl.DeleteBuffer(ebo);

            return vao;
        }

        private unsafe void OnLoad()
        {
            //Getting the opengl api for drawing to the screen.
            Gl = GL.GetApi(window);

            // Generate the engines default shader
            GenerateShaderFromFile("default", Program.modulesDirectory + "rendering/BuiltInShaders/BuiltInShader.vert",
                                              Program.modulesDirectory + "rendering/BuiltInShaders/BuiltInShader.frag");

            VaoA = CreateVertexArrayObject(VerticesA, Indices);
            VaoB = CreateVertexArrayObject(VerticesB, Indices);

            // Here we add the callbacks to the input module (if it is enabled)
            IInputContext input = window.CreateInput();
            for (int i = 0; i < input.Keyboards.Count; i++)
            {
                input.Keyboards[i].KeyDown += Input.Input.InvokeKeyDownEvent;
                input.Keyboards[i].KeyUp += Input.Input.InvokeKeyUpEvent;
            }
        }

        private unsafe void OnRender(double obj)
        {
            //Clear the color channel.
            Gl.Clear((uint)ClearBufferMask.ColorBufferBit);

            DrawVertexArrayObject(VaoA);
            DrawVertexArrayObject(VaoB);

            Gl.BindVertexArray(0); // Why? I added this and its not needed. Might just be good practice.
        }

        private unsafe void DrawVertexArrayObject(uint vao)
        {
            //Bind the geometry and shader.
            Gl.BindVertexArray(vao); // We already bound this ealier
            Gl.UseProgram(shaders["default"]);

            //Draw the geometry.
            Gl.DrawElements(GLEnum.Triangles, (uint)Indices.Length, GLEnum.UnsignedInt, (void*)0);
        }

        private void OnUpdate(double obj)
        {

        }

        private void OnClose()
        {
            Gl.DeleteVertexArray(VaoA);
            Gl.DeleteVertexArray(VaoB);

            Silk.NET.Maths.Matrix4X4<float> m = new Silk.NET.Maths.Matrix4X4<float>(1f,1f,1f,1f,1f,1f,1f,1f,1f,1f,1f,1f,1f,1f,1f,1f);
            


            foreach (uint shader in shaders.Values)
            {
                Gl.DeleteProgram(shader);
            }
        }

        void GenerateShaderFromFile(string name, string vertexPath, string fragmentPath)
        {
            GenerateShaderFromSource(name, System.IO.File.ReadAllText(vertexPath), System.IO.File.ReadAllText(fragmentPath));
        }

        /// <summary>
        /// Compiles vertex and fragment shaders into one shader and saves it in the shaders-dictionary.
        /// </summary>
        void GenerateShaderFromSource(string name, string vertexSource, string fragmentSource)
        {
            if (shaders.ContainsKey(name))
                Console.WriteLine("\"Shader with the name \"" + name + "\" already exists! Overwriting!");

            //Creating a vertex shader.
            uint vertexShader = Gl.CreateShader(ShaderType.VertexShader);
            Gl.ShaderSource(vertexShader, vertexSource);
            Gl.CompileShader(vertexShader);

            //Checking the shader for compilation errors.
            string infoLog = Gl.GetShaderInfoLog(vertexShader);
            if (!string.IsNullOrWhiteSpace(infoLog))
            {
                Console.WriteLine($"Error compiling vertex shader {infoLog}");
            }

            //Creating a fragment shader.
            uint fragmentShader = Gl.CreateShader(ShaderType.FragmentShader);
            Gl.ShaderSource(fragmentShader, fragmentSource);
            Gl.CompileShader(fragmentShader);

            //Checking the shader for compilation errors.
            infoLog = Gl.GetShaderInfoLog(fragmentShader);
            if (!string.IsNullOrWhiteSpace(infoLog))
            {
                Console.WriteLine($"Error compiling fragment shader {infoLog}");
            }

            //Combining the shaders under one shader program.
            shaders.Add(name, Gl.CreateProgram());
            Gl.AttachShader(shaders[name], vertexShader);
            Gl.AttachShader(shaders[name], fragmentShader);
            Gl.LinkProgram(shaders[name]);

            //Checking the linking for errors.
            string shader = Gl.GetProgramInfoLog(shaders[name]);
            if (!string.IsNullOrWhiteSpace(shader))
            {
                Console.WriteLine($"Error linking shader {infoLog}");
            }

            //Delete the no longer useful individual shaders;
            Gl.DeleteShader(vertexShader);
            Gl.DeleteShader(fragmentShader);
        }
    }
}