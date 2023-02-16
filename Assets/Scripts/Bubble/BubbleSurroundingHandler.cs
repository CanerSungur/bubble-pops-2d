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
        private Dictionary<Enums.BubbleDirection, Vector2> _directions = new();
        private List<Enums.BubbleDirection> _emptyDirections = new();
        private Vector2 _directionRight, _directionRightTop, _directionRightBottom, _directionLeft, _directionLeftTop, _directionLeftBottom;
        #endregion

        #region CONSTANTS
        private const float RAY_LENGTH = .6f;
        #endregion

        public void Init(Bubble bubble)
        {
            if (_bubble == null)
            {
                _bubble = bubble;
                CreateRayDirections();
            }

            _bubble.OnCheckSurroundings += CheckSurroundings;
            _bubble.OnEmptyDirectionIsFilled += RemoveEmptyDirection;
        }

        private void OnDisable()
        {
            if (_bubble == null) return;
            _bubble.OnCheckSurroundings -= CheckSurroundings;
            _bubble.OnEmptyDirectionIsFilled -= RemoveEmptyDirection;
        }

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
                AddBubble(bubble);
        }
        private void CheckEmptySlotSpawn()
        {
            if (_emptyDirections != null && _emptyDirections.Count > 0)
            {
                int count = _emptyDirections.Count;
                for (int i = 0; i < count; i++)
                    SpawnEvents.OnSpawnEmptySlot?.Invoke(_bubble, _emptyDirections[0]);
            }
        }
        #endregion

        #region EVENT HANDLER FUNCTIONS
        private void CheckSurroundings()
        {
            foreach (var direction in _directions)
                ShootRay(direction.Key);

            if (_surroundingBubbles != null && _surroundingBubbles.Count > 0)
            {
                foreach (Bubble bubble in _surroundingBubbles)
                    bubble.OnShakeTransform?.Invoke();
            }

            CheckEmptySlotSpawn();
        }
        private void RemoveEmptyDirection(Enums.BubbleDirection direction)
        {
            if (_emptyDirections.Contains(direction))
                _emptyDirections.Remove(direction);
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

            _emptyDirections.Add(direction);
        }
        #endregion
    }
}
