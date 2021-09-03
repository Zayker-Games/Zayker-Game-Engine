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

        private List<RenderRequest> renderQue = new List<RenderRequest>();

        /// <summary>
        /// Rather or not this window is ready to be removed from the Renderer.windows list. 
        /// This is set by the engine and should never be modified! To close a window use the Close method.
        /// </summary>
        public bool _markedForDestruction = false;

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

            foreach (RenderRequest renderRequest in renderQue)
            {
                renderRequest.vao.Draw(renderRequest.material, camera, renderRequest.positionInWorldspace, renderRequest.eulerAnglesInWorldspace, renderRequest.scaleInWorldspace);
            }

            renderQue.Clear();

            Gl.BindVertexArray(0); // Why? I added this and its not needed. Might just be good practice.
        }

        private void OnUpdate(double obj)
        {
            //vaoRenderQue.Add(VaoB);
        }

        private void OnClose()
        {
            renderQue.Clear();
            foreach (Shader shader in shaders.Values)
            {
                shader.Delete();
            }
        }

        private void OnResize(Silk.NET.Maths.Vector2D<int> obj)
        {
            Gl.Viewport(window.Size);
        }

        public void AddToRenderQue(RenderRequest renderRequest)
        {
            if (!window.IsClosing)
            {
                renderQue.Add(renderRequest);
            } else
            {
                Console.WriteLine("Warning! You are trying to add a RenderRequest to a closing window!");
            }
        }

        /// <summary>
        /// Closes this window and handles everything associtated with removing it from the renderer que. 
        /// </summary>
        public void Close()
        {
            window.Close();
        }

        public Shader GetShader(string name)
        {
            return shaders[name];
        }
    }
}
