using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class SliderValueText : MonoBehaviour
    {
        [SerializeField] private string formatText;

        private TextMeshProUGUI sliderValueText;

        private void Awake()
        {
            sliderValueText = GetComponent<TextMeshProUGUI>();

            GetComponentInParent<Slider>().onValueChanged.AddListener(HandleValueChanged);
        }

        private void HandleValueChanged(float value)
        {
            sliderValueText.text = string.Format(formatText, value);
        }
    }
}