using Silk.NET.GLFW;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Notator
{
    internal class Application
    {
        #region Private Properties

        private IWindow MainWindow { get; init; }

        private GL OpenGL { get; set; }

        private uint Vao {  get; set; }

        private uint Vbo { get; set; }

        private uint Ibo { get; set; }

        private uint Program { get; set; }

        private static string VertexShaderSource => @"
        #version 330 core
        layout (location = 0) in vec3 aPosition;
        
        void main()
        {
            gl_Position = vec4(aPosition.x, aPosition.y, aPosition.z, 1.0);
        }
        ";

        private static string FragmentShaderSource => @"
        #version 330 core
        layout (location = 0) out vec4 oColor;

        void main()
        {
            oColor = vec4(1.0f, 0.5f, 0.2f, 1.0f);
        }
        ";

        private static float[] Vertices =>
        [
            //X    Y      Z
             0.5f,  0.5f, 0.0f,
             0.5f, -0.5f, 0.0f,
            -0.5f, -0.5f, 0.0f,
            -0.5f,  0.5f, 0.5f
        ];

        private static uint[] Indices =>
        [
            0u, 1u, 3u,
            1u, 2u, 3u
        ];

        private static Dictionary<ShaderType, string> ShaderNames => new() { { ShaderType.VertexShader, "Vertex" }, {ShaderType.FragmentShader, "Fragment" } };

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor for the application.
        /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public Application()
        #pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            // Create the window options with default size and title
            WindowOptions options = WindowOptions.Default with
            {
                Size = new(800, 600),
                Title = "Notator"
            };

            // Create the window
            MainWindow = Window.Create(options);

            // Attach methods to the window events
            MainWindow.Load += OnLoad;
            MainWindow.Update += OnUpdate;
            MainWindow.Render += OnRender;
            MainWindow.Closing += OnClosing;

            // Tell the window to run
            MainWindow.Run();
        }

        #endregion

        #region Event Methods

        /// <summary>
        /// Called when the window first begins to run.
        /// </summary>
        private unsafe void OnLoad()
        {
            // Create the OpenGL instance
            OpenGL = MainWindow.CreateOpenGL();

            // Set the clear color
            OpenGL.ClearColor(Color.Gray);

            // Create the vertex array
            Vao = OpenGL.GenVertexArray();
            // Bind the vertex array
            OpenGL.BindVertexArray(Vao);

            // Create the vertex buffer
            Vbo = OpenGL.GenBuffer();
            // Bind the vertex buffer
            OpenGL.BindBuffer(BufferTargetARB.ArrayBuffer, Vbo);

            // Buffer the vertex data
            fixed (void* buffer = Vertices)
                OpenGL.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(Vertices.Length * sizeof(float)), buffer, BufferUsageARB.StaticDraw);

            // Create the index buffer
            Ibo = OpenGL.GenBuffer();
            // Bind the index buffer
            OpenGL.BindBuffer(BufferTargetARB.ElementArrayBuffer, Ibo);

            // Buffer the index data
            fixed (void* buffer = Indices)
                OpenGL.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint)(Indices.Length * sizeof(uint)), buffer, BufferUsageARB.StaticDraw);

            // Create the vertex shader
            uint vertexShader = OpenGL.CreateShader(ShaderType.VertexShader);
            // Set the shader source code
            OpenGL.ShaderSource(vertexShader, VertexShaderSource);

            // Compile the vertex shader
            OpenGL.CompileShader(vertexShader);
            // Check that the shader has compiled
            OpenGL.GetShader(vertexShader, ShaderParameterName.CompileStatus, out int vertexStatus);
            if (vertexStatus != (int)GLEnum.True)
                throw new Exception("Vertex shader failed to compile: " + OpenGL.GetShaderInfoLog(vertexShader));

            // Create the fragment shader
            uint fragmentShader = OpenGL.CreateShader(ShaderType.FragmentShader);
            // Set the shader source code
            OpenGL.ShaderSource(fragmentShader, FragmentShaderSource);

            // Compile the fragment shader
            OpenGL.CompileShader(fragmentShader);
            // Check that the shader has compiled
            OpenGL.GetShader(fragmentShader, ShaderParameterName.CompileStatus, out int fragmentStatus);
            if (vertexStatus != (int)GLEnum.True)
                throw new Exception("Fragment shader failed to compile: " + OpenGL.GetShaderInfoLog(fragmentShader));

            // Create the shader program
            Program = OpenGL.CreateProgram();

            // Attach the shaders to the program
            OpenGL.AttachShader(Program, vertexShader);
            OpenGL.AttachShader(Program, fragmentShader);

            // Link the program
            OpenGL.LinkProgram(Program);

            // Check that the program linked
            OpenGL.GetProgram(Program, ProgramPropertyARB.LinkStatus, out int linkStatus);
            if (linkStatus != (int)GLEnum.True)
                throw new Exception("Program failed to link: " + OpenGL.GetProgramInfoLog(Program));

            // Detach the shaders
            OpenGL.DetachShader(Program, vertexShader);
            OpenGL.DetachShader(Program, fragmentShader);

            // Delete the shaders
            OpenGL.DeleteShader(vertexShader);
            OpenGL.DeleteShader(fragmentShader);

            // Register the attribute as 3 floats
            OpenGL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), null);
            // Enable the attribute
            OpenGL.EnableVertexAttribArray(0);
        }

        /// <summary>
        /// Called when an update should run.
        /// </summary>
        /// <param name="deltaTime">The time (in seconds) since the last render call.</param>
        private void OnUpdate(double obj)
        {
        }

        /// <summary>
        /// Called when a frame should be rendered.
        /// </summary>
        /// <param name="deltaTime">The time (in seconds) since the last render call.</param>
        private unsafe void OnRender(double deltaTime)
        {
            // Clear the frame
            OpenGL.Clear(ClearBufferMask.ColorBufferBit);

            // Bind the vertex array
            OpenGL.BindVertexArray(Vao);

            // Bind the program
            OpenGL.UseProgram(Program);

            // Draw the vertex array using 6 indices
            OpenGL.DrawElements(PrimitiveType.Triangles, (uint)Indices.Length, DrawElementsType.UnsignedInt, null);
        }

        private void OnClosing()
        {
            // Delete all the buffers.
            OpenGL.DeleteBuffer(Vbo);
            OpenGL.DeleteBuffer(Ibo);
            OpenGL.DeleteVertexArray(Vao);
            OpenGL.DeleteProgram(Program);

        }

        #endregion

        #region Private Methods

        private uint CreateShader(string vertexSource, string fragmentSource)
        {
            // Create the shader program
            uint program = OpenGL.CreateProgram();

            // Compile the shader source code
            uint vertexShader = CompileShader(ShaderType.VertexShader, vertexSource);
            uint fragmentShader = CompileShader(ShaderType.FragmentShader, fragmentSource);

            // Attach the shaders to the program
            OpenGL.AttachShader(program, vertexShader);
            OpenGL.AttachShader(program, fragmentShader);

            // Link the program
            OpenGL.LinkProgram(program);

            // Validate the program
            OpenGL.ValidateProgram(program);

            // Detach the shaders
            OpenGL.DetachShader(program, vertexShader);
            OpenGL.DetachShader(program, fragmentShader);

            // Delete the shaders
            OpenGL.DeleteShader(vertexShader);
            OpenGL.DeleteShader(fragmentShader);

            // return the program
            return program;
        }

        private uint CompileShader(ShaderType type, string source)
        {
            // Create the shader
            uint shader = OpenGL.CreateShader(type);

            // Set the shader source
            OpenGL.ShaderSource(shader, source);

            // Compile the shader
            OpenGL.CompileShader(shader);

            // Check the compile status of the shaders
            OpenGL.GetShader(shader, ShaderParameterName.CompileStatus, out int status);
            if (status == (int)GLEnum.False)
                throw new Exception($"{ShaderNames[type]} shader failed to compile: " + OpenGL.GetShaderInfoLog(shader));

            // return the shader
            return shader;
        }

        #endregion
    }
}
