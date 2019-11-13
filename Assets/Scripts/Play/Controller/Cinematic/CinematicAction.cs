using System;
using Game;
using UnityEngine;
using static Game.Constants;

[Serializable]
public class CinematicAction
{
    [SerializeField] private CinematicActionType cinematicActionType;
    
    [SerializeField] private Transform cameraTarget;
    [Range(MIN_CINEMATIC_TIME, MAX_CINEMATIC_TIME)][SerializeField] private float duration;
    [Range(MIN_CAM_ORTHOGRAPHIC_SIZE, MAX_CAM_ORTHOGRAPHIC_SIZE)][SerializeField] private float cameraZoom;
    
    [SerializeField] private string sentence;
    [SerializeField] private string name;
    
    [SerializeField] private Targetable targetUnit;
    [SerializeField] private int damage;
    
    [SerializeField] private Transform gameObjectToMove;
    [SerializeField] private Transform gameObjectTarget;
    [SerializeField] private bool cameraFollow;
    
    public Transform CameraTarget => cameraTarget;
    public float Duration => duration;
    public float CameraZoom => cameraZoom;
    public CinematicActionType CinematicActionType => cinematicActionType;
    public Dialogue Dialogue => new Dialogue(new Quote(name, sentence));
    public Targetable TargetUnit => targetUnit;
    public int Damage => damage;
    public Transform GameObjectToMove => gameObjectToMove;
    public Transform GameObjectTarget => gameObjectTarget;
    public bool CameraFollow => cameraFollow;
}
public enum CinematicActionType
{
    Quote,
    CameraMovement,
    GameObjectMovement,
    Damage
}
