using BubblePops.Utility;
using UnityEngine;

namespace BubblePops
{
    public class GameManager : MonoBehaviour
    {
        #region COMPONENTS
        private PoolManager _poolManager;
        private SpawnManager _spawnManager;
        private BubbleMergeManager _mergeManager;
        #endregion

        #region PUBLIC STATICS
        public static Enums.GameState CurrentState { get; private set; }
        #endregion

        private void Awake()
        {
            CurrentState = Enums.GameState.Ready;

            _poolManager = GetComponent<PoolManager>();
            _spawnManager = GetComponent<SpawnManager>();
            _mergeManager = GetComponent<BubbleMergeManager>();

            _poolManager.Init(this);
            _spawnManager.Init(this);
            _mergeManager.Init(this);

            Utils.DoActionAfterDelay(this, 0.5f, () => _mergeManager.TriggerAllBubblesCheckSurroundings());

            GameFlowEvents.OnGameStateChange += ChangeGameState;
        }

        private void OnDisable()
        {
            GameFlowEvents.OnGameStateChange -= ChangeGameState;
        }

        private void ChangeGameState(Enums.GameState gameState)
        {
            CurrentState = gameState;

            if (gameState == Enums.GameState.PreparingNewBubble)
                BubbleManager.SecondThrowableBubble.OnSetAsFirstThrowable?.Invoke();
        }
    }
}
