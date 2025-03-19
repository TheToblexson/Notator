using Notator.source.Rendering;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System.Drawing;
using System.Numerics;
using System.Reflection;
using System.Runtime.ConstrainedExecution;

namespace Notator
{
    //Working on:
    //Move Renderer to Class

    public class Application
    {
        #region Fields

        static readonly float[] vertices =
        [
            200.0f, 200.0f, 0.0f,   0.2f, 0.6f, 1.0f, 1.0f,   0.0f, 1.0f,   1.0f, //bottom-left
            200.0f, 400.0f, 0.0f,   0.2f, 0.6f, 1.0f, 1.0f,   0.0f, 0.0f,   1.0f, //top-left
            400.0f, 400.0f, 0.0f,   0.2f, 0.6f, 1.0f, 1.0f,   1.0f, 0.0f,   1.0f, //top-right
            400.0f, 200.0f, 0.0f,   0.2f, 0.6f, 1.0f, 1.0f,   1.0f, 1.0f,   1.0f, //bottom-right

            600.0f, 200.0f, 0.0f,   1.0f, 1.0f, 0.2f, 1.0f,   0.0f, 1.0f,   2.0f, //bottom-left
            600.0f, 400.0f, 0.0f,   1.0f, 1.0f, 0.2f, 1.0f,   0.0f, 0.0f,   2.0f, //top-left
            800.0f, 400.0f, 0.0f,   1.0f, 1.0f, 0.2f, 1.0f,   1.0f, 0.0f,   2.0f, //top-right
            800.0f, 200.0f, 0.0f,   1.0f, 1.0f, 0.2f, 1.0f,   1.0f, 1.0f,   2.0f, //bottom-right
        ];
        static readonly uint[] indices =
        [
            0u, 1u, 2u, 2u, 3u, 0u,
            4u, 5u, 6u, 6u, 7u, 4u
        ];

        #endregion

        #region Properties

        private static string Title { set; get; } = "Title";
        private static Vector2D<int> Size { set; get; } = new(800, 600);
        private static IWindow AppWindow { set; get; }
        private static Renderer Renderer { set; get; }

        #endregion

        #region Constructor

        // Warning suppressed because there is a check in OnLoad for if OpenGL is null.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        static Application()
        #pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            WindowOptions options = WindowOptions.Default;
            options.Size = Size;
            options.Title = Title;

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

            Renderer = new(AppWindow, "Shader.shader");

            Renderer.AddTexture("silk.png");
            Renderer.AddTexture("silk2.png");
        }

        private static void OnRender(double deltaTime) 
        {
            Renderer.UpdateBuffers(vertices, indices);

            Renderer.Render();
        }

        private static void OnFramebufferResize(Vector2D<int> size)
        {
            Renderer.ResizeWindow(size);
        }

        private static void OnClose()
        {
            Renderer.Dispose();
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
