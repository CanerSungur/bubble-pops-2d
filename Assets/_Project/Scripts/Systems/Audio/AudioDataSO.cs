using UnityEngine;

namespace BubblePops
{
    [CreateAssetMenu(fileName = "Assets/Resources/_AudioData/NewAudioData", menuName = "Create Audio Data/New Audio")]
    public class AudioDataSO : ScriptableObject
    {
        [SerializeField] private Enums.AudioType type;
        [SerializeField] private AudioClip[] clips;

        public Enums.AudioType Type => type;
        public AudioClip[] Clips => clips;
    }
}
