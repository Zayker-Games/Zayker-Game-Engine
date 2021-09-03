using System.Collections.Generic;
using Silk.NET.Windowing;
using System;
using System.Numerics;
using Silk.NET.Input;

namespace ZEngine.Rendering
{
    /// <summary>
    /// Instance of a window. This has its own OpenGl instance. 
    /// </summary>
    public class Window
    {
        public IWindow window;
        public Silk.NET.OpenGL.GL Gl;
        Dictionary<string, Shader> shaders = new Dictionary<string, Shader>();

        private List<VertexArrayObject> vaoRenderQue = new List<VertexArrayObject>();

        /// <summary>
        /// Rather or not this window is ready to be removed from the Renderer.windows list. 
        /// This is set by the engine and should never be modified! To close a window use the Close method.
        /// </summary>
        public bool _markedForDestruction = false;

        private Camera camera;

        Texture testTexture;

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
            options.Title = "Z-Engine";
            window = Silk.NET.Windowing.Window.Create(options);

            window.Load += OnLoad;
            window.Render += OnRender;
            window.Update += OnUpdate;
            window.Closing += OnClose;
            window.Resize += OnResize;
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

            camera = new Camera();

            // Here we add the callbacks to the input module (if it is enabled)
            IInputContext input = window.CreateInput();
            for (int i = 0; i < input.Keyboards.Count; i++)
            {
                input.Keyboards[i].KeyDown += Input.Input.InvokeKeyDownEvent;
                input.Keyboards[i].KeyUp += Input.Input.InvokeKeyUpEvent;
            }

            testTexture = new Texture(Gl, System.IO.Path.Combine(Core.ModuleSystem.GetModuleById("renderer_core").GetDirectory(), "BuiltInTextures/EngineMascotPalette.png"));
        }

        private unsafe void OnRender(double obj)
        {
            //Clear the color channel.
            Gl.Enable(Silk.NET.OpenGL.EnableCap.DepthTest);
            Gl.ClearColor(System.Drawing.Color.Cyan);
            Gl.Clear((uint)(Silk.NET.OpenGL.ClearBufferMask.ColorBufferBit | Silk.NET.OpenGL.ClearBufferMask.DepthBufferBit));

            camera.position.X = 0.0f;
            camera.position.Y = 2.5f;
            camera.position.Z = 5.0f;
            camera.forwards = Vector3.Normalize(new Vector3(0f, -0.5f, -1f));
            camera.aspectRatio = ((float)window.Size.X) / ((float)window.Size.Y);
            camera.fov = 45f;

            //Bind a texture and and set the uTexture0 to use texture0.
            testTexture.Bind(Silk.NET.OpenGL.TextureUnit.Texture0);
            shaders["default"].SetUniform("uTexture0", 0);

            foreach (VertexArrayObject vao in vaoRenderQue)
            {
                vao.Draw(shaders["default"], camera, new Vector3(0f, 0f, 0f), new Vector3(0f, (float)window.Time * 100f, 0f), new Vector3(1f, 1f, 1f));
            }

            vaoRenderQue.Clear();

            Gl.BindVertexArray(0); // Why? I added this and its not needed. Might just be good practice.
        }

        private void OnUpdate(double obj)
        {
            //vaoRenderQue.Add(VaoB);
        }

        private void OnClose()
        {
            vaoRenderQue.Clear();
            foreach (Shader shader in shaders.Values)
            {
                shader.Delete();
            }
        }

        private void OnResize(Silk.NET.Maths.Vector2D<int> obj)
        {
            Gl.Viewport(window.Size);
        }

        public void AddToRenderQue(VertexArrayObject vao)
        {
            if (!window.IsClosing)
            {
                vaoRenderQue.Add(vao);
            } else
            {
                Console.WriteLine("Warning! You are trying to add a VAO to a closing window!");
            }
        }

        public void Close()
        {
            window.Close();
        }
    }
}
