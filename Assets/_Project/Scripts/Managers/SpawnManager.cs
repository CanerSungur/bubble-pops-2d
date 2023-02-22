using UnityEngine;

namespace BubblePops
{
    public class SpawnManager : MonoBehaviour
    {
        [Header("-- CONTAINER SETUP --")]
        [SerializeField] private Transform _bubbleContainerTransform;
        [SerializeField] private Transform _throwableContainerTransform;

        #region GETTERS
        public Transform BubbleContainerTransform => _bubbleContainerTransform;
        #endregion

        #region CONSTANTS
        internal const int ROW_NUMBER = 6;
        internal const int COLUMN_NUMBER = 4;
        private const float ROW_OFFSET = 0.73f;
        public const float COLUMN_OFFSET = -0.63f;
        private const float DOUBLE_ROW_OFFSET = -0.3639999f;
        private const float THROWABLE_OFFSET = -0.75f;
        #endregion

        #region FIELDS
        private int _spawnedColumnCount;
        public static int TopColumnNumber { get; private set; }
        #endregion

        

        public void Init(GameManager gameManager)
        {
            TopColumnNumber = _spawnedColumnCount = 0;

            SpawnBubbles();
            SpawnFirstThrowableBubble();
            SpawnSecondThrowableBubble();

            SpawnEvents.OnSpawnEmptySlot += SpawnEmptySlot;
            SpawnEvents.OnSpawnSecondThrowable += SpawnSecondThrowableBubble;
            SpawnEvents.OnSpawnNewColumn += SpawnNewColumn;
        }

        private void OnDisable()
        {
            SpawnEvents.OnSpawnEmptySlot -= SpawnEmptySlot;
            SpawnEvents.OnSpawnSecondThrowable -= SpawnSecondThrowableBubble;
            SpawnEvents.OnSpawnNewColumn -= SpawnNewColumn;
        }

        #region EVENT HANDLER FUNCTIONS
        private void SpawnNewColumn(Enums.ColumnLeanSide columnLeanSide)
        {
            _spawnedColumnCount++;
            TopColumnNumber++;

            for (int i = 0; i < ROW_NUMBER; i++)
            {
                Bubble bubble = PoolManager.Instance.SpawnFromPool(Enums.PoolStamp.Bubble, Vector3.zero, Quaternion.identity, _bubbleContainerTransform).GetComponent<Bubble>();
                if (columnLeanSide == Enums.ColumnLeanSide.Left)
                    bubble.transform.localPosition = new Vector2((i * ROW_OFFSET) + DOUBLE_ROW_OFFSET, -TopColumnNumber * COLUMN_OFFSET);
                else
                    bubble.transform.localPosition = new Vector2(i * ROW_OFFSET, -TopColumnNumber * COLUMN_OFFSET);

                bubble.InitAsSlotBubble(this, i, -TopColumnNumber, columnLeanSide);
            }
        }
        private void SpawnEmptySlot(Bubble bubble, Enums.BubbleDirection direction, Enums.ColumnLeanSide columnLeanSide)
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

            if (columnLeanSide == Enums.ColumnLeanSide.Left)
                emptySlot.transform.localPosition = new Vector2(((bubble.RowNumber + rowOffset) * ROW_OFFSET) + DOUBLE_ROW_OFFSET, (bubble.ColumnNumber + columnOffset) * COLUMN_OFFSET);
            else
                emptySlot.transform.localPosition = new Vector2((bubble.RowNumber + rowOffset) * ROW_OFFSET, (bubble.ColumnNumber + columnOffset) * COLUMN_OFFSET);

            emptySlot.Init(this, bubble.RowNumber + rowOffset, bubble.ColumnNumber + columnOffset, columnLeanSide);
        }
        #endregion

        #region FIRST SPAWN FUNCTIONS
        private void SpawnBubbles()
        {
            int currentRowNumber = 0, currentColumnNumber = 0;

            for (int i = 0; i < COLUMN_NUMBER; i++)
            {
                for (int j = 0; j < ROW_NUMBER; j++)
                {
                    Bubble bubble = PoolManager.Instance.SpawnFromPool(Enums.PoolStamp.Bubble, Vector3.zero, Quaternion.identity, _bubbleContainerTransform).GetComponent<Bubble>();
                    if (i == 1 || i % 2 != 0)
                    {
                        bubble.InitAsSlotBubble(this, currentRowNumber, currentColumnNumber, Enums.ColumnLeanSide.Left);
                        bubble.transform.localPosition = new Vector2((currentRowNumber * ROW_OFFSET) + DOUBLE_ROW_OFFSET, currentColumnNumber * COLUMN_OFFSET);
                    }
                    else
                    {
                        bubble.InitAsSlotBubble(this, currentRowNumber, currentColumnNumber, Enums.ColumnLeanSide.Right);
                        bubble.transform.localPosition = new Vector2(currentRowNumber * ROW_OFFSET, currentColumnNumber * COLUMN_OFFSET);
                    }
                    currentRowNumber++;
                }
                currentRowNumber = 0;
                currentColumnNumber++;
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
            bubble.transform.localPosition = new Vector2(THROWABLE_OFFSET, 0);
            BubbleManager.SetSecondThrowable(bubble);
        }
        #endregion
    }
}
