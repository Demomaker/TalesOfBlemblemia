using Harmony;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game
{
    public class TilemapObjects
    {
        private GameObject tilemap;
        private GameObject[] gameObjects;

        public GameObject Tilemap => tilemap;
        public GameObject[] GameObjects => gameObjects;

        public TilemapObjects(GameObject tilemap, GameObject[] gameObjects)
        {
            this.tilemap = tilemap;
            this.gameObjects = gameObjects;
        }
    }
}