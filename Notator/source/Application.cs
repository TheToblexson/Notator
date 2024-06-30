﻿using Silk.NET.GLFW;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using StbImageSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static System.Net.Mime.MediaTypeNames;

namespace Notator
{
    //TODO: Textures and Dynamic Geometry

    internal class Application
    {
        #region Structs

        struct Color(float red, float green, float blue, float alpha)
        {
            public float Red { get; set; } = red;
            public float Green { get; set; } = green;
            public float Blue { get; set; } = blue;
            public float Alpha { get; set; } = alpha;
        }

        struct ShaderInfo(string name, int index)
        {
            public readonly string Name => name;
            public readonly int Index => index;
        }

        struct Vertex
        {
            public Vertex(float posX, float posY, float posZ,
                          float red, float green, float blue, float alpha,
                          float texX, float texY, float texIndex)
            {
                Vertices = [posX, posY, posZ, red, green, blue, alpha, texX, texY, texIndex];
            }

            public float[] Vertices { get; private set; } = new float[Count];
            public readonly Vector3D<float> Position
            {
                get => new(Vertices[0], Vertices[1], Vertices[2]);
                set
                {
                    Vertices[0] = value.X;
                    Vertices[1] = value.Y;
                    Vertices[2] = value.Z;
                }
            }
            public readonly Color Color
            {
                get => new(Vertices[3], Vertices[4], Vertices[5], Vertices[6]);
                set
                {
                    Vertices[3] = value.Red;
                    Vertices[4] = value.Green;
                    Vertices[5] = value.Blue;
                    Vertices[6] = value.Alpha;
                }
            }
            public readonly Vector2D<float> TextureCoordinates
            {
                get => new(Vertices[7], Vertices[8]);
                set
                {
                    Vertices[7] = value.X;
                    Vertices[8] = value.Y;
                }
            }
            public readonly float TextureIndex
            {
                get => Vertices[9];
                set => Vertices[9] = value;
            }
            public static uint Count = 10;
            public static uint Size => Count * sizeof(float);
        }

        #endregion

        #region Private Properties

        private IWindow MainWindow { get; init; }

        private GL OpenGL { get; set; }

        private uint Vao { get; set; }

        private uint Vbo { get; set; }

        private uint Ibo { get; set; }

        private uint Program { get; set; }

        private static List<float> Vertices { get; } = [];

        /*private static float[] Vertices =>
        [
          //X       Y       Z       R     G     B     A         TexX  TexY      TexID
            100.0f, 100.0f, 0.0f,   0.5f, 0.0f, 0.5f, 1.0f,     0.0f, 0.0f,     0.0f,
            200.0f, 100.0f, 0.0f,   0.5f, 0.0f, 0.5f, 1.0f,     1.0f, 0.0f,     0.0f,
            200.0f, 200.0f, 0.0f,   0.5f, 0.0f, 0.5f, 1.0f,     1.0f, 1.0f,     0.0f,
            100.0f, 200.0f, 0.0f,   0.5f, 0.0f, 0.5f, 1.0f,     0.0f, 1.0f,     0.0f,

            300.0f, 300.0f, 0.0f,   0.0f, 0.5f, 1.0f, 1.0f,     0.0f, 0.0f,     1.0f,
            400.0f, 300.0f, 0.0f,   0.0f, 0.5f, 1.0f, 1.0f,     1.0f, 0.0f,     1.0f,
            400.0f, 400.0f, 0.0f,   0.0f, 0.5f, 1.0f, 1.0f,     1.0f, 1.0f,     1.0f,
            300.0f, 400.0f, 0.0f,   0.0f, 0.5f, 1.0f, 1.0f,     0.0f, 1.0f,     1.0f,
        ];*/

        // Change to dynamic later
        private static uint[] Indices =>
        [
            0u, 1u, 3u,
            1u, 2u, 3u,

            4u, 5u, 7u,
            5u, 6u, 7u
        ];

        private static Dictionary<ShaderType, ShaderInfo> ShaderTypes => new() 
        { 
            { ShaderType.VertexShader,   new("Vertex",   0) }, 
            { ShaderType.FragmentShader, new("Fragment", 1) } 
        };

        private static uint AttributesCount { get; set; } = 0;
        private static nint AttributesPointer { get; set; } = 0;

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
            OpenGL.ClearColor(System.Drawing.Color.Gray);

            // Create the vertex array
            Vao = OpenGL.GenVertexArray();
            // Bind the vertex array
            OpenGL.BindVertexArray(Vao);

            // Create the vertex buffer
            Vbo = OpenGL.GenBuffer();
            // Bind the vertex buffer
            OpenGL.BindBuffer(BufferTargetARB.ArrayBuffer, Vbo);

            // Create dynamic vertex buffer
            OpenGL.BufferData(BufferTargetARB.ArrayBuffer, Vertex.Size * 1000, null, BufferUsageARB.DynamicDraw);

            /*
            // Create a quad
            Vertices.AddRange(new Vertex(100.0f, 100.0f, 0.0f, 0.5f, 0.0f, 0.5f, 1.0f, 0.0f, 0.0f, 0.0f).Vertices);
            Vertices.AddRange(new Vertex(200.0f, 100.0f, 0.0f, 0.5f, 0.0f, 0.5f, 1.0f, 1.0f, 0.0f, 0.0f).Vertices);
            Vertices.AddRange(new Vertex(200.0f, 200.0f, 0.0f, 0.5f, 0.0f, 0.5f, 1.0f, 1.0f, 1.0f, 0.0f).Vertices);
            Vertices.AddRange(new Vertex(100.0f, 200.0f, 0.0f, 0.5f, 0.0f, 0.5f, 1.0f, 0.0f, 1.0f, 0.0f).Vertices);
            */

            //float[] vertices = Vertices.ToArray();

            // Buffer the vertex data
            //fixed (void* buffer = vertices)
            //    OpenGL.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(vertices.Length * sizeof(float)), buffer, BufferUsageARB.StaticDraw);
            
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
            if (fragmentStatus != (int)GLEnum.True)
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

            // Load the images
            ImageResult image1 = ImageResult.FromMemory(File.ReadAllBytes("resources/textures/silk.png"), ColorComponents.RedGreenBlueAlpha);
            ImageResult image2 = ImageResult.FromMemory(File.ReadAllBytes("resources/textures/silk2.png"), ColorComponents.RedGreenBlueAlpha);

            // Create the textures
            uint texture1 = CreateTexture("silk.png");
            uint texture2 = CreateTexture("silk2.png");

            // Bind the textures to the slots
            OpenGL.BindTextureUnit(0, texture1);
            OpenGL.BindTextureUnit(1, texture2);

            // Set the texture uniform
            int textureLocation = OpenGL.GetUniformLocation(Program, "uTextures");
            int[] samplers = [0, 1];
            OpenGL.Uniform1(textureLocation, 2, samplers);

            // Set the color uniform
            int colorLocation = OpenGL.GetUniformLocation(Program, "uColor");
            OpenGL.Uniform4(colorLocation, 0.2f, 0.3f, 0.8f, 1.0f);

            // Create the projection matrix
            Matrix4X4<float> projectionMatrix = Matrix4X4.CreateOrthographicOffCenter(0.0f, MainWindow.Size.X, 0.0f, MainWindow.Size.Y, -1.0f, 1.0f);

            //Set the mvp uniform
            int mvpLocation = OpenGL.GetUniformLocation(Program, "uMVP");
            OpenGL.UniformMatrix4(mvpLocation, 1, false, (float*)&projectionMatrix);

            // Register the position attribute as 3 floats
            AddAttribute(3);
            // Register the color attribute as 4 floats
            AddAttribute(4);
            // Register the texture coordinate attribute as 2 floats
            AddAttribute(2);
            // Register the texture index attribute as 1 float
            AddAttribute(1);
        }

        private void AddAttribute(uint size)
        {
            OpenGL.VertexAttribPointer(AttributesCount, (int)size, VertexAttribPointerType.Float, false, Vertex.Size, AttributesPointer);
            OpenGL.EnableVertexAttribArray(AttributesCount);
            AttributesCount += 1;
            AttributesPointer += (nint)(size * sizeof(float));
        }

        /// <summary>
        /// Called when an update should run.
        /// </summary>
        /// <param name="deltaTime">The time (in seconds) since the last render call.</param>
        private void OnUpdate(double obj)
        {
            // Create a quad
            Vertices.AddRange(new Vertex(100.0f, 100.0f, 0.0f, 0.5f, 0.0f, 0.5f, 1.0f, 0.0f, 0.0f, 0.0f).Vertices);
            Vertices.AddRange(new Vertex(200.0f, 100.0f, 0.0f, 0.5f, 0.0f, 0.5f, 1.0f, 1.0f, 0.0f, 0.0f).Vertices);
            Vertices.AddRange(new Vertex(200.0f, 200.0f, 0.0f, 0.5f, 0.0f, 0.5f, 1.0f, 1.0f, 1.0f, 0.0f).Vertices);
            Vertices.AddRange(new Vertex(100.0f, 200.0f, 0.0f, 0.5f, 0.0f, 0.5f, 1.0f, 0.0f, 1.0f, 0.0f).Vertices);

            // Create a quad
            Vertices.AddRange(new Vertex(300.0f, 300.0f, 1.0f, 0.5f, 0.0f, 0.5f, 1.0f, 0.0f, 0.0f, 1.0f).Vertices);
            Vertices.AddRange(new Vertex(400.0f, 300.0f, 1.0f, 0.5f, 0.0f, 0.5f, 1.0f, 1.0f, 0.0f, 1.0f).Vertices);
            Vertices.AddRange(new Vertex(400.0f, 400.0f, 1.0f, 0.5f, 0.0f, 0.5f, 1.0f, 1.0f, 1.0f, 1.0f).Vertices);
            Vertices.AddRange(new Vertex(300.0f, 400.0f, 1.0f, 0.5f, 0.0f, 0.5f, 1.0f, 0.0f, 1.0f, 1.0f).Vertices);


            /*private static float[] Vertices =>
            [
              //X       Y       Z       R     G     B     A         TexX  TexY      TexID
                100.0f, 100.0f, 0.0f,   0.5f, 0.0f, 0.5f, 1.0f,     0.0f, 0.0f,     0.0f,
                200.0f, 100.0f, 0.0f,   0.5f, 0.0f, 0.5f, 1.0f,     1.0f, 0.0f,     0.0f,
                200.0f, 200.0f, 0.0f,   0.5f, 0.0f, 0.5f, 1.0f,     1.0f, 1.0f,     0.0f,
                100.0f, 200.0f, 0.0f,   0.5f, 0.0f, 0.5f, 1.0f,     0.0f, 1.0f,     0.0f,

                300.0f, 300.0f, 0.0f,   0.0f, 0.5f, 1.0f, 1.0f,     0.0f, 0.0f,     1.0f,
                400.0f, 300.0f, 0.0f,   0.0f, 0.5f, 1.0f, 1.0f,     1.0f, 0.0f,     1.0f,
                400.0f, 400.0f, 0.0f,   0.0f, 0.5f, 1.0f, 1.0f,     1.0f, 1.0f,     1.0f,
                300.0f, 400.0f, 0.0f,   0.0f, 0.5f, 1.0f, 1.0f,     0.0f, 1.0f,     1.0f,
            ];*/

            //  Bind buffer
            OpenGL.BindBuffer(BufferTargetARB.ArrayBuffer, Vbo);

            // Add subdata to the buffer
            OpenGL.BufferSubData(BufferTargetARB.ArrayBuffer, 0, [.. Vertices]);
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


        private unsafe uint CreateTexture(string fileName)
        {
            // Create the texture object
            uint texture = OpenGL.GenTexture();

            // Set the flip flag because OpenGL reads bottom up.
            StbImage.stbi_set_flip_vertically_on_load(1);

            // Get the image
            ImageResult image = ImageResult.FromMemory(File.ReadAllBytes($"resources/textures/{fileName}"), ColorComponents.RedGreenBlueAlpha);


            // Bind the texture to a texture slot
            OpenGL.ActiveTexture(TextureUnit.Texture1);
            OpenGL.BindTexture(TextureTarget.Texture2D, texture);

            // Attach the image to the texture
            fixed (byte* ptr = image.Data)
                OpenGL.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, (uint)image.Width,
                    (uint)image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, ptr);

            // Set the texture parameters
            int wrapParameter = (int)TextureWrapMode.Repeat;
            int minParameter = (int)TextureMinFilter.NearestMipmapNearest;
            int magParameter = (int)TextureMagFilter.Nearest;
            OpenGL.TexParameterI(GLEnum.Texture2D, GLEnum.TextureWrapS, in wrapParameter);
            OpenGL.TexParameterI(GLEnum.Texture2D, GLEnum.TextureWrapT, in wrapParameter);
            OpenGL.TexParameterI(GLEnum.Texture2D, GLEnum.TextureMinFilter, in minParameter);
            OpenGL.TexParameterI(GLEnum.Texture2D, GLEnum.TextureMagFilter, in magParameter);

            // Enable blending
            OpenGL.Enable(EnableCap.Blend);
            OpenGL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            // Generate Mipmap
            OpenGL.GenerateMipmap(TextureTarget.Texture2D);

            return texture;
        }

        #endregion
    }
}
