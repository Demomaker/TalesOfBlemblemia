using System;
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
        private OverHeadHpController overHeadHpController;

        private LevelController levelController;

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
                    //TODO objet Coroutine Starter
                    Harmony.Finder.LevelController.StartCoroutine(Die());
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

        public virtual IEnumerator Die()
        {
            currentTile.UnlinkUnit();
            gameObject.SetActive(false);
            yield break;
        }

        public virtual void Awake()
        {
            levelController = Harmony.Finder.LevelController;
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
            while (levelController.CinematicController.IsPlayingACinematic)
                yield return null;
            var tile = Finder.GridController.GetTile(initialPosition.x, initialPosition.y);
            transform.position = tile.WorldPosition;
            CurrentTile = tile;
        }
    }
}