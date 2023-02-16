using BubblePops.Utility;
using UnityEngine;

namespace BubblePops
{
    public class GameManager : MonoBehaviour
    {
        #region COMPONENTS
        private PoolManager _poolManager;
        private SpawnManager _spawnManager;
        private BubbleMergeManager _mergeManager;
        #endregion

        private void Awake()
        {
            _poolManager = GetComponent<PoolManager>();
            _spawnManager = GetComponent<SpawnManager>();
            _mergeManager = GetComponent<BubbleMergeManager>();

            _poolManager.Init(this);
            _spawnManager.Init(this);
            _mergeManager.Init(this);

            Utils.DoActionAfterDelay(this, 0.5f, () => _mergeManager.TriggerAllBubblesCheckSurroundings());
        }
    }
}
