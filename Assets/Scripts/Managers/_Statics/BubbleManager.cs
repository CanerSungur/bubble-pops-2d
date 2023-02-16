using System.Collections.Generic;

namespace BubblePops
{
    public static class BubbleManager
    {
        #region FIELDS
        private static Bubble _firstThrowableBubble, _secondThrowableBubble;
        private static List<Bubble> _bubblesInSlot = new();
        #endregion

        #region GETTERS
        public static Bubble FirstThrowableBubble => _firstThrowableBubble;
        public static Bubble SecondThrowableBubble => _secondThrowableBubble;
        public static List<Bubble> BubblesInSlot => _bubblesInSlot;
        #endregion

        #region SETTER FUNCTIONS
        public static void SetFirstThrowable(Bubble bubble)
        {
            _firstThrowableBubble = bubble;
        }
        public static void SetSecondThrowable(Bubble bubble)
        {
            _secondThrowableBubble = bubble;
        }
        #endregion

        #region LIST FUNCTIONS
        public static void AddBubbleInSlot(Bubble bubble)
        {
            if (!_bubblesInSlot.Contains(bubble))
                _bubblesInSlot.Add(bubble);
        }
        public static void RemoveBubbleInSlot(Bubble bubble)
        {
            if (_bubblesInSlot.Contains(bubble))
                _bubblesInSlot.Remove(bubble);
        }
        #endregion
    }
}
