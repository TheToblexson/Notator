using Notator.source.Rendering.Shapes;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System.Drawing;
using System.Numerics;

namespace Notator.source.Rendering
{
    public class Renderer
    {
        #region Fields

        #endregion

        #region Properties

        public GL OpenGL {  get; set; }
        public BufferObject<float> VertexBuffer { get; set; }
        public BufferObject<uint> IndexBuffer { get; set; }
        public VertexArrayObject<float, uint> VertexArray { get; set; }
        public ShaderProgram Shader { get; set; }
        public List<TextureObject> Textures { get; set; } = [];
        public List<int> Samplers { set; get; } = [];
        public Matrix4x4 Model { get; set; }
        public Matrix4x4 View { get; set; }
        public Matrix4x4 Projection { get; set; }

        #endregion

        #region Constructors

        public Renderer(IWindow window, string shaderFilename)
        {
            OpenGL = window.CreateOpenGL();
            if (OpenGL == null)
                throw new Exception("OpenGL failed to initialise");

            OpenGL.ClearColor(Color.DimGray);

            Model = Matrix4x4.Identity;
            View = Matrix4x4.Identity;
            Projection = Matrix4x4.CreateOrthographicOffCenter(0, window.Size.X, 0, window.Size.Y, 1.0f, -1.0f);

            VertexBuffer = new(OpenGL, null, BufferTargetARB.ArrayBuffer);
            IndexBuffer = new(OpenGL, null, BufferTargetARB.ElementArrayBuffer);
            VertexArray = new(OpenGL, VertexBuffer, IndexBuffer);

            Shader = new(OpenGL, $"resources/shaders/{shaderFilename}");

            VertexArray.VertexAttribute(0, 3, VertexAttribPointerType.Float, 10, 0); //Coordinates
            VertexArray.VertexAttribute(1, 4, VertexAttribPointerType.Float, 10, 3); //Colour
            VertexArray.VertexAttribute(2, 2, VertexAttribPointerType.Float, 10, 7); //UV
            VertexArray.VertexAttribute(3, 1, VertexAttribPointerType.Float, 10, 8); //TextureIndex
        }

        public void AddTexture(string filename)
        {
            Shader.Bind();
            Textures.Add(new TextureObject(OpenGL, $"resources/textures/{filename}"));
            int count = Textures.Count;
            Samplers.Add(count);
            Textures[count - 1].Bind(TextureUnit.Texture0 + count);
            Shader.SetUniform("uTextures", Samplers.ToArray());
            Shader.Unbind();
        }

        public void UpdateBuffers(float[] vertices, uint[] indices)
        {
            VertexArray.BindAll();
            VertexBuffer.SetBuffer(new ReadOnlySpan<float>(vertices));
            IndexBuffer.SetBuffer(new ReadOnlySpan<uint>(indices));
            VertexArray.UnbindAll();
        }

        internal void UpdateBuffers(RenderShape[] shapes)
        {
            VertexArray.BindAll();
            //Extract vertices and indices from shapes
            List<float> vertices = new();
            List<uint> indices = new();
            for (uint i = 0; i < shapes.Length; i++)
            {
                RenderShape shape = shapes[i];
                vertices.AddRange(shape.VerticesArray);
                indices.AddRange(shape.GetOffsetIndices(shape.VertexCount * i));
            }
            ReadOnlySpan<float> verticesSpan = new(vertices.ToArray());
            VertexBuffer.SetBuffer(verticesSpan);
            ReadOnlySpan<uint> indicesSpan = new(indices.ToArray());
            IndexBuffer.SetBuffer(indicesSpan);

            VertexArray.UnbindAll();
        }

        public void Render()
        {
            OpenGL.Clear(ClearBufferMask.ColorBufferBit);

            VertexArray.BindAll();
            Shader.Bind();

            Matrix4x4 mvp = Model * View * Projection;

            Shader.SetUniform("uMVP", mvp);

            OpenGL.DrawElements(PrimitiveType.Triangles, IndexBuffer.Length, DrawElementsType.UnsignedInt, new ReadOnlySpan<uint>());

            VertexArray.UnbindAll();
            Shader.Unbind();
        }

        public void ResizeWindow(Vector2D<int> size)
        {
            OpenGL.Viewport(size);
            Projection = Matrix4x4.CreateOrthographicOffCenter(0, size.X, 0, size.Y, 1.0f, -1.0f);
        }

        public void Dispose()
        {
            VertexBuffer.Dispose();
            IndexBuffer.Dispose();
            VertexArray.Dispose();
            Shader.Dispose();
            foreach (var texture in Textures) 
                texture.Dispose();
        }

        #endregion

        #region Private Methods

        #endregion

        #region Protected Methods

        #endregion

        #region Public Methods

        #endregion
    }
}
