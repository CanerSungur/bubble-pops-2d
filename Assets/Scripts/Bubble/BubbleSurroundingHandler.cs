using UnityEngine;
using System.Collections.Generic;

namespace BubblePops
{
    public class BubbleSurroundingHandler : MonoBehaviour
    {
        [SerializeField] private LayerMask _hitLayers;

        #region COMPONENTS
        private Bubble _bubble;
        #endregion

        #region FIELDS
        private List<Bubble> _surroundingBubbles = new();
        private List<Bubble> _mergeableBubbles = new();
        private readonly Dictionary<Enums.BubbleDirection, Vector2> _directions = new();
        private List<Enums.BubbleDirection> _emptyDirections = new();
        private Vector2 _directionRight, _directionRightTop, _directionRightBottom, _directionLeft, _directionLeftTop, _directionLeftBottom;
        #endregion

        #region GETTERS
        public List<Bubble> SurroundingBubbles => _surroundingBubbles;
        public List<Bubble> MergeableBubbles => _mergeableBubbles;
        #endregion

        #region CONSTANTS
        private const float RAY_LENGTH = 0.6f;
        #endregion

        public void Init(Bubble bubble)
        {
            if (_bubble == null)
            {
                _bubble = bubble;
                CreateRayDirections();
            }
        }

        #region PUBLICS
        public void CheckSurroundings()
        {
            UpdateSurroundings();
            //ShakeSurroundingBubbles();
            CheckEmptySlotSpawn();
        }
        public void CheckSurroundings(Bubble thrownBubble)
        {
            UpdateSurroundings();

            foreach (Bubble bubble in _surroundingBubbles)
                bubble.SurroundingHandler.UpdateSurroundings();

            if (_mergeableBubbles.Count > 0)
            {
                if (_bubble.MergeChainCount > 0)
                    _bubble.OnMergeChainHappened?.Invoke();

                BubbleManager.ResetBubblesToMerge();
                CheckForMergeableBubbles(_mergeableBubbles);
                BubbleEvents.OnStartMerge?.Invoke(_bubble);
            }
            else
            {
                if (_bubble.MergeChainCount == 0)
                    ShakeSurroundingBubbles();

                CheckEmptySlotSpawn();
                GameFlowEvents.OnGameStateChange?.Invoke(Enums.GameState.PreparingNewBubble);
            }
        }
        #endregion

        #region HELPERS
        private Vector2 GetDirectionVector2D(float angle) => new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)).normalized;
        private void CreateRayDirections()
        {
            _directionRight = transform.right;
            _directions.Add(Enums.BubbleDirection.Right, _directionRight);
            _directionRightTop = GetDirectionVector2D(45);
            _directions.Add(Enums.BubbleDirection.RightTop, _directionRightTop);
            _directionRightBottom = GetDirectionVector2D(315);
            _directions.Add(Enums.BubbleDirection.RightBottom, _directionRightBottom);
            _directionLeft = -_directionRight;
            _directions.Add(Enums.BubbleDirection.Left, _directionLeft);
            _directionLeftTop = GetDirectionVector2D(135);
            _directions.Add(Enums.BubbleDirection.LeftTop, _directionLeftTop);
            _directionLeftBottom = GetDirectionVector2D(225);
            _directions.Add(Enums.BubbleDirection.LeftBottom, _directionLeftBottom);
        }
        private void ShootRay(Enums.BubbleDirection directionKey)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, _directions[directionKey], RAY_LENGTH, _hitLayers);
            if (hit.collider == null)
                AddEmptyDirection(directionKey);
            else if (hit.transform.TryGetComponent(out Bubble bubble))
            {
                AddBubble(bubble);
                if (_bubble.Exponent == bubble.Exponent)
                    AddMergeableBubble(bubble);
            }
        }
        private void UpdateSurroundings()
        {
            _emptyDirections.Clear();
            _surroundingBubbles.Clear();
            _mergeableBubbles.Clear();
            foreach (var direction in _directions)
                ShootRay(direction.Key);
        }
        // this function checks 4 layer of bubbles but needs to be recursive to be more generic
        private void CheckForMergeableBubbles(List<Bubble> mergeableBubbles)
        {
            if (mergeableBubbles == null || mergeableBubbles.Count == 0) return;

            for (int i = 0; i < mergeableBubbles.Count; i++)
            {
                Bubble firstBubble = mergeableBubbles[i];
                BubbleManager.AddBubblesToMerge(firstBubble);
                for (int j = 0; j < firstBubble.SurroundingHandler.MergeableBubbles.Count; j++)
                {
                    Bubble secondBubble = firstBubble.SurroundingHandler.MergeableBubbles[j];
                    BubbleManager.AddBubblesToMerge(secondBubble);
                    for (int k = 0; k < secondBubble.SurroundingHandler.MergeableBubbles.Count; k++)
                    {
                        Bubble thirdBubble = secondBubble.SurroundingHandler.MergeableBubbles[k];
                        BubbleManager.AddBubblesToMerge(thirdBubble);
                    }
                }
                //bubble.SurroundingHandler.CheckForMergeableBubbles(bubble.SurroundingHandler.MergeableBubbles);
            }

            //for (int i = 0; i < mergeableBubbles.Count; i++)
            //{
            //    Bubble bubble = mergeableBubbles[i];
            //    BubbleManager.AddBubblesToMerge(bubble);
            //    if (bubble.SurroundingHandler.MergeableBubbles.Count > 0)
            //        CheckForMergeableBubbles(bubble.SurroundingHandler.MergeableBubbles);
            //}

            BubbleManager.RemoveBubblesToMerge(_bubble);
            //Debug.Log("Bubbles To Merge: " + BubbleManager.BubblesToMerge.Count);
        }
        #endregion

        #region EVENT HANDLER FUNCTIONS
        private void ShakeSurroundingBubbles()
        {
            if (_surroundingBubbles.Count > 0)
            {
                foreach (Bubble bubble in _surroundingBubbles)
                    bubble.OnShakeTransform?.Invoke();
            }
        }
        private void CheckEmptySlotSpawn()
        {
            if (_emptyDirections.Count > 0)
            {
                int count = _emptyDirections.Count;
                for (int i = 0; i < count; i++)
                {
                    Enums.BubbleDirection emptyDirection = _emptyDirections[0];

                    SpawnEvents.OnSpawnEmptySlot?.Invoke(_bubble, emptyDirection);
                    RemoveEmptyDirection(emptyDirection);
                }
            }
        }
        #endregion

        #region LISTING FUNCTIONS
        private void AddBubble(Bubble bubble)
        {
            if (!_surroundingBubbles.Contains(bubble))
                _surroundingBubbles.Add(bubble);
        }
        private void RemoveBubble(Bubble bubble)
        {
            if (_surroundingBubbles.Contains(bubble))
                _surroundingBubbles.Remove(bubble);
        }
        private void AddEmptyDirection(Enums.BubbleDirection direction)
        {
            if (_bubble.ColumnNumber == 0 && direction is Enums.BubbleDirection.Left or Enums.BubbleDirection.Right or Enums.BubbleDirection.LeftTop or Enums.BubbleDirection.RightTop) return;
            else if (_bubble.RowNumber == 0)
            {
                if (direction is Enums.BubbleDirection.Left or Enums.BubbleDirection.LeftTop) return;
                else if ((_bubble.ColumnNumber == 1 || _bubble.ColumnNumber % 2 != 0) && direction is Enums.BubbleDirection.LeftBottom) return;
            }
            else if (_bubble.RowNumber == SpawnManager.ROW_NUMBER - 1)
            {
                if (direction is Enums.BubbleDirection.Right or Enums.BubbleDirection.RightTop) return;
                else if (_bubble.ColumnNumber % 2 == 0 && direction is Enums.BubbleDirection.RightBottom) return;
            }

            if (!_emptyDirections.Contains(direction))
                _emptyDirections.Add(direction);
        }
        private void RemoveEmptyDirection(Enums.BubbleDirection direction)
        {
            if (_emptyDirections.Contains(direction))
                _emptyDirections.Remove(direction);
        }
        public void AddMergeableBubble(Bubble bubble)
        {
            if (!_mergeableBubbles.Contains(bubble))
                _mergeableBubbles.Add(bubble);
        }
        public void RemoveMergeableBubble(Bubble bubble)
        {
            if (_mergeableBubbles.Contains(bubble))
                _mergeableBubbles.Remove(bubble);
        }
        #endregion
    }
}
