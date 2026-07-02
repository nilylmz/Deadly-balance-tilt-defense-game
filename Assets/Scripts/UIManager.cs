using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TiltDefense
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        private Canvas canvas;
        private GameObject mainMenuPanel;
        private GameObject characterPanel;
        private GameObject hudPanel;
        private GameObject gameOverPanel;

        private Text menuScoreText;
        private Text hudText;
        private Text waveText;
        private Text shieldText;
        private Text gameOverText;

        private readonly Color panelColor = new Color(0.01f, 0.01f, 0.018f, 0.58f);

        private void Awake()
        {
            Instance = this;
            BuildUi();
        }

        private void Start()
        {
            SetState(GameState.MainMenu);
        }

        public void SetState(GameState state)
        {
            mainMenuPanel.SetActive(state == GameState.MainMenu);
            characterPanel.SetActive(state == GameState.CharacterSelect);
            hudPanel.SetActive(state == GameState.Playing);
            gameOverPanel.SetActive(state == GameState.GameOver);

            RefreshHud();
            RefreshMenu();
        }

        public void RefreshHud()
        {
            if (hudText == null || GameManager.Instance == null || GameManager.Instance.player == null) return;

            PlayerController player = GameManager.Instance.player;
            AbilityManager abilities = player.GetComponent<AbilityManager>();
            hudText.text =
                "Health " + Mathf.CeilToInt(player.Health) + "/" + Mathf.CeilToInt(player.MaxHealth) +
                "   Energy " + Mathf.CeilToInt(abilities.Energy) + "/" + Mathf.CeilToInt(abilities.MaxEnergy) +
                "   Ammo " + abilities.Ammo + "/" + abilities.MaxAmmo +
                "   Score " + GameManager.Instance.Score +
                "   Wave " + GameManager.Instance.Wave;

            shieldText.text = abilities.ShieldActive ? "SHIELD ACTIVE" : "";
        }

        public void ShowWaveIncoming(int wave)
        {
            waveText.text = "Wave " + wave + " Incoming";
            CancelInvoke(nameof(ClearWaveText));
            Invoke(nameof(ClearWaveText), 1.6f);
        }

        private void ClearWaveText()
        {
            waveText.text = "";
        }

        private void RefreshMenu()
        {
            if (GameManager.Instance == null || menuScoreText == null) return;
            menuScoreText.text = "High Score " + GameManager.Instance.HighScore;
            gameOverText.text = "Game Over\nFinal Score " + GameManager.Instance.Score;
        }

        private void BuildUi()
        {
            EnsureEventSystem();

            GameObject canvasObject = new GameObject("Canvas");
            canvasObject.transform.SetParent(transform, false);
            canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 500;
            canvasObject.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasObject.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1280, 720);
            canvasObject.AddComponent<GraphicRaycaster>();

            mainMenuPanel = CreatePanel("Main Menu", Vector2.zero, Vector2.one, panelColor);
            Text title = CreateText(mainMenuPanel.transform, "Tilt Defense:\nDark Balance", 56, TextAnchor.MiddleCenter);
            SetRect(title.rectTransform, new Vector2(0.5f, 0.62f), new Vector2(0.5f, 0.62f), new Vector2(640f, 150f));
            Button start = CreateButton(mainMenuPanel.transform, "START", new Vector2(0.5f, 0.43f), new Vector2(240f, 78f));
            start.onClick.AddListener(() => GameManager.Instance.StartGame());
            menuScoreText = CreateText(mainMenuPanel.transform, "", 24, TextAnchor.MiddleCenter);
            SetRect(menuScoreText.rectTransform, new Vector2(0.5f, 0.32f), new Vector2(0.5f, 0.32f), new Vector2(360f, 48f));
            Button characters = CreateButton(mainMenuPanel.transform, "O\n/|\\", new Vector2(0.88f, 0.5f), new Vector2(112f, 112f));
            characters.onClick.AddListener(() => GameManager.Instance.OpenCharacterSelect());

            characterPanel = CreatePanel("Character Selection", Vector2.zero, Vector2.one, panelColor);
            Text selectTitle = CreateText(characterPanel.transform, "Choose Defender", 40, TextAnchor.MiddleCenter);
            SetRect(selectTitle.rectTransform, new Vector2(0.5f, 0.78f), new Vector2(0.5f, 0.78f), new Vector2(520f, 70f));
            AddCharacterButton("Ash Knight", CharacterClass.AshKnight, 0.32f);
            AddCharacterButton("Moon Witch", CharacterClass.MoonWitch, 0.50f);
            AddCharacterButton("Rune Warden", CharacterClass.RuneWarden, 0.68f);
            Button back = CreateButton(characterPanel.transform, "BACK", new Vector2(0.5f, 0.22f), new Vector2(180f, 58f));
            back.onClick.AddListener(() => GameManager.Instance.ReturnToMenu());

            hudPanel = CreatePanel("HUD", Vector2.zero, Vector2.one, new Color(0f, 0f, 0f, 0f));
            hudText = CreateText(hudPanel.transform, "", 22, TextAnchor.UpperLeft);
            SetRect(hudText.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(-28f, 52f), new Vector2(14f, -12f));
            Text hints = CreateText(hudPanel.transform, "Balance Board: Left/Right Arrows   Wrist Gestures: Q Left, E Right, W Charge, S Splash, R Shield   Hand Button: Space", 18, TextAnchor.LowerCenter);
            SetRect(hints.rectTransform, new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(-40f, 46f), new Vector2(20f, 10f));
            waveText = CreateText(hudPanel.transform, "", 38, TextAnchor.MiddleCenter);
            SetRect(waveText.rectTransform, new Vector2(0.5f, 0.66f), new Vector2(0.5f, 0.66f), new Vector2(520f, 70f));
            shieldText = CreateText(hudPanel.transform, "", 24, TextAnchor.MiddleRight);
            SetRect(shieldText.rectTransform, new Vector2(0.72f, 0.88f), new Vector2(0.98f, 0.88f), new Vector2(0f, 40f));

            gameOverPanel = CreatePanel("Game Over", Vector2.zero, Vector2.one, panelColor);
            gameOverText = CreateText(gameOverPanel.transform, "", 46, TextAnchor.MiddleCenter);
            SetRect(gameOverText.rectTransform, new Vector2(0.5f, 0.57f), new Vector2(0.5f, 0.57f), new Vector2(520f, 140f));
            Button retry = CreateButton(gameOverPanel.transform, "RETRY", new Vector2(0.5f, 0.36f), new Vector2(220f, 68f));
            retry.onClick.AddListener(() => GameManager.Instance.StartGame());
            Button menu = CreateButton(gameOverPanel.transform, "MENU", new Vector2(0.5f, 0.25f), new Vector2(180f, 58f));
            menu.onClick.AddListener(() => GameManager.Instance.ReturnToMenu());

            SetState(GameState.MainMenu);
        }

        private void AddCharacterButton(string label, CharacterClass characterClass, float x)
        {
            Button button = CreateButton(characterPanel.transform, label, new Vector2(x, 0.52f), new Vector2(190f, 86f));
            button.onClick.AddListener(() =>
            {
                CharacterSelectionManager.Instance.SelectCharacter(characterClass);
                GameManager.Instance.ReturnToMenu();
            });
        }

        private GameObject CreatePanel(string name, Vector2 anchorMin, Vector2 anchorMax, Color color)
        {
            GameObject panel = new GameObject(name);
            panel.transform.SetParent(canvas.transform, false);
            Image image = panel.AddComponent<Image>();
            image.color = color;
            SetRect(panel.GetComponent<RectTransform>(), anchorMin, anchorMax, Vector2.zero);
            return panel;
        }

        private Button CreateButton(Transform parent, string label, Vector2 centerAnchor, Vector2 size)
        {
            GameObject obj = new GameObject(label + " Button");
            obj.transform.SetParent(parent, false);
            Image image = obj.AddComponent<Image>();
            image.color = new Color(0.2f, 0.09f, 0.28f, 0.96f);
            Button button = obj.AddComponent<Button>();
            ColorBlock colors = button.colors;
            colors.normalColor = image.color;
            colors.highlightedColor = new Color(0.38f, 0.16f, 0.5f, 1f);
            colors.pressedColor = new Color(0.6f, 0.24f, 0.72f, 1f);
            button.colors = colors;
            SetRect(obj.GetComponent<RectTransform>(), centerAnchor, centerAnchor, size);

            Text text = CreateText(obj.transform, label, 26, TextAnchor.MiddleCenter);
            SetRect(text.rectTransform, Vector2.zero, Vector2.one, Vector2.zero);
            return button;
        }

        private Text CreateText(Transform parent, string content, int fontSize, TextAnchor anchor)
        {
            GameObject obj = new GameObject("Text");
            obj.transform.SetParent(parent, false);
            Text text = obj.AddComponent<Text>();
            text.text = content;
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = fontSize;
            text.alignment = anchor;
            text.color = new Color(0.98f, 0.96f, 1f, 1f);
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.raycastTarget = false;
            return text;
        }

        private void SetRect(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 size)
        {
            SetRect(rect, anchorMin, anchorMax, size, Vector2.zero);
        }

        private void SetRect(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 size, Vector2 anchoredPosition)
        {
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.sizeDelta = size;
            rect.anchoredPosition = anchoredPosition;
        }

        private void EnsureEventSystem()
        {
            if (FindObjectOfType<EventSystem>() != null) return;
            GameObject obj = new GameObject("EventSystem");
            obj.AddComponent<EventSystem>();
            obj.AddComponent<StandaloneInputModule>();
        }
    }
}
