using System;
using OpenTK.Mathematics;

namespace RocketGraphics.Camera
{
  public class LockedCamera
  {
    // camera position
    public Vector3 Origin { get; set; }
    private float _distanceFromOrigin = 2f;
    private float _xyRotation = 0f; // 0 to 2*pi radians, clockwise from the top
    private float _verticalRotation = 0f; // -(pi/2 - 0.01) to (pi/2 - 0.01) radians, with positive angle being above the xy plane

    private Vector3 _cameraPosition => Origin + Vector3.TransformVector(
      -Vector3.UnitY * _distanceFromOrigin,
      Matrix4.CreateRotationX(-_verticalRotation) * Matrix4.CreateRotationZ(-_xyRotation)
    );

    // camera properties
    private float _fov = MathHelper.PiOver4;
    private float _nearClipping = 0.1f;
    private float _farClipping = 100f;
    public float Fov {
      get
      {
        return _fov;
      }
      set
      {
        _fov = value;
      }
    }
    public float AspectRatio { get; set; }

    public Matrix4 ViewMatrix => Matrix4.LookAt(_cameraPosition, Origin, Vector3.UnitZ);

    public Matrix4 ProjectionMatrix => Matrix4.CreatePerspectiveFieldOfView(_fov, AspectRatio, _nearClipping, _farClipping);

    public void RotateXY(float angle)
    {
      _xyRotation += angle;
      while (_xyRotation >= MathHelper.TwoPi)
      {
        _xyRotation -= MathHelper.TwoPi;
      }
      while (_xyRotation < 0)
      {
        _xyRotation += MathHelper.TwoPi;
      }
    }

    public void RotateVertical(float angle)
    {
      _verticalRotation = MathHelper.Clamp(
        _verticalRotation + angle,
        -MathHelper.PiOver2 + 0.01f,
        MathHelper.PiOver2 - 0.01f
      );
    }

    public LockedCamera(Vector3 origin, float aspectRatio)
    {
      Origin = origin;
      AspectRatio = aspectRatio;
    }
  }
}
