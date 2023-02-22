using UnityEngine;

namespace BubblePops
{
    public class PlayerAudioHandler : MonoBehaviour
    {
        public void Init(Player player)
        {
            AudioEvents.OnPlayMove += PlayMove;
            AudioEvents.OnPlayPop += PlayPop;
            AudioEvents.OnPlayMerge += PlayMerge;
        }

        private void OnDisable() 
        {
            AudioEvents.OnPlayMove -= PlayMove;
            AudioEvents.OnPlayPop -= PlayPop;
            AudioEvents.OnPlayMerge -= PlayMerge;
        }

        #region EVENT HANDLER FUNCTIONS
        private void PlayMove()
        {
            AudioManager.PlayAudio(Enums.AudioType.Move, 0.5f, Random.Range(0.9f, 1.1f));
        }
        private void PlayPop()
        {
            AudioManager.PlayAudio(Enums.AudioType.Pop);
        }
        private void PlayMerge()
        {
            AudioManager.PlayAudio(Enums.AudioType.Merge, 1f, Random.Range(0.8f, 1.2f));
        }
        #endregion
    }
}
