using UnityEngine;

namespace TiltDefense
{
    public static class VisualFactory
    {
        private static Sprite squareSprite;
        private static Sprite circleSprite;

        public static Sprite SquareSprite
        {
            get
            {
                if (squareSprite == null)
                {
                    Texture2D texture = new Texture2D(8, 8);
                    Color[] pixels = new Color[64];
                    for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.white;
                    texture.SetPixels(pixels);
                    texture.Apply();
                    squareSprite = Sprite.Create(texture, new Rect(0, 0, 8, 8), new Vector2(0.5f, 0.5f), 8);
                }

                return squareSprite;
            }
        }

        public static Sprite CircleSprite
        {
            get
            {
                if (circleSprite == null)
                {
                    Texture2D texture = new Texture2D(96, 96);
                    Vector2 center = new Vector2(47.5f, 47.5f);
                    for (int y = 0; y < 96; y++)
                    {
                        for (int x = 0; x < 96; x++)
                        {
                            float distance = Vector2.Distance(new Vector2(x, y), center) / 47.5f;
                            float alpha = Mathf.Clamp01(1f - Mathf.SmoothStep(0.65f, 1f, distance));
                            texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
                        }
                    }

                    texture.Apply();
                    circleSprite = Sprite.Create(texture, new Rect(0, 0, 96, 96), new Vector2(0.5f, 0.5f), 96);
                }

                return circleSprite;
            }
        }

        public static GameObject CreateCircle(string name, int size, Color color)
        {
            GameObject obj = new GameObject(name);
            SpriteRenderer renderer = obj.AddComponent<SpriteRenderer>();
            renderer.sprite = CircleSprite;
            renderer.color = color;
            renderer.sortingOrder = 5;
            return obj;
        }

        public static GameObject CreateProjectile(string name, Color color, float scale)
        {
            GameObject obj = CreateCircle(name, 64, color);
            obj.transform.localScale = new Vector3(scale, scale, 1f);
            obj.GetComponent<SpriteRenderer>().sortingOrder = 8;
            return obj;
        }

        public static GameObject CreateEnemy(EnemyType type)
        {
            GameObject obj = new GameObject(type + " Enemy");
            SpriteRenderer renderer = obj.AddComponent<SpriteRenderer>();
            renderer.sprite = CircleSprite;
            renderer.sortingOrder = 4;
            return obj;
        }

        public static GameObject CreateRect(string name, Vector3 position, Vector3 scale, Color color, int sortingOrder)
        {
            GameObject obj = new GameObject(name);
            obj.transform.position = position;
            obj.transform.localScale = scale;
            SpriteRenderer renderer = obj.AddComponent<SpriteRenderer>();
            renderer.sprite = SquareSprite;
            renderer.color = color;
            renderer.sortingOrder = sortingOrder;
            return obj;
        }

        public static void SpawnLaneFlash(int lane, Color color)
        {
            GameObject flash = CreateRect("Lane Magic Flash", new Vector3(PlayerController.LaneToX(lane), 0.4f, 0f), new Vector3(0.55f, 8.2f, 1f), color, 7);
            flash.AddComponent<FadeAndDestroy>().Configure(0.35f, true);
        }

        public static void SpawnBurst(Vector3 position, Color color, float scale)
        {
            GameObject burst = CreateCircle("Magic Burst", 96, color);
            burst.transform.position = position;
            burst.transform.localScale = new Vector3(scale, scale, 1f);
            burst.GetComponent<SpriteRenderer>().sortingOrder = 9;
            burst.AddComponent<FadeAndDestroy>().Configure(0.45f, true);
        }
    }

    public class FadeAndDestroy : MonoBehaviour
    {
        private float duration = 0.5f;
        private float age;
        private bool grow;
        private SpriteRenderer spriteRenderer;

        public void Configure(float seconds, bool shouldGrow)
        {
            duration = seconds;
            grow = shouldGrow;
        }

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            age += Time.deltaTime;
            float t = Mathf.Clamp01(age / duration);
            if (grow) transform.localScale *= 1f + Time.deltaTime * 1.8f;

            Color color = spriteRenderer.color;
            color.a *= 1f - t;
            spriteRenderer.color = color;

            if (age >= duration) Destroy(gameObject);
        }
    }
}
