using System;
using System.Collections;
using UnityEngine;

namespace BubblePops.Utility
{
    public static class Utils
    {
        /// <summary>
        /// Call this function if you want to delay a function.
        /// Any function can be but into this with lambda expression like '() =>'
        /// USAGE: this.DoActionAfterDelay(...);
        /// </summary>
        /// <param name="mono">This is required because Coroutine requires MonoBehaviour.</param>
        /// <param name="delayTime">Function will be executed after this time.</param>
        /// <param name="action">Function you want to delay.</param>
        public static void DoActionAfterDelay(this MonoBehaviour mono, float delayTime, Action action)
        {
            if (mono != null || mono.enabled)
                mono.StartCoroutine(ExecuteAction(delayTime, action));
        }

        private static IEnumerator ExecuteAction(float delayTime, Action action)
        {
            yield return new WaitForSecondsRealtime(delayTime);
            action.Invoke();
            yield break;
        }

        /// <summary>
        /// This function converts given int value to K and M type.
        /// i.e. 10.000 to 10K, 1.500.000 to 1.5M
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string IntToStringShortener(int value)
        {
            float val = (float)value;
            if (val < 1000)
                return val.ToString();
            else if (val >= 1000 && val < 1000000)
                return (val / 1000).ToString("#.##") + "K";
            else if (val >= 1000000 && val < 1000000000)
                return (val / 1000000).ToString("#.##") + "M";
            else
                return (val / 1000000000).ToString("#.##") + "B";
        }
    }
}
