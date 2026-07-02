using UnityEngine;

namespace TiltDefense
{
    public class GameBootstrapper : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Bootstrap()
        {
            if (FindObjectOfType<GameManager>() != null) return;

            GameObject root = new GameObject("Tilt Defense Runtime");
            root.AddComponent<GameBootstrapper>().Build(root.transform);
        }

        private void Build(Transform root)
        {
            Camera camera = BuildCamera();
            BuildBackground();

            GameObject systems = new GameObject("Systems");
            systems.transform.SetParent(root, false);

            InputManager inputManager = systems.AddComponent<InputManager>();
            CharacterSelectionManager characterSelection = systems.AddComponent<CharacterSelectionManager>();
            EnemySpawner enemySpawner = systems.AddComponent<EnemySpawner>();
            GameManager gameManager = systems.AddComponent<GameManager>();
            UIManager uiManager = systems.AddComponent<UIManager>();

            GameObject playerObject = BuildPlayer();
            PlayerController player = playerObject.GetComponent<PlayerController>();
            AbilityManager abilities = playerObject.GetComponent<AbilityManager>();
            abilities.ResetForGame();

            gameManager.player = player;
            gameManager.enemySpawner = enemySpawner;
            gameManager.uiManager = uiManager;

            inputManager.name = "Input Manager";
            characterSelection.name = "Character Selection Manager";
            camera.name = "Rear View Camera";
        }

        private Camera BuildCamera()
        {
            Camera camera = Camera.main;
            if (camera == null)
            {
                GameObject cameraObject = new GameObject("Main Camera");
                camera = cameraObject.AddComponent<Camera>();
                cameraObject.tag = "MainCamera";
            }

            camera.transform.position = new Vector3(0f, 0.1f, -10f);
            camera.orthographic = true;
            camera.orthographicSize = 5f;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.035f, 0.03f, 0.06f, 1f);
            camera.gameObject.AddComponent<CameraShake>();
            return camera;
        }

        private GameObject BuildPlayer()
        {
            GameObject player = new GameObject("Selected Defender");
            player.transform.position = new Vector3(0f, -3.85f, 0f);
            SpriteRenderer renderer = player.AddComponent<SpriteRenderer>();
            renderer.sprite = VisualFactory.CircleSprite;
            renderer.sortingOrder = 6;
            player.transform.localScale = new Vector3(0.82f, 1.2f, 1f);
            player.AddComponent<PlayerController>();
            player.AddComponent<AbilityManager>();
            return player;
        }

        private void BuildBackground()
        {
            VisualFactory.CreateRect("Misty Sky", new Vector3(0f, 1.5f, 2f), new Vector3(14f, 8f, 1f), new Color(0.07f, 0.075f, 0.14f, 1f), -10);
            VisualFactory.CreateRect("Distant Fog", new Vector3(0f, 1.1f, 1f), new Vector3(14f, 2.8f, 1f), new Color(0.32f, 0.35f, 0.46f, 0.3f), -4);
            VisualFactory.CreateRect("Battlefield", new Vector3(0f, -2.2f, 1f), new Vector3(12f, 4.6f, 1f), new Color(0.095f, 0.105f, 0.095f, 1f), -8);

            BuildCastle(-2.6f);
            BuildCastle(2.8f);
            BuildDeadTree(-5.2f, -1.25f, 0.9f);
            BuildDeadTree(5.0f, -1.0f, 1.1f);
            BuildDeadTree(-4.1f, 0.7f, 0.55f);
            BuildDeadTree(4.2f, 0.6f, 0.55f);

            for (int lane = 0; lane < PlayerController.LaneCount; lane++)
            {
                float x = PlayerController.LaneToX(lane);
                VisualFactory.CreateRect("Invisible Lane Guide", new Vector3(x, 0.15f, 0f), new Vector3(0.04f, 7.1f, 1f), new Color(0.32f, 0.75f, 1f, 0.2f), -2);
            }

            for (int i = 0; i < 7; i++)
            {
                float y = Mathf.Lerp(4.1f, -3.1f, i / 6f);
                float width = Mathf.Lerp(2.3f, 8.6f, i / 6f);
                VisualFactory.CreateRect("Perspective Fog Band", new Vector3(0f, y, 0f), new Vector3(width, 0.11f, 1f), new Color(0.58f, 0.62f, 0.72f, 0.2f), -1);
            }
        }

        private void BuildCastle(float x)
        {
            Color color = new Color(0.05f, 0.045f, 0.075f, 1f);
            VisualFactory.CreateRect("Castle Keep", new Vector3(x, 2.55f, 0f), new Vector3(1.15f, 1.7f, 1f), color, -6);
            VisualFactory.CreateRect("Castle Tower", new Vector3(x - 0.75f, 2.45f, 0f), new Vector3(0.45f, 2.0f, 1f), color, -6);
            VisualFactory.CreateRect("Castle Tower", new Vector3(x + 0.75f, 2.45f, 0f), new Vector3(0.45f, 2.0f, 1f), color, -6);
            VisualFactory.CreateRect("Magic Window", new Vector3(x, 2.72f, 0f), new Vector3(0.18f, 0.42f, 1f), new Color(0.45f, 0.2f, 0.85f, 0.95f), -5);
        }

        private void BuildDeadTree(float x, float y, float scale)
        {
            Color color = new Color(0.075f, 0.055f, 0.05f, 1f);
            VisualFactory.CreateRect("Dead Tree Trunk", new Vector3(x, y, 0f), new Vector3(0.11f * scale, 1.25f * scale, 1f), color, -3);
            VisualFactory.CreateRect("Dead Branch", new Vector3(x - 0.18f * scale, y + 0.28f * scale, 0f), new Vector3(0.55f * scale, 0.08f * scale, 1f), color, -3).transform.rotation = Quaternion.Euler(0f, 0f, 28f);
            VisualFactory.CreateRect("Dead Branch", new Vector3(x + 0.2f * scale, y + 0.42f * scale, 0f), new Vector3(0.5f * scale, 0.07f * scale, 1f), color, -3).transform.rotation = Quaternion.Euler(0f, 0f, -34f);
        }
    }
}
