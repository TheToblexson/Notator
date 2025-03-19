using Silk.NET.OpenGL;
using StbImageSharp;
using System.Diagnostics;
using System.Xml.Linq;

namespace Notator.source.Rendering
{
    public class TextureObject : IDisposable
    {
        private GL OpenGL { init; get; }
        private uint ID { init; get; }

        public TextureObject(GL openGL, string filepath)
        {
            OpenGL = openGL;
            ID = OpenGL.GenTexture();

            Bind();

            ImageResult image = ImageResult.FromMemory(File.ReadAllBytes(filepath), ColorComponents.RedGreenBlueAlpha);
            ReadOnlySpan<byte> data = new(image.Data);

            OpenGL.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba,
                (uint)image.Width, (uint)image.Height, 0, PixelFormat.Rgba,
                PixelType.UnsignedByte, data);

            SetParameters();

            Unbind();
        }

        private void SetParameters()
        {
            int texWrapMode = (int)GLEnum.ClampToEdge;
            int texMinFilter = (int)GLEnum.LinearMipmapLinear;
            int texMagFilter = (int)GLEnum.Linear;

            OpenGL.TexParameterI(GLEnum.Texture2D, GLEnum.TextureWrapS, in texWrapMode);
            OpenGL.TexParameterI(GLEnum.Texture2D, GLEnum.TextureWrapT, in texWrapMode);
            OpenGL.TexParameterI(GLEnum.Texture2D, GLEnum.TextureMinFilter, in texMinFilter);
            OpenGL.TexParameterI(GLEnum.Texture2D, GLEnum.TextureMagFilter, in texMagFilter);
            OpenGL.TexParameter(GLEnum.Texture2D, GLEnum.TextureBaseLevel, 0);
            OpenGL.TexParameter(GLEnum.Texture2D, GLEnum.TextureMaxLevel, 0);

            OpenGL.GenerateMipmap(TextureTarget.Texture2D);

            OpenGL.Enable(EnableCap.Blend);
            OpenGL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        }

        public void Bind(TextureUnit textureSlot = TextureUnit.Texture0)
        {
            OpenGL.ActiveTexture(textureSlot);
            OpenGL.BindTexture(TextureTarget.Texture2D, ID);
        }

        public void Unbind(TextureUnit textureSlot = TextureUnit.Texture0)
        {
            OpenGL.ActiveTexture(textureSlot);
            OpenGL.BindTexture(TextureTarget.Texture2D, 0);
        }



        public void Dispose()
        {
            OpenGL.DeleteTexture(ID);
        }
    }
}