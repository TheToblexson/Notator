using Notator.Layers;
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
        #region Fields

        /// <summary>
        /// The main window instance.
        /// </summary>
        private IWindow _window;

        /// <summary>
        /// The renderer instance.
        /// </summary>
        private Renderer _renderer;

        /// <summary>
        /// A collection of the UI layers.
        /// </summary>
        private Dictionary<string,Layer> _layers = [];

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
            _window = Window.Create(options);

            // Attach methods to the window events
            _window.Load += OnLoad;
            _window.Update += OnUpdate;
            _window.Render += OnRender;
            _window.Closing += OnClosing;

            // Tell the window to run
            _window.Run();
        }

        #endregion

        #region Event Methods

        /// <summary>
        /// Called when the window first begins to run.
        /// </summary>
        private unsafe void OnLoad()
        {
            // Create the renderer
            _renderer = new Renderer(_window, "Basic.shader");

            // Bind the textures
            _renderer.BindTexture("silk.png",  0);
            _renderer.BindTexture("silk2.png", 1);

            // Add layers
            _layers.Add("Taskbar", new Layer(_renderer));
        }

        /// <summary>
        /// Called when an update should run.
        /// </summary>
        /// <param name="deltaTime">The time (in seconds) since the last render call.</param>
        private void OnUpdate(double deltaTime)
        {
            foreach (Layer layer in _layers.Values)
            {
                layer.Update();
            }

            // update the renderer
            _renderer.Update();
        }

        /// <summary>
        /// Called when a frame should be rendered.
        /// </summary>
        /// <param name="deltaTime">The time (in seconds) since the last render call.</param>
        private unsafe void OnRender(double deltaTime)
        {
            // Tell the renderer to render
            _renderer.Render();
        }

        /// <summary>
        /// Called when the window is about to close.
        /// </summary>
        private void OnClosing()
        {
            // Remove the renderer from OpenGL
            _renderer.Delete();
        }

        #endregion

        #region Private Methods

        #endregion
    }
}
