
namespace BubblePops
{
    public class Enums
    {
        public enum GameState { Ready, MergingBubbles, PreparingNewBubble }
        public enum ColumnLeanSide { Right, Left }
        public enum BubbleStates { InSlot, ThrownFirst, ThrownSecond, }
        public enum BubbleDirection { Left, LeftTop, LeftBottom, Right, RightTop, RightBottom }
        public enum PoolStamp { None, Bubble, EmptySlot, PS_BubbleMerge, PopInfoCanvas, PS_BubblePop }
        public enum AudioType { None, Move, Pop, Merge, ButtonClick }
    }
}
