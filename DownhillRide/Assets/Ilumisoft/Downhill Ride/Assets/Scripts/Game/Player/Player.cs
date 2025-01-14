using System;
using UnityEngine;

namespace Ilumisoft.Game
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private float verticalSpeed = 15.0f;
        [SerializeField] private float horizontalSpeed = 10.0f;

        private IHealthSystem _healthSystem;
        private IHorizontalInputProvider _horizontalInputProvider;
        private Rigidbody _rigidbody;
        private EventManager _eventManager;
        public static Action PlayerResapwn;

        private void Awake()
        {
            _eventManager = FindObjectOfType<EventManager>();

            _rigidbody = GetComponent<Rigidbody>();

            _healthSystem = GetComponent<IHealthSystem>();

            _horizontalInputProvider = new HorizontalInputProvider();

            PlayerResapwn += Respawn;
        }
        private void OnDestroy()
        {
            PlayerResapwn -= Respawn;
        }
        private void Start()
        {
            _rigidbody.velocity = new Vector3(0, 0, -verticalSpeed);

            if (_healthSystem != null)
            {
                _healthSystem.OnHealthEmpty.AddListener(Die);
            }
        }

        private void FixedUpdate()
        {
            ProcessInput();
        }

        private void ProcessInput()
        {
            var horizontalInput = _horizontalInputProvider.GetHorizontalInput();

            var movement = new Vector3(horizontalInput, 0, 0);

            _rigidbody.AddForce(movement * horizontalSpeed);
        }

        private void Die()
        {
            //Invoke Game Over
            GameManager.GameOverEvent();
            gameObject.SetActive(false);
            ObstacleSpawnSystem.ClearAll();
            Time.timeScale = 0;
            //_eventManager.GetEvent<GameOverEvent>().Invoke();
        }
        public void Respawn()
        {
            transform.position = new Vector3(0, 0, transform.position.z);
            gameObject.SetActive(true);
            Time.timeScale = 1;
        }
    }
}