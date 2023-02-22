using UnityEngine;
using DG.Tweening;
using System;

namespace BubblePops
{
    public class SettingsPanel : MonoBehaviour
    {
        [Header("-- SETUP --")]
        [SerializeField] private CustomButton _themesButton;
        [SerializeField] private CustomButton _soundButton;
        [SerializeField] private CustomButton _vibrationButton;
        [SerializeField] private CustomButton _restorePurchasesButton;
        [SerializeField] private CustomButton _rateUsButton;
        [SerializeField] private CustomButton _removeAdsButton;
        [SerializeField] private CustomButton _closeButton;
        [SerializeField] private Animator _soundSliderAnim;
        [SerializeField] private Animator _vibrationSliderAnim;

        #region FIELDS
        private SettingsCanvas _settingsCanvas;
        private const string OPEN_ID = "Open";
        #endregion

        #region SEQUENCE
        private Sequence _enableSequence, _disableSequence;
        private Guid _enableSequenceID, _disableSequenceID;
        #endregion

        public void Init(SettingsCanvas settingsCanvas)
        {
            _settingsCanvas = settingsCanvas;
            UpdateSoundSliderAnim();
            UpdateVibrationSliderAnim();
            transform.localScale = Vector2.zero;

            _themesButton.onClick.AddListener(() => _themesButton.TriggerClick(ThemesButtonClicked));
            _soundButton.onClick.AddListener(() => _soundButton.TriggerClick(SoundButtonClicked));
            _vibrationButton.onClick.AddListener(() => _vibrationButton.TriggerClick(VibrationButtonClicked));
            _restorePurchasesButton.onClick.AddListener(() => _restorePurchasesButton.TriggerClick(RestorePurchaseButtonClicked));
            _rateUsButton.onClick.AddListener(() => _rateUsButton.TriggerClick(RateUsButtonClicked));
            _removeAdsButton.onClick.AddListener(() => _removeAdsButton.TriggerClick(RemoveAdsButtonClicked));
            _closeButton.onClick.AddListener(() => _closeButton.TriggerClick(CloseSettingsPanel)); 
        }

        private void OnDisable() 
        {
            _themesButton.onClick.RemoveListener(() => _themesButton.TriggerClick(ThemesButtonClicked));
            _soundButton.onClick.RemoveListener(() => _soundButton.TriggerClick(SoundButtonClicked));
            _vibrationButton.onClick.RemoveListener(() => _vibrationButton.TriggerClick(VibrationButtonClicked));
            _restorePurchasesButton.onClick.RemoveListener(() => _restorePurchasesButton.TriggerClick(RestorePurchaseButtonClicked));
            _rateUsButton.onClick.RemoveListener(() => _rateUsButton.TriggerClick(RateUsButtonClicked));
            _removeAdsButton.onClick.RemoveListener(() => _removeAdsButton.TriggerClick(RemoveAdsButtonClicked));
            _closeButton.onClick.RemoveListener(() => _closeButton.TriggerClick(CloseSettingsPanel));     
        }

        #region PUBLICS
        public void Activate()
        {
            CreateEnableSequence();
            _enableSequence.Play();
        }
        #endregion

        #region EVENT HANDLER FUNCTIONS
        private void ThemesButtonClicked() => Debug.Log("Themes button clicked.", this);
        private void SoundButtonClicked()
        {
            SettingsManager.SoundOn = !SettingsManager.SoundOn;
            UpdateSoundSliderAnim();            
        }
        private void VibrationButtonClicked()
        {
            SettingsManager.VibrationOn = !SettingsManager.VibrationOn;
            UpdateVibrationSliderAnim();
        }
        private void RestorePurchaseButtonClicked() => Debug.Log("Restore purchase button clicked,", this);
        private void RateUsButtonClicked() => Debug.Log("Rate us button clicked.", this);
        private void RemoveAdsButtonClicked() => Debug.Log("Remove ads button clicked.");
        private void CloseSettingsPanel()
        {
            CreateDisableSequence();
            _disableSequence.Play();
        }
        #endregion

        #region HELPERS
        private void UpdateSoundSliderAnim() => _soundSliderAnim.SetBool(OPEN_ID, SettingsManager.SoundOn);
        private void UpdateVibrationSliderAnim() => _vibrationSliderAnim.SetBool(OPEN_ID, SettingsManager.VibrationOn);
        #endregion

        #region DOTWEEN FUNCTIONS
        private void CreateEnableSequence()
        {
            if (_enableSequence == null)
            {
                _enableSequence = DOTween.Sequence();
                _enableSequenceID = Guid.NewGuid();
                _enableSequence.id = _enableSequenceID;

                _enableSequence.Append(transform.DOScale(Vector2.one, 0.25f).From(Vector2.zero))
                    .OnComplete(() => {
                        DeleteEnableSequence();
                    });
            }
        }
        private void DeleteEnableSequence()
        {
            DOTween.Kill(_enableSequenceID);
            _enableSequence = null;
        }
        // ##########################
        private void CreateDisableSequence()
        {
            if (_disableSequence == null)
            {
                _disableSequence = DOTween.Sequence();
                _disableSequenceID = Guid.NewGuid();
                _disableSequence.id = _disableSequenceID;

                _disableSequence.Append(transform.DOScale(Vector2.zero, 0.25f).From(Vector2.one))
                    .OnComplete(() => {
                        DeleteDisableSequence();
                        _settingsCanvas.EnablePauseMenu();
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
