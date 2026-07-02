using UnityEngine;

namespace TiltDefense
{
    public enum AbilityCommand
    {
        LeftShockwave,
        RightShockwave,
        ReloadCharge,
        EnergySplash,
        TemporaryShield,
        EmergencyShot
    }

    public interface IGameInputProvider
    {
        int ConsumeLaneStep();
        bool ConsumeAbility(AbilityCommand command);
    }

    public class KeyboardInputProvider : IGameInputProvider
    {
        public int ConsumeLaneStep()
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow)) return -1;
            if (Input.GetKeyDown(KeyCode.RightArrow)) return 1;
            return 0;
        }

        public bool ConsumeAbility(AbilityCommand command)
        {
            switch (command)
            {
                case AbilityCommand.LeftShockwave: return Input.GetKeyDown(KeyCode.Q);
                case AbilityCommand.RightShockwave: return Input.GetKeyDown(KeyCode.E);
                case AbilityCommand.ReloadCharge: return Input.GetKeyDown(KeyCode.W);
                case AbilityCommand.EnergySplash: return Input.GetKeyDown(KeyCode.S);
                case AbilityCommand.TemporaryShield: return Input.GetKeyDown(KeyCode.R);
                case AbilityCommand.EmergencyShot: return Input.GetKeyDown(KeyCode.Space);
                default: return false;
            }
        }
    }

    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance { get; private set; }

        public IGameInputProvider Provider { get; private set; }

        private void Awake()
        {
            Instance = this;
            Provider = new KeyboardInputProvider();
        }

        public int ConsumeLaneStep()
        {
            return Provider.ConsumeLaneStep();
        }

        public bool ConsumeAbility(AbilityCommand command)
        {
            return Provider.ConsumeAbility(command);
        }

        public void SetInputProvider(IGameInputProvider provider)
        {
            Provider = provider ?? new KeyboardInputProvider();
        }
    }
}
