using Notator.source;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using StbImageSharp;
using System.Diagnostics;
using System.Drawing;

namespace Notator
{
    //Next: 1.5

    public class Application
    {
        #region Fields

        static float[] vertices =
        [
             0.5f,  0.5f, 0.5f,     1.0f, 0.0f,
             0.5f, -0.5f, 0.0f,     1.0f, 1.0f,
            -0.5f, -0.5f, 0.0f,     0.0f, 1.0f,
            -0.5f,  0.5f, 0.0f,     0.0f, 0.0f
        ];
        static uint[] indices =
        [
            0u, 1u, 3u,
            1u, 2u, 3u
        ];

        #endregion

        #region Properties

        private static IWindow AppWindow { set; get; }
        private static GL OpenGL { set; get; }
        private static BufferObject<float> VertexBuffer { set; get; }
        private static BufferObject<uint> IndexBuffer { set; get; }
        private static VertexArrayObject<float, uint> VertexArray { set; get; }
        private static ShaderProgram Shader {  set; get; }
        private static TextureObject Texture { set; get; }

        #endregion

        #region Constructor

        // Warning suppressed because there is a check in OnLoad for if OpenGL is null.
        #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        static Application()
        #pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            WindowOptions options = WindowOptions.Default with
            {
                Size = new Vector2D<int>(800, 600),
                Title = "Title"
            };

            AppWindow = Window.Create(options);
            if (AppWindow == null)
                throw new Exception("Window failed to create!");

            AppWindow.Load += OnLoad;
            AppWindow.Render += OnRender;
            AppWindow.FramebufferResize += OnFramebufferResize;
            AppWindow.Closing += OnClose;

            AppWindow.Run();
        }

        #endregion

        #region Private Methods

        private static void OnLoad()
        {

            IInputContext input = AppWindow.CreateInput();

            foreach (IKeyboard keyboard in input.Keyboards)
                keyboard.KeyDown += KeyDown;

            OpenGL = AppWindow.CreateOpenGL();
            if (OpenGL == null)
                throw new Exception("OpenGL failed to initialise");

            OpenGL.ClearColor(Color.CornflowerBlue);

            VertexBuffer = new(OpenGL, new ReadOnlySpan<float>(vertices), BufferTargetARB.ArrayBuffer);
            IndexBuffer = new(OpenGL, new ReadOnlySpan<uint>(indices), BufferTargetARB.ElementArrayBuffer);
            VertexArray = new(OpenGL, VertexBuffer, IndexBuffer);

            VertexArray.VertexAttribute(0, 3, VertexAttribPointerType.Float, 5, 0);
            VertexArray.VertexAttribute(1, 2, VertexAttribPointerType.Float, 5, 3);

            Shader = new(OpenGL, "resources/shaders/Shader.shader");

            Texture = new(OpenGL, "resources/textures/silk.png");
        }

        private static void OnRender(double deltaTime) 
        {
            OpenGL.Clear(ClearBufferMask.ColorBufferBit);

            VertexArray.Bind();
            Shader.Use();
            Texture.Bind(TextureUnit.Texture0);

            Shader.SetUniform("uTexture", 0);

            OpenGL.DrawElements(PrimitiveType.Triangles, (uint)indices.Length, DrawElementsType.UnsignedInt, new ReadOnlySpan<uint>());
        }

        private static void OnFramebufferResize(Vector2D<int> size)
        {
            OpenGL.Viewport(size);
        }

        private static void OnClose()
        {
            VertexBuffer.Dispose();
            IndexBuffer.Dispose();
            VertexArray.Dispose();
            Shader.Dispose();
            Texture.Dispose();
        }

        private static void KeyDown(IKeyboard keyboard, Key key, int keyCode)
        {
            if (key == Key.Escape)
                AppWindow.Close();
        }

        #endregion

        #region Protected Methods

        #endregion

        #region Public Methods

        #endregion
    }
}
