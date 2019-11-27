using System.Linq;
using Harmony;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game
{
    /// <summary>
    /// Generates each level's grid
    /// Authors : Mike Bédard, Jérémie Bertrand
    /// </summary>
    public class GridGenerator : MonoBehaviour
    {
        [SerializeField] private Vector2Int size;
        [Header("Prefabs")] [SerializeField] private GameObject emptyCellPrefab = null;
        [SerializeField] private GameObject forestCellPrefab = null;
        [SerializeField] private GameObject obstacleCellPrefab = null;
        [SerializeField] private GameObject fortressCellPrefab = null;

        [Header("Tiles")] [SerializeField] private TileBase forestTile = null;
        [SerializeField] private TileBase[] obstacleTiles = null;
        [SerializeField] private TileBase fortressTile = null;

        [Header("TileMap")] [SerializeField] private Tilemap interactiveTilemap = null;
        [SerializeField] private Tilemap backgroundTilemap = null;
        [SerializeField] private Tilemap tilemapOfTileToIncludeIfEmptyTile;
        
        private void CreateGridCells()
        {
            var bounds = new BoundsInt(Vector3Int.zero,  new Vector3Int(size.x, size.y, 1));
            var allTiles = interactiveTilemap.GetTilesBlock(bounds);
            var emptyTiles = tilemapOfTileToIncludeIfEmptyTile.GetTilesBlock(bounds);
            var backgroundTiles = backgroundTilemap.GetTilesBlock(bounds);

            var canvas = transform.parent.GetComponent<Canvas>();
            if(canvas != null) canvas.GetComponent<RectTransform>().sizeDelta = size;
            
            for (var j = size.y - 1; j >= 0; j--)
            {
                for (var i = 0; i < size.x; i++)
                {
                
                    var currentTile = allTiles[i + j * size.x];
                    var createdGameObject = InstantiateCellPrefabFrom(currentTile);
                    if (currentTile == null)
                        currentTile = emptyTiles[i + j * size.x] == null ? backgroundTiles[i + j * size.x] : emptyTiles[i + j * size.x];
                    if (createdGameObject != null)
                        createdGameObject.AddComponent<TileSprite>().SetSprite(((UnityEngine.Tilemaps.Tile)currentTile).sprite);
                }
            }
        }


        private GameObject InstantiateCellPrefabFrom(TileBase tile)
        {
            var spawningPrefab = emptyCellPrefab;
            
            if (tile == forestTile)
            {
                spawningPrefab = forestCellPrefab;
            }
            else if (obstacleTiles.Contains(tile))
            {
                spawningPrefab = obstacleCellPrefab;
            }
            else if (tile == fortressTile)
            {
                spawningPrefab = fortressCellPrefab;
            }

            return Instantiate(spawningPrefab, transform);
        }

        public void ClearGrid()
        {
            for (var i = transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }
        
        public void GenerateGrid()
        {
            ClearGrid();
            CreateGridCells();
        }
    }
}
    
