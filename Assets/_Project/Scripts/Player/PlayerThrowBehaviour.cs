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
        #endregion

        #region GETTERS
        public EmptySlot CurrentEmptySlot => _currentEmptySlot;
        #endregion

        public void Init(Player player)
        {
            _player = player;
        }

        #region THROW FUNCTIONS
        public void TriggerThrow(Vector2? bouncePosition = null)
        {
            BubbleManager.FirstThrowableBubble.SetRowAndColumn(_currentEmptySlot);

            if (bouncePosition == null)
                BubbleManager.FirstThrowableBubble.ThrowHandler.GetThrown(_currentEmptySlot);
            else
                BubbleManager.FirstThrowableBubble.ThrowHandler.GetThrown(_currentEmptySlot, bouncePosition);

            GameEvents.OnGameStateChange?.Invoke(Enums.GameState.MergingBubbles);
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
