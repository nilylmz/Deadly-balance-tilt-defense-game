using UnityEngine;

namespace TiltDefense
{
    public class ProjectileController : MonoBehaviour
    {
        private int lane;
        private float damage;
        private float speed;
        private float lifetime;
        private bool singleHit;

        public void Configure(int targetLane, float projectileDamage, float projectileSpeed, float secondsToLive, bool hitOnce)
        {
            lane = targetLane;
            damage = projectileDamage;
            speed = projectileSpeed;
            lifetime = secondsToLive;
            singleHit = hitOnce;
        }

        private void Update()
        {
            transform.position += Vector3.up * speed * Time.deltaTime;
            lifetime -= Time.deltaTime;
            if (lifetime <= 0f || transform.position.y > 5.4f)
            {
                Destroy(gameObject);
                return;
            }

            EnemyController[] enemies = FindObjectsOfType<EnemyController>();
            for (int i = 0; i < enemies.Length; i++)
            {
                EnemyController enemy = enemies[i];
                if (enemy.Lane != lane) continue;

                float hitDistance = Mathf.Abs(enemy.transform.position.y - transform.position.y);
                if (hitDistance < 0.42f)
                {
                    enemy.TakeDamage(damage);
                    VisualFactory.SpawnBurst(enemy.transform.position, Color.cyan, 0.45f);
                    if (singleHit)
                    {
                        Destroy(gameObject);
                        return;
                    }
                }
            }
        }
    }
}
