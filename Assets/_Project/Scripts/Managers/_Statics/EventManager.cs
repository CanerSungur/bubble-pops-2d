using System;

namespace BubblePops
{
    public class EventManager { }

    public static class GameEvents
    {
        public static Action<Enums.GameState> OnGameStateChange;
    }

    public static class CameraEvents
    {
        public static Action OnShakeCamera;
    }

    public static class MapEvents
    {
        public static Action OnCheckMapMovement, OnMoveMapUpwards, OnMoveMapDownwards;
    }

    public static class SpawnEvents
    {
        public static Action OnSpawnSecondThrowable;
        public static Action<Bubble, Enums.BubbleDirection, Enums.ColumnLeanSide> OnSpawnEmptySlot;
        public static Action<Enums.ColumnLeanSide> OnSpawnNewColumn;
    }

    public static class BubbleEvents
    {
        public static Action OnCheckSurroundings;
        public static Action<Bubble> OnStartMerge, OnPositionChanged;
    }

    public static class PlayerEvents
    {
        public static Action<int> OnIncreaseScore, OnIncreaseExperience;
    }

    public static class UiEvents
    {
        public static Action<int> OnUpdateScoreText, OnUpdateCurrentLevel, OnUpdateCurrentExperience, OnActivateMergeChainText;
        public static Action OnActivateLevelUpCanvas, OnActivatePerfectText;
    }

    public static class AudioEvents
    {
        public static Action OnPlayMove, OnPlayMerge, OnPlayPop, OnPlayButtonClick;
    }

    public static class HapticEvents
    {
        public static Action OnPlayMerge, OnPlayPop;
    }
}
