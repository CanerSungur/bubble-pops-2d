using UnityEngine;
using DG.Tweening;
using System;

namespace BubblePops
{
    public class MapMovementManager : MonoBehaviour
    {
        [Header("-- SETUP --")]
        [SerializeField] private Transform _bubbleContainerTransform;
        [SerializeField] private int _oneRowSpawnTriggerCount = 25;
        [SerializeField] private int _twoRowSpawnTriggerCount = 12;
        private int _threeRowSpawnTriggerCount;
        private Enums.ColumnLeanSide _topColumnLeanSide;
        private bool _firstThrow, _upwardsMovementRequested, _downwardMovementRequested;

        #region CONSTANTS
        public const int MIN_DISTANCE_TO_BOTTOM = 2;
        public const float TOP_COLUMN_Y_AXIS = 3.5f; // 3.135f
        #endregion

        #region SEQUENCE
        private Sequence _moveSequence;
        private Guid _moveSequenceID;
        #endregion

        public void Init(GameManager gameManager)
        {  
            _firstThrow = true;
            _upwardsMovementRequested = _downwardMovementRequested = false;
            _threeRowSpawnTriggerCount = 6; 
            _topColumnLeanSide = Enums.ColumnLeanSide.Right;

            MapEvents.OnCheckMapMovement += CheckMapMovement;
            MapEvents.OnMoveMapUpwards += MoveMapUpwards;
            MapEvents.OnMoveMapDownwards += MoveMapDownwards;
        }

        private void OnDisable() 
        {
            MapEvents.OnCheckMapMovement -= CheckMapMovement;    
            MapEvents.OnMoveMapUpwards -= MoveMapUpwards;
            MapEvents.OnMoveMapDownwards -= MoveMapDownwards;
        }

        #region EVENT HANDLER FUNCTIONS
        private void CheckMapMovement()
        {
            if (_firstThrow)
            {
                _firstThrow = false;
                return;
            }

            if (_upwardsMovementRequested)
            {
                StartMoveSequence(1, true);
                _upwardsMovementRequested = false;
            }
            else if (BubbleManager.BubblesInSlot.Count < _oneRowSpawnTriggerCount)
            {
                int rowCount = BubbleManager.BubblesInSlot.Count <= _threeRowSpawnTriggerCount ? 3 
                            : BubbleManager.BubblesInSlot.Count <= _twoRowSpawnTriggerCount ? 2 : 1;
                
                TriggerNewColumnSpawn(rowCount);
                StartMoveSequence(rowCount);
            }
            else if (_downwardMovementRequested)
            {
                StartMoveSequence(1);
                _downwardMovementRequested = false;
            }
        }
        private void MoveMapUpwards()
        {
            _upwardsMovementRequested = true;
        }
        private void MoveMapDownwards()
        {
            _downwardMovementRequested = true;
        }
        #endregion

        #region HELPERS
        private void TriggerNewColumnSpawn(int rowCount)
        {
            for (int i = 0; i < rowCount; i++)
            {
                Enums.ColumnLeanSide newColumnLeanSide = _topColumnLeanSide == Enums.ColumnLeanSide.Right ? Enums.ColumnLeanSide.Left : Enums.ColumnLeanSide.Right;
                SpawnEvents.OnSpawnNewColumn?.Invoke(newColumnLeanSide);
                _topColumnLeanSide = newColumnLeanSide;    
            }
        }
        #endregion

        #region DOTWEEN FUNCTIONS
        private void StartMoveSequence(int rowCount, bool moveUp = false)
        {
            CreateMoveSequence(rowCount, moveUp);
            _moveSequence.Play();
        }
        private void CreateMoveSequence(int rowCount, bool moveUp = false)
        {
            if (_moveSequence == null)
            {
                _moveSequence = DOTween.Sequence();
                _moveSequenceID = Guid.NewGuid();
                _moveSequence.id = _moveSequenceID;

                float newYPos = moveUp ? _bubbleContainerTransform.localPosition.y + (SpawnManager.COLUMN_OFFSET * -rowCount)
                                    : _bubbleContainerTransform.localPosition.y + (SpawnManager.COLUMN_OFFSET * rowCount);

                _moveSequence.Append(_bubbleContainerTransform.DOLocalMoveY(newYPos, 0.5f)).SetEase(Ease.InSine)
                .OnComplete(() => {
                    _bubbleContainerTransform.localPosition = new Vector2(_bubbleContainerTransform.localPosition.x, newYPos);
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
