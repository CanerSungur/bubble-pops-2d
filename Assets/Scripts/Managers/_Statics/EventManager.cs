using UnityEngine;
using System;

namespace BubblePops
{
    public class EventManager { }

    public static class GameFlowEvents
    {

    }

    public static class SpawnEvents
    {
        public static Action OnSpawnSecondThrowable;
        public static Action<Bubble, Enums.BubbleDirection> OnSpawnEmptySlot;
    }

    public static class ThrowEvents
    {

    }

    public static class PlayerEvents
    {
        public static Action OnThrowSuccessful;
    }
}
