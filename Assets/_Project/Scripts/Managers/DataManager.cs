using UnityEngine;

namespace BubblePops
{
    public class DataManager : MonoBehaviour
    {
        #region FIELDS
        private static int _totalScore, _currentLevel, _requiredExpForThisLevel, _currentExp;
        #endregion

        #region GETTERS
        public static int CurrentLevel => _currentLevel;
        public static int Multiplier => _currentLevel < 5 ? 1 
                                    : _currentLevel >= 5 && _currentLevel < 10 ? 2 
                                    : _currentLevel >=15 && _currentLevel < 25 ? 3 : 4; 
        #endregion

        #region CONSTANTS
        private const int BASE_REQUIRED_EXP = 25;
        private const int REQUIRED_EXP_INCREMENT = 5;
        private const int BASE_POINT_PER_MERGE = 1;
        #endregion
        
        public void Init(GameManager gameManager)
        {
            LoadData();
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

            SaveData();
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

        #region SAVE-LOAD FUNCTIONS
        private void LoadData()
        {
            _currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
            _currentExp = PlayerPrefs.GetInt("CurrentExp", 0);             
            _totalScore = PlayerPrefs.GetInt("TotalScore", 0);
        }
        private void SaveData()
        {
            PlayerPrefs.SetInt("CurrentLevel", _currentLevel);
            PlayerPrefs.SetInt("CurrentExp", _currentExp);             
            PlayerPrefs.SetInt("TotalScore", _totalScore);
            PlayerPrefs.Save();
        }
        private void OnApplicationPause(bool pauseStatus) => SaveData();
        private void OnApplicationQuit() => SaveData();
        #endregion
    }
}
