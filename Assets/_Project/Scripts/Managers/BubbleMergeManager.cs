using System.Collections.Generic;
using UnityEngine;

namespace BubblePops
{
    public class BubbleMergeManager : MonoBehaviour
    {
        public void Init(GameManager gameManager)
        {
            BubbleEvents.OnStartMerge += StartMerge;
            BubbleEvents.OnCheckSurroundings += TriggerAllBubblesCheckSurroundings;
        }

        private void OnDisable()
        {
            BubbleEvents.OnStartMerge -= StartMerge;
            BubbleEvents.OnCheckSurroundings -= TriggerAllBubblesCheckSurroundings;
        }

        private void StartMerge(Bubble mainBubble)
        {
            mainBubble.StartedMerging();

            if (BubbleManager.BubbleForNextMerge == null)
                mainBubble.MergeHandler.StartMergeSequence(GetTopMostBubble(BubbleManager.BubblesToMerge, mainBubble));
            else
                mainBubble.MergeHandler.StartMergeSequence(BubbleManager.BubbleForNextMerge);

            foreach (Bubble bubble in BubbleManager.BubblesToMerge)
            {
                bubble.StartedMerging();
                bubble.transform.SetParent(mainBubble.transform);
                bubble.MergeHandler.StartMoveForOtherBubblesSequence();
            }

            TriggerAllBubblesCheckSurroundings();
        }

        #region PUBLICS
        public void TriggerAllBubblesCheckSurroundings()
        {
            DeleteEmptySlots();
            for (int i = 0; i < BubbleManager.BubblesInSlot.Count; i++)
                if (BubbleManager.BubblesInSlot[i].CurrentState == Enums.BubbleStates.InSlot) BubbleManager.BubblesInSlot[i].SurroundingHandler.CheckSurroundings();
        }
        #endregion

        #region HELPERS
        private Bubble GetTopMostBubble(List<Bubble> bubbles, Bubble mainBubble)
        {
            Bubble topBubble = mainBubble;
            foreach (Bubble bubble in bubbles)
                topBubble = bubble.transform.position.y > topBubble.transform.position.y ? bubble : topBubble;

            return topBubble;
        }
        private void DeleteEmptySlots()
        {
            int count = BubbleManager.EmptySlots.Count;
            for (int i = 0; i < count; i++)
                BubbleManager.EmptySlots[0].gameObject.SetActive(false);
            #endregion
        }
    }
}
