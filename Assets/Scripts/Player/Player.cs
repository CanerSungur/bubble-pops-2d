using UnityEngine;

namespace BubblePops
{
    public class Player : MonoBehaviour
    {
        #region COMPONENTS
        private PlayerTouchInput _touchInput;
        private PlayerThrowBehaviour _throwBehaviour;
        private PlayerAudioHandler _audioHandler;
        #endregion

        #region GETTERS
        public PlayerTouchInput TouchInput => _touchInput;
        public PlayerThrowBehaviour ThrowBehaviour => _throwBehaviour;
        #endregion

        private void Awake()
        {
            _touchInput = GetComponent<PlayerTouchInput>();
            _throwBehaviour = GetComponent<PlayerThrowBehaviour>();
            _audioHandler = GetComponent<PlayerAudioHandler>();
        }

        private void Start()
        {
            _touchInput.Init(this);
            _throwBehaviour.Init(this);
            _audioHandler.Init(this);
        }
    }
}
