using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioClips : MonoBehaviour
{
    [SerializeField] private AudioClip maleAttackSound;
    [SerializeField] private AudioClip femaleAttackSound;
    [SerializeField] private AudioClip morkAttackSound;
    [SerializeField] private AudioClip hurtSound;
    [SerializeField] private AudioClip dodgeSound;
    [SerializeField] private AudioClip unitMoveSound;
    [SerializeField] private AudioClip unitDeathSound;
    [SerializeField] private AudioClip buttonClickSound;
    [SerializeField] private AudioClip overworldMusic;
    [SerializeField] private AudioClip forestMusic;
    [SerializeField] private AudioClip castleMusic;
    [SerializeField] private AudioClip bossMusic;
    [SerializeField] private AudioClip sadMusic;
    [SerializeField] private AudioClip levelVictoryMusic;
    [SerializeField] private AudioClip mainMenuMusic;
    
    public AudioClip MaleAttackSound => maleAttackSound;

    public AudioClip FemaleAttackSound => femaleAttackSound;

    public AudioClip MorkAttackSound => morkAttackSound;

    public AudioClip HurtSound => hurtSound;
    public AudioClip DodgeSound => dodgeSound;
    public AudioClip UnitMoveSound => unitMoveSound;
    public AudioClip UnitDeathSound => unitDeathSound;

    public AudioClip ButtonClickSound => buttonClickSound;
    
    public AudioClip OverworldMusic => overworldMusic;

    public AudioClip ForestMusic => forestMusic;

    public AudioClip CastleMusic => castleMusic;

    public AudioClip BossMusic => bossMusic;

    public AudioClip SadMusic => sadMusic;

    public AudioClip LevelVictoryMusic => levelVictoryMusic;

    public AudioClip MainMenuMusic => mainMenuMusic;
}
