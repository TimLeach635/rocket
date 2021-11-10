using System;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace RocketGraphics
{
    public class Iss
    {
        private int _elementBufferObject;
        private readonly uint[] _indices;
        private Matrix4 _model;
        private int _modelUniformLocation;
        private int _projectionUniformLocation;
        private Shader _shader;
        private readonly Texture _texture;
        private readonly int _textureInt;
        private readonly string _textureName;
        private int _textureUniformLocation;
        private readonly TextureUnit _textureUnit;
        private int _vertexArrayObject;
        private int _vertexBufferObject;
        private readonly float[] _vertices;
        private int _viewUniformLocation;
        private readonly float _worldUnitsPerMetre;

        public Iss(float worldUnitsPerMetre, Texture texture, TextureUnit textureUnit)
        {
            _worldUnitsPerMetre = worldUnitsPerMetre;
            _texture = texture;
            _textureUnit = textureUnit;
            switch (textureUnit)
            {
                case TextureUnit.Texture0:
                    _textureName = "texture0";
                    _textureInt = 0;
                    break;
                case TextureUnit.Texture1:
                    _textureName = "texture1";
                    _textureInt = 1;
                    break;
                default:
                    throw new NotImplementedException(
                        "Tim needs to add another case to the TextureUnit switch statement!");
            }

            // texture is 10px per metre, and is 1350px wide, therefore 135 "metres" across
            _vertices = new[]
            {
                // x                             y   z                              tx ty
                _worldUnitsPerMetre * 135f / 2, 0, _worldUnitsPerMetre * 90f / 2, 1, 1,
                -_worldUnitsPerMetre * 135f / 2, 0, _worldUnitsPerMetre * 90f / 2, 0, 1,
                _worldUnitsPerMetre * 135f / 2, 0, -_worldUnitsPerMetre * 90f / 2, 1, 0,
                -_worldUnitsPerMetre * 135f / 2, 0, -_worldUnitsPerMetre * 90f / 2, 0, 0
            };
            _indices = new uint[]
            {
                0, 1, 2,
                1, 2, 3
            };
            Model = Matrix4.Identity;
        }

        public Matrix4 Model
        {
            get => _model;
            set => _model = value;
        }

        public void Initialise()
        {
            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(
                BufferTarget.ArrayBuffer,
                _vertices.Length * sizeof(float),
                _vertices,
                BufferUsageHint.StaticDraw
            );

            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            _elementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
            GL.BufferData(
                BufferTarget.ElementArrayBuffer,
                _indices.Length * sizeof(uint),
                _indices,
                BufferUsageHint.StaticDraw
            );
            _shader = new Shader(
                @"
          #version 330 core
          layout (location = 0) in vec3 aPos;
          layout (location = 1) in vec2 aTex;

          uniform mat4 model;
          uniform mat4 view;
          uniform mat4 projection;

          out vec2 tex;

          void main()
          {
            tex = aTex;
            gl_Position = vec4(aPos, 1.0) * model * view * projection;
          }
        ",
                @$"
          #version 330 core
          out vec4 outColour;

          in vec2 tex;

          uniform sampler2D {_textureName};

          void main()
          {{
            vec4 texColour = texture({_textureName}, tex);
            if (texColour.a < 0.1)
              discard;
            outColour = texColour;
          }}
        "
            );
            _shader.Use();
            _modelUniformLocation = GL.GetUniformLocation(_shader.Handle, "model");
            _viewUniformLocation = GL.GetUniformLocation(_shader.Handle, "view");
            _projectionUniformLocation = GL.GetUniformLocation(_shader.Handle, "projection");
            _textureUniformLocation = GL.GetUniformLocation(_shader.Handle, _textureName);

            var vertexLocation = GL.GetAttribLocation(_shader.Handle, "aPos");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(
                vertexLocation,
                3,
                VertexAttribPointerType.Float,
                false,
                5 * sizeof(float),
                0
            );

            var texCoordLocation = GL.GetAttribLocation(_shader.Handle, "aTex");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(
                texCoordLocation,
                2,
                VertexAttribPointerType.Float,
                false,
                5 * sizeof(float),
                3 * sizeof(float)
            );

            _texture.Use(TextureUnit.Texture0);
            GL.UseProgram(_shader.Handle);
            GL.Uniform1(_textureUniformLocation, _textureInt);
        }

        public void Render(Matrix4 view, Matrix4 projection)
        {
            _texture.Use(_textureUnit);
            _shader.Use();
            GL.UniformMatrix4(_modelUniformLocation, true, ref _model);
            GL.UniformMatrix4(_viewUniformLocation, true, ref view);
            GL.UniformMatrix4(_projectionUniformLocation, true, ref projection);

            GL.BindVertexArray(_vertexArrayObject);
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
        }
    }
}