using UnityEngine;
using Random = UnityEngine.Random;

namespace Game
{
    //Author : Antoine Lessard
    //Code taken here : https://medium.com/@mattThousand/basic-2d-screen-shake-in-unity-9c27b56b516
    //and adapted to current usage by Antoine Lessard
    public class CameraShake : MonoBehaviour
    {
        [Header("Shake Values")] 
        [SerializeField] private float shakeDuration;
        [SerializeField] private float shakeMagnitude;
        [SerializeField] private float dampingSpeed;

        private float duration;
        private Vector3 initialPosition;
        
        private void Awake()
        {
            duration = 0f;
        }

        private void OnEnable()
        {
            initialPosition = transform.localPosition;
        }

        private void Update()
        {
            if (duration > 0)
            {
                transform.localPosition = initialPosition + Random.insideUnitSphere * shakeMagnitude;
                duration -= Time.deltaTime * dampingSpeed;
            }
            else
            {
                duration = 0f;
            }
        }

        public void TriggerShake()
        {
            initialPosition = transform.localPosition;
            duration = shakeDuration;
        }
    }
}