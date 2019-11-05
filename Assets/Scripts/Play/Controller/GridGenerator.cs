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
        [SerializeField] private Tilemap tilemapOfTileToIncludeIfEmptyTile;

        public void CreateGridCells()
        {
            BoundsInt bounds = backgroundTilemap.cellBounds;
            TileBase[] allTiles = interactiveTilemap.GetTilesBlock(bounds);
            TileBase[] emptyTiles = tilemapOfTileToIncludeIfEmptyTile.GetTilesBlock(bounds);
            TileBase[] backgroundTiles = backgroundTilemap.GetTilesBlock(bounds);
            Rect cellGridRectangle = GetComponent<RectTransform>().rect;

            var minX = GetMinX(bounds, cellGridRectangle);
            var minY = GetMinY(bounds, cellGridRectangle);
            var maxX = GetMaxX(bounds, cellGridRectangle);
            var maxY = GetMaxY(bounds, cellGridRectangle);

            GetComponent<RectTransform>().sizeDelta = new Vector2(maxX - minX, maxY - minY);

            for (var y = maxY - 1; y >= minY; y--)
            {
                for (var x = minX; x < maxX; x++)
                {
                    TileBase currentTile = allTiles[(int)(x + y * bounds.size.x)];
                    GameObject createdGameObject = InstantiateCellPrefabFrom(currentTile);
                    if (currentTile == null)
                        currentTile = emptyTiles[(int)(x + y * bounds.size.x)];
                    if (currentTile == null)
                        currentTile = backgroundTiles[(int)(x + y * bounds.size.x)];
                    if (createdGameObject != null)
                        createdGameObject.AddComponent<TileSprite>().SetSprite(((UnityEngine.Tilemaps.Tile)currentTile).sprite);
                }
            }
        }

        private float GetMinX(BoundsInt bounds, Rect cellGridRectangle)
        {
            var minX =  ( bounds.size.x - cellGridRectangle.width) / 2;
            if (minX < 0) minX = 0;
            return minX;
        }

        private float GetMinY(BoundsInt bounds, Rect cellGridRectangle)
        {
            var minY = ( bounds.size.y - cellGridRectangle.height) / 2;
            if (minY < 0) minY = 0;
            return minY;
        }

        private float GetMaxX(BoundsInt bounds, Rect cellGridRectangle)
        {
            var maxX = bounds.size.x - GetMinX(bounds, cellGridRectangle);
            if (maxX > bounds.size.x) maxX = bounds.size.x;
            return maxX;
        }

        private float GetMaxY(BoundsInt bounds, Rect cellGridRectangle)
        {
            var maxY = bounds.size.y - GetMinY(bounds, cellGridRectangle);
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
    
