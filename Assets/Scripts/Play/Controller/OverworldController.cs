using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game;
using Harmony;
using UnityEngine;
using UnityEngine.SceneManagement;
using Finder = Game.Finder;

public class OverworldController : MonoBehaviour
{
    // Start is called before the first frame update
    private OnOverworldEnter onOverworldEnter;
    void Start()
    {
        onOverworldEnter = new OnOverworldEnter();
        onOverworldEnter.Publish(this);
    }

}
