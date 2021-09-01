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

        private static uint VaoA;
        private static uint VaoB;

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
        /// Creates a VertexArrayObject from vertice positions and the index array. 
        /// </summary>
        unsafe uint CreateVertexArrayObject(float[] vertices, uint[] indices)
        {
            //Creating the vertex array, storing all data. 
            uint vao;
            Gl.CreateVertexArrays(1, out vao);
            Gl.BindVertexArray(vao);

            //Initializing a vertex buffer that holds the vertex data.
            uint vbo;
            Gl.CreateBuffers(1, out vbo);
            Gl.BindBuffer(Silk.NET.OpenGL.BufferTargetARB.ArrayBuffer, vbo);
            fixed (void* v = &vertices[0])
            {
                Gl.BufferData(Silk.NET.OpenGL.BufferTargetARB.ArrayBuffer, (uint)(vertices.Length * sizeof(uint)), v, Silk.NET.OpenGL.BufferUsageARB.StaticDraw); //Setting buffer data.
            }

            //Initializing a element buffer that holds the index data.
            uint ebo;
            Gl.CreateBuffers(1, out ebo); //Creating the buffer.
            Gl.BindBuffer(Silk.NET.OpenGL.BufferTargetARB.ElementArrayBuffer, ebo); //Binding the buffer.
            fixed (void* i = &indices[0])
            {
                Gl.BufferData(Silk.NET.OpenGL.BufferTargetARB.ElementArrayBuffer, (uint)(indices.Length * sizeof(uint)), i, Silk.NET.OpenGL.BufferUsageARB.StaticDraw); //Setting buffer data.
            }

            //Tell opengl how to give the data to the shaders.
            Gl.VertexAttribPointer(0, 3, Silk.NET.OpenGL.VertexAttribPointerType.Float, false, 3 * sizeof(float), null);
            Gl.EnableVertexAttribArray(0);

            // vbo and ebo should be deleted when the program stops. I cant do that right now, because there are no references passed out. 
            // Gl.DeleteBuffer(vbo);
            // Gl.DeleteBuffer(ebo);

            return vao;
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
            Gl.Enable(Silk.NET.OpenGL.EnableCap.DepthTest);
            Gl.Clear((uint)(Silk.NET.OpenGL.ClearBufferMask.ColorBufferBit | Silk.NET.OpenGL.ClearBufferMask.DepthBufferBit));

            DrawVertexArrayObject(VaoA);
            DrawVertexArrayObject(VaoB);

            Gl.BindVertexArray(0); // Why? I added this and its not needed. Might just be good practice.
        }


        private const int Width = 800;
        private const int Height = 700;

        //Setup the camera's location, directions, and movement speed
        private static Vector3 CameraPosition = new Vector3(0.0f, 0.0f, 3.0f);
        private static Vector3 CameraFront = new Vector3(0.0f, 0.0f, -1.0f);
        private static Vector3 CameraUp = Vector3.UnitY;
        private static Vector3 CameraDirection = Vector3.Zero;
        private static float CameraYaw = -90f;
        private static float CameraPitch = 0f;
        private static float CameraZoom = 45f;

        private unsafe void DrawVertexArrayObject(uint vao)
        {
            //Bind the geometry and shader.
            Gl.BindVertexArray(vao); // We already bound this ealier
            Gl.UseProgram(shaders["default"].handle);

            //Use elapsed time to convert to radians to allow our cube to rotate over time
            var difference = (float)(window.Time * 100);

            var model = Matrix4x4.CreateRotationY(DegreesToRadians(difference)) * Matrix4x4.CreateRotationX(DegreesToRadians(difference));
            var view = Matrix4x4.CreateLookAt(CameraPosition, CameraPosition + CameraFront, CameraUp);
            var projection = Matrix4x4.CreatePerspectiveFieldOfView(DegreesToRadians(CameraZoom), Width / Height, 0.1f, 100.0f);

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
            Gl.DeleteVertexArray(VaoA);
            Gl.DeleteVertexArray(VaoB);

            foreach (Shader shader in shaders.Values)
            {
                shader.Delete();
            }
        }
    }
}