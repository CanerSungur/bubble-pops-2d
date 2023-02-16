using BubblePops.Utility;
using UnityEngine;

namespace BubblePops
{
    public class GameManager : MonoBehaviour
    {
        #region COMPONENTS
        private SpawnManager _spawnManager;
        private BubbleMergeManager _mergeManager;
        #endregion

        private void Awake()
        {
            _spawnManager = GetComponent<SpawnManager>();
            _mergeManager = GetComponent<BubbleMergeManager>();

            _spawnManager.Init(this);
            _mergeManager.Init(this);

            Utils.DoActionAfterDelay(this, 0.5f, () => _mergeManager.TriggerAllBubblesCheckSurroundings());
        }
    }
}
