using UnityEngine;
using DG.Tweening;
using System;

namespace BubblePops
{
    public class MapMovementManager : MonoBehaviour
    {
        [Header("-- SETUP --")]
        [SerializeField] private Transform _bubbleContainerTransform;
        [SerializeField] private int _minBubbleCountTrigger = 26;
        [SerializeField] private int _minDistanceToBottom = 3;
        private Enums.ColumnLeanSide _topColumnLeanSide;
        private bool _firstThrow;

        #region SEQUENCE
        private Sequence _moveSequence;
        private Guid _moveSequenceID;
        #endregion

        public void Init(GameManager gameManager)
        {  
            _firstThrow = true;
            _topColumnLeanSide = Enums.ColumnLeanSide.Right;
            GameEvents.OnCheckMapMovement += CheckMapMovement;
        }

        private void OnDisable() 
        {
            GameEvents.OnCheckMapMovement -= CheckMapMovement;    
        }

        private void Update() 
        {
            if (Input.GetKeyDown(KeyCode.M))
                GameEvents.OnCheckMapMovement?.Invoke();    
        }

        #region EVENT HANDLER FUNCTIONS
        private void CheckMapMovement()
        {
            if (_firstThrow)
            {
                _firstThrow = false;
                return;
            }

            // if (BubbleManager.BubblesInSlot.Count < _minBubbleCountTrigger)
            // {
            //     TriggerNewColumnSpawn();
            //     StartMoveSequence();
            // }   

            TriggerNewColumnSpawn();
            StartMoveSequence();       
            Debug.Log("GO DOWN!");      
        }
        #endregion

        #region HELPERS
        private void TriggerNewColumnSpawn()
        {
            Enums.ColumnLeanSide newColumnLeanSide = _topColumnLeanSide == Enums.ColumnLeanSide.Right ? Enums.ColumnLeanSide.Left : Enums.ColumnLeanSide.Right;
            SpawnEvents.OnSpawnNewColumn?.Invoke(newColumnLeanSide);
            _topColumnLeanSide = newColumnLeanSide;
        }
        #endregion

        #region DOTWEEN FUNCTIONS
        private void StartMoveSequence()
        {
            CreateMoveSequence();
            _moveSequence.Play();
        }
        private void CreateMoveSequence()
        {
            if (_moveSequence == null)
            {
                _moveSequence = DOTween.Sequence();
                _moveSequenceID = Guid.NewGuid();
                _moveSequence.id = _moveSequenceID;

                float newYPos = _bubbleContainerTransform.localPosition.y + SpawnManager.COLUMN_OFFSET;

                _moveSequence.Append(_bubbleContainerTransform.DOLocalMoveY(newYPos, 0.5f)).SetEase(Ease.InSine)
                .OnComplete(() => {
                    _bubbleContainerTransform.localPosition = new Vector2(_bubbleContainerTransform.localPosition.x, newYPos);
                    // BubbleEvents.OnCheckSurroundings?.Invoke();
                    DeleteMoveSequence();
                });
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
