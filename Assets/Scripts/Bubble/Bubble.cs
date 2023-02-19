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
        private BubbleMergeHandler _mergeHandler;

        private SpriteRenderer _spriteRenderer;
        private TextMeshProUGUI _numberText;
        #endregion

        #region FIELDS
        private Enums.BubbleStates _currentState;
        private int _number, _exponent, _rowNumber, _columnNumber, _mergeChainCount;
        private bool _isPopped;
        #endregion

        #region GETTERS
        public Color Color => _spriteRenderer.color;
        public BubbleThrowHandler ThrowHandler => _throwHandler;
        public BubbleSurroundingHandler SurroundingHandler => _surroundingHandler;
        public BubbleMergeHandler MergeHandler => _mergeHandler;
        public Enums.BubbleStates CurrentState => _currentState;

        public int RowNumber => _rowNumber;
        public int ColumnNumber => _columnNumber;
        public int Exponent => _exponent;
        public int MergeChainCount => _mergeChainCount;
        public bool IsPopped => _isPopped;
        #endregion

        #region CONSTANTS
        private const int BASE_NUMBER = 2;
        private const int THROWABLE_BUBBLE_MAX_EXPONENT = 10; // 6 will be default
        public const int MAX_EXPONENT = 11;

        private const string BUBBLE_LAYER = "Bubble";
        private const string THROWABLE_BUBBLE_LAYER = "ThrowableBubble";

        private const float SET_AS_THROWABLE_SEQUENCE_DURATION = 0.5f;
        private const float SHAKE_TRANSFORM_DURATION = 0.5f;
        #endregion

        #region EVENTS
        public Action OnThrowSuccessful, OnSetAsFirstThrowable, OnMergeChainHappened, OnPopNeighbours;
        public Action OnShakeTransform, OnShakeSurroundingBubbles;
        public Action OnCheckEmptySlotSpawn;
        public Action<int> OnMergeHappened;
        #endregion

        #region SEQUENCES
        private Sequence _shakeTransform, _setAsThrowableSequence, _enableSequence;
        private Guid _shakeTransformID, _setAsThrowableSequenceID, _enableSequenceID;
        #endregion

        #region INITIALIZERS
        public void InitAsSlotBubble(SpawnManager spawnManager, int rowNumber, int columnNumber)
        {
            if (_spawnManager == null)
            {
                _spawnManager = spawnManager;
                _throwHandler = GetComponent<BubbleThrowHandler>();
                _surroundingHandler = GetComponent<BubbleSurroundingHandler>();
                _mergeHandler = GetComponent<BubbleMergeHandler>();
                _spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
                _numberText = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
            }

            StartEnableSequence();
            BubbleManager.AddBubbleInSlot(this);

            _rowNumber = rowNumber;
            _columnNumber = columnNumber;
            _currentState = Enums.BubbleStates.InSlot;
            _isPopped = false;

            AssignRandomNumber(false);
            UpdateText();
            UpdateColor();

            _throwHandler.Init(this);
            _surroundingHandler.Init(this);
            _mergeHandler.Init(this);

            OnThrowSuccessful += ThrowSuccessful;
            OnSetAsFirstThrowable += SetAsFirstThrowable;
            OnShakeTransform += StartShakeTransform;
            OnMergeHappened += MergeHappened;
            OnMergeChainHappened += () => {
                _mergeChainCount++;
                Debug.Log("Chain: " + _mergeChainCount +"x");
            };
        }
        public void InitAsThrowableBubble(SpawnManager spawnManager, bool isItFirstThrowable)
        {
            if (_spawnManager == null)
            {
                _spawnManager = spawnManager;
                _throwHandler = GetComponent<BubbleThrowHandler>();
                _surroundingHandler = GetComponent<BubbleSurroundingHandler>();
                _mergeHandler = GetComponent<BubbleMergeHandler>();
                _spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
                _numberText = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
            }

            _currentState = isItFirstThrowable == true ? Enums.BubbleStates.ThrownFirst : Enums.BubbleStates.ThrownSecond;
            StartEnableSequence();

            _isPopped = false;

            AssignRandomNumber(true);
            UpdateText();
            UpdateColor();

            _throwHandler.Init(this);
            _surroundingHandler.Init(this);
            _mergeHandler.Init(this);

            OnThrowSuccessful += ThrowSuccessful;
            OnSetAsFirstThrowable += SetAsFirstThrowable;
            OnShakeTransform += StartShakeTransform;
            OnMergeHappened += MergeHappened;
            OnMergeChainHappened += () => {
                _mergeChainCount++;
                Debug.Log("Chain: " + _mergeChainCount + "x");
            };
        }
        #endregion

        private void OnDisable()
        {
            BubbleManager.RemoveBubbleInSlot(this);
            BubbleManager.RemoveBubblesToMerge(this);

            if (_spawnManager == null) return;

            OnThrowSuccessful -= ThrowSuccessful;
            OnSetAsFirstThrowable -= SetAsFirstThrowable;
            OnShakeTransform -= StartShakeTransform;
            OnMergeHappened -= MergeHappened;
            OnMergeChainHappened -= () => {
                _mergeChainCount++;
                Debug.Log("Chain: " + _mergeChainCount + "x");
            };
        }

        #region PUBLICS
        public void SetRowAndColumn(EmptySlot emptySlot)
        {
            _rowNumber = emptySlot.RowNumber;
            _columnNumber = emptySlot.ColumnNumber;
        }
        public void SetRowAndColumn(Bubble bubble)
        {
            _rowNumber = bubble.RowNumber;
            _columnNumber = bubble.ColumnNumber;
        }
        public void Pop()
        {
            Debug.Log("POP!", this);
            //StopAllCoroutines();
            gameObject.SetActive(false);
        }
        #endregion

        #region EVENT HANDLER FUNCTIONS
        private void ThrowSuccessful()
        {
            BubbleManager.AddBubbleInSlot(this);
            BubbleManager.SetBubbleForNextMerge(null);

            _currentState = Enums.BubbleStates.InSlot;
            gameObject.layer = LayerMask.NameToLayer(BUBBLE_LAYER);
            transform.SetParent(_spawnManager.BubbleContainerTransform);

            _surroundingHandler.CheckSurroundings(this);
        }
        private void SetAsFirstThrowable()
        {
            _mergeChainCount = 0;
            _currentState = Enums.BubbleStates.ThrownFirst;
            StartSetAsThrowableSequence();
            SpawnEvents.OnSpawnSecondThrowable?.Invoke();
        }
        private void MergeHappened(int mergeCount)
        {
            if (_mergeChainCount == 0) _mergeChainCount = 1;

            _exponent += mergeCount;
            _number = (int)Mathf.Pow(BASE_NUMBER, _exponent);
            _isPopped = _exponent >= MAX_EXPONENT;

            //if (_exponent >= MAX_EXPONENT)
            //{
            //    _surroundingHandler.UpdateSurroundings();
            //    for (int i = 0; i < _surroundingHandler.SurroundingBubbles.Count; i++)
            //        _surroundingHandler.SurroundingBubbles[i].Pop();

            //    Pop();
            //    BubbleEvents.OnCheckSurroundings?.Invoke();
            //    return;
            //}

            UpdateText();
            UpdateColor();
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
        private void UpdateColor() => _spriteRenderer.color = _exponent > _spawnManager.ExponentColors.Length
            ? _spawnManager.ExponentColors[_spawnManager.ExponentColors.Length - 1]
            : _spawnManager.ExponentColors[_exponent - 1];
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

                _shakeTransform.Append(transform.GetChild(0).DOShakeScale(SHAKE_TRANSFORM_DURATION, 0.1f))
                    .OnComplete(DeleteShakeTransform);
            }
        }
        private void DeleteShakeTransform()
        {
            DOTween.Kill(_shakeTransformID);
            _shakeTransform = null;
        }
        // #########################
        private void StartSetAsThrowableSequence()
        {
            CreateSetAsThrowableSequence();
            _setAsThrowableSequence.Play();
        }
        private void CreateSetAsThrowableSequence()
        {
            if (_setAsThrowableSequence == null)
            {
                _setAsThrowableSequence = DOTween.Sequence();
                _setAsThrowableSequenceID = Guid.NewGuid();
                _setAsThrowableSequence.id = _setAsThrowableSequenceID;

                _setAsThrowableSequence.Append(transform.DOLocalMove(Vector2.zero, SET_AS_THROWABLE_SEQUENCE_DURATION * 0.5f))
                    .Join(transform.DOScale(Vector2.one, SET_AS_THROWABLE_SEQUENCE_DURATION * 0.5f).SetEase(Ease.OutElastic))
                    .Append(transform.DOShakePosition(SET_AS_THROWABLE_SEQUENCE_DURATION, new Vector2(0.07f, 0f), 10, 0))
                    .OnComplete(() => {
                        BubbleManager.SetFirstThrowable(this);
                        GameFlowEvents.OnGameStateChange?.Invoke(Enums.GameState.Ready);
                        DeleteSetAsThrowableSequence();
                    });
            }
        }
        private void DeleteSetAsThrowableSequence()
        {
            DOTween.Kill(_setAsThrowableSequenceID);
            _setAsThrowableSequence = null;
        }
        // #########################
        private void StartEnableSequence()
        {
            CreateEnableSequence();
            _enableSequence.Play();
        }
        private void CreateEnableSequence()
        {
            if (_enableSequence == null)
            {
                _enableSequence = DOTween.Sequence();
                _enableSequenceID = Guid.NewGuid();
                _enableSequence.id = _enableSequenceID;

                transform.localScale = Vector2.zero;
                Vector2 targetScale = _currentState == Enums.BubbleStates.ThrownSecond ? Vector2.one * 0.75f : Vector2.one;

                _enableSequence.Append(transform.DOScale(targetScale, 0.5f).SetEase(Ease.OutBounce))
                    .OnComplete(DeleteEnableSequence);
            }
        }
        private void DeleteEnableSequence()
        {
            DOTween.Kill(_enableSequenceID);
            _enableSequence = null;
        }
        #endregion
    }
}
