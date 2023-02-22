using UnityEngine;
using TMPro;
using DG.Tweening;
using System;

namespace BubblePops
{
    public class PopInfoCanvas : MonoBehaviour
    {
        #region COMPONENTS
        private TextMeshProUGUI _numberText;
        #endregion

        #region SEQUENCES
        private Sequence _fadeOutSequence;
        private Guid _fadeOutSequenceID;
        #endregion

        public void Init(int number)
        {
            if (_numberText == null)
                _numberText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();

            _numberText.text = number.ToString();
            StartFadeOutSequence();
        }

        #region DOTWEEN FUNCTIONS
        private void StartFadeOutSequence()
        {
            CreateFadeOutSequence();
            _fadeOutSequence.Play();
        }
        private void CreateFadeOutSequence()
        {
            if (_fadeOutSequence == null)
            {
                _fadeOutSequence = DOTween.Sequence();
                _fadeOutSequenceID = Guid.NewGuid();
                _fadeOutSequence.id = _fadeOutSequenceID;

                _fadeOutSequence.Append(transform.DOMoveY(transform.position.y + 0.75f, 0.75f))
                .AppendInterval(0.5f)
                .Append(_numberText.DOFade(0f, 0.5f).From(1f))
                .OnComplete(() => {
                    DeleteFadeOutSequence();
                    gameObject.SetActive(false);
                });
            }
        }
        private void DeleteFadeOutSequence()
        {
            DOTween.Kill(_fadeOutSequenceID);
            _fadeOutSequence = null;
        }
        #endregion
    }
}
