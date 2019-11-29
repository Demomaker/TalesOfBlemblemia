using System.Collections;
using UnityEngine;

namespace Game
{
    //Author: Jérémie Bertrand, Zacharie Lavigne, Pierre-Luc Maltais
    public abstract class Targetable : MonoBehaviour
    {
        [SerializeField] private Vector2Int initialPosition;
        [SerializeField] private bool isEnemyTarget;

        protected Tile currentTile;
        private int currentHealthPoints;
        protected CoroutineStarter coroutineStarter;
        protected LevelController levelController;

        public bool IsEnemyTarget => isEnemyTarget;
        public bool NoHealthLeft => CurrentHealthPoints <= 0;
        
        public int CurrentHealthPoints
        {
            get => currentHealthPoints;
            protected internal set
            {
                currentHealthPoints = value;
                if (NoHealthLeft) 
                    coroutineStarter.StartCoroutine(Die());
                if(this is Unit) (this as Unit)?.OnHealthChange.Publish();
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
                levelController.IncrementTileUpdate();
            }
        }

        
        public virtual void Awake()
        {
            coroutineStarter = Harmony.Finder.CoroutineStarter;
            levelController = Harmony.Finder.LevelController;
        }
        
        protected virtual void Start()
        {
            coroutineStarter.StartCoroutine(InitPosition());
        }

        protected virtual IEnumerator Die()
        {
            currentTile.UnlinkUnit();
            gameObject.SetActive(false);
            yield return null;
        }
        
        private IEnumerator InitPosition()
        {
            yield return new WaitForEndOfFrame();
            while (levelController.CinematicController.IsPlayingACinematic)
                yield return null;
            var tile = Harmony.Finder.GridController.GetTile(initialPosition.x, initialPosition.y);
            transform.position = tile.WorldPosition;
            CurrentTile = tile;
        }
    }
}