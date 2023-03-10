using DG.Tweening;
using UnityEngine;
using System;
using BubblePops.Utility;

namespace BubblePops
{
    public class BubbleMergeHandler : MonoBehaviour
    {
        #region COMPONENTS
        private Bubble _bubble;
        #endregion

        #region SEQUENCE
        private Sequence _mergeSequence, _moveForOtherBubblesSequence;
        private Guid _mergeSequenceID, _moveForOtherBubblesSequenceID;
        #endregion

        #region CONSTANTS
        private const float MERGE_SEQUENCE_DURATION = 0.3f;
        private const float MOVE_FOR_OTHER_BUBBLES_SEQ_DURATION = 0.3f;
        private const float DURATION_DECREASE_AMOUNT = 0.035f;
        private const float MIN_DURATION = 0.1f;
        #endregion

        public void Init(Bubble bubble)
        {
            if (_bubble == null)
                _bubble = bubble;
        }

        #region PUBLICS
        public void StartMergeSequence(Bubble topMostBubble)
        {
            DeleteMergeSequence();
            CreateMergeSequence(topMostBubble);
            _mergeSequence.Play();
        }
        public void StartMoveForOtherBubblesSequence()
        {
            _bubble.EffectHandler.TriggerTextFadeSequence();
            _bubble.EffectHandler.SpawnBubbleMergePS(_bubble.Color);

            CreateMoveForOtherBubblesSequence();
            _moveForOtherBubblesSequence.Play();
        }
        #endregion

        #region DOTWEEN FUNCTIONS
        private void CreateMergeSequence(Bubble topMostBubble)
        {
            if (_mergeSequence == null)
            {
                _mergeSequence = DOTween.Sequence();
                _mergeSequenceID = Guid.NewGuid();
                _mergeSequence.id = _mergeSequenceID;

                int mergeCount = BubbleManager.BubblesToMerge.Count;
                float duration = MERGE_SEQUENCE_DURATION - (_bubble.MergeChainCount * DURATION_DECREASE_AMOUNT);
                if (duration < MIN_DURATION) duration = MIN_DURATION;

                _mergeSequence.Append(transform.DOLocalMove(topMostBubble.transform.localPosition, duration))
                    .OnComplete(() => {
                        _bubble.SetRowAndColumn(topMostBubble);
                        _bubble.OnMergeHappened?.Invoke(mergeCount);
                        DeleteMergeSequence();

                        Utils.DoActionAfterDelay(_bubble, 0.1f, () => {
                            _bubble.StoppedMerging();
                            BubbleEvents.OnPositionChanged?.Invoke(_bubble);
                            BubbleEvents.OnCheckSurroundings?.Invoke();
                            _bubble.SurroundingHandler.TryMerging(_bubble);
                            });
                    });
            }
        }
        private void DeleteMergeSequence()
        {
            DOTween.Kill(_mergeSequenceID);
            _mergeSequence = null;
        }
        // ###################
        private void CreateMoveForOtherBubblesSequence()
        {
            if (_moveForOtherBubblesSequence == null)
            {
                _moveForOtherBubblesSequence = DOTween.Sequence();
                _moveForOtherBubblesSequenceID = Guid.NewGuid();
                _moveForOtherBubblesSequence.id = _moveForOtherBubblesSequenceID;

                float duration = MOVE_FOR_OTHER_BUBBLES_SEQ_DURATION - (_bubble.MergeChainCount * DURATION_DECREASE_AMOUNT);
                if (duration < MIN_DURATION) duration = MIN_DURATION;

                _moveForOtherBubblesSequence.Append(transform.DOLocalMove(Vector2.zero, duration))
                    .OnComplete(() => {
                        BubbleEvents.OnPositionChanged?.Invoke(_bubble);
                        DeleteMoveForOtherBubblesSequence();
                        _bubble.StoppedMerging();
                        gameObject.SetActive(false);
                    });
            }
        }
        private void DeleteMoveForOtherBubblesSequence()
        {
            DOTween.Kill(_moveForOtherBubblesSequenceID);
            _moveForOtherBubblesSequence = null;
        }
        #endregion
    }
}
