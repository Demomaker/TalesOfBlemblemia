using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    //Code taken here : https://unity3d.college/2017/06/29/unity-slider-label-text/
    //and adapted to current usage by Antoine Lessard
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