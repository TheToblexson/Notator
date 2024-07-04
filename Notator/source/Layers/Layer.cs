using Notator.Rendering;
using Notator.Rendering.Shapes;
using Notator.source.Components;
using Silk.NET.Windowing;
using System.Drawing;

namespace Notator.Layers
{
    public class Layer
    {
        #region Fields

        /// <summary>
        /// The window instance
        /// </summary>
        private IWindow _window;

        /// <summary>
        /// The renderer instance.
        /// </summary>
        private Renderer _renderer;

        /// <summary>
        /// A collection of the layer's components.
        /// </summary>
        private static List<Component> _components = [];

        #endregion

        #region Private Properties

        /// <summary>
        /// The y coordinate of the top of the layer.
        /// </summary>
        private float WindowTop => _window.Size.Y;

        /// <summary>
        /// The y coordinate of the bottom of the layer (zero).
        /// </summary>
        private float WindowBottom => 0;

        /// <summary>
        /// The x position of the right of the layer.
        /// </summary>
        private float WindowRight => _window.Size.X;

        /// <summary>
        /// The x position of the left of the layer (zero).
        /// </summary>
        private float WindowLeft => 0;

        #endregion

        #region Constructors

        /// <summary>
        /// A UI layer
        /// </summary>
        /// <param name="renderer">The renderer</param>
        public Layer(Renderer renderer)
        {
            _window = renderer.Window;
            _renderer = renderer;
        }

        public void Update()
        {
            var taskbarPanel = new Panel("TaskbarPanel", WindowLeft, WindowTop - 30f, 0f, WindowRight, 30f, Color.Black);
            var fileButton = new Panel("FileButton", WindowLeft + 5f, WindowTop - 25f, 0f, 50f, 20f, Color.Red);
            var editButton = new Panel("EditButton", WindowLeft + 55f, WindowTop - 25f, 0f, 50f, 20f, Color.Red);
            var viewButton = new Panel("ViewButton", WindowLeft + 105f, WindowTop - 25f, 0f, 50f, 20f, Color.Green);

            // This works
            _renderer.AddShape(taskbarPanel.Name, taskbarPanel.Quad);

            // This works
            List<Component> components = [];
            components.Add(fileButton);
            _renderer.AddShape(components[0].Name, components[0].Quad);

            // This doesn't work
            _components.Add(editButton);
            _renderer.AddShape(_components[0].Name, _components[0].Quad);

            // this doesn't work
            Component[] copy = new Component[_components.Count];
            Array.Copy(_components.ToArray(), copy, _components.Count);
            _renderer.AddShape(copy[0].Name, copy[0].Quad);
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
