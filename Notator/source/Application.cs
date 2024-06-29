using Silk.NET.GLFW;
using Silk.NET.Maths;
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
        struct ShaderInfo(string name, int index)
        {
            public readonly string Name => name;
            public readonly int Index => index;
        }

        #region Private Properties

        private IWindow MainWindow { get; init; }

        private GL OpenGL { get; set; }

        private uint Vao {  get; set; }

        private uint Vbo { get; set; }

        private uint Ibo { get; set; }

        private uint Program { get; set; }

        private static float[] Vertices =>
        [
            //X    Y      Z
            100.0f, 100.0f, 0.0f,
            200.0f, 100.0f, 0.0f,
            200.0f, 200.0f, 0.0f,
            100.0f, 200.0f, 0.0f
        ];

        private static uint[] Indices =>
        [
            0u, 1u, 3u,
            1u, 2u, 3u
        ];

        private static Dictionary<ShaderType, ShaderInfo> ShaderTypes => new() 
        { 
            { ShaderType.VertexShader,   new("Vertex",   0) }, 
            { ShaderType.FragmentShader, new("Fragment", 1) } 
        };

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

            // Get the shader source code
            string[] shaderSource = ReadShaderFile("Basic.shader");

            // Create the vertex shader
            uint vertexShader = OpenGL.CreateShader(ShaderType.VertexShader);
            // Set the shader source code
            OpenGL.ShaderSource(vertexShader, shaderSource[0]);

            // Compile the vertex shader
            OpenGL.CompileShader(vertexShader);
            // Check that the shader has compiled
            OpenGL.GetShader(vertexShader, ShaderParameterName.CompileStatus, out int vertexStatus);
            if (vertexStatus != (int)GLEnum.True)
                throw new Exception("Vertex shader failed to compile: " + OpenGL.GetShaderInfoLog(vertexShader));

            // Create the fragment shader
            uint fragmentShader = OpenGL.CreateShader(ShaderType.FragmentShader);
            // Set the shader source code
            OpenGL.ShaderSource(fragmentShader, shaderSource[1]);

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

            //Bind the shader
            OpenGL.UseProgram(Program);

            // Set the color uniform
            int colorLocation = OpenGL.GetUniformLocation(Program, "uColor");
            OpenGL.Uniform4(colorLocation, 0.2f, 0.3f, 0.8f, 1.0f);

            // Create the projection matrix
            Matrix4X4<float> projectionMatrix = Matrix4X4.CreateOrthographicOffCenter(0.0f, MainWindow.Size.X, 0.0f, MainWindow.Size.Y, -1.0f, 1.0f);

            //Set the mvp uniform
            int mvpLocation = OpenGL.GetUniformLocation(Program, "uMVP");
            OpenGL.UniformMatrix4(mvpLocation, 1, false, (float*)&projectionMatrix);

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


        /// <summary>
        /// Read and process a shader file.
        /// </summary>
        /// <param name="fileName">The file to read, including the file extension. Must be within resources/shaders.</param>
        /// <returns>An array containing the shader files. [0] is vertex, [1] is fragment.</returns>
        private string[] ReadShaderFile(string fileName)
        {
            // Read the shader file
            IEnumerable<string> lines = File.ReadLines($"resources/shaders/{fileName}");

            // Initialise a string writer array
            StringWriter[] shaderWriter = new StringWriter[ShaderTypes.Count];

            // Initialise the writers inside the array
            for (int i = 0; i < shaderWriter.Length; i++)
            {
                shaderWriter[i] = new StringWriter();
            }

            int shaderType = -1;
            foreach (string line in lines)
            {
                // If the line is a shader type declaration...
                if (line.Contains("#shader"))
                {
                    // For each type in the type list...
                    foreach (ShaderInfo info in ShaderTypes.Values)
                    {
                        // Check to see if it matches the name...
                        if (line.Contains(info.Name, StringComparison.CurrentCultureIgnoreCase))
                        {
                            // And if so, set the type to the index.
                            shaderType = info.Index;
                        }
                    }
                }
                // Else the line is shader code...
                else
                {
                    // So write to the correct string writer
                    shaderWriter[shaderType].WriteLine(line);
                }
            }

            // Convert string writer array to string
            string[] output = new string[shaderWriter.Length];
            for (int i = 0; i < shaderWriter.Length; i++)
            {
                output[i] = shaderWriter[i].ToString();
            }

            return output;
        }

        #endregion
    }
}
