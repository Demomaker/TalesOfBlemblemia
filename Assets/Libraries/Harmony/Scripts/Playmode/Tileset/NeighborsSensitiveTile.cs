using UnityEngine;
using UnityEngine.Tilemaps;

namespace Harmony
{
    /// <summary>
    /// Tile whose sprite change according to it's neighbours.
    /// </summary>
    [CreateAssetMenu(fileName = "New NeighborsSensitiveTile", menuName = "Game/NeighborsSensitiveTile")]
    public class NeighborsSensitiveTile : TileBase
    {
        //Due to the lack of documentation in "TileBase", I have documented
        //some methods here to help you understand what it does. You're welcome.

        //Sprite indexes
        // 0 1 2
        // 3 4 5
        // 6 7 8
        [SerializeField] private Sprite[] sprites = new Sprite[9];

        /// <summary>
        /// Refresh the tile.
        /// </summary>
        /// <param name="position">Position.</param>
        /// <param name="tilemap">Tilemap.</param>
        public override void RefreshTile(Vector3Int position, ITilemap tilemap)
        {
            for (var i = -1; i <= 1; i++)
            {
                for (var j = -1; j <= 1; j++)
                {
                    var neighborPositon = new Vector3Int(position.x + i, position.y + j, position.z);
                    var neighborTile = tilemap.GetTile(neighborPositon);
                    if (HasSameTileAssetThan(neighborTile))
                    {
                        //Please note : Here, we do not call "neighborTile.RefreshTile", but
                        //              "tilemap.RefreshTile". Tilemap holds a list of tiles to be refreshed
                        //              by calling "GetTileData".
                        tilemap.RefreshTile(neighborPositon);
                    }
                }
            }
        }

        /// <summary>
        /// Determines what the Tile looks like on the Tilemap (the sprite used).
        /// </summary>
        /// <param name="position">Position.</param>
        /// <param name="tilemap">Tilemap.</param>
        /// <param name="tileData">Output. Tile configuration.</param>
        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            //Basic settings
            tileData.transform = Matrix4x4.identity;
            tileData.color = Color.white;
            tileData.flags = TileFlags.LockTransform | TileFlags.LockColor;
            tileData.colliderType = Tile.ColliderType.Sprite;

            //Sprite setting
            var leftNeighbor = tilemap.GetTile(new Vector3Int(position.x - 1, position.y, position.z));
            var rightNeighbor = tilemap.GetTile(new Vector3Int(position.x + 1, position.y, position.z));
            var topNeighbor = tilemap.GetTile(new Vector3Int(position.x, position.y + 1, position.z));
            var bottomNeighbor = tilemap.GetTile(new Vector3Int(position.x, position.y - 1, position.z));

            //Conditions are ordered by "most likely to happen first" for performance purposes.
            //Is center ?
            if (HasSameTileAssetThan(topNeighbor) &&
                HasSameTileAssetThan(leftNeighbor) &&
                HasSameTileAssetThan(rightNeighbor) &&
                HasSameTileAssetThan(bottomNeighbor))
            {
                tileData.sprite = sprites[4];
            }
            //Has no neighbors ?
            else if (topNeighbor == null &&
                     leftNeighbor == null &&
                     rightNeighbor == null &&
                     bottomNeighbor == null)
            {
                tileData.sprite = sprites[1];
            }
            //Is top left corner ?
            else if (HasNotSameTileAssetThan(topNeighbor) &&
                     HasNotSameTileAssetThan(leftNeighbor) &&
                     HasSameTileAssetThan(rightNeighbor))
            {
                tileData.sprite = sprites[0];
            }
            //Is top right corner ?
            else if (HasNotSameTileAssetThan(topNeighbor) &&
                     HasNotSameTileAssetThan(rightNeighbor) &&
                     HasSameTileAssetThan(leftNeighbor))
            {
                tileData.sprite = sprites[2];
            }
            //Is bottom left corner ?
            else if (HasNotSameTileAssetThan(leftNeighbor) &&
                     HasNotSameTileAssetThan(bottomNeighbor) &&
                     HasSameTileAssetThan(rightNeighbor))
            {
                tileData.sprite = sprites[6];
            }
            //Is bottom right corner ?
            else if (HasNotSameTileAssetThan(rightNeighbor) &&
                     HasNotSameTileAssetThan(bottomNeighbor) &&
                     HasSameTileAssetThan(leftNeighbor))
            {
                tileData.sprite = sprites[8];
            }
            //Is top ?
            else if (HasNotSameTileAssetThan(topNeighbor) &&
                     HasSameTileAssetThan(leftNeighbor) &&
                     HasSameTileAssetThan(rightNeighbor))
            {
                tileData.sprite = sprites[1];
            }
            //Is bottom ?
            else if (HasNotSameTileAssetThan(bottomNeighbor) &&
                     HasSameTileAssetThan(leftNeighbor) &&
                     HasSameTileAssetThan(rightNeighbor))
            {
                tileData.sprite = sprites[7];
            }
            //Is left ?
            else if (HasNotSameTileAssetThan(leftNeighbor) &&
                     HasSameTileAssetThan(topNeighbor) &&
                     HasSameTileAssetThan(rightNeighbor) &&
                     HasSameTileAssetThan(bottomNeighbor))
            {
                tileData.sprite = sprites[3];
            }
            //Is right ?
            else if (HasNotSameTileAssetThan(rightNeighbor) &&
                     HasSameTileAssetThan(topNeighbor) &&
                     HasSameTileAssetThan(leftNeighbor) &&
                     HasSameTileAssetThan(bottomNeighbor))
            {
                tileData.sprite = sprites[5];
            }
            //Has only neighbors on top ?
            else if (HasSameTileAssetThan(topNeighbor) &&
                     HasNotSameTileAssetThan(leftNeighbor) &&
                     HasNotSameTileAssetThan(rightNeighbor))
            {
                tileData.sprite = sprites[4];
            }
            //Has only neighbors on bottom ?
            else if (HasSameTileAssetThan(bottomNeighbor) &&
                     HasNotSameTileAssetThan(leftNeighbor) &&
                     HasNotSameTileAssetThan(rightNeighbor))
            {
                tileData.sprite = sprites[1];
            }
            //Has only neighbors on top or bottom ?
            else if ((HasSameTileAssetThan(topNeighbor) || HasSameTileAssetThan(bottomNeighbor)) &&
                     HasNotSameTileAssetThan(leftNeighbor) &&
                     HasNotSameTileAssetThan(rightNeighbor))
            {
                tileData.sprite = sprites[4];
            }
            else
            {
                //Defaults to top center tile
                tileData.sprite = sprites[1];
            }
        }

        private bool HasSameTileAssetThan(TileBase otherTile)
        {
            //There's only one instance of each Tile asset. Thus, if it's the same reference,
            //it's the same tile asset.
            return otherTile != null && this == otherTile;
        }

        private bool HasNotSameTileAssetThan(TileBase otherTile)
        {
            //There's only one instance of each Tile asset. Thus, if it's the same reference,
            //it's the same tile asset.
            return otherTile == null || this != otherTile;
        }
    }
}