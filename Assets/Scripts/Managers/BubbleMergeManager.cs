using UnityEngine;

namespace BubblePops
{
    public class BubbleMergeManager : MonoBehaviour
    {
        public void Init(GameManager gameManager)
        {
            
        }

        private void OnDisable()
        {
            
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                TriggerAllBubblesCheckSurroundings();
            }
        }

        #region PUBLICS
        public void TriggerAllBubblesCheckSurroundings()
        {
            for (int i = 0; i < BubbleManager.BubblesInSlot.Count; i++)
                BubbleManager.BubblesInSlot[i].OnCheckSurroundings?.Invoke();
        }
        #endregion
    }
}
