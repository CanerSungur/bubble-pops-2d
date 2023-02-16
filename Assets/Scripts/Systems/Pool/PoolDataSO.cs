using UnityEngine;

namespace BubblePops
{
    [CreateAssetMenu(fileName = "Assets/Resources/_PoolData/NewPoolData", menuName = "Create Pool Data/New Pool Object")]
    public class PoolDataSO : ScriptableObject
    {
        public Enums.PoolStamp PoolStamp;
        public GameObject[] Prefabs;
        public int Size;
    }
}
