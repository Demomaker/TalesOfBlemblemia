using UnityEngine;

namespace Game
{
    /// <summary>
    /// Manages tile blinking (made especially for usage with a Pointing Arrow)
    /// Author : Mike Bédard
    /// </summary>
    public class TileBlinker : MonoBehaviour
    {
        [SerializeField] private Tile tileToBlink;
        [SerializeField] private Sprite blinkSprite;
        
        private Coroutine blinkCoroutine;
        
        private void Start()
        {
            if(tileToBlink != null)
                blinkCoroutine = Harmony.Finder.CoroutineStarter.StartCoroutine(tileToBlink.Blink(blinkSprite));
        }

        private void OnDisable()
        {
            if(blinkCoroutine != null)
                StopCoroutine(blinkCoroutine);
            if(tileToBlink != null)
                tileToBlink.ResetTileImage();
        }
    }
}
