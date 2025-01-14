using System;
using UnityEngine;

namespace Ilumisoft.Game
{
    public class ObstacleSpawnSystem : SpawnSystem
    {
        [SerializeField] private GameObject prefab = null;

        [SerializeField] private int poolSize = 20;

        //The pool holding all spawnable objects
        private GameObject[] _pool;

        //The index of the next object spawned
        private int _index;
        public static Action ClearAll;

        private void Awake()
        {
            ClearAll += ClearAllObstacle;
            _pool = new GameObject[poolSize];

            for (int i = 0; i < poolSize; i++)
            {
                var instance = Instantiate(prefab);

                instance.SetActive(false);

                _pool[i] = instance;
            }

            _index = 0;
        }
        private void OnDestroy()
        {
            ClearAll -= ClearAllObstacle;
        }
        public void ClearAllObstacle()
        {
            for(int i=0;i<_pool.Length;i++)
            {
                _pool[i].SetActive(false);
            }
        }
        /// <summary>
        /// Spawns an element from the pool at the given position
        /// </summary>
        /// <param name="position"></param>
        public override void Spawn(Vector3 position)
        {
            var instance = _pool[_index];

            instance.transform.position = position;

            instance.SetActive(true);

            UpdatePoolIndex();
        }

        /// <summary>
        /// Increases the index, defining which element will be returned next, when the spawn method is called 
        /// </summary>
        private void UpdatePoolIndex()
        {
            _index++;

            if (_index >= _pool.Length)
            {
                _index = 0;
            }
        }
    }
}