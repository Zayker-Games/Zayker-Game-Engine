using System.Collections.Generic;
using Silk.NET.Windowing;
using System;
using System.Numerics;
using Silk.NET.Input;

namespace ZEngine.Rendering
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

    /// <summary>
    /// Instance of a window. This has its own OpenGl instance. 
    /// </summary>
    public class Window
    {
        public IWindow window;
        private static Silk.NET.OpenGL.GL Gl;
        Dictionary<string, Shader> shaders = new Dictionary<string, Shader>();

        private static VertexArrayObject VaoA;
        private static VertexArrayObject VaoB;

        private Camera camera;

        //Vertex data, uploaded to the VBO.
        private static readonly float[] VerticesA =
        {
            //X    Y      Z
             0.5f,  0.9f, 0.5f,
             0.5f,  0.1f, 0.5f,
            -0.5f,  0.1f, 0.5f,
            -0.5f,  0.9f, 0.5f
        };
        private static readonly float[] VerticesB =
        {
            //X    Y      Z
             0.5f, -0.1f, 0.0f,
             0.5f, -0.9f, 0.0f,
            -0.5f, -0.9f, 0.0f,
            -0.5f, -0.1f, 0.0f
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

        /// <summary>
        /// Runs once when the window is created. Initializes openGl.
        /// </summary>
        private unsafe void OnLoad()
        {
            //Getting the opengl api for drawing to the screen.
            Gl = Silk.NET.OpenGL.GL.GetApi(window);

            shaders.Add("default", Shader.FromFiles(Gl, System.IO.Path.Combine(Core.ModuleSystem.GetModuleById("renderer_core").GetDirectory(), "BuiltInShaders/BuiltInShader.vert"),
                                              System.IO.Path.Combine(Core.ModuleSystem.GetModuleById("renderer_core").GetDirectory(), "BuiltInShaders/BuiltInShader.frag")));

            VaoA = new VertexArrayObject(Gl, VerticesA, Indices);
            VaoB = new VertexArrayObject(Gl, VerticesB, Indices);

            camera = new Camera();

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
            Gl.Enable(Silk.NET.OpenGL.EnableCap.DepthTest);
            Gl.Clear((uint)(Silk.NET.OpenGL.ClearBufferMask.ColorBufferBit | Silk.NET.OpenGL.ClearBufferMask.DepthBufferBit));

            DrawVertexArrayObject(VaoA._handle);
            DrawVertexArrayObject(VaoB._handle);

            Gl.BindVertexArray(0); // Why? I added this and its not needed. Might just be good practice.
        }


        private const int Width = 800;
        private const int Height = 700;

        
        private unsafe void DrawVertexArrayObject(uint vao)
        {
            //Bind the geometry and shader.
            Gl.BindVertexArray(vao); // We already bound this ealier
            shaders["default"].Use();

            //Use elapsed time to convert to radians to allow our cube to rotate over time
            var difference = (float)(window.Time * 100);

            var model = Matrix4x4.CreateRotationY(DegreesToRadians(difference)) * Matrix4x4.CreateRotationX(DegreesToRadians(difference));
            var view = Matrix4x4.CreateLookAt(camera.position, camera.position + camera.forwards, camera.up);
            var projection = Matrix4x4.CreatePerspectiveFieldOfView(DegreesToRadians(camera.fov), Width / Height, 0.1f, 100.0f);

            int matrixIDModel = Gl.GetUniformLocation(shaders["default"].handle, "uModel");
            Gl.UniformMatrix4(matrixIDModel, 1, false, (float*)&model);
            int matrixIDView = Gl.GetUniformLocation(shaders["default"].handle, "uView");
            Gl.UniformMatrix4(matrixIDView, 1, false, (float*)&view);
            int matrixIDProjection = Gl.GetUniformLocation(shaders["default"].handle, "uProjection");
            Gl.UniformMatrix4(matrixIDProjection, 1, false, (float*)&projection);

            //Draw the geometry.
            Gl.DrawElements(Silk.NET.OpenGL.GLEnum.Triangles, (uint)Indices.Length, Silk.NET.OpenGL.GLEnum.UnsignedInt, (void*)0);
        }

        public static float DegreesToRadians(float degrees)
        {
            return MathF.PI / 180f * degrees;
        }

        private void OnUpdate(double obj)
        {

        }

        private void OnClose()
        {
            VaoA.Dispose();
            VaoB.Dispose();

            foreach (Shader shader in shaders.Values)
            {
                shader.Delete();
            }
        }
    }
}