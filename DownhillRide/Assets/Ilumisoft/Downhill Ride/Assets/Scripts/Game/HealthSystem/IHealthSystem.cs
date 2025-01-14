using UnityEngine.Events;

namespace Ilumisoft.Game
{
    public interface IHealthSystem
    {
        HealthChangedEvent OnHealthChanged { get; }

        UnityEvent OnHealthEmpty { get; }

        void ModifyHealth(int amount);
    }
}