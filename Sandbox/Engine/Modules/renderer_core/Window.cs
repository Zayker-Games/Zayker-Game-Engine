using System.Collections.Generic;
using Silk.NET.Windowing;
using System;
using System.Numerics;
using Silk.NET.Input;
using System.Linq;

namespace ZEngine.Rendering
{
    /// <summary>
    /// Instance of a window. This has its own OpenGl instance. 
    /// </summary>
    public class Window
    {
        public IWindow window;
        private IInputContext input;
        public Silk.NET.OpenGL.GL Gl;
        Dictionary<string, Shader> shaders = new Dictionary<string, Shader>();

        private List<RenderRequest> renderQue = new List<RenderRequest>();
        private List<Silk.NET.OpenGL.Extensions.ImGui.ImGuiController> imGuiRenderQue = new List<Silk.NET.OpenGL.Extensions.ImGui.ImGuiController>();

        /// <summary>
        /// Rather or not this window is ready to be removed from the Renderer.windows list. 
        /// This is set by the engine and should never be modified! To close a window use the Close method.
        /// </summary>
        public bool _markedForDestruction = false;

        public Camera camera;

        public enum CursorBehavior
        {
            normal,
            confined,
            wraps
        }
        public CursorBehavior cursorBehavior = CursorBehavior.wraps;

        public enum BuiltInShaders {
            lit,
            unlit,
            screenspace
        }

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

            // Enable transparency in openGl
            Gl.Enable(Silk.NET.OpenGL.EnableCap.Blend);
            Gl.BlendFunc(Silk.NET.OpenGL.BlendingFactor.SrcAlpha, Silk.NET.OpenGL.BlendingFactor.OneMinusSrcAlpha);

            // Load the builtin shaders
            LoadStandardShaders();

            // Create a camera for this window
            camera = new Camera();

            // Here we add the callbacks to the input module (if it is enabled)
            input = window.CreateInput();
            for (int i = 0; i < input.Keyboards.Count; i++)
            {
                input.Keyboards[i].KeyDown += Input.InputModule.InvokeKeyDownEvent;
                input.Keyboards[i].KeyUp += Input.InputModule.InvokeKeyUpEvent;
            }
            for (int i = 0; i < input.Mice.Count; i++)
            {
                input.Mice[i].MouseMove += Input.InputModule.InvokeMouseMoveEvent;
                input.Mice[i].MouseDown += Input.InputModule.InvokeMouseDownEvent;
                input.Mice[i].MouseUp += Input.InputModule.InvokeMouseUpEvent;
            }

        }

        void UpdateCursorBehaviour()
        {
            foreach (Silk.NET.Input.IMouse mouse in input.Mice)
            {
                if (cursorBehavior == CursorBehavior.confined)
                {
                    mouse.Position = new System.Numerics.Vector2(Math.Clamp(mouse.Position.X, 0f, window.Size.X), Math.Clamp(mouse.Position.Y, 0f, window.Size.Y));
                }
                else if (cursorBehavior == CursorBehavior.wraps)
                {
                    mouse.Position = new System.Numerics.Vector2(Math.Wrap(mouse.Position.X, 1f, window.Size.X-1f), Math.Wrap(mouse.Position.Y, 1f, window.Size.Y-1f));
                }
            }
        }

        private void LoadStandardShaders()
        {
            shaders.Add("builtin_lit", Shader.FromFiles(
                Gl,
                System.IO.Path.Combine(Core.ModuleSystem.GetModuleById("renderer_core").GetDirectory(), "BuiltInShaders/Lit.vert"),
                System.IO.Path.Combine(Core.ModuleSystem.GetModuleById("renderer_core").GetDirectory(), "BuiltInShaders/Lit.frag")));

            shaders.Add("builtin_screenspace", Shader.FromFiles(
                Gl,
                System.IO.Path.Combine(Core.ModuleSystem.GetModuleById("renderer_core").GetDirectory(), "BuiltInShaders/Screenspace.vert"),
                System.IO.Path.Combine(Core.ModuleSystem.GetModuleById("renderer_core").GetDirectory(), "BuiltInShaders/Screenspace.frag")));
            GetBuiltinShader(BuiltInShaders.screenspace).screenspace = true;
        }

        private unsafe void OnRender(double obj)
        {
            //Clear the color channel.
            Gl.Enable(Silk.NET.OpenGL.EnableCap.DepthTest);
            Gl.ClearColor(System.Drawing.Color.Cyan);
            Gl.Clear((uint)(Silk.NET.OpenGL.ClearBufferMask.ColorBufferBit | Silk.NET.OpenGL.ClearBufferMask.DepthBufferBit));

            camera.aspectRatio = ((float)window.Size.X) / ((float)window.Size.Y);

            // Precalculate the view and projection matrices, since they stay the same for all objects
            var view = Matrix4x4.CreateLookAt((System.Numerics.Vector3)camera.position, (System.Numerics.Vector3)(camera.position - camera.forwards), (System.Numerics.Vector3)camera.up);
            var projection = Matrix4x4.CreatePerspectiveFieldOfView(Math.DegreesToRadians(camera.fov), camera.aspectRatio, 0.1f, 100.0f);

            // Gather all opaque objects and sort them by material for minimum Material.Use() calls
            List<RenderRequest> opaqueRenderQue = renderQue.Where(r => !r.material.transparent).ToList();
            opaqueRenderQue.OrderBy(r => r.material);
            Material usedMaterial = null;

            // Render opaque objects first
            foreach (RenderRequest opaqueRenderRequest in opaqueRenderQue)
            {
                if(usedMaterial != opaqueRenderRequest.material)
                {
                    // Load material
                    usedMaterial = opaqueRenderRequest.material;
                    opaqueRenderRequest.material.Use();

                    // If the shader is not screenspace, send perspective
                    if (!opaqueRenderRequest.material.shader.screenspace)
                    {
                        // Set view and projection matrices
                        opaqueRenderRequest.material.shader.SetUniform("uView", view);
                        opaqueRenderRequest.material.shader.SetUniform("uProjection", projection);
                    }
                }
                opaqueRenderRequest.vao.Draw(opaqueRenderRequest.material, opaqueRenderRequest.positionInWorldspace, opaqueRenderRequest.eulerAnglesInWorldspace, opaqueRenderRequest.scaleInWorldspace);
            }

            // Gather all transparent objects and order them by distance to the camera
            List<RenderRequest> transparentRenderQue = renderQue.Where(r => r.material.transparent).ToList();
            transparentRenderQue.OrderBy(r => (r.positionInWorldspace - camera.position).magnitude);

            foreach (RenderRequest transparentRenderRequest in transparentRenderQue)
            {
                if (usedMaterial != transparentRenderRequest.material)
                {
                    // Load material
                    usedMaterial = transparentRenderRequest.material;
                    transparentRenderRequest.material.Use();

                    // If the shader is not screenspace, send perspective
                    if (!transparentRenderRequest.material.shader.screenspace)
                    {
                        // Set view and projection matrices
                        transparentRenderRequest.material.shader.SetUniform("uView", view);
                        transparentRenderRequest.material.shader.SetUniform("uProjection", projection);
                    }
                }
                transparentRenderRequest.vao.Draw(transparentRenderRequest.material, transparentRenderRequest.positionInWorldspace, transparentRenderRequest.eulerAnglesInWorldspace, transparentRenderRequest.scaleInWorldspace);
            }

            // Render ImGui controllers
            foreach (Silk.NET.OpenGL.Extensions.ImGui.ImGuiController guiController in imGuiRenderQue)
            {
                guiController.Render();
            }

            // Update and Render additional Platform Windows
            //ImGuiNET.ImGui.UpdatePlatformWindows();
            //ImGuiNET.ImGui.RenderPlatformWindowsDefault();

            renderQue.Clear();
            opaqueRenderQue.Clear();
            transparentRenderQue.Clear();
            imGuiRenderQue.Clear();

            Gl.BindVertexArray(0); // Why? I added this and its not needed. Might just be good practice.
        }

        private void OnUpdate(double obj)
        {
            UpdateCursorBehaviour();
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

        /// <summary>
        /// Send a request to this window, to render the given request in the next frame. 
        /// </summary>
        public bool AddToRenderQue(RenderRequest renderRequest)
        {
            if (window != null && !window.IsClosing)
            {
                renderQue.Add(renderRequest);
                return true;
            } else
            {
                Debugging.Console.WriteToMain("You are trying to add a RenderRequest to a closing window!", "", Debugging.Console.LogLevel.warning);
                return false;
            }
        }

        public void AddImGuiRenderQue(Silk.NET.OpenGL.Extensions.ImGui.ImGuiController requestedController)
        {
            if (window != null && !window.IsClosing)
            {
                imGuiRenderQue.Add(requestedController);
            }
            else
            {
                Debugging.Console.WriteToMain("You are trying to add a RenderRequest to a closing window!", "", Debugging.Console.LogLevel.warning);
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

        public Shader GetBuiltinShader(BuiltInShaders shader)
        {
            if (shader == BuiltInShaders.lit)
                return GetShader("builtin_lit");
            else if (shader == BuiltInShaders.unlit)
                return GetShader("builtin_unlit");
            else if (shader == BuiltInShaders.screenspace)
                return GetShader("builtin_screenspace");
            else
                return null;
        }
    
        public void SetFullscreen(bool fullscreen)
        {
            window.WindowState = fullscreen ? WindowState.Fullscreen : WindowState.Normal;
        }

        public void SetTitle(string title)
        {
            window.Title = title;
        }
    }
}
