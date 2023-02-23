using UnityEngine;

namespace BubblePops
{
    public class CameraBehaviour : MonoBehaviour
    {
        [Header("-- SETUP --")]
        [SerializeField, Tooltip("How long the object should shake for")] private float _shakeDuration = 0.2f;
        [SerializeField, Tooltip("Amplitude of the shake. A larger value shakes the camera harder")] private float _shakeAmount = 0.7f;
        private bool _startShaking;
        private float _shakeFinishTime;
        private Vector3 _originalPos;

        private void OnEnable()
        {
            _startShaking = false;
            _originalPos = transform.localPosition;

            CameraEvents.OnShakeCamera += () => {
                _startShaking = true;
                _shakeFinishTime = Time.time + _shakeDuration;
            };
        }

        private void OnDisable() 
        {
            CameraEvents.OnShakeCamera -= () => {
                _startShaking = true;
                _shakeFinishTime = Time.time + _shakeDuration;
            };
        }

        private void Update()
        {
            if (_startShaking)
            {
                if (_shakeFinishTime > Time.time)
                    transform.localPosition = _originalPos + Random.insideUnitSphere * _shakeAmount;
                else
                {
                    _startShaking = false;
                    transform.localPosition = _originalPos;
                }
            }
        }
    }
}
