using System;
using Harmony;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Game
{
    [Findable("UIController")]
    public class UIController : MonoBehaviour
    {
        [Header("TileInfo")] 
        [SerializeField] private TMP_Text tileType;
        [SerializeField] private TMP_Text tileDefense;
        [SerializeField] private TMP_Text tileMouvementEffect;
        [SerializeField] private Image tileTexture;

        [Header("Turn")] [SerializeField] private TMP_Text turnCounter;

        [Header("Victory Condition")] [SerializeField] private TMP_Text victoryCondition;
        
        [FormerlySerializedAs("nameCharacter")]
        [Header("CharacterInfos")]
        [SerializeField] private TMP_Text characterName;
        [SerializeField] private TMP_Text characterClass;
        [SerializeField] private TMP_Text weapon;
        [SerializeField] private TMP_Text mouvement;
        [SerializeField] private TMP_Text atk;
        [SerializeField] private TMP_Text hp;
    
        private Canvas canvas;
        
        //todo load the ui texture.

        private void Awake()
        {
            canvas = GetComponent<Canvas>();
        }

        private void Start()
        {
            canvas.enabled = true;
        }

        public void ModifyUI(Tile tile)
        {
            tileType.text = tile.TileType.ToString();
            tileDefense.text = (tile.DefenseRate * 100).ToString();
            tileMouvementEffect.text = tile.CostToMove.ToString();
            //todo change the texture;

            if (tile.LinkedUnit != null)
            {
                //characterName = tile.LinkedUnit.
                //characterClass = tile.LinkedUnit.
            }
        }
    }
}