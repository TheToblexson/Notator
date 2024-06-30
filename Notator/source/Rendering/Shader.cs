using Silk.NET.Maths;
using Silk.NET.OpenGL;
using StbImageSharp;

namespace Notator.Rendering
{
    internal class Shader
    {
        #region Structs

        /// <summary>
        /// A holder for the display name and index of a shader type.
        /// </summary>
        /// <param name="name">The display name of the shader. (ie. 'Fragment')</param>
        /// <param name="index">The location index of the shader.</param>
        struct ShaderInfo(string name, int index, ShaderType type)
        {
            /// <summary>
            /// The display name of the shader.
            /// </summary>
            public readonly string Name => name;

            /// <summary>
            /// The location index of the shader.
            /// </summary>
            public readonly int Index => index;
            public readonly ShaderType Type => type;
        }

        /// <summary>
        /// A collection of the shader information for the shader types.
        /// </summary>
        private static ShaderInfo[] ShaderTypes =>
        [
            new("Vertex",   0, ShaderType.VertexShader),
            new("Fragment", 1, ShaderType.FragmentShader)
        ];

        #endregion

        #region Private Properties

        /// <summary>
        /// A reference to the OpenGL instance.
        /// </summary>
        private GL OpenGL { get; init; }

        /// <summary>
        /// The id of the vertex array.
        /// </summary>
        public uint Id { get; init; }

        /// <summary>
        /// If the vertex array is bound.
        /// </summary>
        private bool IsBound { get; set; } = false;

        /// <summary>
        /// The array of the texture sampler ids.
        /// </summary>
        private int[] Samplers { get; init; }

        /// <summary>
        /// The cache for the uniform location ids.
        /// </summary>
        private Dictionary<string, int> LocationCache { get; } = [];

        #endregion

        #region Constructors

        /// <summary>
        /// Creates and provides access to a shader program.
        /// </summary>
        /// <param name="openGL">The OpenGL instance.</param>
        /// <param name="fileName">The name of the shader file in resources/shaders.</param>
        /// <param name="textureSamplerCount">The desired size of the texture sampler array.</param>
        public Shader(GL openGL, string fileName, uint textureSamplerCount, Vector2D<int> windowSize)
        {
            OpenGL = openGL;
            Id = OpenGL.CreateProgram();
            Samplers = BuildSamplerArray(textureSamplerCount);

            // Set the flip flag because OpenGL reads bottom up.
            StbImage.stbi_set_flip_vertically_on_load(1);

            // Create the shader program
            CreateProgram(fileName);

            // Set the texture (samplers) uniform
            SetUniform1("uTextures", Samplers);

            // Create the projection matrix
            Matrix4X4<float> projectionMatrix = Matrix4X4.CreateOrthographicOffCenter(0.0f, windowSize.X, 0.0f, windowSize.Y, -1.0f, 1.0f);

            //Set the mvp uniform
            SetUniformMatrix("uMVP",  projectionMatrix);
        }

        #endregion


        #region Private Methods

        /// <summary>
        /// Create and populate the sampler array.
        /// </summary>
        /// <param name="size">The desired size of the texture sampler array.</param>
        private int[] BuildSamplerArray(uint size)
        {
            int[] array = new int[size];
            for (int i = 0; i < size; i++)
                array[i] = i;
            return array;
        }

        /// <summary>
        /// Create the shader program and attach the shader.
        /// </summary>
        /// <param name="fileName">The name of the shader file in resources/shaders.</param>
        private void CreateProgram(string fileName)
        {
            // Get the shader code, compile it, and then attach it to the shader program
            uint[] shaders = CompileAndAttachShaders(ReadShaderFile("Basic.shader"));

            // Link the program
            OpenGL.LinkProgram(Id);

            // Check that the program linked
            OpenGL.GetProgram(Id, ProgramPropertyARB.LinkStatus, out int linkStatus);
            if (linkStatus != (int)GLEnum.True)
                throw new Exception("Program failed to link: " + OpenGL.GetProgramInfoLog(Id));

            // Detach and delete the shaders
            foreach (uint shader in shaders)
            {
                OpenGL.DeleteShader(shader);
                OpenGL.DeleteShader(shader);
            }

            //Bind the shader
            Bind();
        }

        /// <summary>
        /// Create and compile the shaders and then attach them to the program
        /// </summary>
        /// <param name="shaderSource">The array container the shaders</param>
        private uint[] CompileAndAttachShaders(string[] shaderSource)
        {
            uint[] shaders = new uint[shaderSource.Length];

            for (int i = 0; i < shaderSource.Length; i++)
            {
                // Create the vertex shader
                uint shader = OpenGL.CreateShader(ShaderTypes[i].Type);

                // Set the shader source code
                OpenGL.ShaderSource(shader, shaderSource[i]);

                // Compile the vertex shader
                OpenGL.CompileShader(shader);

                // Check that the shader has compiled
                OpenGL.GetShader(shader, ShaderParameterName.CompileStatus, out int status);
                if (status != (int)GLEnum.True)
                    throw new Exception($"{ShaderTypes[i].Name} shader failed to compile: " + OpenGL.GetShaderInfoLog(shader));

                // Attach the shaders to the program
                OpenGL.AttachShader(Id, shader);

                shaders[i] = shader;
            }
            return shaders;
        }

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
            StringWriter[] shaderWriter = new StringWriter[ShaderTypes.Length];

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
                    foreach (ShaderInfo info in ShaderTypes)
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

        /// <summary>
        /// Get the texture unit from the slot index.
        /// </summary>
        /// <param name="slotIndex">The slot index.</param>
        private static TextureUnit GetTextureUnit(uint slotIndex)
        {
            return (TextureUnit)((int)TextureUnit.Texture0 + slotIndex);
        }

        /// <summary>
        /// Get the uniform location from the cache or OpenGL. 
        /// </summary>
        /// <param name="name">The name of the uniform.</param>
        private int GetLocation(string name)
        {
            // Check if the location cache contains this location
            if (LocationCache.TryGetValue(name, out int value))
                return value;

            // Get the uniform location
            int location = OpenGL.GetUniformLocation(Id, name);

            // Output a warning if the location is -1 (not found), because this may or may not be intentional
            if (location == -1)
            {
                //Todo: Write logging class
                Console.WriteLine($"Warning, uniform location '{name}' not found.");
            }
            LocationCache[name] = location;
            return location;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Bind a texture to the selected texture slot.
        /// </summary>
        /// <param name="fileName">The name of the file inside resources/textures</param>
        /// <param name="slotIndex">The slot index.</param>
        public unsafe void BindTexture(string fileName, uint slotIndex)
        {
            // Create the texture object
            uint texture = OpenGL.GenTexture();

            // Set the flip flag because OpenGL reads bottom up.
            StbImage.stbi_set_flip_vertically_on_load(1);

            // Get the image
            ImageResult image = ImageResult.FromMemory(File.ReadAllBytes($"resources/textures/{fileName}"), ColorComponents.RedGreenBlueAlpha);

            // Bind the texture to a texture slot
            OpenGL.ActiveTexture(GetTextureUnit(slotIndex));
            OpenGL.BindTexture(TextureTarget.Texture2D, texture);

            // Attach the image to the texture
            fixed (byte* ptr = image.Data)
                OpenGL.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, (uint)image.Width,
                    (uint)image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, ptr);

            // Set the texture parameters
            int wrapParameter = (int)TextureWrapMode.ClampToEdge;
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

            // Bind the texture to the slot
            OpenGL.BindTextureUnit(slotIndex, texture);
        }

        /// <summary>
        /// Bind the shader program.
        /// </summary>
        public void Bind()
        {
            if (!IsBound)
            {
                OpenGL.UseProgram(Id);
                IsBound = true;
            }
        }

        /// <summary>
        /// Unbind the shader program.
        /// </summary>
        public void Unbind()
        {
            if (IsBound)
            {
                OpenGL.UseProgram(0);
                IsBound = false;
            }
        }

        /// <summary>
        /// Delete the shader program from OpenGL.
        /// </summary>
        public void Delete()
        {
            OpenGL.DeleteProgram(Id);
        }

        #endregion

        #region Public Uniform Methods

        /// <summary>
        /// Set a 1 dimensional uniform.
        /// </summary>
        /// <param name="name">The name of the uniform.</param>
        /// <param name="array">The array to assign to the uniform.</param>
        private void SetUniform1(string name, int[] array)
        {
            OpenGL.Uniform1(GetLocation(name), array);
        }

        /// <summary>
        /// Set a matrix uniform
        /// </summary>
        /// <param name="name">The name of the uniform.</param>
        /// <param name="matrix">The matrix to assign to the uniform.</param>
        private unsafe void SetUniformMatrix(string name, Matrix4X4<float> matrix)
        {
            OpenGL.UniformMatrix4(GetLocation(name), 1, false, (float*)&matrix);
        }

        #endregion
    }
}
