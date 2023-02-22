using UnityEngine;
using DG.Tweening;
using System;
using System.Collections.Generic;

namespace BubblePops
{
    public class EmptySlot : MonoBehaviour
    {
        #region COMPONENTS
        private SpriteRenderer _spriteRenderer;
        private Color _currentColor;
        #endregion

        #region FIELDS
        private Enums.ColumnLeanSide _columnLeanSide;
        private int _rowNumber, _columnNumber;
        private bool _isEnabled;
        private Vector2 _directionRight, _directionRightTop, _directionRightBottom, _directionLeft, _directionLeftTop, _directionLeftBottom;
        private readonly Dictionary<Enums.BubbleDirection, Vector2> _directions = new();
        #endregion

        #region GETTERS
        public Enums.ColumnLeanSide ColumnLeanSide => _columnLeanSide;
        public int RowNumber => _rowNumber;
        public int ColumnNumber => _columnNumber;
        public bool IsEnabled => _isEnabled;
        #endregion

        #region SEQUENCE
        private Sequence _enableSequence;
        private Guid _enableSequenceID;
        private const float ENABLE_SEQUENCE_DURATION = 0.25f;
        #endregion

        public void Init(SpawnManager spawnManager, int rowNumber, int columnNumber, Enums.ColumnLeanSide columnLeanSide)
        {
            if (_spriteRenderer == null)
                _spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();

            SetColor(1, Color.white);

            _isEnabled = false;
            _columnLeanSide = columnLeanSide;
            _rowNumber = rowNumber;
            _columnNumber = columnNumber;

            BubbleManager.AddEmptySlot(this);
        }

        private void OnDisable()
        {
            transform.localPosition = new Vector2(50, -50);
            BubbleManager.RemoveEmptySlot(this);
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
            SetColor(1);
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
