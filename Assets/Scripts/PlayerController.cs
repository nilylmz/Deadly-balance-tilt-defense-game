using UnityEngine;

namespace TiltDefense
{
    public class PlayerController : MonoBehaviour
    {
        public const int LaneCount = 5;
        public const int CenterLane = 2;

        public int CurrentLane { get; private set; } = CenterLane;
        public float Health { get; private set; }
        public float MaxHealth { get; private set; } = 100f;

        [Header("Lane Movement")]
        public float bottomY = -3.85f;
        public float laneSpacing = 1.35f;
        public float laneMoveSpeed = 8f;

        private SpriteRenderer bodyRenderer;
        private Transform shieldVisual;

        private void Awake()
        {
            bodyRenderer = GetComponent<SpriteRenderer>();
            if (bodyRenderer == null) bodyRenderer = gameObject.AddComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (GameManager.Instance.State != GameState.Playing) return;

            int laneStep = InputManager.Instance.ConsumeLaneStep();
            if (laneStep != 0)
            {
                SetLane(CurrentLane + laneStep);
            }

            Vector3 target = GetLanePosition(CurrentLane);
            transform.position = Vector3.Lerp(transform.position, target, 1f - Mathf.Exp(-laneMoveSpeed * Time.deltaTime));
        }

        public void ResetForGame()
        {
            Health = MaxHealth;
            CurrentLane = CenterLane;
            transform.position = GetLanePosition(CurrentLane);
            ApplyCharacter(CharacterSelectionManager.Instance.SelectedCharacter);
            SetShieldVisible(false);
        }

        public void ApplyCharacter(CharacterClass characterClass)
        {
            if (bodyRenderer == null) return;

            switch (characterClass)
            {
                case CharacterClass.MoonWitch:
                    bodyRenderer.color = new Color(0.55f, 0.75f, 1f, 1f);
                    MaxHealth = 85f;
                    break;
                case CharacterClass.RuneWarden:
                    bodyRenderer.color = new Color(0.55f, 1f, 0.75f, 1f);
                    MaxHealth = 120f;
                    break;
                default:
                    bodyRenderer.color = new Color(0.95f, 0.65f, 0.38f, 1f);
                    MaxHealth = 100f;
                    break;
            }

            Health = Mathf.Min(Health <= 0f ? MaxHealth : Health, MaxHealth);
        }

        public void TakeDamage(float damage)
        {
            AbilityManager abilityManager = GetComponent<AbilityManager>();
            if (abilityManager != null && abilityManager.ShieldActive)
            {
                damage *= 0.25f;
                FloatingText.Spawn(transform.position + Vector3.up * 0.8f, "Shield", Color.cyan);
            }

            Health = Mathf.Max(0f, Health - damage);
            CameraShake.Shake(0.15f, 0.08f);
        }

        public void SetShieldVisible(bool visible)
        {
            if (shieldVisual == null)
            {
                GameObject shield = VisualFactory.CreateCircle("Shield Aura", 96, new Color(0.2f, 0.8f, 1f, 0.35f));
                shield.transform.SetParent(transform, false);
                shield.transform.localScale = new Vector3(1.5f, 1.5f, 1f);
                shieldVisual = shield.transform;
            }

            shieldVisual.gameObject.SetActive(visible);
        }

        public static float LaneToX(int lane, float spacing = 1.35f)
        {
            return (lane - CenterLane) * spacing;
        }

        public Vector3 GetLanePosition(int lane)
        {
            return new Vector3(LaneToX(lane, laneSpacing), bottomY, 0f);
        }

        private void SetLane(int lane)
        {
            CurrentLane = Mathf.Clamp(lane, 0, LaneCount - 1);
        }
    }
}
