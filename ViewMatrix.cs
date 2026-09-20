using System;
using Client;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AotForms
{
    internal static class ViewMatrix
    {
        // Store current view matrix
        private static Matrix4x4 _viewMatrix = Matrix4x4.Identity;
        private static bool _hasMatrix = false;
        
        // Camera position (for calculations)
        private static Vector3 _cameraPosition = Vector3.Zero;
        private static Vector3 _cameraForward = Vector3.UnitZ;
        private static Vector3 _cameraUp = Vector3.UnitY;
        private static Vector3 _cameraRight = Vector3.UnitX;
        
        // Field of View (in degrees)
        private static float _fov = 90f;
        
        // Screen dimensions
        private static int _screenWidth = 1920;
        private static int _screenHeight = 1080;

        // Initialize view matrix from game
        public static void UpdateFromGame()
        {
            try
            {
                // Read view matrix from game memory
                if (Core.CameraMatrix != Matrix4x4.Identity)
                {
                    _viewMatrix = Core.CameraMatrix;
                    _hasMatrix = true;
                }
                
                // Also try to read from camera transform
                if (Core.LocalPlayer != 0 && Offsets.MainCameraTransform != 0)
                {
                    var cameraTransform = InternalMemory.Read<uint>(Core.LocalPlayer + Offsets.MainCameraTransform, out var camTransform);
                    if (cameraTransform && camTransform != 0)
                    {
                        if (Transform.GetPosition(camTransform, out var camPos))
                        {
                            _cameraPosition = camPos;
                        }
                    }
                }
            }
            catch (Exception)
            {
                _hasMatrix = false;
            }
        }

        // Build view matrix manually (if game matrix not available)
        public static void BuildViewMatrix(Vector3 position, Vector3 target, Vector3 up)
        {
            _cameraPosition = position;
            
            // Calculate forward vector
            Vector3 forward = Vector3.Normalize(target - position);
            Vector3 right = Vector3.Normalize(Vector3.Cross(forward, up));
            Vector3 newUp = Vector3.Cross(right, forward);
            
            // Build view matrix
            _viewMatrix.M11 = right.X;
            _viewMatrix.M12 = newUp.X;
            _viewMatrix.M13 = -forward.X;
            _viewMatrix.M14 = 0;
            
            _viewMatrix.M21 = right.Y;
            _viewMatrix.M22 = newUp.Y;
            _viewMatrix.M23 = -forward.Y;
            _viewMatrix.M24 = 0;
            
            _viewMatrix.M31 = right.Z;
            _viewMatrix.M32 = newUp.Z;
            _viewMatrix.M33 = -forward.Z;
            _viewMatrix.M34 = 0;
            
            _viewMatrix.M41 = -Vector3.Dot(right, position);
            _viewMatrix.M42 = -Vector3.Dot(newUp, position);
            _viewMatrix.M43 = Vector3.Dot(forward, position);
            _viewMatrix.M44 = 1;
            
            _hasMatrix = true;
        }

        // Build projection matrix
        public static Matrix4x4 BuildProjectionMatrix(float fov, float aspect, float nearPlane, float farPlane)
        {
            float yScale = 1.0f / (float)Math.Tan(fov * 0.5f * Math.PI / 180.0f);
            float xScale = yScale / aspect;
            
            Matrix4x4 proj = new Matrix4x4();
            proj.M11 = xScale;
            proj.M22 = yScale;
            proj.M33 = farPlane / (farPlane - nearPlane);
            proj.M34 = 1;
            proj.M43 = -nearPlane * farPlane / (farPlane - nearPlane);
            proj.M44 = 0;
            
            return proj;
        }

        // Get combined view projection matrix
        public static Matrix4x4 GetViewProjectionMatrix()
        {
            float aspect = (float)_screenWidth / _screenHeight;
            Matrix4x4 proj = BuildProjectionMatrix(_fov, aspect, 0.1f, 1000f);
            return _viewMatrix * proj;
        }

        // World to Screen using game's view matrix
        public static Vector2 WorldToScreen(Vector3 worldPos)
        {
            return WorldToScreen(worldPos, _viewMatrix, _screenWidth, _screenHeight);
        }

        // World to Screen with custom matrix
        public static Vector2 WorldToScreen(Vector3 worldPos, Matrix4x4 viewMatrix, int width, int height)
        {
            Vector2 result = new Vector2(-1, -1);
            
            // Transform world position to clip space
            Vector4 clipCoords = new Vector4(
                worldPos.X * viewMatrix.M11 + worldPos.Y * viewMatrix.M21 + worldPos.Z * viewMatrix.M31 + viewMatrix.M41,
                worldPos.X * viewMatrix.M12 + worldPos.Y * viewMatrix.M22 + worldPos.Z * viewMatrix.M32 + viewMatrix.M42,
                worldPos.X * viewMatrix.M13 + worldPos.Y * viewMatrix.M23 + worldPos.Z * viewMatrix.M33 + viewMatrix.M43,
                worldPos.X * viewMatrix.M14 + worldPos.Y * viewMatrix.M24 + worldPos.Z * viewMatrix.M34 + viewMatrix.M44
            );
            
            // Check if position is in front of camera
            if (clipCoords.W >= 0.001f)
            {
                float invW = 1.0f / clipCoords.W;
                float ndcX = clipCoords.X * invW;
                float ndcY = clipCoords.Y * invW;
                
                // Convert to screen coordinates
                result.X = (width / 2f) * (ndcX + 1f);
                result.Y = (height / 2f) * (1f - ndcY);
            }
            
            return result;
        }

        // Check if position is on screen
        public static bool IsOnScreen(Vector3 worldPos)
        {
            Vector2 screenPos = WorldToScreen(worldPos);
            return screenPos.X > 0 && screenPos.Y > 0 && screenPos.X < _screenWidth && screenPos.Y < _screenHeight;
        }

        // Get distance from crosshair
        public static float DistanceFromCrosshair(Vector3 worldPos)
        {
            Vector2 screenPos = WorldToScreen(worldPos);
            if (screenPos.X <= 0 || screenPos.Y <= 0)
                return float.MaxValue;
                
            Vector2 crosshair = new Vector2(_screenWidth / 2f, _screenHeight / 2f);
            return Vector2.Distance(screenPos, crosshair);
        }

        // Get camera position
        public static Vector3 GetCameraPosition()
        {
            return _cameraPosition;
        }

        // Get camera forward direction
        public static Vector3 GetCameraForward()
        {
            return _cameraForward;
        }

        // Update screen dimensions
        public static void UpdateScreenSize(int width, int height)
        {
            _screenWidth = width;
            _screenHeight = height;
        }

        // Set field of view
        public static void SetFOV(float fov)
        {
            _fov = fov;
        }

        // Check if matrix is valid
        public static bool HasValidMatrix()
        {
            return _hasMatrix && _viewMatrix != Matrix4x4.Identity;
        }

        // Get current view matrix
        public static Matrix4x4 GetCurrentMatrix()
        {
            return _viewMatrix;
        }

        // Manual World to Screen with all parameters
        public static Vector2 WorldToScreenManual(Vector3 worldPos, Vector3 cameraPos, Vector3 cameraForward, Vector3 cameraUp, float fov, int width, int height)
        {
            Vector2 result = new Vector2(-1, -1);
            
            Vector3 delta = worldPos - cameraPos;
            Vector3 forward = Vector3.Normalize(cameraForward);
            Vector3 right = Vector3.Normalize(Vector3.Cross(forward, cameraUp));
            Vector3 up = Vector3.Cross(right, forward);
            
            float dotForward = Vector3.Dot(delta, forward);
            if (dotForward <= 0.1f)
                return result;
                
            float dotRight = Vector3.Dot(delta, right);
            float dotUp = Vector3.Dot(delta, up);
            
            float fovRad = fov * (float)Math.PI / 180f;
            float tanHalfFov = (float)Math.Tan(fovRad / 2f);
            
            float screenX = (dotRight / dotForward) / tanHalfFov;
            float screenY = (dotUp / dotForward) / tanHalfFov;
            
            result.X = (screenX + 1f) * width / 2f;
            result.Y = (1f - screenY) * height / 2f;
            
            return result;
        }

        // Get 2D bounding box for ESP
        public static (Vector2 top, Vector2 bottom) GetBoundingBox(Vector3 headPos, Vector3 feetPos)
        {
            Vector2 headScreen = WorldToScreen(headPos);
            Vector2 feetScreen = WorldToScreen(feetPos);
            return (headScreen, feetScreen);
        }

        // Calculate distance from camera to position
        public static float GetDistance(Vector3 worldPos)
        {
            return Vector3.Distance(_cameraPosition, worldPos);
        }

        // Check if position is within FOV
        public static bool IsWithinFOV(Vector3 worldPos, float fovRadius)
        {
            float distance = DistanceFromCrosshair(worldPos);
            return distance <= fovRadius;
        }
    }
}