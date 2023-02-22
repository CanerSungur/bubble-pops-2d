using UnityEngine.UI;
using System;
using DG.Tweening;

namespace BubblePops
{
    /// <summary>
    /// Plays sound and animation first, then executes the action given.
    /// ############
    /// TO USE: subscribe to button onClick event using _customButton.TriggerClick(action)
    /// </summary>
    public class CustomButton : Button
    {
        private bool _clicked;
        private const float BOUNCE_DURATION = 0.2f;

        #region SEQUENCE
        private Sequence _bounceSequence;
        private Guid _bounceSequenceID;
        #endregion

        protected override void OnEnable()
        {
            _clicked = false;
        }

        private void Clicked(Action action)
        {
            if (_clicked) return;
            _clicked = true;

            AudioManager.PlayAudio(Enums.AudioType.ButtonClick);
            StartBounceSequence(action);
        }

        #region PUBLICS
        public void TriggerClick(Action action, Action simultaniousAction = null)
        {
            if (_clicked) return;
            simultaniousAction?.Invoke();
            Clicked(action);
        }
        #endregion

        #region DOTWEEN FUNCTIONS
        private void StartBounceSequence(Action action)
        {
            CreateBounceSequence(action);
            _bounceSequence.Play();
        }
        private void CreateBounceSequence(Action action)
        {
            if (_bounceSequence == null)
            {
                _bounceSequence = DOTween.Sequence();
                _bounceSequenceID = Guid.NewGuid();
                _bounceSequence.id = _bounceSequenceID;

                _bounceSequence.Append(transform.DOShakeScale(BOUNCE_DURATION, 0.25f))
                    .OnComplete(() => {
                        DeleteBounceSequence();
                        _clicked = false;
                        action();
                    });
            }
        }
        private void DeleteBounceSequence()
        {
            DOTween.Kill(_bounceSequenceID);
            _bounceSequence = null;
        }
        #endregion
    }
}
