using UnityEngine;

namespace Ilumisoft.Game
{
    public abstract class SpawnSystem : MonoBehaviour
    {
        public abstract void Spawn(Vector3 position);
    }
}