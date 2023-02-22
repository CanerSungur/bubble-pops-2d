using UnityEngine;

namespace BubblePops
{
    public class DataManager : MonoBehaviour
    {
        #region FIELDS
        private static int _totalScore;
        private static int _currentLevel;
        private static  int _requiredExpForThisLevel;
        private static int _currentExp;
        #endregion

        #region GETTERS
        public static int CurrentLevel => _currentLevel;
        #endregion

        #region CONSTANTS
        private const int BASE_REQUIRED_EXP = 25;
        private const int REQUIRED_EXP_INCREMENT = 5;
        private const int BASE_POINT_PER_MERGE = 1;
        #endregion
        
        public void Init(GameManager gameManager)
        {
            _totalScore = _currentExp = 0;
            _currentLevel = 1;
            _requiredExpForThisLevel = BASE_REQUIRED_EXP + (_currentLevel * REQUIRED_EXP_INCREMENT);

            PlayerEvents.OnIncreaseScore += IncreaseTotalScore;
            PlayerEvents.OnIncreaseExperience += IncreaseExperience;
        }

        private void Start() 
        {
            UiEvents.OnUpdateCurrentExperience?.Invoke(_currentExp);
            UiEvents.OnUpdateCurrentLevel?.Invoke(_currentLevel);
            UiEvents.OnUpdateScoreText?.Invoke(_totalScore);
        }

        private void OnDisable() 
        {
            PlayerEvents.OnIncreaseScore -= IncreaseTotalScore;
            PlayerEvents.OnIncreaseExperience -= IncreaseExperience;
        }

        #region PUBLIC STATICS
        public static float GetCurrentExpNormalized() => (float)_currentExp / (float)_requiredExpForThisLevel;
        #endregion

        #region EVENT HANDLER FUNCTIONS
        private void IncreaseTotalScore(int amount)
        {
            _totalScore += amount;
            UiEvents.OnUpdateScoreText?.Invoke(_totalScore);
        }
        private void IncreaseExperience(int mergeCount)
        {
            _currentExp += (mergeCount * BASE_POINT_PER_MERGE);
            if (_currentExp >= _requiredExpForThisLevel)
            {
                _currentLevel++;

                _currentExp = 0;
                _requiredExpForThisLevel = BASE_REQUIRED_EXP + (_currentLevel * REQUIRED_EXP_INCREMENT);

                UiEvents.OnUpdateCurrentLevel?.Invoke(_currentLevel);
                UiEvents.OnActivateLevelUpCanvas?.Invoke();
            }

            UiEvents.OnUpdateCurrentExperience?.Invoke(_currentExp);
        }
        #endregion
    }
}
