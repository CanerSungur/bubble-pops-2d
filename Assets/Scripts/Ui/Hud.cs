using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BubblePops.Utility;

namespace BubblePops
{
    public class Hud : MonoBehaviour
    {
        [Header("-- SETUP --")]
        [SerializeField] private TextMeshProUGUI _totalScoreText;
        [SerializeField] private TextMeshProUGUI _currentLevelText;
        [SerializeField] private TextMeshProUGUI _nextLevelText;
        [SerializeField] private Image _expBarFillImage;
        [SerializeField] private Image _currentLevelKnob;
        [SerializeField] private Image _nextLevelKnob;

        public void Init(UiManager uiManager)
        {
            UiEvents.OnUpdateScoreText += UpdateScoreText;
            UiEvents.OnUpdateCurrentExperience += UpdateCurrentExperience;
            UiEvents.OnUpdateCurrentLevel += UpdateCurrentLevel;
        }

        private void OnDisable() 
        {
            UiEvents.OnUpdateScoreText -= UpdateScoreText;
            UiEvents.OnUpdateCurrentExperience -= UpdateCurrentExperience;
            UiEvents.OnUpdateCurrentLevel -= UpdateCurrentLevel;    
        }

        #region EVENT HANDLER FUNCTIONS
        private void UpdateScoreText(int totalScore)
        {
            _totalScoreText.text = Utils.IntToStringShortener(totalScore);
        }
        private void UpdateCurrentExperience(int currentExp)
        {
            _expBarFillImage.fillAmount = DataManager.GetCurrentExpNormalized();
            _expBarFillImage.color = GameManager.GetColor(DataManager.CurrentLevel);
        }
        private void UpdateCurrentLevel(int currentLevel)
        {
            _currentLevelText.text = currentLevel.ToString();
            _nextLevelText.text = (currentLevel + 1).ToString();

            _currentLevelKnob.color = GameManager.GetColor(DataManager.CurrentLevel);
            _nextLevelKnob.color = GameManager.GetColor(DataManager.CurrentLevel + 1);
        }
        #endregion
    }
}
