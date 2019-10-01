﻿using System.Collections;
using Game;
using UnityEngine;

//Authors: Jérémie Bertrand & Mike Bédard
public abstract class Unit : MonoBehaviour
{
    [SerializeField] private Vector2Int initialPosition;
    private GridController gridController;
    private Tile currentTile = null;
    private bool canPlay = false;
    public bool CanPlay
    {
        get => canPlay;
        set => canPlay = value;
    }
    public bool IsCurrentlySelected => gridController.SelectedUnit == this;

    /// <summary>
    /// Value representing if a unit is an enemy 
    /// </summary>
    private bool isEnemy;
    public bool IsEnemy => isEnemy;
    /// <summary>
    /// The unit's class stats
    /// </summary>
    [SerializeField]
    private UnitStats classStats;
    /// <summary>
    /// The unit's weapon
    /// </summary>
    [SerializeField]
    private Weapon weapon;
    /// <summary>
    /// Array representing the movement cost needed to move to every tile on the grid
    /// </summary>
    private int[,] movementCosts;
    public int[,] MovementCosts => movementCosts;
    /// <summary>
    /// The unit's current health
    /// </summary>
    private int currentHealthPoints;
    public int CurrentHealthPoints => currentHealthPoints;
    /// <summary>
    /// The unit's stats
    /// </summary>
    public UnitStats UnitStats
    {
    get { return classStats + weapon.WeaponStats; }
    }
    /// <summary>
    /// The unit's weapon type
    /// </summary>
    public WeaponType WeaponType
    {
    get { return weapon.WeaponType; }
    }
    /// <summary>
    /// The weapon type this unit has advantage on 
    /// </summary>
    public WeaponType WeaponAdvantage
    {
    get { return weapon.Advantage; }
    }
    /// <summary>
    /// The health points a unit would gain by resting
    /// Resting replenishes half your health points without exceeding the unit's max health
    /// </summary>
    public int HpGainedByResting
    {
    get
    {
        int maxGain = UnitStats.MaxHealthPoints / 2;
        if (currentHealthPoints + maxGain > UnitStats.MaxHealthPoints)
            return UnitStats.MaxHealthPoints - currentHealthPoints;
        return maxGain;
    }
    }

    private int movesLeft = 0;
    public int MovesLeft => movesLeft;
    public bool CanMove => MovesLeft > 0;
    public bool IsDead => currentHealthPoints <= 0;
    public int MovementRange => UnitStats.MoveSpeed;
    public int AttackRange => 1;
    
    
    protected Unit()
    {
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
        movesLeft = MovementRange;
    }

    private void MoveTo(Vector3 position)
    {
        transform.position = position;
    }

    public void MoveTo(Tile tile)
    {
        movesLeft -= 1;
        if (currentTile != null) currentTile.UnlinkCharacter();
        currentTile = tile;
        if(currentTile != null && currentTile.LinkCharacter(this)) MoveTo(currentTile.WorldPosition);
    }
    
    private IEnumerator InitPosition()
    {
        yield return new WaitForEndOfFrame();
        movesLeft += 1;
        MoveTo(Finder.GridController.GetTile(initialPosition.x, initialPosition.y));
    }

    public void Attack(Unit unit)
    {
        movesLeft -= 1;
        unit.currentHealthPoints -= 2;
    }

    public void Die()
    {
        Destroy(gameObject);
    }


}
