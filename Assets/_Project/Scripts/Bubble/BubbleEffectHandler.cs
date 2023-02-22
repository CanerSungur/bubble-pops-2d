using UnityEngine;
using TMPro;
using DG.Tweening;
using System;

namespace BubblePops
{
    public class BubbleEffectHandler : MonoBehaviour
    {
        #region COMPONENTS
        private TextMeshProUGUI _numberText;
        #endregion
        
        #region SEQUENCES
        private Sequence _textFadeSequence;
        private Guid _textFadeSequenceID;
        #endregion

        public void Init(Bubble bubble)
        {
            if (_numberText == null)
                _numberText = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();

            ResetTextColor();
        }

        private void OnDisable() 
        {
            if (_textFadeSequence != null && _textFadeSequence.IsPlaying())
                DeleteTextFadeSequence();    
        }

        private void ResetTextColor()
        {
            Color color = _numberText.color;
            color.a = 1f;
            _numberText.color = color;
        }

        #region PUBLIC
        public void TriggerTextFadeSequence()
        {
            CreateTextFadeSequence();
            _textFadeSequence.Play();
        }
        public void SpawnBubbleMergePS(Color bubbleColor)
        {
            ParticleSystem ps = PoolManager.Instance.SpawnFromPool(Enums.PoolStamp.PS_BubbleMerge, transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
            
            var main = ps.main;
            main.startColor = bubbleColor;
            ps.Play();
        }
        public void SpawnPopNumberInfo(int number)
        {
            PopInfoCanvas infoCanvas = PoolManager.Instance.SpawnFromPool(Enums.PoolStamp.PopInfoCanvas, transform.position, Quaternion.identity).GetComponent<PopInfoCanvas>();
            infoCanvas.Init(number);
        }
        public void SpawnBubblePopPS()
        {
            PoolManager.Instance.SpawnFromPool(Enums.PoolStamp.PS_BubblePop, transform.position, Quaternion.identity);
        }
        #endregion

        #region DOTWEEUN FUNCTIONS
        private void CreateTextFadeSequence()
        {
            if (_textFadeSequence == null)
            {
                _textFadeSequence = DOTween.Sequence();
                _textFadeSequenceID = Guid.NewGuid();
                _textFadeSequence.id = _textFadeSequenceID;

                _textFadeSequence.Append(_numberText.DOFade(0f, 0.1f).From(1f))
                .OnComplete(DeleteTextFadeSequence);
            }
        }
        private void DeleteTextFadeSequence()
        {
            DOTween.Kill(_textFadeSequenceID);
            _textFadeSequence = null;
        }
        #endregion
    }
}
