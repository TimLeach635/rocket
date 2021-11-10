using OpenTK.Mathematics;

namespace RocketGraphics.Camera
{
    public class LockedCamera
    {
        private float _distanceFromOrigin = 2f;
        private readonly float _farClipping = 100f;

        // camera properties
        private readonly float _maximumDistance = 5f;
        private readonly float _minimumDistance = 0.2f;
        private readonly float _nearClipping = 0.1f;

        private float
            _verticalRotation; // -(pi/2 - 0.01) to (pi/2 - 0.01) radians, with positive angle being above the xy plane

        private float _xyRotation; // 0 to 2*pi radians, clockwise from the top

        public LockedCamera(Vector3 origin, float aspectRatio)
        {
            Origin = origin;
            AspectRatio = aspectRatio;
        }

        // camera position
        public Vector3 Origin { get; set; }

        private Vector3 _cameraPosition => Origin + Vector3.TransformVector(
            -Vector3.UnitY * _distanceFromOrigin,
            Matrix4.CreateRotationX(-_verticalRotation) * Matrix4.CreateRotationZ(-_xyRotation)
        );

        public float Fov { get; set; } = MathHelper.PiOver4;

        public float AspectRatio { get; set; }

        public Matrix4 ViewMatrix => Matrix4.LookAt(_cameraPosition, Origin, Vector3.UnitZ);

        public Matrix4 ProjectionMatrix =>
            Matrix4.CreatePerspectiveFieldOfView(Fov, AspectRatio, _nearClipping, _farClipping);

        public void RotateXY(float angle)
        {
            _xyRotation += angle;
            while (_xyRotation >= MathHelper.TwoPi) _xyRotation -= MathHelper.TwoPi;
            while (_xyRotation < 0) _xyRotation += MathHelper.TwoPi;
        }

        public void RotateVertical(float angle)
        {
            _verticalRotation = MathHelper.Clamp(
                _verticalRotation + angle,
                -MathHelper.PiOver2 + 0.01f,
                MathHelper.PiOver2 - 0.01f
            );
        }

        public void ZoomIn(float amount)
        {
            _distanceFromOrigin = MathHelper.Clamp(
                _distanceFromOrigin - amount,
                _minimumDistance,
                _maximumDistance
            );
        }
    }
}