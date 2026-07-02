using UnityEngine;

namespace TiltDefense
{
    public enum GameState
    {
        MainMenu,
        CharacterSelect,
        Playing,
        GameOver
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public GameState State { get; private set; } = GameState.MainMenu;
        public int Score { get; private set; }
        public int HighScore { get; private set; }
        public int Wave { get; private set; } = 1;

        [Header("Scene References")]
        public PlayerController player;
        public EnemySpawner enemySpawner;
        public UIManager uiManager;

        private const string HighScoreKey = "TiltDefenseHighScore";

        private void Awake()
        {
            Instance = this;
            HighScore = PlayerPrefs.GetInt(HighScoreKey, 0);
        }

        private void Start()
        {
            SetState(GameState.MainMenu);
        }

        public void StartGame()
        {
            Score = 0;
            Wave = 1;
            player.ResetForGame();
            player.GetComponent<AbilityManager>().ResetForGame();
            enemySpawner.ResetSpawner();
            SetState(GameState.Playing);
            enemySpawner.BeginWaves();
        }

        public void OpenCharacterSelect()
        {
            SetState(GameState.CharacterSelect);
        }

        public void ReturnToMenu()
        {
            enemySpawner.StopWaves();
            SetState(GameState.MainMenu);
        }

        public void AddScore(int amount)
        {
            Score += amount;
            uiManager.RefreshHud();
        }

        public void AdvanceWave(int wave)
        {
            Wave = wave;
            uiManager.ShowWaveIncoming(wave);
            uiManager.RefreshHud();
        }

        public void DamagePlayer(float amount)
        {
            if (State != GameState.Playing) return;

            player.TakeDamage(amount);
            uiManager.RefreshHud();

            if (player.Health <= 0f)
            {
                GameOver();
            }
        }

        private void GameOver()
        {
            enemySpawner.StopWaves();
            HighScore = Mathf.Max(HighScore, Score);
            PlayerPrefs.SetInt(HighScoreKey, HighScore);
            SetState(GameState.GameOver);
        }

        private void SetState(GameState state)
        {
            State = state;
            uiManager.SetState(state);
        }
    }
}
