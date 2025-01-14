using UnityEngine.Events;

namespace Ilumisoft.Game
{
    public interface IGameEvent
    {
        void AddListener(UnityAction call);
        void RemoveListener(UnityAction call);
        void Invoke();
    }
}