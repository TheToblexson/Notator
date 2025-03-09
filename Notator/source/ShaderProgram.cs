using Silk.NET.Core;
using Silk.NET.OpenGL;

namespace Notator
{
    internal class ShaderProgram : IDisposable
    {
        private GL OpenGL { init; get; }
        private uint ID { init; get; }

        public ShaderProgram(GL openGL, string filepath)
        {
            OpenGL = openGL;

            string[] files = ParseShader(filepath);

            uint vertexShader = CompileShader(ShaderType.VertexShader, files[(int)ShaderTypeIndex.Vertex]);
            uint fragmentShader = CompileShader(ShaderType.FragmentShader, files[(int)ShaderTypeIndex.Fragment]);

            ID = OpenGL.CreateProgram();

            OpenGL.AttachShader(ID, vertexShader);
            OpenGL.AttachShader(ID, fragmentShader);
            OpenGL.LinkProgram(ID);

            OpenGL.GetProgram(ID, ProgramPropertyARB.LinkStatus, out int status);
            if (status != (int)GLEnum.True)
                throw new Exception($"Program failed to link: {OpenGL.GetProgramInfoLog(ID)}");

            OpenGL.DetachShader(ID, vertexShader);
            OpenGL.DetachShader(ID, fragmentShader);
            OpenGL.DeleteShader(vertexShader);
            OpenGL.DeleteShader(fragmentShader);
        }

        public void Use()
        {
            OpenGL.UseProgram(ID);
        }

        public void SetUniform(string name, int value)
        {
            int location = OpenGL.GetUniformLocation(ID, name);
            if (location == -1)
                throw new Exception($"{name} uniform not found on shader.");
            OpenGL.Uniform1(location, value);
        }

        private uint CompileShader(ShaderType type, string file)
        {
            uint shader = OpenGL.CreateShader(type);
            OpenGL.ShaderSource(shader, file);
            OpenGL.CompileShader(shader);
            OpenGL.GetShader(shader, ShaderParameterName.CompileStatus, out int vStatus);
            if (vStatus != (int)GLEnum.True)
                throw new Exception($"Shader failed to compile: {OpenGL.GetShaderInfoLog(shader)}");
            return shader;

        }

        private static string[] ParseShader(string filepath)
        {
            StreamReader reader = new(filepath);
            string? line = reader.ReadLine();
            string[] files = new string[2];
            ShaderTypeIndex mode = ShaderTypeIndex.None;
            while (line != null)
            {
                if (line.Contains("#shader"))
                {
                    if (line.Contains("vertex"))
                        mode = ShaderTypeIndex.Vertex;
                    else if (line.Contains("fragment"))
                        mode = ShaderTypeIndex.Fragment;
                }
                else if (mode != ShaderTypeIndex.None)
                {
                    files[(int)mode] += line + "\n";
                }
                line = reader.ReadLine();
            }
            reader.Close();
            return files;
        }

        public void Dispose()
        {
            OpenGL.DeleteProgram(ID);
        }

        public enum ShaderTypeIndex
        {
            None = -1,
            Vertex = 0,
            Fragment = 1,
        }
    }
}