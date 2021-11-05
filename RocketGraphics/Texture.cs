using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL4;

namespace RocketGraphics
{
  public class Texture
  {
    public readonly int Handle;

    public static Texture LoadFromFile(string path)
    {
      int handle = GL.GenTexture();
      GL.ActiveTexture(TextureUnit.Texture0);
      GL.BindTexture(TextureTarget.Texture2D, handle);

      using (var image = new Bitmap(path))
      {
        image.RotateFlip(RotateFlipType.RotateNoneFlipY);
        var data = image.LockBits(
          new Rectangle(0, 0, image.Width, image.Height),
          ImageLockMode.ReadOnly,
          System.Drawing.Imaging.PixelFormat.Format32bppArgb
        );
        GL.TexImage2D(
          TextureTarget.Texture2D,
          0,
          PixelInternalFormat.Rgba,
          image.Width,
          image.Height,
          0,
          OpenTK.Graphics.OpenGL4.PixelFormat.Bgra,
          PixelType.UnsignedByte,
          data.Scan0
        );
      }

      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
      GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

      return new Texture(handle);
    }

    public Texture(int glHandle)
    {
      Handle = glHandle;
    }

    public void Use(TextureUnit unit)
    {
      GL.ActiveTexture(unit);
      GL.BindTexture(TextureTarget.Texture2D, Handle);
    }
  }
}
