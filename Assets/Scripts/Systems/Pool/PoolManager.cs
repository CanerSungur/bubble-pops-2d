using System.Collections.Generic;
using UnityEngine;

namespace BubblePops
{
    public class PoolManager : MonoBehaviour
    {
        // Default is this object. You can serialize this to be another container object.
        private Transform _poolContainer;

        private PoolDataSO[] _poolData;
        private Dictionary<Enums.PoolStamp, Queue<GameObject>> _poolDictionary;

        // Quick singleton to access easily.
        #region Singleton

        public static PoolManager Instance;

        //private void Awake()
        //{
        //    Instance = this;
        //    _poolContainer = this.transform;

        //    _poolData = Resources.LoadAll<PoolDataSO>("_PoolData/");
        //}

        #endregion

        //private void Start()
        //{
        //    _poolDictionary = new Dictionary<Enums.PoolStamp, Queue<GameObject>>();

        //    foreach (PoolDataSO pool in _poolData)
        //    {
        //        Queue<GameObject> objectPool = new Queue<GameObject>();

        //        for (int i = 0; i < pool.Size; i++)
        //        {
        //            GameObject obj = Instantiate(pool.Prefabs[Random.Range(0, pool.Prefabs.Length)], _poolContainer);

        //            obj.SetActive(false);
        //            objectPool.Enqueue(obj);
        //        }

        //        _poolDictionary.Add(pool.PoolStamp, objectPool);
        //    }
        //}

        public void Init(GameManager gameManager)
        {
            Instance = this;
            _poolContainer = this.transform;

            _poolData = Resources.LoadAll<PoolDataSO>("_PoolData/");

            _poolDictionary = new Dictionary<Enums.PoolStamp, Queue<GameObject>>();

            foreach (PoolDataSO pool in _poolData)
            {
                Queue<GameObject> objectPool = new Queue<GameObject>();

                for (int i = 0; i < pool.Size; i++)
                {
                    GameObject obj = Instantiate(pool.Prefabs[Random.Range(0, pool.Prefabs.Length)], _poolContainer);

                    obj.SetActive(false);
                    objectPool.Enqueue(obj);
                }

                _poolDictionary.Add(pool.PoolStamp, objectPool);
            }
        }

#nullable enable
        public GameObject SpawnFromPool(Enums.PoolStamp poolStamp, Vector3 position, Quaternion rotation, Transform? parent = null)
#nullable disable
        {
            if (!_poolDictionary.ContainsKey(poolStamp))
            {
                Debug.LogError($"Pool with stamp: '{poolStamp}' doesn't exist.");
                return null;
            }

            // Pull out first element and store it
            GameObject objectToSpawn = _poolDictionary[poolStamp].Dequeue();

            objectToSpawn.SetActive(true);
            objectToSpawn.transform.SetPositionAndRotation(position, rotation);
            if (parent != null)
                objectToSpawn.transform.SetParent(parent);

            // Add it back to our queue to use it later.
            _poolDictionary[poolStamp].Enqueue(objectToSpawn);

            return objectToSpawn;
        }
    }
}
