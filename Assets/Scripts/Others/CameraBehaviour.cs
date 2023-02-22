using UnityEngine;

namespace BubblePops
{
    public class CameraBehaviour : MonoBehaviour
    {
        [Header("-- SETUP --")]
        [SerializeField, Tooltip("How long the object should shake for")] private float _shakeDuration = 0.2f;
        [SerializeField, Tooltip("Amplitude of the shake. A larger value shakes the camera harder")] private float _shakeAmount = 0.7f;
        [SerializeField] private float _decreaseFactor = 1.0f;
        private bool _startShaking;
        private float _shakeFinishTime;
        private Vector3 _originalPos;

        void OnEnable()
        {
            _startShaking = false;
            _originalPos = transform.localPosition;

            GameEvents.OnShakeCamera += () => {
                _startShaking = true;
                _shakeFinishTime = Time.time + _shakeDuration;
            };
        }

        private void OnDisable() 
        {
            GameEvents.OnShakeCamera -= () => {
                _startShaking = true;
                _shakeFinishTime = Time.time + _shakeDuration;
            };
        }

        void Update()
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
            // if (_shakeDuration > 0)
            // {
            //     transform.localPosition = _originalPos + Random.insideUnitSphere * _shakeAmount;
            //     _shakeDuration -= Time.deltaTime * _decreaseFactor;
            // }
            // else
            // {
            //     _shakeDuration = 0f;
            //     transform.localPosition = _originalPos;
            // }
        }
    }
}
