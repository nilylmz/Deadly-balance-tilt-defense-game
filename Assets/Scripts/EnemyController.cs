using UnityEngine;

namespace TiltDefense
{
    public enum EnemyType
    {
        Basic,
        Fast,
        Heavy,
        Zigzag,
        LaneSwitcher
    }

    public class EnemyController : MonoBehaviour
    {
        public int Lane { get; private set; }
        public EnemyType Type { get; private set; }

        private float health;
        private float baseSpeed;
        private float speedMultiplier = 1f;
        private float slowTimer;
        private float switchTimer;
        private float laneSpacing = 1.35f;
        private SpriteRenderer spriteRenderer;
        private float phase;

        public void Configure(EnemyType type, int lane, float waveSpeedBonus)
        {
            Type = type;
            Lane = lane;
            spriteRenderer = GetComponent<SpriteRenderer>();
            phase = Random.Range(0f, 100f);

            switch (type)
            {
                case EnemyType.Fast:
                    health = 18f;
                    baseSpeed = 1.25f + waveSpeedBonus;
                    spriteRenderer.color = new Color(0.95f, 0.35f, 0.35f, 0.85f);
                    break;
                case EnemyType.Heavy:
                    health = 62f;
                    baseSpeed = 0.55f + waveSpeedBonus * 0.7f;
                    spriteRenderer.color = new Color(0.55f, 0.35f, 0.85f, 0.9f);
                    break;
                case EnemyType.Zigzag:
                    health = 26f;
                    baseSpeed = 0.9f + waveSpeedBonus;
                    spriteRenderer.color = new Color(0.55f, 1f, 0.55f, 0.85f);
                    break;
                case EnemyType.LaneSwitcher:
                    health = 34f;
                    baseSpeed = 0.8f + waveSpeedBonus;
                    switchTimer = Random.Range(1.2f, 2.4f);
                    spriteRenderer.color = new Color(1f, 0.65f, 0.25f, 0.85f);
                    break;
                default:
                    health = 30f;
                    baseSpeed = 0.78f + waveSpeedBonus;
                    spriteRenderer.color = new Color(0.85f, 0.85f, 0.9f, 0.85f);
                    break;
            }
        }

        private void Update()
        {
            if (GameManager.Instance.State != GameState.Playing)
            {
                Destroy(gameObject);
                return;
            }

            if (slowTimer > 0f)
            {
                slowTimer -= Time.deltaTime;
                if (slowTimer <= 0f) speedMultiplier = 1f;
            }

            if (Type == EnemyType.LaneSwitcher)
            {
                switchTimer -= Time.deltaTime;
                if (switchTimer <= 0f)
                {
                    Lane = Mathf.Clamp(Lane + (Random.value > 0.5f ? 1 : -1), 0, PlayerController.LaneCount - 1);
                    switchTimer = Random.Range(1.1f, 2.2f);
                }
            }

            float targetX = PlayerController.LaneToX(Lane, laneSpacing);
            if (Type == EnemyType.Zigzag)
            {
                targetX += Mathf.Sin(Time.time * 5.5f + phase) * 0.34f;
            }

            Vector3 position = transform.position;
            position.y -= baseSpeed * speedMultiplier * Time.deltaTime;
            position.x = Mathf.Lerp(position.x, targetX, 1f - Mathf.Exp(-5f * Time.deltaTime));
            transform.position = position;

            float depth = Mathf.InverseLerp(4.7f, -3.25f, transform.position.y);
            float scale = Mathf.Lerp(0.32f, 1.15f, depth);
            transform.localScale = new Vector3(scale, scale, 1f);
            Color color = spriteRenderer.color;
            color.a = Mathf.Lerp(0.32f, 1f, depth);
            spriteRenderer.color = color;

            if (transform.position.y <= -3.35f)
            {
                GameManager.Instance.DamagePlayer(Type == EnemyType.Heavy ? 22f : 13f);
                VisualFactory.SpawnBurst(transform.position, Color.red, 0.8f);
                Destroy(gameObject);
            }
        }

        public void TakeDamage(float damage)
        {
            health -= damage;
            FloatingText.Spawn(transform.position + Vector3.up * 0.25f, Mathf.RoundToInt(damage).ToString(), Color.white);
            if (health <= 0f)
            {
                GameManager.Instance.AddScore(Type == EnemyType.Heavy ? 35 : 20);
                VisualFactory.SpawnBurst(transform.position, new Color(0.8f, 0.25f, 1f, 0.85f), 0.75f);
                Destroy(gameObject);
            }
        }

        public void ApplySlow(float duration, float multiplier)
        {
            slowTimer = duration;
            speedMultiplier = multiplier;
        }
    }
}
