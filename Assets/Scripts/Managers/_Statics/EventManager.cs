using UnityEngine;
using System;

namespace BubblePops
{
    public class EventManager { }

    public static class GameFlowEvents
    {
        public static Action<Enums.GameState> OnGameStateChange;
    }

    public static class SpawnEvents
    {
        public static Action OnSpawnSecondThrowable;
        public static Action<Bubble, Enums.BubbleDirection> OnSpawnEmptySlot;
    }

    public static class BubbleEvents
    {
        public static Action OnCheckSurroundings;
        public static Action<Bubble> OnStartMerge;
    }

    public static class EmptySlotEvents
    {
        public static Action OnCheckForActivation;
    }
}
