using UnityEngine;
using DG.Tweening;
using System;

namespace BubblePops
{
    public class BubbleSequenceHandler : MonoBehaviour
    {
        private Bubble _bubble;

        #region SEQUENCES
        private Sequence _shakeTransformSequence, _setAsThrowableSequence, _enableSequence, _disableSequence;
        private Guid _shakeTransformSequenceID, _setAsThrowableSequenceID, _enableSequenceID, _disableSequenceID;
        #endregion

        private const float SET_AS_THROWABLE_SEQUENCE_DURATION = 0.5f;
        private const float SHAKE_TRANSFORM_DURATION = 0.2f;

        public void Init(Bubble bubble)
        {
            if (_bubble == null)
                _bubble = bubble;
        }

        #region PUBLICS
        public void StartShakeTransform(Bubble bubble)
        {
            Enums.BubbleDirection moveDirection;
            if (bubble.ColumnNumber == _bubble.ColumnNumber)
                moveDirection = bubble.RowNumber <= _bubble.RowNumber ? Enums.BubbleDirection.Right : Enums.BubbleDirection.Left;
            else if (bubble.ColumnNumber > _bubble.ColumnNumber)
            {
                moveDirection = bubble.RowNumber <= _bubble.RowNumber ? Enums.BubbleDirection.RightTop : Enums.BubbleDirection.LeftTop;
                moveDirection = _bubble.ColumnLeanSide == Enums.ColumnLeanSide.Left && bubble.RowNumber == _bubble.RowNumber ? Enums.BubbleDirection.LeftTop : Enums.BubbleDirection.RightTop;
            }
            else
                moveDirection = bubble.RowNumber <= _bubble.RowNumber ? Enums.BubbleDirection.RightBottom : Enums.BubbleDirection.LeftBottom;

            CreateShakeTransformSequence(moveDirection);
            _shakeTransformSequence.Play();
        }
        public void StartEnableSequence()
        {
            CreateEnableSequence();
            _enableSequence.Play();
        }
        public void StartSetAsThrowableSequence()
        {
            CreateSetAsThrowableSequence();
            _setAsThrowableSequence.Play();
        }
        public void StartDisableSequence()
        {
            CreateDisableSequence();
            _disableSequence.Play();
        }
        #endregion

        #region DOTWEEN FUNCTIONS
        private void CreateShakeTransformSequence(Enums.BubbleDirection direction)
        {
            if (_shakeTransformSequence == null)
            {
                _shakeTransformSequence = DOTween.Sequence();
                _shakeTransformSequenceID = Guid.NewGuid();
                _shakeTransformSequence.id = _shakeTransformSequenceID;

                Vector3 moveDirection = _bubble.SurroundingHandler.Directions[direction] * 0.09f;
                _shakeTransformSequence.Append(_bubble.MeshTransform.DOLocalMove(moveDirection, SHAKE_TRANSFORM_DURATION * 0.35f))
                    .Append(_bubble.MeshTransform.DOLocalMove(Vector2.zero, SHAKE_TRANSFORM_DURATION))
                    .OnComplete(() =>
                    {

                        DeleteShakeTransformSequence();
                    });
            }
        }
        private void DeleteShakeTransformSequence()
        {
            DOTween.Kill(_shakeTransformSequenceID);
            _shakeTransformSequence = null;
        }
        // #########################
        private void CreateSetAsThrowableSequence()
        {
            if (_setAsThrowableSequence == null)
            {
                _setAsThrowableSequence = DOTween.Sequence();
                _setAsThrowableSequenceID = Guid.NewGuid();
                _setAsThrowableSequence.id = _setAsThrowableSequenceID;

                _setAsThrowableSequence.Append(transform.DOLocalMove(Vector2.zero, SET_AS_THROWABLE_SEQUENCE_DURATION * 0.5f))
                    .Join(_bubble.MeshTransform.DOScale(Vector2.one * 0.14f, SET_AS_THROWABLE_SEQUENCE_DURATION * 0.5f).SetEase(Ease.OutElastic))
                    .Append(transform.DOShakePosition(SET_AS_THROWABLE_SEQUENCE_DURATION, new Vector2(0.07f, 0f), 10, 0))
                    .OnComplete(() => {
                        //BubbleManager.SetFirstThrowable(this);
                        _bubble.MeshTransform.localScale = Vector2.one * 0.14f;
                        GameEvents.OnGameStateChange?.Invoke(Enums.GameState.Ready);
                        BubbleEvents.OnCheckSurroundings?.Invoke();
                        DeleteSetAsThrowableSequence();
                    });
            }
        }
        private void DeleteSetAsThrowableSequence()
        {
            DOTween.Kill(_setAsThrowableSequenceID);
            _setAsThrowableSequence = null;
        }
        // #########################
        private void CreateEnableSequence()
        {
            if (_enableSequence == null)
            {
                _enableSequence = DOTween.Sequence();
                _enableSequenceID = Guid.NewGuid();
                _enableSequence.id = _enableSequenceID;

                _bubble.MeshTransform.localScale = Vector2.zero;
                Vector2 targetScale = _bubble.CurrentState == Enums.BubbleStates.ThrownSecond ? Vector2.one * 0.1f : Vector2.one * 0.14f;

                _enableSequence.Append(_bubble.MeshTransform.DOScale(targetScale, 0.5f).SetEase(Ease.OutBounce))
                    .OnComplete(DeleteEnableSequence);
            }
        }
        private void DeleteEnableSequence()
        {
            DOTween.Kill(_enableSequenceID);
            _enableSequence = null;
        }
        // #########################
        private void CreateDisableSequence()
        {
            if (_disableSequence == null)
            {
                _disableSequence = DOTween.Sequence();
                _disableSequenceID = Guid.NewGuid();
                _disableSequence.id = _disableSequenceID;

                _disableSequence.Append(_bubble.MeshTransform.DOScale(Vector2.zero, 0.5f).SetEase(Ease.OutBounce))
                    .OnComplete(() => {
                        DeleteDisableSequence();
                        gameObject.SetActive(false);
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
