using UnityEngine;

namespace BubblePops
{
    public class PlayerThrowBehaviour : MonoBehaviour
    {
        #region COMPONENTS
        private Player _player;
        #endregion

        #region FIELDS
        private EmptySlot _currentEmptySlot = null;
        private bool _throwSequenceIsFinished;
        #endregion

        #region GETTERS
        public EmptySlot CurrentEmptySlot => _currentEmptySlot;
        public bool ThrowSequenceIsFinished => _throwSequenceIsFinished;
        #endregion

        public void Init(Player player)
        {
            _player = player;
            _throwSequenceIsFinished = true;

            PlayerEvents.OnThrowSuccessful += ThrowSuccessful;
        }

        private void OnDisable()
        {
            if (_player == null) return;
            PlayerEvents.OnThrowSuccessful -= ThrowSuccessful;
        }

        #region EVENT HANDLER FUNCTIONS
        private void ThrowSuccessful()
        {
            _throwSequenceIsFinished = true;
        }
        #endregion

        #region THROW FUNCTIONS
        public void TriggerThrow(Vector2? bouncePosition = null)
        {
            BubbleManager.FirstThrowableBubble.SetRowAndColumn(_currentEmptySlot);

            if (bouncePosition == null)
                BubbleManager.FirstThrowableBubble.ThrowHandler.GetThrown(_currentEmptySlot);
            else
                BubbleManager.FirstThrowableBubble.ThrowHandler.GetThrown(_currentEmptySlot, bouncePosition);

            _throwSequenceIsFinished = false;
        }
        #endregion

        #region EMPTY SLOT FUNCTIONS
        public void UpdateEmptySlot(EmptySlot emptySlot)
        {
            if (emptySlot.IsEnabled) return;

            if (_currentEmptySlot == null)
            {
                _currentEmptySlot = emptySlot;
                _currentEmptySlot.Enable();
            }
            else
            {
                if (_currentEmptySlot == emptySlot)
                    _currentEmptySlot.Enable();
                else
                {
                    _currentEmptySlot.Disable();
                    _currentEmptySlot = emptySlot;
                    _currentEmptySlot.Enable();
                }
            }
        }
        public void DisableEmptySlot()
        {
            if (_currentEmptySlot == null || !_currentEmptySlot.IsEnabled) return;
            _currentEmptySlot.Disable();
            _currentEmptySlot = null;
        }
        #endregion
    }
}
