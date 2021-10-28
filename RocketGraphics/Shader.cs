using System;
using OpenTK.Graphics.OpenGL;

namespace RocketGraphics
{
  public class Shader
  {
    public readonly int Handle;

    public Shader(string vertexShaderSource, string fragmentShaderSource)
    {
      int vertexShader = GL.CreateShader(ShaderType.VertexShader);
      GL.ShaderSource(vertexShader, vertexShaderSource);
      GL.CompileShader(vertexShader);
      GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out var code);
      if (code != (int)All.True)
      {
          var infoLog = GL.GetShaderInfoLog(vertexShader);
          throw new Exception($"Error occurred whilst compiling Shader({vertexShader}).\n\n{infoLog}");
      }

      int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
      GL.ShaderSource(fragmentShader, fragmentShaderSource);
      GL.CompileShader(fragmentShader);
      GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out code);
      if (code != (int)All.True)
      {
        var infoLog = GL.GetShaderInfoLog(vertexShader);
        throw new Exception($"Error occurred whilst compiling Shader({vertexShader}).\n\n{infoLog}");
      }

      Handle = GL.CreateProgram();
      GL.AttachShader(Handle, vertexShader);
      GL.AttachShader(Handle, fragmentShader);
      GL.LinkProgram(Handle);
      GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out code);
      if (code != (int)All.True)
      {
        throw new Exception($"Error occurred whilst linking Program({Handle})");
      }

      GL.DetachShader(Handle, vertexShader);
      GL.DetachShader(Handle, fragmentShader);
      GL.DeleteShader(vertexShader);
      GL.DeleteShader(fragmentShader);
    }

    public void Use()
    {
      GL.UseProgram(Handle);
    }
  }
}
