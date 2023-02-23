using System.Linq;
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
        private BubbleEffectHandler _effectHandler;
        private BubbleSequenceHandler _sequenceHandler;

        private Rigidbody2D _rigidbody;
        private SpriteRenderer _spriteRenderer;
        private GameObject _outline;
        private TextMeshProUGUI _numberText;
        private Transform _meshTransform;
        #endregion

        #region FIELDS
        private Enums.BubbleStates _currentState;
        private Enums.ColumnLeanSide _columnLeanSide;
        private int _number, _exponent, _rowNumber, _columnNumber, _mergeChainCount;
        private bool _isMerging, _isPopped, _isDropped, _isTooClose;
        private bool _shouldGoDown => _currentState == Enums.BubbleStates.InSlot && !_isDropped && _columnNumber == -SpawnManager.TopColumnNumber && transform.position.y > (MapMovementManager.TOP_COLUMN_Y_AXIS);
        #endregion

        #region GETTERS
        public Color Color => _spriteRenderer.color;
        public BubbleThrowHandler ThrowHandler => _throwHandler;
        public BubbleSurroundingHandler SurroundingHandler => _surroundingHandler;
        public BubbleMergeHandler MergeHandler => _mergeHandler;
        public BubbleEffectHandler EffectHandler => _effectHandler;

        public Enums.BubbleStates CurrentState => _currentState;
        public Enums.ColumnLeanSide ColumnLeanSide => _columnLeanSide;
        public Transform MeshTransform => _meshTransform;
        public int RowNumber => _rowNumber;
        public int ColumnNumber => _columnNumber;
        public int Exponent => _exponent;
        public int MergeChainCount => _mergeChainCount;
        public bool IsPopped => _isPopped;
        public bool IsDropped => _isDropped;
        public bool IsTooClose => _isTooClose;
        public bool IsMerging => _isMerging;
        #endregion

        #region CONSTANTS
        private const int BASE_NUMBER = 2;
        private const int THROWABLE_BUBBLE_MAX_EXPONENT = 7; // 6 will be default
        public const int MAX_EXPONENT = 11;

        private const string BUBBLE_LAYER = "Bubble";
        private const string THROWABLE_BUBBLE_LAYER = "ThrowableBubble";

        private const int OUTLINE_ENABLE_NUMBER = 1000;
        #endregion

        #region EVENTS
        public Action OnThrowSuccessful, OnSetAsFirstThrowable, OnMergeChainHappened;
        public Action<int> OnMergeHappened;
        public Action<Bubble> OnShakeTransform;
        #endregion

        #region INITIALIZERS
        public void InitAsSlotBubble(SpawnManager spawnManager, int rowNumber, int columnNumber, Enums.ColumnLeanSide columnLeanSide)
        {
            if (_spawnManager == null)
            {
                _spawnManager = spawnManager;
                _throwHandler = GetComponent<BubbleThrowHandler>();
                _surroundingHandler = GetComponent<BubbleSurroundingHandler>();
                _mergeHandler = GetComponent<BubbleMergeHandler>();
                _effectHandler = GetComponent<BubbleEffectHandler>();
                _sequenceHandler = GetComponent<BubbleSequenceHandler>();

                _rigidbody = GetComponent<Rigidbody2D>();
                _spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
                _outline = transform.GetChild(0).GetChild(1).gameObject;
                _numberText = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
                _meshTransform = transform.GetChild(0);
            }

            _rigidbody.isKinematic = true;

            BubbleManager.AddBubbleInSlot(this);

            _columnLeanSide = columnLeanSide;
            _rowNumber = rowNumber;
            _columnNumber = columnNumber;
            _currentState = Enums.BubbleStates.InSlot;
            _isMerging = _isPopped = _isDropped = _isTooClose = false;

            AssignRandomNumber(false);
            UpdateText();
            UpdateColor();

            _throwHandler.Init(this);
            _surroundingHandler.Init(this);
            _mergeHandler.Init(this);
            _effectHandler.Init(this);
            _sequenceHandler.Init(this);

            _sequenceHandler.StartEnableSequence();

            BubbleEvents.OnCheckSurroundings += CheckForDistanceToBottom;

            OnThrowSuccessful += ThrowSuccessful;
            OnSetAsFirstThrowable += SetAsFirstThrowable;
            OnShakeTransform += _sequenceHandler.StartShakeTransform;
            OnMergeHappened += MergeHappened;
            OnMergeChainHappened += () => {
                _mergeChainCount++;
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
                _effectHandler = GetComponent<BubbleEffectHandler>();
                _sequenceHandler = GetComponent<BubbleSequenceHandler>();

                _rigidbody = GetComponent<Rigidbody2D>();
                _spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
                _outline = transform.GetChild(0).GetChild(1).gameObject;
                _numberText = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
                _meshTransform = transform.GetChild(0);
            }

            _rigidbody.isKinematic = true;

            _currentState = isItFirstThrowable == true ? Enums.BubbleStates.ThrownFirst : Enums.BubbleStates.ThrownSecond;

            _isMerging = _isPopped = _isDropped = _isTooClose = false;

            AssignRandomNumber(true);
            UpdateText();
            UpdateColor();

            _throwHandler.Init(this);
            _surroundingHandler.Init(this);
            _mergeHandler.Init(this);
            _effectHandler.Init(this);
            _sequenceHandler.Init(this);

            _sequenceHandler.StartEnableSequence();

            BubbleEvents.OnCheckSurroundings += CheckForDistanceToBottom;

            OnThrowSuccessful += ThrowSuccessful;
            OnSetAsFirstThrowable += SetAsFirstThrowable;
            OnShakeTransform += _sequenceHandler.StartShakeTransform;
            OnMergeHappened += MergeHappened;
            OnMergeChainHappened += () => {
                _mergeChainCount++;
            };
        }
        #endregion

        private void OnDisable()
        {
            StopAllCoroutines();
            BubbleManager.RemoveBubbleInSlot(this);
            BubbleManager.RemoveBubblesToMerge(this);

            if (_spawnManager == null) return;

            BubbleEvents.OnCheckSurroundings -= CheckForDistanceToBottom;

            OnThrowSuccessful -= ThrowSuccessful;
            OnSetAsFirstThrowable -= SetAsFirstThrowable;
            OnShakeTransform -= _sequenceHandler.StartShakeTransform;
            OnMergeHappened -= MergeHappened;
            OnMergeChainHappened -= () => {
                _mergeChainCount++;
            };
        }

        #region PUBLICS
        public void StartedMerging() => _isMerging = true;
        public void StoppedMerging() => _isMerging = false;
        public void SetRowAndColumn(EmptySlot emptySlot)
        {
            _columnLeanSide = emptySlot.ColumnLeanSide;
            _rowNumber = emptySlot.RowNumber;
            _columnNumber = emptySlot.ColumnNumber;
        }
        public void SetRowAndColumn(Bubble bubble)
        {
            _columnLeanSide = bubble.ColumnLeanSide;
            _rowNumber = bubble.RowNumber;
            _columnNumber = bubble.ColumnNumber;
        }
        public void Pop(bool isItSourceBubble)
        {
            CameraEvents.OnShakeCamera?.Invoke();
            AudioEvents.OnPlayPop?.Invoke();
            if (SettingsManager.VibrationOn) HapticEvents.OnPlayPop?.Invoke();
            _effectHandler.SpawnBubbleMergePS(Color);

            if (isItSourceBubble)
                gameObject.SetActive(false);
            else
                _sequenceHandler.StartDisableSequence();

            if (BubbleManager.ScreenIsCleared())
                UiEvents.OnActivatePerfectText?.Invoke();
        }
        public void Drop()
        {
            if (_isDropped || _columnNumber == -SpawnManager.TopColumnNumber) return;

            _isDropped = true;
            gameObject.layer = LayerMask.NameToLayer("DroppedBubble");
            _rigidbody.isKinematic = false;
            _rigidbody.AddForce(new Vector2(Random.Range(-1.5f, 1.5f), Random.Range(1f, 2f)), ForceMode2D.Impulse);

            if (_surroundingHandler.DependantBubbles.Count > 0)
            {
                for (int i = 0; i < _surroundingHandler.DependantBubbles.Count; i++)
                    _surroundingHandler.DependantBubbles.ElementAt(i).Value.Drop();
            }
            else
                BubbleEvents.OnCheckSurroundings?.Invoke();

            BubbleManager.RemoveThisFromAllLists(this);
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

            BubbleEvents.OnCheckSurroundings?.Invoke();
            _surroundingHandler.TryMerging(this);
            MapEvents.OnCheckMapMovement?.Invoke();
        }
        private void SetAsFirstThrowable()
        {
            if (_currentState == Enums.BubbleStates.ThrownFirst) return;

            _mergeChainCount = 0;
            _currentState = Enums.BubbleStates.ThrownFirst;
            BubbleManager.SetFirstThrowable(this);
            SpawnEvents.OnSpawnSecondThrowable?.Invoke();
            _sequenceHandler.StartSetAsThrowableSequence();
        }
        private void MergeHappened(int mergeCount)
        {
            if (_mergeChainCount == 0) _mergeChainCount = 1;

            _exponent += mergeCount;
            _number = (int)Mathf.Pow(BASE_NUMBER, _exponent);
            _isPopped = _exponent >= MAX_EXPONENT;

            UpdateText();
            UpdateColor();
            if (_exponent < MAX_EXPONENT)
                _effectHandler.SpawnPopNumberInfo(_number);

            UiEvents.OnActivateMergeChainText?.Invoke(_mergeChainCount);
            AudioEvents.OnPlayMerge?.Invoke();
            if (SettingsManager.VibrationOn) HapticEvents.OnPlayMerge?.Invoke();
            PlayerEvents.OnIncreaseScore?.Invoke(_number);
            PlayerEvents.OnIncreaseExperience?.Invoke(mergeCount);
        }
        private void CheckForDistanceToBottom() 
        {
            _isTooClose = _currentState == Enums.BubbleStates.InSlot && transform.position.y - Player.PositionY <= MapMovementManager.MIN_DISTANCE_TO_BOTTOM;
            if (_shouldGoDown)
                MapEvents.OnMoveMapDownwards?.Invoke();
        }
        #endregion

        #region HELPERS
        private void AssignLayer(string layerName)
        {
            gameObject.layer = LayerMask.NameToLayer(layerName);
            _meshTransform.gameObject.layer = LayerMask.NameToLayer(layerName);
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
        private void UpdateColor() 
        {
            _spriteRenderer.color = _exponent > GameManager.ExponentColors.Length
                ? GameManager.ExponentColors[GameManager.ExponentColors.Length - 1]
                : GameManager.ExponentColors[_exponent - 1];

            _outline.SetActive(_number > OUTLINE_ENABLE_NUMBER);
        }

        private void UpdateText() => _numberText.text = _number > 1000 ? (_number / 1000) + "K" : _number.ToString();
        #endregion

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                _effectHandler.SpawnBubbleMergePS(Color);
                AudioEvents.OnPlayMerge?.Invoke();
                if (SettingsManager.VibrationOn) HapticEvents.OnPlayMerge?.Invoke();
                MapEvents.OnCheckMapMovement?.Invoke();

                gameObject.SetActive(false);
            }
        }
    }
}
