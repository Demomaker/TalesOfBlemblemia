using System;
using System.Collections;
using UnityEngine;

namespace Game
{
    public abstract class Targetable : MonoBehaviour
    {
        [SerializeField] private Vector2Int initialPosition;
        [SerializeField] private bool isEnemyTarget;

        protected Tile currentTile;
        private int currentHealthPoints;
        private OverHeadHpController overHeadHpController;
        private CinematicController cinematicController;

        public bool IsEnemyTarget => isEnemyTarget;
        public bool NoHealthLeft => CurrentHealthPoints <= 0;
        
        public int CurrentHealthPoints
        {
            get => currentHealthPoints;
            protected internal set
            {
                currentHealthPoints = value;
                if(overHeadHpController != null) overHeadHpController.ModifyOverHeadHp(currentHealthPoints);
                if (NoHealthLeft) 
                    Die();
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

        public virtual void Awake()
        {
            cinematicController = Harmony.Finder.LevelController.CinematicController;
            try
            {
                overHeadHpController = gameObject.GetComponent<OverHeadHpController>();
            }
            catch (Exception e)
            {
                Debug.Log("The gameobject doesn't have a overheadHp object and it requires it");
            }
        }

        protected virtual void Start()
        {
            StartCoroutine(InitPosition());
        }
        
        private IEnumerator InitPosition()
        {
            yield return new WaitForEndOfFrame();
            while (cinematicController.IsPlayingACinematic)
                yield return null;
            var tile = Finder.GridController.GetTile(initialPosition.x, initialPosition.y);
            if(transform != null && tile != null)
            {
                transform.position = tile.WorldPosition;
            }

            CurrentTile = tile;
        }
    }
}