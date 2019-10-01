﻿using System;
 using UnityEngine;
using UnityEngine.UI;

 namespace Game
 {
    //Authors: Jérémie Bertrand & Mike Bédard
    public class GridController : MonoBehaviour
    {
        private GridLayoutGroup gridLayoutGroup;
        [SerializeField] private Sprite movementTileSprite = null;
        [SerializeField] private Sprite normalTileSprite = null;
        [SerializeField] private Sprite selectedTileSprite = null;
        [SerializeField] private Sprite attackableTileSprite = null;

        public Character SelectedCharacter { get; private set; } = null;
        public Sprite AvailabilitySprite => movementTileSprite;
        public Sprite NormalSprite => normalTileSprite;
        public Sprite SelectedSprite => selectedTileSprite;
        public Sprite AttackableTileSprite => attackableTileSprite;

        public bool ACharacterIsCurrentlySelected => SelectedCharacter != null;

        public int NbColumns { get; private set; } = 0;
        public int NbLines { get; private set; } = 0;
        
        private void Awake()
        {
            gridLayoutGroup = GetComponent<GridLayoutGroup>();
            NbColumns = (int)(GetComponent<RectTransform>().rect.width / gridLayoutGroup.cellSize.x);
            NbLines = (int)(GetComponent<RectTransform>().rect.height / gridLayoutGroup.cellSize.y);
        }

        public void SelectCharacter(Character character)
        {
            if(SelectedCharacter != null) DeselectCharacter();
            SelectedCharacter = character;
        }

        public void DeselectCharacter()
        {
            SelectedCharacter = null;
            foreach (Transform child in transform)
            {
                child.GetComponent<Tile>().HideActionPossibility();
            }
        }
        
        public void DisplayPossibleActionsFrom(Tile fromTile)
        {
            fromTile.DisplaySelectedTile();
            var linkedCharacter = fromTile.LinkedCharacter;
            for (int i = -linkedCharacter.MovementRange; i <= linkedCharacter.MovementRange; i++)
            {
                for(int j = -linkedCharacter.MovementRange; j <= linkedCharacter.MovementRange ; j++)
                {
                    if (i != 0 || j != 0)
                    {
                        Vector2Int position = fromTile.LogicalPosition + new Vector2Int(i, j);
                        if (IsValidGridPosition(position.x, position.y))
                        {
                            int distance = Math.Abs(i) + Math.Abs(j);
                            Tile tile = GetTile(position.x, position.y);
                            if (distance <= linkedCharacter.MovementRange && tile.IsAvailable)
                            {
                                tile.DisplayMoveActionPossibility();
                            }
                            else if (distance <= linkedCharacter.AttackRange && tile.LinkedCharacter is Enemy)
                            {
                                tile.DisplayAttackActionPossibility();
                            }
                        }
                    }
                }
            }
        }

        public Tile GetTile(int x, int y)
        {
            return transform.GetChild(x + y * NbColumns).GetComponent<Tile>();
        }
        
        public bool IsValidGridPosition(int x, int y)
        {
            return x >= 0 && y >= 0 && x < NbColumns && y < NbLines;
        }
    }
 }
