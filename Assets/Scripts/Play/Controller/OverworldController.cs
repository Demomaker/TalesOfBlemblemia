using System.Collections;
using System.Collections.Generic;
using Harmony;
using UnityEngine;
using UnityEngine.SceneManagement;
using Finder = Game.Finder;

public class OverworldController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Finder.SoundManager.StopCurrentMusic();
        Finder.SoundManager.PlayMusic(Finder.SoundClips.OverworldMusic);
    }

}
