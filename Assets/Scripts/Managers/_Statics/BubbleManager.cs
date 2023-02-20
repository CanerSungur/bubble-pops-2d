using System.Diagnostics;
using System.Collections.Generic;

namespace BubblePops
{
    public static class BubbleManager
    {
        #region FIELDS
        private static Bubble _firstThrowableBubble, _secondThrowableBubble, _bubbleForNextMerge;
        private static List<Bubble> _bubblesInSlot = new();
        private static List<EmptySlot> _emptySlots = new();
        private static List<Bubble> _bubblesToMerge = new();
        #endregion

        #region GETTERS
        public static Bubble FirstThrowableBubble => _firstThrowableBubble;
        public static Bubble SecondThrowableBubble => _secondThrowableBubble;
        public static Bubble BubbleForNextMerge => _bubbleForNextMerge;
        public static List<Bubble> BubblesInSlot => _bubblesInSlot;
        public static List<EmptySlot> EmptySlots => _emptySlots;
        public static List<Bubble> BubblesToMerge => _bubblesToMerge;
        #endregion

        #region SETTER FUNCTIONS
        public static void SetFirstThrowable(Bubble bubble) => _firstThrowableBubble = bubble;
        public static void SetSecondThrowable(Bubble bubble) => _secondThrowableBubble = bubble;
        public static void SetBubbleForNextMerge(Bubble bubble) => _bubbleForNextMerge = bubble;
        #endregion

        #region PUBLICS
        public static void RemoveThisFromAllLists(Bubble bubble)
        {
            RemoveBubbleInSlot(bubble);
            RemoveBubblesToMerge(bubble);
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
        public static void AddEmptySlot(EmptySlot emptySlot)
        {
            if (!_emptySlots.Contains(emptySlot))
            {
                #region Check if an empty slot already spawned at the same location
                foreach (EmptySlot slot in _emptySlots)
                {
                    if (emptySlot.RowNumber == slot.RowNumber && emptySlot.ColumnNumber == slot.ColumnNumber)
                    {
                        emptySlot.gameObject.SetActive(false);
                        return;
                    }
                }
                #endregion

                _emptySlots.Add(emptySlot);
            }
        }
        public static void RemoveEmptySlot(EmptySlot emptySlot)
        {
            if (_emptySlots.Contains(emptySlot))
                _emptySlots.Remove(emptySlot);
        }
        public static void AddBubblesToMerge(Bubble bubble)
        {
            if (!_bubblesToMerge.Contains(bubble))
                _bubblesToMerge.Add(bubble);
        }
        public static void RemoveBubblesToMerge(Bubble bubble)
        {
            if (_bubblesToMerge.Contains(bubble))
                _bubblesToMerge.Remove(bubble);
        }
        public static void ResetBubblesToMerge() => _bubblesToMerge.Clear();
        #endregion
    }
}
