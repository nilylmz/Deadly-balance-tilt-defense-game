using System.Collections;
using UnityEngine;

namespace TiltDefense
{
    public class EnemySpawner : MonoBehaviour
    {
        private Coroutine waveRoutine;
        private int currentWave = 1;

        public void BeginWaves()
        {
            StopWaves();
            waveRoutine = StartCoroutine(WaveRoutine());
        }

        public void StopWaves()
        {
            if (waveRoutine != null) StopCoroutine(waveRoutine);
            waveRoutine = null;

            EnemyController[] enemies = FindObjectsOfType<EnemyController>();
            for (int i = 0; i < enemies.Length; i++)
            {
                Destroy(enemies[i].gameObject);
            }
        }

        public void ResetSpawner()
        {
            currentWave = 1;
        }

        private IEnumerator WaveRoutine()
        {
            while (GameManager.Instance.State == GameState.Playing)
            {
                GameManager.Instance.AdvanceWave(currentWave);
                yield return new WaitForSeconds(1.4f);

                int enemyCount = 5 + currentWave * 2;
                float spawnDelay = Mathf.Max(0.35f, 1.1f - currentWave * 0.05f);

                for (int i = 0; i < enemyCount; i++)
                {
                    SpawnEnemy(currentWave);
                    yield return new WaitForSeconds(spawnDelay);
                }

                yield return new WaitForSeconds(2.2f);
                currentWave++;
            }
        }

        private void SpawnEnemy(int wave)
        {
            int lane = Random.Range(0, PlayerController.LaneCount);
            EnemyType type = PickEnemyType(wave);

            GameObject enemyObject = VisualFactory.CreateEnemy(type);
            enemyObject.transform.position = new Vector3(PlayerController.LaneToX(lane), 4.55f, 0f);
            EnemyController enemy = enemyObject.AddComponent<EnemyController>();
            enemy.Configure(type, lane, Mathf.Min(0.65f, wave * 0.045f));
        }

        private EnemyType PickEnemyType(int wave)
        {
            if (wave < 2) return EnemyType.Basic;

            int maxType = Mathf.Clamp(wave, 1, 5);
            int roll = Random.Range(0, maxType);
            return (EnemyType)roll;
        }
    }
}
