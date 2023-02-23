using UnityEngine;

namespace BubblePops
{
    public class CameraResolutionScaler : MonoBehaviour
    {
        private Camera _camera;
        // Default values are for 1920x1080
        private float _yAxis, _orthographicSize;

        private void Awake() 
        {
            _camera = Camera.main;  
            SetRelevantResolution();
        }

        private void SetRelevantResolution()
        {
            if (Screen.width >= 1284)
            {
                _yAxis = 0.6f;
                _orthographicSize = 4.4f;
            }
            else
            {
                if (Screen.height <= 1920 && Screen.height != 1792)
                {
                    _yAxis = 0.2f;
                    _orthographicSize = 4.5f;
                }
                else if ((Screen.height > 1920 && Screen.height < 2600) || Screen.height == 1792)
                {
                    _yAxis = -0.5f;
                    _orthographicSize = 5.45f;
                }
            }

            transform.position = new Vector3(0, _yAxis, -10);
            _camera.orthographicSize = _orthographicSize;
        }
    }
}
