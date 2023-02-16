using UnityEngine;
using DG.Tweening;
using System;

namespace BubblePops
{
    public class EmptySlot : MonoBehaviour
    {
        #region COMPONENTS
        private SpriteRenderer _spriteRenderer;
        private Color _currentColor;
        #endregion

        #region FIELDS
        private int _rowNumber, _columnNumber;
        private bool _isEnabled;
        #endregion

        #region GETTERS
        public int RowNumber => _rowNumber;
        public int ColumnNumber => _columnNumber;
        public bool IsEnabled => _isEnabled;
        #endregion

        #region SEQUENCE
        private Sequence _enableSequence;
        private Guid _enableSequenceID;
        private const float ENABLE_SEQUENCE_DURATION = 0.5f;
        #endregion

        public void Init(SpawnManager spawnManager, int rowNumber, int columnNumber)
        {
            if (_spriteRenderer == null)
                _spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();

            SetColor(0, Color.white);

            _isEnabled = false;
            _rowNumber = rowNumber;
            _columnNumber = columnNumber;
        }

        #region PUBLICS
        public void Enable()
        {
            if (BubbleManager.FirstThrowableBubble == null || _isEnabled) return;

            _isEnabled = true;
            SetColor(0.5f, BubbleManager.FirstThrowableBubble.Color);
            StartEnableSequence();
        }
        public void Disable()
        {
            if (!_isEnabled) return;
            _isEnabled = false;
            SetColor(0f);
        }
        #endregion

        #region HELPERS
        private void SetColor(float alpha, Color? color = null)
        {
            if (color != null)
                _currentColor = (Color)color;

            _currentColor.a = alpha;
            _spriteRenderer.color = _currentColor;
        }
        #endregion

        #region DOTWEEN FUNCTIONS
        private void StartEnableSequence()
        {
            CreateEnableSequence();
            _enableSequence.Play();
        }
        private void CreateEnableSequence()
        {
            if (_enableSequence == null)
            {
                _enableSequence = DOTween.Sequence();
                _enableSequenceID = Guid.NewGuid();
                _enableSequence.id = _enableSequenceID;

                transform.localScale = Vector2.zero;
                _enableSequence.Append(transform.DOScale(Vector2.one, ENABLE_SEQUENCE_DURATION))
                    .OnComplete(DeleteEnableSequence);
            }
        }
        private void DeleteEnableSequence()
        {
            DOTween.Kill(_enableSequenceID);
            _enableSequence = null;
        }
        #endregion
    }
}
