using UnityEngine;
using DG.Tweening;
using System;

namespace BubblePops
{
    public class SettingsCanvas : MonoBehaviour
    {
        [Header("-- SETUP --")]
        [SerializeField] private Transform _pauseMenuTransform;
        [SerializeField] private CustomButton _playButton;
        [SerializeField] private CustomButton _noAdsButton;
        [SerializeField] private CustomButton _settingsButton;
        [SerializeField] private SettingsPanel _settingsPanel;
        [SerializeField] private Transform _titleTransform;
        
        private CustomButton _pauseButton;

        #region STATICS
        public static bool IsOpen { get; private set; }
        #endregion

        #region SEQUENCE
        private Sequence _enableSequence, _disableSequence;
        private Guid _enableSequenceID, _disableSequenceID;
        #endregion

        public void Init(UiManager uiManager)
        {
            IsOpen = false;
            _pauseMenuTransform.localScale = Vector3.zero;
            _pauseButton = transform.GetChild(0).GetComponent<CustomButton>();
            _settingsPanel.Init(this);

            DisablePauseMenu();

            _pauseButton.onClick.AddListener(() => _pauseButton.TriggerClick(EnablePauseMenu));
            _playButton.onClick.AddListener(() => _playButton.TriggerClick(DisablePauseMenu));
            _noAdsButton.onClick.AddListener(() => _noAdsButton.TriggerClick(NoAdsButtonClicked));
            _settingsButton.onClick.AddListener(() => _settingsButton.TriggerClick(EnableSettings));
        }

        private void OnDisable() 
        {
            _pauseButton.onClick.RemoveListener(() => _pauseButton.TriggerClick(EnablePauseMenu));
            _playButton.onClick.RemoveListener(() => _playButton.TriggerClick(DisablePauseMenu));
            _noAdsButton.onClick.RemoveListener(() => _noAdsButton.TriggerClick(NoAdsButtonClicked));
            _settingsButton.onClick.RemoveListener(() => _settingsButton.TriggerClick(EnableSettings));
        }

        #region EVENT HANDLER FUNCTIONS
        public void EnablePauseMenu()
        {
            CreateEnableSequence();
            _enableSequence.Play();
        }
        private void DisablePauseMenu()
        {
            CreateDisableSequence();
            _disableSequence.Play();
        }
        private void EnableSettings() => CreateDisableSequence(() => _settingsPanel.Activate());
        private void NoAdsButtonClicked() => Debug.Log("No ads button clicked", this);
        #endregion

        #region DOTWEEN FUNCTIONS
        private void CreateEnableSequence()
        {
            if (_enableSequence == null)
            {
                _enableSequence = DOTween.Sequence();
                _enableSequenceID = Guid.NewGuid();
                _enableSequence.id = _enableSequenceID;

                _enableSequence.Append(_pauseMenuTransform.DOScale(Vector2.one, 0.25f).From(Vector2.zero))
                    .Append(_titleTransform.DOShakeScale(0.5f, 0.25f))
                    .OnComplete(() => {
                        DeleteEnableSequence();
                        IsOpen = true;
                    });
            }
        }
        private void DeleteEnableSequence()
        {
            DOTween.Kill(_enableSequenceID);
            _enableSequence = null;
        }
        // ##########################
        private void CreateDisableSequence(Action action = null)
        {
            if (_disableSequence == null)
            {
                _disableSequence = DOTween.Sequence();
                _disableSequenceID = Guid.NewGuid();
                _disableSequence.id = _disableSequenceID;

                _disableSequence.Append(_pauseMenuTransform.DOScale(Vector2.zero, 0.25f).From(Vector2.one))
                    .OnComplete(() => {
                        DeleteDisableSequence();

                        if (action == null)
                            IsOpen = false;
                        else
                            action.Invoke();
                    });
            }
        }
        private void DeleteDisableSequence()
        {
            DOTween.Kill(_disableSequenceID);
            _disableSequence = null;
        }
        #endregion
    }
}
