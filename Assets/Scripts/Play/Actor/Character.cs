﻿using System.Collections;
using Game;
using UnityEngine;

//Authors: Jérémie Bertrand & Mike Bédard
public abstract class Character : MonoBehaviour
{
    [SerializeField] private Vector2Int initialPosition;
    private GridController gridController;
    private Tile currentTile = null;
    private readonly int movementRange;
    private readonly int attackRange;
    private int healthPoints = Constants.DEFAULT_CHARACTER_HEALTH_POINTS;
    private int movesLeft = Constants.NUMBER_OF_MOVES_PER_CHARACTER_PER_TURN;
    private bool canPlay = false;
    public int MovesLeft
    {
        get => movesLeft;
        set => movesLeft = value;
    }
    public bool CanPlay
    {
        get => canPlay;
        set => canPlay = value;
    }
    public bool IsCurrentlySelected => gridController.SelectedCharacter == this;
    public bool CanMove => MovesLeft > 0;
    public bool IsDead => healthPoints <= 0;
    public int MovementRange => movementRange;
    public int AttackRange => attackRange;

    protected Character(int movementRange, int attackRange)
    {
        this.movementRange = movementRange;
        this.attackRange = attackRange;
    }
    private void Awake()
    {
        gridController = Finder.GridController;
    }

    protected void Start()
    {
        StartCoroutine(InitPosition());
    }
    
    public void ResetNumberOfMovesLeft()
    {
        MovesLeft = Constants.NUMBER_OF_MOVES_PER_CHARACTER_PER_TURN;
    }

    private void MoveTo(Vector3 position)
    {
        transform.position = position;
    }

    public void MoveTo(Tile tile)
    {
        MovesLeft -= 1;
        if (currentTile != null) currentTile.UnlinkCharacter();
        currentTile = tile;
        if(currentTile != null && currentTile.LinkCharacter(this)) MoveTo(currentTile.WorldPosition);
    }
    

    private IEnumerator InitPosition()
    {
        yield return new WaitForEndOfFrame();
        MovesLeft += 1;
        MoveTo(Finder.GridController.GetTile(initialPosition.x, initialPosition.y));
    }

    public void Attack(Character character)
    {
        MovesLeft -= 1;
        character.healthPoints -= 2;
    }

    public void Die()
    {
        Destroy(gameObject);
    }


}
