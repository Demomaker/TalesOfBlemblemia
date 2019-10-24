using System;
using System.Collections;
using System.Linq;
using Boo.Lang;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

namespace Game
{
    public class GridGenerator : MonoBehaviour
    {
        [Header("Prefabs")] [SerializeField] private GameObject emptyCellPrefab = null;
        [SerializeField] private GameObject forestCellPrefab = null;
        [SerializeField] private GameObject obstacleCellPrefab = null;
        [SerializeField] private GameObject fortressCellPrefab = null;

        [Header("Tiles")] [SerializeField] private TileBase forestTile = null;
        [SerializeField] private TileBase[] obstacleTiles = null;
        [SerializeField] private TileBase fortressTile = null;

        [Header("Tilemap")] [SerializeField] private Tilemap interactiveTilemap = null;
        [SerializeField] private Tilemap backgroundTilemap = null;
        [SerializeField] private Tilemap tilemapOfTilesToIncludeIfEmptyTile;

        public void CreateGridCells()
        {
            BoundsInt bounds = backgroundTilemap.cellBounds;
            TileBase[] allTiles = interactiveTilemap.GetTilesBlock(bounds);
            TileBase[] emptyTiles = tilemapOfTilesToIncludeIfEmptyTile.GetTilesBlock(bounds);
            TileBase[] backgroundTiles = backgroundTilemap.GetTilesBlock(bounds);
            Rect cellGridRectangle = GetComponent<RectTransform>().rect;

            int minX = GetMinX(bounds, cellGridRectangle);
            int minY = GetMinY(bounds, cellGridRectangle);
            int maxX = GetMaxX(bounds, cellGridRectangle);
            int maxY = GetMaxY(bounds, cellGridRectangle);

            GetComponent<RectTransform>().sizeDelta = new Vector2(maxX - minX, maxY - minY);

            for (int y = maxY - 1; y >= minY; y--)
            {
                for (int x = minX; x < maxX; x++)
                {
                    TileBase currentTile = allTiles[x + y * bounds.size.x];
                    GameObject createdGameObject = InstantiateCellPrefabFrom(currentTile);
                    if (currentTile == null)
                        currentTile = emptyTiles[x + y * bounds.size.x];
                    if (currentTile == null)
                        currentTile = backgroundTiles[x + y * bounds.size.x];
                    if (createdGameObject != null)
                        createdGameObject.AddComponent<TileSprite>().SetSprite(((UnityEngine.Tilemaps.Tile)currentTile).sprite);
                }
            }
        }

        private int GetMinX(BoundsInt bounds, Rect cellGridRectangle)
        {
            int minX = ((int) bounds.size.x - (int) cellGridRectangle.width) / 2;
            if (minX < 0) minX = 0;
            return minX;
        }

        private int GetMinY(BoundsInt bounds, Rect cellGridRectangle)
        {
            int minY = ((int) bounds.size.y - (int) cellGridRectangle.height) / 2;
            if (minY < 0) minY = 0;
            return minY;
        }

        private int GetMaxX(BoundsInt bounds, Rect cellGridRectangle)
        {
            int maxX = bounds.size.x - GetMinX(bounds, cellGridRectangle);
            if (maxX > bounds.size.x) maxX = bounds.size.x;
            return maxX;
        }

        private int GetMaxY(BoundsInt bounds, Rect cellGridRectangle)
        {
            int maxY = bounds.size.y - GetMinY(bounds, cellGridRectangle);
            if (maxY > bounds.size.y) maxY = bounds.size.y;
            return maxY;
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
            for (int i = transform.childCount - 1; i >= 0; i--)
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
    
