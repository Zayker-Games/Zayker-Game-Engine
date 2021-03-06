using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace ZEngine.Rendering
{
    public class Shader
    {
        private uint _handle;
        private GL _gl;

        /// <summary>
        /// Rather or not the shader uses the three projection matrices (Model, View, Perspective)
        /// </summary>
        public bool screenspace = false;

        /// <summary>
        /// Deletes the program from the OpenGl instance.
        /// </summary>
        public void Delete()
        {
            _gl.DeleteProgram(_handle);
        }

        public void Use()
        {
            _gl.UseProgram(_handle);
        }

        /// <summary>
        /// Set a uniform 4x4 Matrix for this shader.
        /// </summary>
        public unsafe void SetUniform(string name, Matrix4x4 value)
        {
            int location = _gl.GetUniformLocation(_handle, name);
            if (location == -1)
                throw new Exception($"{name} uniform not found on shader.");
            _gl.UniformMatrix4(location, 1, false, (float*)&value);
        }

        public void SetUniform(string name, float value)
        {
            int location = _gl.GetUniformLocation(_handle, name);
            if (location == -1)
            {
                throw new Exception($"{name} uniform not found on shader.");
            }
            _gl.Uniform1(location, value);
        }

        // Create a new Shader from two files (vertex and fragment shader)
        public static Shader FromFiles(GL Gl, string vertexPath, string fragmentPath)
        {
            return FromSource(Gl, System.IO.File.ReadAllText(vertexPath), System.IO.File.ReadAllText(fragmentPath));
        }

        /// <summary>
        /// Compiles vertex and fragment shaders into one shader and saves it in the shaders-dictionary.
        /// </summary>
        public static Shader FromSource(GL Gl, string vertexSource, string fragmentSource)
        {
            Shader newShader = new Shader();
            newShader._gl = Gl;

            //Creating a vertex shader.
            uint vertexShader = Gl.CreateShader(ShaderType.VertexShader);
            Gl.ShaderSource(vertexShader, vertexSource);
            Gl.CompileShader(vertexShader);

            //Checking the shader for compilation errors.
            string infoLog = Gl.GetShaderInfoLog(vertexShader);
            if (!string.IsNullOrWhiteSpace(infoLog))
            {
                Debugging.Console.WriteToMain("Error compiling vertex shader!", infoLog, Debugging.Console.LogLevel.error);
            }

            //Creating a fragment shader.
            uint fragmentShader = Gl.CreateShader(ShaderType.FragmentShader);
            Gl.ShaderSource(fragmentShader, fragmentSource);
            Gl.CompileShader(fragmentShader);

            //Checking the shader for compilation errors.
            infoLog = Gl.GetShaderInfoLog(fragmentShader);
            if (!string.IsNullOrWhiteSpace(infoLog))
            {
                Debugging.Console.WriteToMain("Error compiling fragment shader!", infoLog, Debugging.Console.LogLevel.error);
            }

            //Combining the shaders under one shader program.
            newShader._handle = Gl.CreateProgram();
            Gl.AttachShader(newShader._handle, vertexShader);
            Gl.AttachShader(newShader._handle, fragmentShader);
            Gl.LinkProgram(newShader._handle);

            //Checking the linking for errors.
            string shader = Gl.GetProgramInfoLog(newShader._handle);
            if (!string.IsNullOrWhiteSpace(shader))
            {
                Debugging.Console.WriteToMain("Error linking shader!", infoLog, Debugging.Console.LogLevel.error);
            }

            //Delete the no longer useful individual shaders;
            Gl.DeleteShader(vertexShader);
            Gl.DeleteShader(fragmentShader);

            return newShader;
        }
    }
}
