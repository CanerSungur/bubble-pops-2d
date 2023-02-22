using UnityEngine;
using DG.Tweening;
using System;

namespace BubblePops
{
    public class BubbleThrowHandler : MonoBehaviour
    {
        [Header("-- SETUP --")]
        [SerializeField] private float _throwDuration = 0.4f;

        #region FIELDS
        private Bubble _bubble;
        #endregion

        #region SEQUENCES
        private Sequence _moveSequence;
        private Guid _moveSequenceID;
        #endregion

        public void Init(Bubble bubble)
        {
            if (_bubble == null)
                _bubble = bubble;
        }

        #region PUBLICS
        public void GetThrown(EmptySlot emptySlot, Vector2? bouncePos = null)
        {
            StartMoveSequence(emptySlot, bouncePos);
            // Destroy(emptySlot.gameObject);
            emptySlot.gameObject.SetActive(false);
        }
        #endregion

        #region DOTWEEN FUNCTIONS
        private void StartMoveSequence(EmptySlot emptySlot, Vector2? bounceTarget = null)
        {
            CreateMoveSequence(emptySlot, bounceTarget);
            _moveSequence.Play();
        }
        private void CreateMoveSequence(EmptySlot emptySlot, Vector2? bounceTarget = null)
        {
            if (_moveSequence == null)
            {
                _moveSequence = DOTween.Sequence();
                _moveSequenceID = Guid.NewGuid();
                _moveSequence.id = _moveSequenceID;

                Vector2 targetPos = emptySlot.transform.position;

                if (bounceTarget == null)
                {
                    _moveSequence.Append(transform.DOMove(targetPos, _throwDuration * 0.5f))
                    .OnComplete(() => {
                        transform.position = targetPos;
                        _bubble.OnThrowSuccessful?.Invoke();
                        // emptySlot.gameObject.SetActive(false);
                        // Destroy(emptySlot.gameObject);
                        DeleteMoveSequence();
                    });
                }
                else
                {
                    _moveSequence.Append(transform.DOMove((Vector2)bounceTarget, _throwDuration * 0.5f))
                        .Append(transform.DOMove(targetPos, _throwDuration * 0.5f))
                        .OnComplete(() => {
                            transform.position = targetPos;
                            _bubble.OnThrowSuccessful?.Invoke();
                            // emptySlot.gameObject.SetActive(false);
                            // Destroy(emptySlot.gameObject);
                            DeleteMoveSequence();
                    });
                }
            }
        }
        private void DeleteMoveSequence()
        {
            DOTween.Kill(_moveSequenceID);
            _moveSequence = null;
        }
        #endregion
    }
}
