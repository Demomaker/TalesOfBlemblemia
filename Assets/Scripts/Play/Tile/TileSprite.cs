using UnityEngine;

namespace Game
{
    //Author: Mike Bédard
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