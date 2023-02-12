using UnityEngine;
using TMPro;

namespace BubblePops
{
    public class Bubble : MonoBehaviour
    {
        #region COMPONENTS
        private BubbleSpawnManager _bubbleSpawnManager;
        private SpriteRenderer _spriteRenderer;
        private TextMeshProUGUI _numberText;
        #endregion

        #region FIELDS
        private int _number, _exponent;
        #endregion

        #region CONSTANTS
        private const int BASE_NUMBER = 2;
        #endregion

        public void Init(BubbleSpawnManager bubbleSpawnManager)
        {
            if (_bubbleSpawnManager == null)
            {
                _bubbleSpawnManager = bubbleSpawnManager;
                _spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
                _numberText = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
            }

            AssignRandomNumber();
            UpdateText();
            UpdateColor();
        }

        #region HELPERS
        private void AssignRandomNumber()
        {
            // Exponent is between 1 and 11. 1 is 2, 11 is 2048
            _exponent = Random.Range(1, 9);
            _number = (int)Mathf.Pow(BASE_NUMBER, _exponent);
        }
        private void UpdateColor() => _spriteRenderer.color = _bubbleSpawnManager.ExponentColors[_exponent - 1];
        private void UpdateText() => _numberText.text = _number.ToString();
        #endregion
    }
}
