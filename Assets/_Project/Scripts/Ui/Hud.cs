using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BubblePops.Utility;
using DG.Tweening;
using System;

namespace BubblePops
{
    public class Hud : MonoBehaviour
    {
        [Header("-- SETUP --")]
        [SerializeField] private TextMeshProUGUI _totalScoreText;
        [SerializeField] private TextMeshProUGUI _multiplierText;
        [SerializeField] private TextMeshProUGUI _currentLevelText;
        [SerializeField] private TextMeshProUGUI _nextLevelText;
        [SerializeField] private Image _expBarFillImage;
        [SerializeField] private Image _currentLevelKnob;
        [SerializeField] private Image _nextLevelKnob;
        [SerializeField] private TextMeshProUGUI _perfectText;

        #region SEQUENCES
        private Sequence _perfectTextSequence;
        private Guid _perfectTextSequenceID; 
        #endregion

        public void Init(UiManager uiManager)
        {
            _perfectText.transform.localScale = Vector2.zero;

            UiEvents.OnUpdateScoreText += UpdateScoreText;
            UiEvents.OnUpdateCurrentExperience += UpdateCurrentExperience;
            UiEvents.OnUpdateCurrentLevel += UpdateCurrentLevel;
            UiEvents.OnActivatePerfectText += ActivatePerfectText;
        }

        private void OnDisable() 
        {
            UiEvents.OnUpdateScoreText -= UpdateScoreText;
            UiEvents.OnUpdateCurrentExperience -= UpdateCurrentExperience;
            UiEvents.OnUpdateCurrentLevel -= UpdateCurrentLevel;  
            UiEvents.OnActivatePerfectText -= ActivatePerfectText;  
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

            _multiplierText.text = "x" + DataManager.Multiplier;
        }
        private void ActivatePerfectText()
        {
            DeletePerfectTextSequence();
            CreatePerfectTextSequence();
            _perfectTextSequence.Play();
        }
        #endregion

        #region DOTWEEN
        private void CreatePerfectTextSequence()
        {
            if (_perfectTextSequence == null)
            {
                _perfectTextSequence = DOTween.Sequence();
                _perfectTextSequenceID = Guid.NewGuid();
                _perfectTextSequence.id = _perfectTextSequenceID;

                _perfectTextSequence.Append(_perfectText.transform.DOScale(Vector2.one, 0.5f).From(Vector2.zero))
                    .Join(_perfectText.transform.DOMoveY(1.5f, 0.5f).From(-3f))
                    .Append(_perfectText.transform.DOShakeScale(0.5f, 0.1f))
                    .Join(_perfectText.transform.DOShakePosition(1f, new Vector2(20f, 0f), 10, 0))
                    .Append(_perfectText.DOFade(0f, 1f).From(1f))
                    .OnComplete(DeletePerfectTextSequence);
            }
        }
        private void DeletePerfectTextSequence()
        {
            DOTween.Kill(_perfectTextSequenceID);
            _perfectTextSequence = null;
        }
        #endregion
    }
}
