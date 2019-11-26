using UnityEngine;

namespace Game
{
    /// <summary>
    /// A tile sprite
    /// Author : Mike Bédard
    /// </summary>
    public class TileSprite : MonoBehaviour
    {
        [SerializeField] private Sprite tileSprite;
        
        public void SetSprite(Sprite sprite)
        {
            tileSprite = sprite;
        }

        public Sprite GetSprite()
        {
            return tileSprite;
        }
    }
}