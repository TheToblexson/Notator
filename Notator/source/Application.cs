using Notator.Rendering;
using Notator.Rendering.Shapes;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System.Drawing;

namespace Notator
{ 
    public class Application
    {
        #region Private Properties

        /// <summary>
        /// The main window instance.
        /// </summary>
        private IWindow MainWindow { get; init; }

        /// <summary>
        /// The renderer instance.
        /// </summary>
        private Renderer Renderer { get; set; }

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
            // Create the renderer
            Renderer = new Renderer(MainWindow, "Basic.shader");

            // Bind the textures
            Renderer.BindTexture("silk.png",  0);
            Renderer.BindTexture("silk2.png", 1);
        }

        /// <summary>
        /// Called when an update should run.
        /// </summary>
        /// <param name="deltaTime">The time (in seconds) since the last render call.</param>
        private void OnUpdate(double deltaTime)
        {
            // Create 2 quads
            RenderShape quad1 = new RenderQuad(100f, 100f, 0f, 100f, 100f, new RenderColor(Color.Aqua));
            RenderShape quad2 = new RenderQuad(300f, 300f, 0f, 100f, 100f, 0);

            // Add the quads to the renderer
            Renderer.AddShape("quad1", quad1);
            Renderer.AddShape("quad2", quad2);

            // update the renderer
            Renderer.Update();
        }

        /// <summary>
        /// Called when a frame should be rendered.
        /// </summary>
        /// <param name="deltaTime">The time (in seconds) since the last render call.</param>
        private unsafe void OnRender(double deltaTime)
        {
            // Tell the renderer to render
            Renderer.Render();
        }

        /// <summary>
        /// Called when the window is about to close.
        /// </summary>
        private void OnClosing()
        {
            // Remove the renderer from OpenGL
            Renderer.Delete();
        }

        #endregion

        #region Private Methods

        #endregion
    }
}
