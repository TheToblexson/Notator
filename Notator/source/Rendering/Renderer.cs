using Notator.Rendering.Shapes;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System.Diagnostics;
using System.Drawing;

namespace Notator.Rendering
{
    public class Renderer
    {
        #region Private Properties

        /// <summary>
        /// The number of attribute sets in the vertices.
        /// </summary>
        private static uint AttributesCount { get; set; } = 0;

        /// <summary>
        /// A pointer to the end of the attributes
        /// </summary>
        private static nint AttributesPointer { get; set; } = 0;

        /// <summary>
        /// A collection of the shapes to render.
        /// </summary>
        private Dictionary<string, RenderShape> Shapes { get; } = [];

        private uint VertexCount { get; set; } = 0;

        #endregion

        #region Public Properties

        /// <summary>
        /// The window that this renderer is attached to.
        /// </summary>
        public IWindow MainWindow { get; }

        /// <summary>
        /// The OpenGL instance
        /// </summary>
        public GL OpenGL { get; }

        /// <summary>
        /// The vertex array instace.
        /// </summary>
        public VertexArray VertexArray { get; }

        /// <summary>
        /// The vertex buffer instance.
        /// </summary>
        public VertexBuffer VertexBuffer { get; }

        /// <summary>
        /// The index buffer instance.
        /// </summary>
        public IndexBuffer IndexBuffer { get; }

        /// <summary>
        /// The shader instance.
        /// </summary>
        public Shader Shader { get; }

        #endregion 

        #region Constructors

        /// <summary>
        /// The renderer for the given window.
        /// </summary>
        /// <param name="mainWindow">The window to attach the renderer to.</param>
        /// <param name="shaderFileName">The file name of the shader (inside resources/shaders).</param>
        public Renderer(IWindow mainWindow, string shaderFileName)
        {
            MainWindow = mainWindow;

            // Create the OpenGL instance
            OpenGL = MainWindow.CreateOpenGL();

            // Set the clear color
            OpenGL.ClearColor(Color.Gray);

            // Create a vertex array and bind it
            VertexArray = new VertexArray(OpenGL);
            VertexArray.Bind();

            // Create the vertex buffer and bind it
            VertexBuffer = new VertexBuffer(OpenGL, RenderVertex.Size * 1000);
            VertexBuffer.Bind();

            // Create the index buffer (which binds the buffer)
            IndexBuffer = new IndexBuffer(OpenGL, sizeof(uint) * 1000);
            IndexBuffer.Bind();

            // Create the shader program
            Shader = new Shader(OpenGL, "Basic.shader", 8, MainWindow.Size);

            // Add the position, color, texture coordinate and texture index attributes
            AddAttribute(3, 4, 2, 1);

            Unbind();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Register an attribute.
        /// </summary>
        /// <param name="count">The number of values in the attribute.</param>
        private void AddAttribute(uint count)
        {
            OpenGL.VertexAttribPointer(AttributesCount, (int)count, VertexAttribPointerType.Float, false, RenderVertex.Size, AttributesPointer);
            OpenGL.EnableVertexAttribArray(AttributesCount);
            AttributesCount += 1;
            AttributesPointer += (nint)(count * sizeof(float));
        }

        /// <summary>
        /// Register attributes.
        /// </summary>
        /// <param name="count">The number of values in the attribute.</param>
        private void AddAttribute(params uint[] counts)
        {
            foreach (uint count in counts)
                AddAttribute(count);
        }

        /// <summary>
        /// Unbind the vertex array, buffers and shader.
        /// </summary>
        private void Unbind()
        {
            VertexArray.Unbind();
            VertexBuffer.Unbind();
            IndexBuffer.Unbind();
            Shader.Unbind();
        }

        /// <summary>
        /// Bind the vertex array, buffers and shader.
        /// </summary>
        private void Bind()
        {
            VertexArray.Bind();
            VertexBuffer.Bind();
            IndexBuffer.Bind();
            Shader.Bind();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Bind a texture to the given slot index.
        /// </summary>
        /// <param name="fileNmae">The file name of the texture (inside resources/textures)</param>
        /// <param name="slotIndex">The slot index to insert the texture into.</param>
        public void BindTexture(string fileNmae, uint slotIndex)
        {
            Shader.Bind();
            Shader.BindTexture(fileNmae, slotIndex);
            Shader.Unbind();
        }

        /// <summary>
        /// Add a shape to the renderer.
        /// </summary>
        /// <param name="name">The name of the shape. This must be unique.</param>
        /// <param name="shape">The shape to add to the renderer.</param>
        public void AddShape(string name, RenderShape shape)
        {
            // Offset indices
            shape.OffsetIndices(VertexCount);

            // Increment the vertex count;
            VertexCount += shape.VertexCount;

            // Add to the dictionary if it isn't already in there.
            Shapes.TryAdd(name, shape);
        }

        /// <summary>
        /// Update the renderer. 
        /// </summary>
        public void Update()
        {
            List<float> vertices = [];
            List<uint> indices = [];

            foreach (RenderShape shape in Shapes.Values)
            {
                vertices.AddRange(shape.Vertices);
                indices.AddRange(shape.Indices);
            }

            // Add subdata to the buffer
            VertexBuffer.BufferSubData([.. vertices]);
            IndexBuffer.BufferSubData([.. indices]);
        }

        public unsafe void Render()
        {
            Bind();

            // Clear the frame
            OpenGL.Clear(ClearBufferMask.ColorBufferBit);

            // Get the indices count
            uint indicesCount = 0;
            foreach (var shape in Shapes.Values)
                indicesCount += (uint)shape.Indices.Count;

            // Draw the vertex array
            OpenGL.DrawElements(PrimitiveType.Triangles, indicesCount, DrawElementsType.UnsignedInt, null);

            Unbind();
        }

        /// <summary>
        /// delete all the content from OpenGL.
        /// </summary>
        public void Delete()
        {
            // Delete all the buffers.
            VertexBuffer.Delete();
            IndexBuffer.Delete();
            VertexArray.Delete();
            Shader.Delete();
        }

        #endregion
    }
}
