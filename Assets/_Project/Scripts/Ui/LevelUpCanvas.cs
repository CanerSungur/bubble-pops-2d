using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.UI;

namespace BubblePops
{
    public class LevelUpCanvas : MonoBehaviour
    {
        [Header("-- SETUP --")]
        [SerializeField] private CustomButton _closeButton;
        [SerializeField] private Transform _panel;
        [SerializeField] private Transform _panelParent;
        [SerializeField] private Transform _ribbonTransform;
        [SerializeField] private ParticleSystem _levelUpParticleSystem;
        private Image _panelParentImage;

        #region SEQUENCE
        private Sequence _enableSequence, _disableSequence;
        private Guid _enableSequenceID, _disableSequenceID;
        #endregion

        #region STATICS
        public static bool IsOpen { get; private set; }
        #endregion

        public void Init(UiManager uiManager)
        {
            _panelParentImage = _panelParent.GetComponent<Image>();
            _panelParent.transform.localScale = _panel.transform.localScale = Vector2.zero;
            IsOpen = false;

            _closeButton.onClick.AddListener(() => _closeButton.TriggerClick(CloseCanvas));
            UiEvents.OnActivateLevelUpCanvas += EnableCanvas;
        }

        private void OnDisable() 
        {
            _closeButton.onClick.RemoveListener(() => _closeButton.TriggerClick(CloseCanvas));
            UiEvents.OnActivateLevelUpCanvas -= EnableCanvas;
        }

        #region EVENT HANDLER FUNCTIONS
        private void CloseCanvas()
        {
            _levelUpParticleSystem.Stop();
            CreateDisableSequence();
            _disableSequence.Play();
        }
        private void EnableCanvas()
        {
            var main = _levelUpParticleSystem.main;
            main.startColor = GameManager.GetColor(DataManager.CurrentLevel);
            _levelUpParticleSystem.Play();

            CreateEnableSequence();
            _enableSequence.Play();
        }
        #endregion

        #region DOTWEEN FUNCTIONS
        private void CreateEnableSequence()
        {
            if (_enableSequence == null)
            {
                _enableSequence = DOTween.Sequence();
                _enableSequenceID = Guid.NewGuid();
                _enableSequence.id = _enableSequenceID;

                _panelParent.transform.localScale = Vector2.one;

                _enableSequence.Append(_panelParentImage.DOFade(0.5f, 0.25f).From(0f))
                    .Append(_panel.transform.DOScale(Vector2.one, 0.25f))
                    .Append(_panel.transform.DOShakeScale(0.5f, 0.25f))
                    .Append(_ribbonTransform.DOShakePosition(0.5f,new Vector3(20f, 0f, 0f), 10, 0))
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
        private void CreateDisableSequence()
        {
            if (_disableSequence == null)
            {
                _disableSequence = DOTween.Sequence();
                _disableSequenceID = Guid.NewGuid();
                _disableSequence.id = _disableSequenceID;

                _disableSequence.Append(_panelParentImage.DOFade(0f, 0.5f).From(0.25f))
                    .Join(_panel.transform.DOScale(Vector2.zero, 0.25f))
                    .OnComplete(() => {
                        DeleteDisableSequence();
                        IsOpen = false;
                        _panelParent.transform.localScale = _panel.transform.localScale = Vector2.zero;
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
