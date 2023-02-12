using UnityEngine;

namespace BubblePops
{
    public class TouchInput : MonoBehaviour
    {
        [Header("-- SETUP --")]
        [SerializeField] private LineRenderer _line;

        #region FIELDS
        private Camera _camera;
        private RaycastHit2D _firstRayHit, _secondRayHit;
        private Vector3 _touchPosition;
        #endregion

        #region PROPERTIES
        public bool ReadyToShoot { get; private set; }
        #endregion

        #region CONSTANTS
        private const string WALL_LAYER = "Wall";
        private const string BUBBLE_LAYER = "Bubble";
        private const float SECOND_RAY_START_OFFSET = 0.01f;
        private const float DISABLE_INPUT_HEIGHT_THRESHOLD_PERC = 0.2f;
        #endregion

        private void Awake()
        {
            _camera = Camera.main;
        }

        private void Start()
        {
            ReadyToShoot = false;
        }

        private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                _touchPosition = _camera.ScreenToWorldPoint(Input.mousePosition);

                CheckForCalcelInputThreshold();

                ShootFirstRay(_touchPosition);

                if (_firstRayHit)
                {
                    if (CheckHitLayer(_firstRayHit, WALL_LAYER))
                    {
                        ShootSecondRay(GetFirstRayReflectionDirection());

                        _line.positionCount = 3;
                        _line.SetPosition(2, _secondRayHit.point);

                        #region DEBUGGING
                        // Draw lines to show the incoming "beam" and the reflection.
                        //Debug.DrawLine(transform.position, _firstRayHit.point, Color.red);
                        //Debug.DrawRay(_firstRayHit.point, GetFirstRayReflectionDirection(), Color.green);
                        #endregion
                    }
                    else if (CheckHitLayer(_firstRayHit, BUBBLE_LAYER))
                        _line.positionCount = 2;

                    _line.SetPosition(0, transform.position);
                    _line.SetPosition(1, _firstRayHit.point);
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                _line.positionCount = 0;
                ReadyToShoot = false;
            }
        }

        #region HELPERS
        private void CheckForCalcelInputThreshold()
        {
            if (Input.mousePosition.y < Screen.height * DISABLE_INPUT_HEIGHT_THRESHOLD_PERC)
            {
                ReadyToShoot = _line.enabled = false;
                return;
            }
            else
                ReadyToShoot = _line.enabled = true;
        }
        private void ShootFirstRay(Vector3 touchPosition)
        {
            Vector3 rayDir = touchPosition - transform.position;
            _firstRayHit = Physics2D.Raycast(transform.position, rayDir, 100);
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
            _secondRayHit = Physics2D.Raycast(secondRayWithOffset.origin, secondRayWithOffset.direction);
        }
        private bool CheckHitLayer(RaycastHit2D hit, string layerName)
        {
            return hit.transform.gameObject.layer == LayerMask.NameToLayer(layerName);
        }
        #endregion
    }
}
