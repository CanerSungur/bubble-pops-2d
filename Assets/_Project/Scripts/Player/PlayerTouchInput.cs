using UnityEngine;

namespace BubblePops
{
    public class PlayerTouchInput : MonoBehaviour
    {
        [Header("-- SETUP --")]
        [SerializeField] private LineRenderer _line;
        [SerializeField] private LayerMask _hittableLayers;
        [SerializeField] private LayerMask _emptySlotLayer;

        #region COMPONENTS
        private Player _player;
        #endregion

        #region FIELDS
        private Camera _camera;
        private RaycastHit2D _firstRayHit, _secondRayHit, _emptySlotRayHit;
        private Vector3 _touchPosition;
        private float _delayedTime;
        private bool _mouseIsInPosition;
        #endregion

        #region PROPERTIES
        public bool ReadyToShoot { get; private set; }
        #endregion

        #region GETTERS
        public RaycastHit2D FirstRayHit => _firstRayHit;
        public RaycastHit2D SecondRayHit => _secondRayHit;
        public bool CantTakeInput => LevelUpCanvas.IsOpen || SettingsCanvas.IsOpen || GameManager.CurrentState != Enums.GameState.Ready || Time.time < _delayedTime; 
        #endregion

        #region CONSTANTS
        private const string WALL_LAYER = "Wall";
        private const string BUBBLE_LAYER = "Bubble";

        private const float SECOND_RAY_START_OFFSET = 0.01f;
        private const float DISABLE_INPUT_MIN_HEIGHT_THRESHOLD_PERC = 0.18f;
        private const float DISABLE_INPUT_MAX_HEIGHT_THRESHOLD_PERC = 0.9f;

        private const float TAKE_INPUT_DELAY = 0.3f;
        #endregion

        public void Init(Player player)
        {
            _player = player;
            _delayedTime = 0;
            _camera = Camera.main;
            ReadyToShoot = false;
            
            GameEvents.OnGameStateChange += ((Enums.GameState gameState) => {
                if (gameState == Enums.GameState.Ready) _delayedTime = Time.time + TAKE_INPUT_DELAY;
            });
        }

        private void OnDisable()
        {
            GameEvents.OnGameStateChange -= ((Enums.GameState gameState) => {
                if (gameState == Enums.GameState.Ready) _delayedTime = Time.time + TAKE_INPUT_DELAY;
            });
        }

        private void Update()
        {
            if (CantTakeInput) return;

            CheckForCalcelInputThreshold();

            _line.enabled = ReadyToShoot;

            if (Input.GetMouseButton(0) && _mouseIsInPosition)
            {
                _touchPosition = _camera.ScreenToWorldPoint(Input.mousePosition);

                ShootFirstRay(_touchPosition);

                if (_firstRayHit)
                {
                    if (_firstRayHit && CheckHitLayer(_firstRayHit, WALL_LAYER))
                    {
                        ShootSecondRay(GetFirstRayReflectionDirection());

                        _line.positionCount = 3;
                        _line.SetPosition(2, _secondRayHit.point);

                        if (_secondRayHit && CheckHitLayer(_secondRayHit, BUBBLE_LAYER))
                        {
                            ShootEmptySlotRay(true);

                            if (_emptySlotRayHit && _emptySlotRayHit.transform.TryGetComponent(out EmptySlot emptySlot))
                            {
                                ReadyToShoot = true;
                                _player.ThrowBehaviour.UpdateEmptySlot(emptySlot);
                            }
                        }
                        else if (_secondRayHit && CheckHitLayer(_secondRayHit, WALL_LAYER))
                        {
                            if (_player.ThrowBehaviour.CurrentEmptySlot != null)
                                _player.ThrowBehaviour.DisableEmptySlot();

                            ReadyToShoot = false;
                            _line.positionCount = 0;
                        }
                            
                        #region DEBUGGING
                        // Draw lines to show the incoming "beam" and the reflection.
                        //Debug.DrawLine(transform.position, _firstRayHit.point, Color.red);
                        //Debug.DrawRay(_firstRayHit.point, GetFirstRayReflectionDirection(), Color.green);
                        #endregion
                    }
                    else if (_firstRayHit && CheckHitLayer(_firstRayHit, BUBBLE_LAYER))
                    {
                        ShootEmptySlotRay(false);

                        //ReadyToShoot = true;
                        _secondRayHit = new RaycastHit2D(); // empty the second ray hit
                        _line.positionCount = 2;

                        if (_emptySlotRayHit && _emptySlotRayHit.transform.TryGetComponent(out EmptySlot emptySlot))
                        {
                            ReadyToShoot = true;
                            _player.ThrowBehaviour.UpdateEmptySlot(emptySlot);
                        }
                    }

                    if (_line.positionCount != 0)
                    {
                        _line.SetPosition(0, transform.position);
                        _line.SetPosition(1, _firstRayHit.point);
                    }
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                if (ReadyToShoot && _mouseIsInPosition)
                {
                    if (_firstRayHit && CheckHitLayer(_firstRayHit, WALL_LAYER))
                        _player.ThrowBehaviour.TriggerThrow(_firstRayHit.point);
                    else
                        _player.ThrowBehaviour.TriggerThrow();

                    AudioEvents.OnPlayMove?.Invoke();
                }

                ResetRaycastHits();
                _line.positionCount = 0;
                ReadyToShoot = false;
                _player.ThrowBehaviour.DisableEmptySlot();
            }
        }

        #region HELPERS
        private void ResetRaycastHits() => _firstRayHit = _secondRayHit = new RaycastHit2D();
        private void CheckForCalcelInputThreshold()
        {
            if (Input.mousePosition.y > Screen.height * DISABLE_INPUT_MAX_HEIGHT_THRESHOLD_PERC || Input.mousePosition.y < Screen.height * DISABLE_INPUT_MIN_HEIGHT_THRESHOLD_PERC)
            {
                if (_mouseIsInPosition) 
                {
                    _player.ThrowBehaviour.DisableEmptySlot();
                    ResetRaycastHits();
                }
                
                _mouseIsInPosition = ReadyToShoot = false;
            }
            else
                _mouseIsInPosition = true;
        }
        private void ShootFirstRay(Vector3 touchPosition)
        {
            Vector3 rayDir = touchPosition - transform.position;
            _firstRayHit = Physics2D.Raycast(transform.position, rayDir, 100, _hittableLayers);
        }
        private Vector2 GetFirstRayReflectionDirection()
        {
            Vector2 firstRayDir = _firstRayHit.point - (Vector2)transform.position;
            // Use the point's normal to calculate the reflection vector.
            return Vector2.Reflect(firstRayDir, _firstRayHit.normal);
        }
        private void ShootSecondRay(Vector2 reflectionDirection)
        {
            Ray2D secondRayWithOffset = new Ray2D(_firstRayHit.point + (SECOND_RAY_START_OFFSET * reflectionDirection), reflectionDirection);
            _secondRayHit = Physics2D.Raycast(secondRayWithOffset.origin, secondRayWithOffset.direction, 100, _hittableLayers);
        }
        private void ShootEmptySlotRay(bool didItBounce)
        {
            Vector3 direction;
            if (didItBounce)
            {
                direction = _firstRayHit.point - _secondRayHit.point;
                _emptySlotRayHit = Physics2D.Raycast(_secondRayHit.point, direction, 100, _emptySlotLayer);
                // Debug.DrawRay(_secondRayHit.point, direction, Color.blue);
            }
            else
            {
                direction = (Vector2)transform.position - _firstRayHit.point;
                _emptySlotRayHit = Physics2D.Raycast(_firstRayHit.point, direction, 100, _emptySlotLayer);
                // Debug.DrawRay(_firstRayHit.point, direction, Color.red);
            }
        }
        private bool CheckHitLayer(RaycastHit2D hit, string layerName)
        {
            return hit.transform.gameObject.layer == LayerMask.NameToLayer(layerName);
        }
        #endregion
    }
}
