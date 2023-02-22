using UnityEngine;
using TMPro;
using DG.Tweening;
using System;

namespace BubblePops
{
    public class MergeChainText : MonoBehaviour
    {
        #region COMPONENTS
        private TextMeshProUGUI _mergeChainText;
        #endregion

        #region SEQUENCES
        private Sequence _chainSequence;
        private Guid _chainSequenceID;
        #endregion

        private void Awake() 
        {
            _mergeChainText = GetComponent<TextMeshProUGUI>();
            _mergeChainText.transform.localScale = Vector2.zero;

            UiEvents.OnActivateMergeChainText += ActivateMergeChainText;
        }

        private void OnDisable() 
        {
            UiEvents.OnActivateMergeChainText -= ActivateMergeChainText;
        }

        #region EVENT HANDLER FUNCTIONS
        private void ActivateMergeChainText(int chainCount)
        {
            if (chainCount <= 1) return;
            _mergeChainText.text = chainCount + "X";

            DeleteChainSequence();
            CreateChainSequence();
            _chainSequence.Play();
        }
        #endregion

        #region DOTWEEN FUNCTIONS
        private void CreateChainSequence()
        {
            if (_chainSequence == null)
            {
                _chainSequence = DOTween.Sequence();
                _chainSequenceID = Guid.NewGuid();
                _chainSequence.id = _chainSequenceID;

                _mergeChainText.transform.localScale = Vector2.zero;
                _chainSequence.Append(_mergeChainText.transform.DOScale(Vector2.one, 1.5f))
                    .Append(_mergeChainText.DOFade(0f, 0.5f).From(1f))
                    .OnComplete(() => {
                        _mergeChainText.transform.localScale = Vector2.zero;
                        DeleteChainSequence();
                    });
            }
        }
        private void DeleteChainSequence()
        {
            DOTween.Kill(_chainSequenceID);
            _chainSequence = null;
        }
        #endregion
    }
}
