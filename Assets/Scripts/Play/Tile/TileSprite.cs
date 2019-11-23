using UnityEngine;

namespace Game
{
    /// <summary>
    /// A tile sprite
    /// Author : Mike Bédard
    /// </summary>
    public class TileSprite : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField] private Sprite tileSprite;
        #endregion Serialized Fields
        #region TileSprite-related Functions
        public void SetSprite(Sprite sprite)
        {
            tileSprite = sprite;
        }

        public Sprite GetSprite()
        {
            return tileSprite;
        }
        #endregion TileSprite-related Functions
    }
}