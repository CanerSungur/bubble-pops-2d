using BubblePops.Utility;
using UnityEngine;

namespace BubblePops
{
    public class GameManager : MonoBehaviour
    {
        [Header("-- COLOR DATA --")]
        [SerializeField] private Color[] _exponentColors;

        #region COMPONENTS
        private PoolManager _poolManager;
        private SpawnManager _spawnManager;
        private BubbleMergeManager _mergeManager;
        private MapMovementManager _mapMovementManager;
        private DataManager _dataManager;
        private UiManager _uiManager;
        private SettingsManager _settingsManager;
        private HapticManager _hapticManager;
        #endregion

        #region GETTERS
        public SettingsManager SettingsManager => _settingsManager;
        #endregion

        #region PUBLIC STATICS
        public static Enums.GameState CurrentState { get; private set; }
        public static Color[] ExponentColors { get; private set; }
        #endregion

        private void Awake()
        {
            CurrentState = Enums.GameState.Ready;
            ExponentColors = _exponentColors;

            _settingsManager = GetComponent<SettingsManager>();
            _poolManager = GetComponent<PoolManager>();
            _spawnManager = GetComponent<SpawnManager>();
            _mergeManager = GetComponent<BubbleMergeManager>();
            _mapMovementManager = GetComponent<MapMovementManager>();
            _dataManager = GetComponent<DataManager>();
            _uiManager = GetComponent<UiManager>();
            _hapticManager = GetComponent<HapticManager>();

            _settingsManager.Init(this);
            _poolManager.Init(this);
            _spawnManager.Init(this);
            _mergeManager.Init(this);
            _mapMovementManager.Init(this);
            _dataManager.Init(this);
            _uiManager.Init(this);
            _hapticManager.Init(this);

            Utils.DoActionAfterDelay(this, 0.5f, () => _mergeManager.TriggerAllBubblesCheckSurroundings());

            GameEvents.OnGameStateChange += ChangeGameState;
        }

        private void OnDisable()
        {
            GameEvents.OnGameStateChange -= ChangeGameState;
        }

        private void Update() 
        {
            if (Input.GetKeyDown(KeyCode.P))
                UiEvents.OnActivatePerfectText?.Invoke();
        }

        #region EVENT HANDLER FUNCTIONS
        private void ChangeGameState(Enums.GameState gameState)
        {
            CurrentState = gameState;

            if (gameState == Enums.GameState.PreparingNewBubble)
                BubbleManager.SecondThrowableBubble.OnSetAsFirstThrowable?.Invoke();
        }
        #endregion

        #region PUBLIC STATICS
        public static Color GetColor(int levelOrExponent)
        {
            int givenIndex = levelOrExponent - 1;
            int newIndex = givenIndex < ExponentColors.Length ? givenIndex : (givenIndex % ExponentColors.Length);
            // Debug.Log(newIndex);
            return ExponentColors[newIndex];
        }
        #endregion
    }
}
