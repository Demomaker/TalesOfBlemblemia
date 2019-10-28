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

        [Header("Texture")] 
        [SerializeField] private Sprite grassTexture;
        [SerializeField] private Sprite mountainTexture;
        [SerializeField] private Sprite fortressTexture;
        [SerializeField] private Sprite forestTexture;

        [Header("Battle Report")]
        [SerializeField] private TMP_Text dmgGiven;
        [SerializeField] private TMP_Text allyStatus;
        [SerializeField] private TMP_Text dmgTaken;
        [SerializeField] private TMP_Text enemyStatus;
    
        private Canvas canvas;
        private GameObject battleReport;

        //todo load the ui texture.

        private void Awake()
        {
            canvas = GetComponent<Canvas>();
            battleReport = GameObject.FindWithTag("BattleReport");
        }

        private void Start()
        {
            canvas.enabled = true;
            battleReport.SetActive(false);
        }

        public void PrepareBattleReport(int dmg, bool status, bool isEnemy)
        {
            if (isEnemy)
            {
                dmgTaken.text = dmg.ToString();
                allyStatus.text = status ? "Alive" : "Dead";
            }
            else
            {
                dmgGiven.text = dmg.ToString();
                enemyStatus.text = status ? "Alive" : "Dead";
            }
        }
        
        public void LaunchBattleReport()
        {
            battleReport.SetActive(true);
        }

        public void DeactivateBattleReport()
        {
            battleReport.SetActive(false);
        }

        public void ModifyPlayerUI(Tile tile)
        {
            tileType.text = tile.TileType.ToString();
            tileDefense.text = (tile.DefenseRate * 100) + "%";
            tileMouvementEffect.text = tile.CostToMove.ToString();
            if (tileMouvementEffect.text == int.MaxValue.ToString()) tileMouvementEffect.text = "Unreachable";

            if (tileType.text == "Obstacle")
            {
                tileTexture.sprite = mountainTexture;
            }
            else if (tileType.text == "Forest")
            {
                tileTexture.sprite = forestTexture;
            }
            else if (tileType.text == "Fortress")
            {
                tileTexture.sprite = fortressTexture;
            }
            else if (tileType.text == "Empty")
            {
                tileTexture.sprite = grassTexture;
            }
            

            if (tile.LinkedUnit != null && tile.LinkedUnit.UnitInfos != null)
            {
                characterName.text = tile.LinkedUnit.UnitInfos.characterName;
                characterClass.text = tile.LinkedUnit.UnitInfos.className;
                weapon.text = tile.LinkedUnit.UnitInfos.weaponName;
                mouvement.text = tile.LinkedUnit.MovementRange.ToString();
                atk.text = tile.LinkedUnit.Stats.AttackStrength.ToString();
                hp.text = tile.LinkedUnit.CurrentHealthPoints.ToString();
            }
        }

        public void ModifyTurnCounter(int turns)
        {
            turnCounter.text = turns.ToString(("00"));
        }

        public void ModifyVictoryCondition(string victoryCondition)
        {
            this.victoryCondition.text = victoryCondition;
        }
    }
}