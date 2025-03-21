using Notator.source.Rendering;
using Notator.source.Rendering.Shapes;
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
    //RenderLayers

    public class Application
    {
        #region Fields

        static readonly RenderShape[] shapes =
            [
                new RenderRectangle(new(200f,200f,0f), new(200f,200f), new(0.2f,0.6f,1.0f,1.0f), 1),
                new RenderRectangle(new(400f,200f,0f), new(200f,200f), new(1.0f,1.0f,0.2f,1.0f), 2),
            ];

        static float change = 0.5f;

        #endregion

        #region Properties

        private static string Title { set; get; } = "Title";
        private static Vector2D<int> Size { set; get; } = new(800, 600);
        private static IWindow AppWindow { set; get; }
        private static Renderer Renderer { set; get; }

        #endregion

        #region Constructor

        // Warning suppressed because the renderer is created in OnLoad
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
            var colour = shapes[0].GetColour();
            float newR = colour.R + (change * (float)deltaTime);
            if (newR > 1 || newR < 0)
            {
                change *= -1;
            }

            shapes[0].SetColour(new(newR, colour.G, colour.B, colour.A));

            Renderer.UpdateBuffers(shapes);
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
