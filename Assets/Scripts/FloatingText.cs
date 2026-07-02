using UnityEngine;

namespace TiltDefense
{
    public class FloatingText : MonoBehaviour
    {
        private TextMesh textMesh;
        private float age;

        public static void Spawn(Vector3 position, string text, Color color)
        {
            GameObject obj = new GameObject("Floating Text");
            obj.transform.position = position;
            FloatingText floatingText = obj.AddComponent<FloatingText>();
            floatingText.textMesh = obj.AddComponent<TextMesh>();
            floatingText.textMesh.text = text;
            floatingText.textMesh.characterSize = 0.22f;
            floatingText.textMesh.anchor = TextAnchor.MiddleCenter;
            floatingText.textMesh.alignment = TextAlignment.Center;
            floatingText.textMesh.color = color;
            obj.GetComponent<MeshRenderer>().sortingOrder = 20;
        }

        private void Update()
        {
            age += Time.deltaTime;
            transform.position += Vector3.up * Time.deltaTime * 0.65f;
            Color color = textMesh.color;
            color.a = 1f - age;
            textMesh.color = color;
            if (age >= 1f) Destroy(gameObject);
        }
    }
}
