using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game
{
    //Author : Antoine Lessard
    public class CameraShake : MonoBehaviour
    {
        [Header("Shake Values")] 
        [SerializeField] private float shakeDuration;
        [SerializeField] private float shakeMagnitude;
        [SerializeField] private float dampingSpeed;

        private float duration;
        private Transform transform;
        private Vector3 initialPosition;
        
        private void Awake()
        {
            transform = GetComponent<Transform>();
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