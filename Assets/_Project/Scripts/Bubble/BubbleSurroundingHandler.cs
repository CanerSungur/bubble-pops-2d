using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace BubblePops
{
    public class BubbleSurroundingHandler : MonoBehaviour
    {
        [SerializeField] private LayerMask _hitLayers;
        [SerializeField] private LayerMask _bubbleLayer;

        #region COMPONENTS
        private Bubble _bubble;
        #endregion

        #region FIELDS
        private List<Bubble> _surroundingBubbles = new();
        private List<Bubble> _mergeableBubbles = new();
        private Dictionary<Enums.BubbleDirection, Bubble> _affiliatedBubbles = new();
        private Dictionary<Enums.BubbleDirection, Bubble> _dependantBubbles = new();
        private readonly Dictionary<Enums.BubbleDirection, Vector2> _directions = new();
        private List<Enums.BubbleDirection> _emptyDirections = new();
        private Vector2 _directionRight, _directionRightTop, _directionRightBottom, _directionLeft, _directionLeftTop, _directionLeftBottom;
        #endregion

        #region GETTERS
        public List<Bubble> SurroundingBubbles => _surroundingBubbles;
        public List<Bubble> MergeableBubbles => _mergeableBubbles;
        public Dictionary<Enums.BubbleDirection, Bubble> AffiliatedBubbles => _affiliatedBubbles;
        public Dictionary<Enums.BubbleDirection, Bubble> DependantBubbles => _dependantBubbles;
        public Dictionary<Enums.BubbleDirection, Vector2> Directions => _directions;
        #endregion

        #region CONSTANTS
        private const float RAY_LENGTH = 0.75f;
        #endregion

        public void Init(Bubble bubble)
        {
            if (_bubble == null)
            {
                _bubble = bubble;
                CreateRayDirections();
            }

            BubbleEvents.OnPositionChanged += HandlePositionChange;
        }

        private void OnDisable() 
        {
            ClearAllLists();    

            if (_bubble == null) return;
            BubbleEvents.OnPositionChanged -= HandlePositionChange;
        }

        #region PUBLICS
        
        public void CheckSurroundings()
        {
            UpdateSurroundings();
            CheckEmptySlotSpawn();
            CheckForBubbleDrop();

            // if (_bubble.IsTooClose)
            //     MapEvents.OnMoveMapUpwards?.Invoke();
        }
        public void TryMerging(Bubble thrownBubble)
        {
            CheckForBubblePop();
            if (_bubble.MergeChainCount == 0)
                ShakeSurroundingBubbles();

            foreach (Bubble bubble in _surroundingBubbles)
                bubble.SurroundingHandler.UpdateSurroundings();

            if (_mergeableBubbles.Count > 0)
                CheckForMergeActivation();
            else
            {
                CheckEmptySlotSpawn();
                GameEvents.OnGameStateChange?.Invoke(Enums.GameState.PreparingNewBubble);
                // GameEvents.OnCheckMapMovement?.Invoke();

            }

            if (_bubble.IsTooClose)
                MapEvents.OnMoveMapUpwards?.Invoke();
        }
        public void UpdateSurroundings()
        {
            _emptyDirections.Clear();
            _surroundingBubbles.Clear();
            _mergeableBubbles.Clear();
            foreach (var direction in _directions)
                ShootRay(direction.Key);
        }
        #endregion

        #region DIRECTION RELATED FUNCTIONS
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
            {
                AddEmptyDirection(directionKey);

                if (directionKey is Enums.BubbleDirection.LeftTop or Enums.BubbleDirection.RightTop)
                    RemoveBubbleFromDictionary(_affiliatedBubbles, directionKey);
                if (directionKey is Enums.BubbleDirection.LeftBottom or Enums.BubbleDirection.RightBottom)
                    RemoveBubbleFromDictionary(_dependantBubbles, directionKey);
            }
            else if (hit.transform.TryGetComponent(out Bubble bubble))
            {
                AddBubbleToList(_surroundingBubbles, bubble);
                if (_bubble.Exponent == bubble.Exponent)
                    AddBubbleToList(_mergeableBubbles, bubble);

                if (directionKey is Enums.BubbleDirection.LeftTop or Enums.BubbleDirection.RightTop)
                    AddBubbleToDictionary(_affiliatedBubbles, directionKey, bubble);
                if (directionKey is Enums.BubbleDirection.LeftBottom or Enums.BubbleDirection.RightBottom)
                    AddBubbleToDictionary(_dependantBubbles, directionKey, bubble);
            }
            else
            {
                if (directionKey is Enums.BubbleDirection.LeftTop or Enums.BubbleDirection.RightTop)
                    RemoveBubbleFromDictionary(_affiliatedBubbles, directionKey);
                if (directionKey is Enums.BubbleDirection.LeftBottom or Enums.BubbleDirection.RightBottom)
                    RemoveBubbleFromDictionary(_dependantBubbles, directionKey);
            }
        }
        #endregion

        #region UPDATE & CHECK FUNCTIONS
        // this function checks 5 layer of bubbles but needs to be recursive to be more generic
        private void CheckForPossibleMergeableBubbles(List<Bubble> mergeableBubbles)
        {
            if (mergeableBubbles == null || mergeableBubbles.Count == 0) return;

            for (int i = 0; i < mergeableBubbles.Count; i++)
            {
                #region This part should be recursive
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
                        for (int l = 0; l < thirdBubble.SurroundingHandler.MergeableBubbles.Count; l++)
                        {
                            Bubble fourthBubble = thirdBubble.SurroundingHandler.MergeableBubbles[l];
                            BubbleManager.AddBubblesToMerge(fourthBubble);
                        }
                    }
                }
                #endregion
            }

            int newExponent = _bubble.Exponent + (BubbleManager.BubblesToMerge.Count - 1);// -1 because thrown bubble is inclusive
            SetBubbleForNextPossibleMerge(newExponent);

            // remove thrown bubble to calculate better
            BubbleManager.RemoveBubblesToMerge(_bubble);
        }
        private void SetBubbleForNextPossibleMerge(int newExponent)
        {
            Bubble bubbleForNextMerge = null;
            int potentialMergeCount = 0;

            for (int i = 0; i < BubbleManager.BubblesToMerge.Count; i++)
            {
                Bubble bubble = BubbleManager.BubblesToMerge[i];

                int mergeCount = 0;
                for (int j = 0; j < bubble.SurroundingHandler.SurroundingBubbles.Count; j++)
                {
                    Bubble potentialMerge = bubble.SurroundingHandler.SurroundingBubbles[j];
                    if (potentialMerge.Exponent == newExponent) mergeCount++;
                }
                if (mergeCount > potentialMergeCount)
                    bubbleForNextMerge = bubble;
            }
            BubbleManager.SetBubbleForNextMerge(bubbleForNextMerge);
        }
        private void CheckForBubblePop()
        {
            if (_bubble.IsPopped)
            {
                for (int i = 0; i < _surroundingBubbles.Count; i++)
                    _surroundingBubbles[i].Pop(false);

                _bubble.Pop(true);
                _bubble.EffectHandler.SpawnBubblePopPS();
                // BubbleEvents.OnCheckSurroundings?.Invoke();
                return;
            }
        }
        private void CheckForBubbleDrop()
        {
            if (!_bubble.IsMerging && _affiliatedBubbles.Count == 0 && !IsAttachedToABubble())
                _bubble.Drop();
        }
        private void CheckForMergeActivation()
        {
            if (_bubble.MergeChainCount > 0)
                _bubble.OnMergeChainHappened?.Invoke();

            BubbleManager.ResetBubblesToMerge();
            CheckForPossibleMergeableBubbles(_mergeableBubbles);
            BubbleEvents.OnStartMerge?.Invoke(_bubble);
        }
        private bool IsAttachedToABubble()
        {
            foreach (Bubble bubble in _surroundingBubbles)
            {
                if (!bubble.IsDropped && !_affiliatedBubbles.ContainsValue(bubble) && !_dependantBubbles.ContainsValue(bubble))
                {
                    foreach (Bubble surroundingBubble in bubble.SurroundingHandler.SurroundingBubbles)
                        if (surroundingBubble.SurroundingHandler.AffiliatedBubbles.Count == 0)
                            return false;
                        else
                        {
                            for (int i = 0; i < surroundingBubble.SurroundingHandler.AffiliatedBubbles.Count; i++)
                                if (surroundingBubble.SurroundingHandler.AffiliatedBubbles.ElementAt(i).Value.ColumnNumber >= surroundingBubble.ColumnNumber || surroundingBubble.ColumnNumber == bubble.ColumnNumber)
                                    return true;
                        }
                }
            }
            return false;

            // foreach (Bubble bubble in _surroundingBubbles)
            // {
            //     if (!bubble.IsDropped && !_affiliatedBubbles.ContainsValue(bubble) && !_dependantBubbles.ContainsValue(bubble))
            //     {
            //         foreach (Bubble surroundingBubble in bubble.SurroundingHandler.SurroundingBubbles)
            //             if (surroundingBubble.SurroundingHandler.AffiliatedBubbles.Count != 0)
            //                 return true;
            //     }
            // }
            // return false;
        }
        #endregion

        #region EVENT HANDLER FUNCTIONS
        private void ShakeSurroundingBubbles()
        {
            if (_surroundingBubbles.Count > 0)
            {
                foreach (Bubble bubble in _surroundingBubbles)
                    bubble.OnShakeTransform?.Invoke(_bubble);
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

                    Enums.ColumnLeanSide columnLeanSide = emptyDirection is Enums.BubbleDirection.Left or Enums.BubbleDirection.Right ? _bubble.ColumnLeanSide 
                    : _bubble.ColumnLeanSide == Enums.ColumnLeanSide.Left ? Enums.ColumnLeanSide.Right : Enums.ColumnLeanSide.Left;

                    SpawnEvents.OnSpawnEmptySlot?.Invoke(_bubble, emptyDirection, columnLeanSide);
                    RemoveEmptyDirection(emptyDirection);
                }
            }
        }
        private void HandlePositionChange(Bubble bubble)
        {
            RemoveBubbleFromList(_surroundingBubbles, bubble);
            RemoveBubbleFromList(_mergeableBubbles, bubble);
        }
        #endregion

        #region LISTING FUNCTIONS
        private void AddBubbleToList(List<Bubble> list, Bubble bubble)
        {
            if (!list.Contains(bubble))
                list.Add(bubble);
        }
        private void RemoveBubbleFromList(List<Bubble> list, Bubble bubble)
        {
            if (list.Contains(bubble))
                list.Remove(bubble);
        }
        private void AddEmptyDirection(Enums.BubbleDirection direction)
        {
            if (_bubble.ColumnNumber == -SpawnManager.TopColumnNumber && direction is Enums.BubbleDirection.Left or Enums.BubbleDirection.Right or Enums.BubbleDirection.LeftTop or Enums.BubbleDirection.RightTop) return;
            else if (_bubble.RowNumber == 0)
            {
                if (direction is Enums.BubbleDirection.Left or Enums.BubbleDirection.LeftTop) return;
                else if ((_bubble.ColumnNumber == 1 || _bubble.ColumnNumber == -1 || _bubble.ColumnNumber % 2 != 0) && direction is Enums.BubbleDirection.LeftBottom) return;
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
        private void AddBubbleToDictionary(Dictionary<Enums.BubbleDirection, Bubble> dict, Enums.BubbleDirection direction, Bubble bubble)
        {
            if (dict.ContainsKey(direction))
                dict[direction] = bubble;
            else
                dict.Add(direction, bubble);
        }
        private void RemoveBubbleFromDictionary(Dictionary<Enums.BubbleDirection, Bubble> dict, Enums.BubbleDirection direction)
        {
            if (dict.ContainsKey(direction))
                dict.Remove(direction);
        }
        private void ClearAllLists()
        {
            _emptyDirections.Clear();
            _surroundingBubbles.Clear();
            _mergeableBubbles.Clear();
            _affiliatedBubbles.Clear();
            _dependantBubbles.Clear();
        }
        #endregion
    }
}
