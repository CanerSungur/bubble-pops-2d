using UnityEngine;
using TMPro;
using System;
using Random = UnityEngine.Random;
using DG.Tweening;

namespace BubblePops
{
    public class Bubble : MonoBehaviour
    {
        #region COMPONENTS
        private SpawnManager _spawnManager;
        private BubbleThrowHandler _throwHandler;
        private BubbleSurroundingHandler _surroundingHandler;
        private SpriteRenderer _spriteRenderer;
        private TextMeshProUGUI _numberText;
        #endregion

        #region FIELDS
        private Enums.BubbleStates _currentState;
        private int _number, _exponent, _rowNumber, _columnNumber;
        #endregion

        #region GETTERS
        public Color Color => _spriteRenderer.color;
        public BubbleThrowHandler ThrowHandler => _throwHandler;
        public Enums.BubbleStates CurrentState => _currentState;
        public int RowNumber => _rowNumber;
        public int ColumnNumber => _columnNumber;
        #endregion

        #region CONSTANTS
        private const int BASE_NUMBER = 2;
        private const int THROWABLE_BUBBLE_MAX_EXPONENT = 6;
        private const string BUBBLE_LAYER = "Bubble";
        private const string THROWABLE_BUBBLE_LAYER = "ThrowableBubble";
        #endregion

        #region EVENTS
        public Action OnThrowSuccessful, OnSetAsFirstThrowable, OnShakeTransform, OnCheckSurroundings;
        public Action<Enums.BubbleDirection> OnEmptyDirectionIsFilled;
        #endregion

        #region SEQUENCES
        private Sequence _shakeTransform;
        private Guid _shakeTransformID;
        #endregion

        #region INITIALIZERS
        public void InitAsSlotBubble(SpawnManager spawnManager, int rowNumber, int columnNumber)
        {
            if (_spawnManager == null)
            {
                _spawnManager = spawnManager;
                _throwHandler = GetComponent<BubbleThrowHandler>();
                _surroundingHandler = GetComponent<BubbleSurroundingHandler>();
                _spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
                _numberText = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
            }

            _rowNumber = rowNumber;
            _columnNumber = columnNumber;
            _currentState = Enums.BubbleStates.InSlot;

            _throwHandler.Init(this);
            _surroundingHandler.Init(this);

            AssignRandomNumber(false);
            UpdateText();
            UpdateColor();

            OnThrowSuccessful += ThrowSuccessful;
            OnSetAsFirstThrowable += SetAsFirstThrowable;
            OnShakeTransform += StartShakeTransform;
        }
        public void InitAsThrowableBubble(SpawnManager spawnManager, bool isItFirstThrowable)
        {
            if (_spawnManager == null)
            {
                _spawnManager = spawnManager;
                _throwHandler = GetComponent<BubbleThrowHandler>();
                _surroundingHandler = GetComponent<BubbleSurroundingHandler>();
                _spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
                _numberText = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
            }

            _currentState = isItFirstThrowable == true ? Enums.BubbleStates.ThrownFirst : Enums.BubbleStates.ThrownSecond;

            _throwHandler.Init(this);
            _surroundingHandler.Init(this);

            AssignRandomNumber(true);
            UpdateText();
            UpdateColor();

            OnThrowSuccessful += ThrowSuccessful;
            OnSetAsFirstThrowable += SetAsFirstThrowable;
            OnShakeTransform += StartShakeTransform;
        }
        #endregion

        private void OnDisable()
        {
            if (_spawnManager == null) return;
            OnThrowSuccessful -= ThrowSuccessful;
            OnSetAsFirstThrowable -= SetAsFirstThrowable;
            OnShakeTransform -= StartShakeTransform;
        }

        #region PUBLICS
        public void SetRowAndColumn(EmptySlot emptySlot)
        {
            _rowNumber = emptySlot.RowNumber;
            _columnNumber = emptySlot.ColumnNumber;
        }
        #endregion

        #region EVENT HANDLER FUNCTIONS
        private void ThrowSuccessful()
        {
            _currentState = Enums.BubbleStates.InSlot;
            gameObject.layer = LayerMask.NameToLayer(BUBBLE_LAYER);
            transform.SetParent(_spawnManager.BubbleContainerTransform);
            BubbleManager.SecondThrowableBubble.OnSetAsFirstThrowable?.Invoke();

            SpawnEvents.OnSpawnSecondThrowable?.Invoke();
            OnCheckSurroundings?.Invoke();
        }
        private void SetAsFirstThrowable()
        {
            _currentState = Enums.BubbleStates.ThrownFirst;
            transform.localPosition = Vector2.zero;
            transform.localScale = Vector2.one;
            BubbleManager.SetFirstThrowable(this);

            PlayerEvents.OnThrowSuccessful?.Invoke();
        }
        #endregion

        #region HELPERS
        private void AssignLayer(string layerName)
        {
            gameObject.layer = LayerMask.NameToLayer(layerName);
            transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer(layerName);
        }
            
        private void AssignRandomNumber(bool isItThrowable)
        {
            if (isItThrowable)
            {
                _exponent = Random.Range(1, THROWABLE_BUBBLE_MAX_EXPONENT + 1);
                AssignLayer(THROWABLE_BUBBLE_LAYER);
            }
            else
            {
                // Exponent is between 1 and 11. 1 is 2, 11 is 2048
                _exponent = Random.Range(1, 9);
                AssignLayer(BUBBLE_LAYER);
            }

            _number = (int)Mathf.Pow(BASE_NUMBER, _exponent);
        }
        private void UpdateColor() => _spriteRenderer.color = _spawnManager.ExponentColors[_exponent - 1];
        private void UpdateText() => _numberText.text = _number.ToString();
        #endregion

        #region DOTWEEN FUNCTIONS
        private void StartShakeTransform()
        {
            CreateShakeTransform();
            _shakeTransform.Play();
        }
        private void CreateShakeTransform()
        {
            if (_shakeTransform == null)
            {
                _shakeTransform = DOTween.Sequence();
                _shakeTransformID = Guid.NewGuid();
                _shakeTransform.id = _shakeTransformID;

                _shakeTransform.Append(transform.DOShakeScale(0.5f, 0.2f))
                    .OnComplete(DeleteShakeTransform);
            }
        }
        private void DeleteShakeTransform()
        {
            DOTween.Kill(_shakeTransformID);
            _shakeTransform = null;
        }
        #endregion
    }
}
