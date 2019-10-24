using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        for (int i = 0; i < Finder.GameController.LevelsCompleted.Count; i++)
        {
            Debug.Log("Level completed : " + Finder.GameController.LevelsCompleted.ElementAt(i));
        }
    }

}
