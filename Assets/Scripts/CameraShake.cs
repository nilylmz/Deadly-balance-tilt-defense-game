using System.Collections;
using UnityEngine;

namespace TiltDefense
{
    public class CameraShake : MonoBehaviour
    {
        public static CameraShake Instance { get; private set; }

        private Vector3 basePosition;
        private Coroutine routine;

        private void Awake()
        {
            Instance = this;
            basePosition = transform.position;
        }

        public static void Shake(float duration, float magnitude)
        {
            if (Instance == null) return;
            if (Instance.routine != null) Instance.StopCoroutine(Instance.routine);
            Instance.routine = Instance.StartCoroutine(Instance.ShakeRoutine(duration, magnitude));
        }

        private IEnumerator ShakeRoutine(float duration, float magnitude)
        {
            float time = 0f;
            while (time < duration)
            {
                time += Time.deltaTime;
                transform.position = basePosition + (Vector3)Random.insideUnitCircle * magnitude;
                yield return null;
            }

            transform.position = basePosition;
        }
    }
}
