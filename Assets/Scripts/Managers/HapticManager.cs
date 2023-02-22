using UnityEngine;
using Lofelt.NiceVibrations;

namespace BubblePops
{
    public class HapticManager : MonoBehaviour
    {
        private bool HapticIsSupportedOnThisDevide => DeviceCapabilities.isVersionSupported;

        public void Init(GameManager gameManager)
        {
            if (!HapticIsSupportedOnThisDevide)
            {
                gameManager.SettingsManager.DisableVibrations();
                Debug.LogWarning("This device does not support HAPTIC!");
                return;
            }

            HapticEvents.OnPlayMerge += PlayMediumHaptic;
            HapticEvents.OnPlayPop += PlayHeavyHaptic;
        }

        private void OnDisable() 
        {
            HapticEvents.OnPlayMerge -= PlayMediumHaptic;
            HapticEvents.OnPlayPop -= PlayHeavyHaptic;    
        }

        private void PlaySoftHaptic() => HapticPatterns.PlayPreset(HapticPatterns.PresetType.SoftImpact);
        private void PlayMediumHaptic() => HapticPatterns.PlayPreset(HapticPatterns.PresetType.MediumImpact);
        private void PlayHeavyHaptic() => HapticPatterns.PlayPreset(HapticPatterns.PresetType.HeavyImpact);
    }
}
