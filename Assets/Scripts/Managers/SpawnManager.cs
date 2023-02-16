using UnityEngine;

namespace BubblePops
{
    public class SpawnManager : MonoBehaviour
    {
        [Header("-- PREFAB SETUP --")]
        [SerializeField] private Bubble _bubblePrefab;
        [SerializeField] private EmptySlot _emptySlotPrefab;

        [Header("-- CONTAINER SETUP --")]
        [SerializeField] private Transform _bubbleContainerTransform;
        [SerializeField] private Transform _throwableContainerTransform;

        [Header("-- BUBBLE COLOR SETUP --")]
        [SerializeField] private Color[] _exponentColors;

        #region FIELDS
        private int _currentRowNumber = 0;
        private int _currentColumnNumber = 0;
        #endregion

        #region GETTERS
        public Color[] ExponentColors => _exponentColors;
        public Transform BubbleContainerTransform => _bubbleContainerTransform;
        #endregion

        #region CONSTANTS
        internal const int ROW_NUMBER = 6;
        internal const int COLUMN_NUMBER = 3;
        private const float BUBBLE_ROW_OFFSET = 0.73f;
        private const float BUBBLE_COLUMN_OFFSET = -0.63f;
        private const float BUBBLE_DOUBLE_ROW_OFFSET = -0.3639999f;

        private const float THROWABLE_BUBBLE_OFFSET = -0.75f;
        #endregion

        public void Init(GameManager gameManager)
        {
            SpawnBubbles();
            SpawnEmptySlots();

            SpawnFirstThrowableBubble();
            SpawnSecondThrowableBubble();

            SpawnEvents.OnSpawnEmptySlot += SpawnEmptySlot;
            SpawnEvents.OnSpawnSecondThrowable += SpawnSecondThrowableBubble;
        }

        private void OnDisable()
        {
            SpawnEvents.OnSpawnEmptySlot -= SpawnEmptySlot;
            SpawnEvents.OnSpawnSecondThrowable -= SpawnSecondThrowableBubble;
        }

        #region EVENT HANDLER FUNCTIONS
        private void SpawnEmptySlot(Bubble bubble, Enums.BubbleDirection direction)
        {
            EmptySlot emptySlot = PoolManager.Instance.SpawnFromPool(Enums.PoolStamp.EmptySlot, Vector3.zero, Quaternion.identity, _bubbleContainerTransform).GetComponent<EmptySlot>();
            int columnOffset = direction is Enums.BubbleDirection.LeftTop or Enums.BubbleDirection.RightTop ? -1
                : direction is Enums.BubbleDirection.Left or Enums.BubbleDirection.Right ? 0
                : 1;

            int rowOffset = direction is Enums.BubbleDirection.Left ? -1
                : direction is Enums.BubbleDirection.Right ? +1
                : (bubble.ColumnNumber == 0 || bubble.ColumnNumber % 2 == 0) ? ((direction is Enums.BubbleDirection.LeftTop or Enums.BubbleDirection.LeftBottom) ? 0 : 1)
                : (bubble.ColumnNumber == 1 || bubble.ColumnNumber % 2 != 0) && direction is Enums.BubbleDirection.RightTop or Enums.BubbleDirection.RightBottom ? 0
                : -1;

            if ((bubble.ColumnNumber + columnOffset) == 1 || (bubble.ColumnNumber + columnOffset) % 2 != 0)
                emptySlot.transform.localPosition = new Vector2(((bubble.RowNumber + rowOffset) * BUBBLE_ROW_OFFSET) + BUBBLE_DOUBLE_ROW_OFFSET, (bubble.ColumnNumber + columnOffset) * BUBBLE_COLUMN_OFFSET);
            else
                emptySlot.transform.localPosition = new Vector2((bubble.RowNumber + rowOffset) * BUBBLE_ROW_OFFSET, (bubble.ColumnNumber + columnOffset) * BUBBLE_COLUMN_OFFSET);

            emptySlot.Init(this, bubble.RowNumber + rowOffset, bubble.ColumnNumber + columnOffset);
            bubble.OnEmptyDirectionIsFilled(direction);
        }
        #endregion

        #region FIRST SPAWN FUNCTIONS
        private void SpawnBubbles()
        {
            for (int i = 0; i < COLUMN_NUMBER; i++)
            {
                for (int j = 0; j < ROW_NUMBER; j++)
                {
                    Bubble bubble = PoolManager.Instance.SpawnFromPool(Enums.PoolStamp.Bubble, Vector3.zero, Quaternion.identity, _bubbleContainerTransform).GetComponent<Bubble>();
                    BubbleManager.AddBubbleInSlot(bubble);
                    bubble.InitAsSlotBubble(this, _currentRowNumber, _currentColumnNumber);
                    if (i == 1 || i % 2 != 0)
                        bubble.transform.localPosition = new Vector2((_currentRowNumber * BUBBLE_ROW_OFFSET) + BUBBLE_DOUBLE_ROW_OFFSET, _currentColumnNumber * BUBBLE_COLUMN_OFFSET);
                    else
                        bubble.transform.localPosition = new Vector2(_currentRowNumber * BUBBLE_ROW_OFFSET, _currentColumnNumber * BUBBLE_COLUMN_OFFSET);
                    _currentRowNumber++;
                }
                _currentRowNumber = 0;
                _currentColumnNumber++;
            }
        }
        private void SpawnEmptySlots()
        {
            for (int i = 0; i < ROW_NUMBER; i++)
            {
                EmptySlot emptySlot = PoolManager.Instance.SpawnFromPool(Enums.PoolStamp.EmptySlot, Vector3.zero, Quaternion.identity, _bubbleContainerTransform).GetComponent<EmptySlot>();
                emptySlot.Init(this, i, COLUMN_NUMBER);

                //if (COLUMN_NUMBER == 0 || COLUMN_NUMBER % 2 == 0)
                //    emptySlot.transform.localPosition = new Vector2(i * BUBBLE_ROW_OFFSET, COLUMN_NUMBER * BUBBLE_COLUMN_OFFSET);
                if (COLUMN_NUMBER == 1 || COLUMN_NUMBER % 2 != 0)
                    emptySlot.transform.localPosition = new Vector2((i * BUBBLE_ROW_OFFSET) + BUBBLE_DOUBLE_ROW_OFFSET, COLUMN_NUMBER * BUBBLE_COLUMN_OFFSET);
            }
        }
        #endregion

        #region THROWABLE BUBBLE FUNCTIONS
        private void SpawnFirstThrowableBubble()
        {
            Bubble bubble = PoolManager.Instance.SpawnFromPool(Enums.PoolStamp.Bubble, Vector3.zero, Quaternion.identity, _throwableContainerTransform).GetComponent<Bubble>();
            bubble.InitAsThrowableBubble(this, true);
            bubble.transform.localPosition = Vector2.zero;
            BubbleManager.SetFirstThrowable(bubble);
        }
        private void SpawnSecondThrowableBubble()
        {
            Bubble bubble = PoolManager.Instance.SpawnFromPool(Enums.PoolStamp.Bubble, Vector3.zero, Quaternion.identity, _throwableContainerTransform).GetComponent<Bubble>();
            bubble.InitAsThrowableBubble(this, false);
            bubble.transform.localPosition = new Vector2(THROWABLE_BUBBLE_OFFSET, 0);
            bubble.transform.localScale = Vector3.one * 0.75f;
            BubbleManager.SetSecondThrowable(bubble);
        }
        #endregion
    }
}
