using System.Collections;
using UnityEngine;

namespace TiltDefense
{
    public class AbilityManager : MonoBehaviour
    {
        public float Energy { get; private set; }
        public float MaxEnergy { get; private set; } = 100f;
        public int Ammo { get; private set; }
        public int MaxAmmo { get; private set; } = 6;
        public bool ShieldActive { get; private set; }

        [Header("Ability Costs")]
        public float shockwaveCost = 18f;
        public float splashCost = 45f;
        public float shieldCost = 32f;
        public float energyRegenPerSecond = 8f;

        private PlayerController player;
        private Coroutine shieldRoutine;

        private void Awake()
        {
            player = GetComponent<PlayerController>();
        }

        private void Update()
        {
            if (GameManager.Instance.State != GameState.Playing) return;

            Energy = Mathf.Min(MaxEnergy, Energy + energyRegenPerSecond * Time.deltaTime);

            if (InputManager.Instance.ConsumeAbility(AbilityCommand.LeftShockwave)) CastShockwave(0, 1);
            if (InputManager.Instance.ConsumeAbility(AbilityCommand.RightShockwave)) CastShockwave(3, 4);
            if (InputManager.Instance.ConsumeAbility(AbilityCommand.ReloadCharge)) Reload();
            if (InputManager.Instance.ConsumeAbility(AbilityCommand.EnergySplash)) CastEnergySplash();
            if (InputManager.Instance.ConsumeAbility(AbilityCommand.TemporaryShield)) CastShield();
            if (InputManager.Instance.ConsumeAbility(AbilityCommand.EmergencyShot)) EmergencyShot();

            UIManager.Instance.RefreshHud();
        }

        public void ResetForGame()
        {
            Energy = MaxEnergy;
            Ammo = MaxAmmo;
            ShieldActive = false;
            player.SetShieldVisible(false);
        }

        private bool SpendEnergy(float amount)
        {
            if (Energy < amount)
            {
                FloatingText.Spawn(player.transform.position + Vector3.up, "Low Energy", Color.yellow);
                return false;
            }

            Energy -= amount;
            return true;
        }

        private void CastShockwave(int laneA, int laneB)
        {
            if (!SpendEnergy(shockwaveCost)) return;

            SpawnShockwaveProjectile(laneA);
            SpawnShockwaveProjectile(laneB);
            VisualFactory.SpawnLaneFlash(laneA, new Color(0.45f, 0.9f, 1f, 0.65f));
            VisualFactory.SpawnLaneFlash(laneB, new Color(0.45f, 0.9f, 1f, 0.65f));
            CameraShake.Shake(0.18f, 0.06f);
        }

        private void SpawnShockwaveProjectile(int lane)
        {
            GameObject projectileObject = VisualFactory.CreateProjectile("Shockwave", new Color(0.45f, 0.95f, 1f, 0.9f), 0.45f);
            projectileObject.transform.position = new Vector3(PlayerController.LaneToX(lane), -3.1f, 0f);
            ProjectileController projectile = projectileObject.AddComponent<ProjectileController>();
            projectile.Configure(lane, 24f, 5.3f, 16f, false);
        }

        private void Reload()
        {
            Ammo = MaxAmmo;
            Energy = Mathf.Min(MaxEnergy, Energy + 15f);
            VisualFactory.SpawnBurst(player.transform.position + Vector3.up * 0.55f, new Color(0.6f, 0.4f, 1f, 0.8f), 1.2f);
            FloatingText.Spawn(player.transform.position + Vector3.up * 1.2f, "Charged", new Color(0.75f, 0.55f, 1f, 1f));
        }

        private void CastEnergySplash()
        {
            if (!SpendEnergy(splashCost)) return;

            EnemyController[] enemies = FindObjectsOfType<EnemyController>();
            for (int i = 0; i < enemies.Length; i++)
            {
                enemies[i].ApplySlow(2.4f, 0.45f);
                enemies[i].TakeDamage(32f);
            }

            for (int lane = 0; lane < PlayerController.LaneCount; lane++)
            {
                VisualFactory.SpawnLaneFlash(lane, new Color(0.9f, 0.25f, 1f, 0.7f));
            }

            VisualFactory.SpawnBurst(new Vector3(0f, -2.2f, 0f), new Color(0.9f, 0.2f, 1f, 0.8f), 3.2f);
            CameraShake.Shake(0.35f, 0.18f);
        }

        private void CastShield()
        {
            if (!SpendEnergy(shieldCost)) return;

            if (shieldRoutine != null) StopCoroutine(shieldRoutine);
            shieldRoutine = StartCoroutine(ShieldRoutine());
        }

        private IEnumerator ShieldRoutine()
        {
            ShieldActive = true;
            player.SetShieldVisible(true);
            yield return new WaitForSeconds(5f);
            ShieldActive = false;
            player.SetShieldVisible(false);
        }

        private void EmergencyShot()
        {
            if (Ammo <= 0)
            {
                FloatingText.Spawn(player.transform.position + Vector3.up, "Reload", Color.yellow);
                return;
            }

            Ammo--;
            GameObject projectileObject = VisualFactory.CreateProjectile("Emergency Shot", new Color(1f, 0.75f, 0.35f, 0.95f), 0.28f);
            projectileObject.transform.position = player.transform.position + Vector3.up * 0.55f;
            ProjectileController projectile = projectileObject.AddComponent<ProjectileController>();
            projectile.Configure(player.CurrentLane, 18f, 7.5f, 8f, true);
        }
    }
}
