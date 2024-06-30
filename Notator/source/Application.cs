using Notator.Rendering;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using StbImageSharp;
using Shader = Notator.Rendering.Shader;
using VertexArray = Notator.Rendering.VertexArray;

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

        struct QuadIndices
        {
            private static uint[] Indices =>
            [
            0u, 1u, 3u,
            1u, 2u, 3u
            ];

            public static uint[] GetIndices(uint offset)
            {
                uint[] indices = new uint[6];
                for (int i = 0; i < 6; i++)
                {
                    indices[i] = Indices[i] + offset;
                }
                return indices;
            }
        }
        
        #endregion

        #region Private Properties

        private IWindow MainWindow { get; init; }

        private GL OpenGL { get; set; }

        private VertexArray VertexArray { get; set; }

        private VertexBuffer VertexBuffer { get; set; }

        private IndexBuffer IndexBuffer { get; set; }

        private Shader Shader { get; set; }

        private static List<float> Vertices { get; } = [];

        private static List<uint> Indices { get; } = [];

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

            // Create a vertex array and bind it
            VertexArray = new VertexArray(OpenGL);
            VertexArray.Bind();

            // Create the vertex buffer and bind it
            VertexBuffer = new VertexBuffer(OpenGL, Vertex.Size * 1000);
            VertexBuffer.Bind();

            // Create the index buffer (which binds the buffer)
            IndexBuffer = new IndexBuffer(OpenGL, sizeof(uint) * 1000);
            IndexBuffer.Bind();

            // Create the shader program
            Shader = new Shader(OpenGL, "Basic.shader", 8, MainWindow.Size);

            Shader.BindTexture("silk.png",  1);
            Shader.BindTexture("silk2.png", 2);

            // Register the position attribute as 3 floats
            AddAttribute(3);
            // Register the color attribute as 4 floats
            AddAttribute(4);
            // Register the texture coordinate attribute as 2 floats
            AddAttribute(2);
            // Register the texture index attribute as 1 float
            AddAttribute(1);

            // Unbind
            VertexArray.Unbind();
            VertexBuffer.Unbind();
            IndexBuffer.Unbind();

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
            // Bind the vertex array
            VertexArray.Bind();

            // Create a quad
            Vertices.AddRange(new Vertex(100.0f, 100.0f, 0.0f, 0.5f, 0.0f, 0.5f, 1.0f, 0.0f, 0.0f, 1.0f).Vertices);
            Vertices.AddRange(new Vertex(200.0f, 100.0f, 0.0f, 0.5f, 0.0f, 0.5f, 1.0f, 1.0f, 0.0f, 1.0f).Vertices);
            Vertices.AddRange(new Vertex(200.0f, 200.0f, 0.0f, 0.5f, 0.0f, 0.5f, 1.0f, 1.0f, 1.0f, 1.0f).Vertices);
            Vertices.AddRange(new Vertex(100.0f, 200.0f, 0.0f, 0.5f, 0.0f, 0.5f, 1.0f, 0.0f, 1.0f, 1.0f).Vertices);
            Indices.AddRange(QuadIndices.GetIndices(0));


            // Create a quad
            Vertices.AddRange(new Vertex(300.0f, 300.0f, 1.0f, 0.5f, 0.0f, 0.5f, 1.0f, 0.0f, 0.0f, 2.0f).Vertices);
            Vertices.AddRange(new Vertex(400.0f, 300.0f, 1.0f, 0.5f, 0.0f, 0.5f, 1.0f, 1.0f, 0.0f, 2.0f).Vertices);
            Vertices.AddRange(new Vertex(400.0f, 400.0f, 1.0f, 0.5f, 0.0f, 0.5f, 1.0f, 1.0f, 1.0f, 2.0f).Vertices);
            Vertices.AddRange(new Vertex(300.0f, 400.0f, 1.0f, 0.5f, 0.0f, 0.5f, 1.0f, 0.0f, 1.0f, 2.0f).Vertices);
            Indices.AddRange(QuadIndices.GetIndices(4));

            // Add subdata to the buffer
            VertexBuffer.BufferSubData([..Vertices]);
            IndexBuffer.BufferSubData([.. Indices]);

            VertexArray.Unbind();
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
            VertexArray.Bind();
            IndexBuffer.Bind();

            // Draw the vertex array using 6 indices
            OpenGL.DrawElements(PrimitiveType.Triangles, (uint)Indices.Count, DrawElementsType.UnsignedInt, null);

            VertexArray.Unbind();
            IndexBuffer.Unbind();
        }

        private void OnClosing()
        {
            // Delete all the buffers.
            VertexBuffer.Delete();
            IndexBuffer.Delete();
            VertexArray.Delete();
            Shader.Delete();
        }

        #endregion

        #region Private Methods

        #endregion
    }
}
