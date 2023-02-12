using UnityEngine;

namespace BubblePops
{
    public class BubbleSpawnManager : MonoBehaviour
    {
        [Header("-- SETUP --")]
        [SerializeField] private Transform _bubbleContainerTransform;
        [SerializeField] private Bubble _bubblePrefab;

        [Header("-- BUBBLE COLOR SETUP --")]
        [SerializeField] private Color[] _exponentColors;

        #region FIELDS
        private int _currentRowNumber = 0;
        private int _currentColumnNumber = 0;
        #endregion

        #region PROPERTIES
        public Color[] ExponentColors => _exponentColors;
        #endregion

        #region CONSTANTS
        private const int ROW_NUMBER = 6;
        private const int COLUMN_NUMBER = 4;
        private const float BUBBLE_ROW_OFFSET = 0.75f;
        private const float BUBBLE_COLUMN_OFFSET = -0.65f;
        private const float BUBBLE_DOUBLE_ROW_OFFSET = -0.35f;
        #endregion

        private void Start()
        {
            SpawnBubbles();
        }

        private void SpawnBubbles()
        {
            for (int i = 0; i < COLUMN_NUMBER; i++)
            {
                for (int j = 0; j < ROW_NUMBER; j++)
                {
                    Bubble bubble = Instantiate(_bubblePrefab, _bubbleContainerTransform);
                    bubble.Init(this);
                    if (i == 1 || i %2 != 0)
                        bubble.transform.localPosition = new Vector2((_currentRowNumber * BUBBLE_ROW_OFFSET) + BUBBLE_DOUBLE_ROW_OFFSET, _currentColumnNumber * BUBBLE_COLUMN_OFFSET);
                    else
                        bubble.transform.localPosition = new Vector2(_currentRowNumber * BUBBLE_ROW_OFFSET, _currentColumnNumber * BUBBLE_COLUMN_OFFSET);
                    _currentRowNumber++;
                }
                _currentRowNumber = 0;
                _currentColumnNumber++;
            }
        }
    }
}
