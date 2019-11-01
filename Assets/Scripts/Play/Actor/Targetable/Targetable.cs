using System.Collections;
using UnityEngine;

namespace Game
{
    public abstract class Targetable : MonoBehaviour
    {
        [SerializeField] private Vector2Int initialPosition;
        
        protected Tile currentTile;
        private int currentHealthPoints;
        
        public bool NoHealthLeft => CurrentHealthPoints <= 0;
        
        public int CurrentHealthPoints
        {
            get { return currentHealthPoints; }
            protected internal set
            {
                currentHealthPoints = value;
                if (NoHealthLeft)
                {
                    Die();
                }
            }
        }
        
        public Tile CurrentTile
        {
            get => currentTile;
            set
            {
                if (currentTile != null) currentTile.UnlinkUnit();
                currentTile = value;
                if (value != null) value.LinkTargetable(this);
                Harmony.Finder.LevelController.IncrementTileUpdate();
            }
        }

        public virtual void Die()
        {
            currentTile.UnlinkUnit();
            gameObject.SetActive(false);
        }
        
        protected virtual void Start()
        {
            StartCoroutine(InitPosition());
        }
        
        private IEnumerator InitPosition()
        {
            yield return new WaitForEndOfFrame();
            var tile = Finder.GridController.GetTile(initialPosition.x, initialPosition.y);
            transform.position = tile.WorldPosition;
            CurrentTile = tile;
        }
    }
}