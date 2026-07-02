using UnityEngine;

namespace TiltDefense
{
    public enum CharacterClass
    {
        AshKnight,
        MoonWitch,
        RuneWarden
    }

    public class CharacterSelectionManager : MonoBehaviour
    {
        public static CharacterSelectionManager Instance { get; private set; }

        public CharacterClass SelectedCharacter { get; private set; } = CharacterClass.AshKnight;

        private void Awake()
        {
            Instance = this;
        }

        public void SelectCharacter(CharacterClass characterClass)
        {
            SelectedCharacter = characterClass;
            if (GameManager.Instance != null && GameManager.Instance.player != null)
            {
                GameManager.Instance.player.ApplyCharacter(characterClass);
            }
        }
    }
}
