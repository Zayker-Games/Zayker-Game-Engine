using System.Collections.Generic;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System;
using Silk.NET.Input;

namespace Zayker_Game_Engine.Modules.Renderer
{
    class Renderer : Core.EngineModules.EngineModule
    {
        /// <summary>
        /// List of all windows. 
        /// </summary>
        public List<Window> windows = new List<Window>();


        public Renderer()
        {
            this.id = "engine_renderer";
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
            Window window = new Modules.Renderer.Window();
            windows.Add(window);
            return window;
        }

        
    }

    public class Window
    {
        public IWindow window;
        private static GL Gl;
        Dictionary<string, uint> shaders = new Dictionary<string, uint>();

        private static uint Vbo;
        private static uint Ebo;
        private static uint Vao;

        /*
        //Vertex shaders are run on each vertex.
        private static readonly string VertexShaderSource = @"
        #version 330 core //Using version GLSL version 3.3
        layout (location = 0) in vec4 vPos;
        void main()
        {
            gl_Position = vec4(vPos.x, vPos.y, vPos.z, 1.0);
        }
        ";

        //Fragment shaders are run on each fragment/pixel of the geometry.
        private static readonly string FragmentShaderSource = @"
        #version 400 core
        out vec4 FragColor;
        void main()
        {
            FragColor = vec4(gl_FragCoord.x/500, gl_FragCoord.y/500, 0.0f, 1.0f);
        }
        ";
        */

        //Vertex data, uploaded to the VBO.
        private static readonly float[] Vertices =
        {
            //X    Y      Z
             0.5f,  0.5f, 0.0f,
             0.5f, -0.5f, 0.0f,
            -0.5f, -0.5f, 0.0f,
            -0.5f,  0.5f, 0.6f
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

        private unsafe void OnLoad()
        {

            //Getting the opengl api for drawing to the screen.
            Gl = GL.GetApi(window);

            //Creating a vertex array.
            Gl.CreateVertexArrays(1, out Vao);
            Gl.BindVertexArray(Vao);

            //Initializing a vertex buffer that holds the vertex data.
            Gl.CreateBuffers(1, out Vbo); //Creating the buffer.
            Gl.BindBuffer(BufferTargetARB.ArrayBuffer, Vbo); //Binding the buffer.
            fixed (void* v = &Vertices[0])
            {
                Gl.BufferData(BufferTargetARB.ArrayBuffer, (uint)(Vertices.Length * sizeof(uint)), v, BufferUsageARB.StaticDraw); //Setting buffer data.
            }

            //Initializing a element buffer that holds the index data.
            Gl.CreateBuffers(1, out Ebo); //Creating the buffer.
            Gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, Ebo); //Binding the buffer.
            fixed (void* i = &Indices[0])
            {
                Gl.BufferData(BufferTargetARB.ElementArrayBuffer, (uint)(Indices.Length * sizeof(uint)), i, BufferUsageARB.StaticDraw); //Setting buffer data.
            }

            GenerateShaderFromFile("default", Program.modulesDirectory+ "Renderer/BuiltInShaders/BuiltInShader.vert", 
                                              Program.modulesDirectory + "Renderer/BuiltInShaders/BuiltInShader.frag");

            //Tell opengl how to give the data to the shaders.
            Gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), null);
            Gl.EnableVertexAttribArray(0);
        }

        private unsafe void OnRender(double obj)
        {
            //Clear the color channel.
            Gl.Clear((uint)ClearBufferMask.ColorBufferBit);

            //Bind the geometry and shader.
            Gl.BindVertexArray(Vao);
            Gl.UseProgram(shaders["default"]);

            //Draw the geometry.
            Gl.DrawElements(GLEnum.Triangles, (uint)Indices.Length, GLEnum.UnsignedInt, (void*)0);
        }

        private void OnUpdate(double obj)
        {

        }

        private void OnClose()
        {
            Gl.DeleteBuffer(Vbo);
            Gl.DeleteBuffer(Ebo);
            Gl.DeleteVertexArray(Vao);
            foreach (uint shader in shaders.Values)
            {
                Gl.DeleteProgram(shader);
            }
        }

        private void KeyDown(IKeyboard arg1, Key arg2, int arg3)
        {
            if (arg2 == Key.Escape)
            {
                window.Close();
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