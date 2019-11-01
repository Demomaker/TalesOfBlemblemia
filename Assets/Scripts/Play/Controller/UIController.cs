using System;
using System.Collections;
using System.Threading.Tasks;
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

        [Header("Canvas")] [SerializeField] private Canvas canvas;
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
        [SerializeField] private GameObject battleReports;
        
        [SerializeField] private GameObject battleReportAlly;
        [SerializeField] private LayoutGroup playerHealthBar;
        [SerializeField] private GameObject playerDeathSymbol;
        
        [SerializeField] private GameObject battleReportEnemy;
        [SerializeField] private LayoutGroup enemyHealthBar;
        [SerializeField] private GameObject enemyDeathSymbol;

        [Header("Colors")] 
        [SerializeField] private Color green;
        [SerializeField] private Color red;
        [SerializeField] private Color grey;


        private const string UNREACHABLE_TILE_TEXT = "Unreachable";

        private Animator playerAnimator;
        private Animator enemyAnimator;
        private static readonly int IS_ATTACKING = Animator.StringToHash("IsAttacking");

        private bool animationIsplaying = false;

        public bool IsBattleReportActive => battleReports.activeSelf;

        private void Start()
        {
            canvas.enabled = true;
            playerAnimator = battleReportAlly.GetComponent<Animator>();
            enemyAnimator = battleReportEnemy.GetComponent<Animator>();
            DeactivateBattleReport();
        }

        public void PrepareBattleReport(int maxHealthPoints, 
        int currentHealthPoint, int targetMaxHealthPoint, int targetCurrentHealthPoint, bool isEnemy)
        {
            if (isEnemy)
            {
                EnemyHealthBarSetup(maxHealthPoints, currentHealthPoint, grey);
                PlayerHealthBarSetup(targetMaxHealthPoint, targetCurrentHealthPoint, grey);
            }
            else
            {
                PlayerHealthBarSetup(maxHealthPoints, currentHealthPoint,grey);
                EnemyHealthBarSetup(targetMaxHealthPoint, targetCurrentHealthPoint, grey);
            }
        }

        private void PlayerHealthBarSetup(int maxHealthPoints, int currentHealthPoint, Color color)
        {
            GameObject[] healthBar = playerHealthBar.Children();
            for (int i = healthBar.Length; i > maxHealthPoints; i--)
            {
                healthBar[i - 1].SetActive(false);
            }

            for (int i = maxHealthPoints; i > currentHealthPoint; i--)
            {
                RawImage healthBarImage = healthBar[i - 1].GetComponentInChildren<RawImage>();
                healthBarImage.color = color;
            }
        }

        private void EnemyHealthBarSetup(int maxHealthPoints, int currentHealthPoint, Color color)
        {
            GameObject[] healthBar = enemyHealthBar.Children();
            for (int i = healthBar.Length; i > maxHealthPoints; i--)
            {
                healthBar[i - 1].SetActive(false);
            }

            for (int i = maxHealthPoints; i > currentHealthPoint; i--)
            {
                RawImage healthBarImage = healthBar[i - 1].GetComponentInChildren<RawImage>();
                healthBarImage.color = color;
            }
        }

        public Coroutine LaunchBattleReport(bool isEnemy, int maxHealthPoint, int currentHealthPoint)
        {
            Coroutine battleReportHandle;
            
            battleReports.SetActive(true);
            if (isEnemy)
            {
                battleReportHandle = StartCoroutine(EnemyAttackAnimation(maxHealthPoint, currentHealthPoint));
            }
            else
            {
                battleReportHandle = StartCoroutine(PlayerAttackAnimation(maxHealthPoint,currentHealthPoint));
            }

            playerDeathSymbol.SetActive(false);
            enemyDeathSymbol.SetActive(false);
            
            if (currentHealthPoint <= 0)
            {
                if (isEnemy) playerDeathSymbol.SetActive(true);
                else enemyDeathSymbol.SetActive(true);
            }
            
            return battleReportHandle;
        }

        private IEnumerator EnemyAttackAnimation(int maxHealthPoint, int currentHealthPoint)
        {
            while (animationIsplaying)
            {
                yield return null;
            }
            animationIsplaying = true;
            enemyAnimator.SetBool(IS_ATTACKING, true);
            yield return new WaitForSeconds(0.5f);
            EnemyHealthBarSetup(maxHealthPoint, currentHealthPoint, red);
            yield return  new WaitForSeconds(0.5f);
            enemyAnimator.SetBool(IS_ATTACKING, false);
            yield return new WaitForSeconds(0.5f);
            animationIsplaying = false;
        }

        private IEnumerator PlayerAttackAnimation(int maxHealthPoint, int currentHealthPoint)
        {
            while (animationIsplaying)
            {
                yield return null;
            } 
            animationIsplaying = true;
            playerAnimator.SetBool(IS_ATTACKING,true);
            yield return new WaitForSeconds(0.5f);
            EnemyHealthBarSetup(maxHealthPoint, currentHealthPoint, red);
            yield return new WaitForSeconds(0.5f);
            playerAnimator.SetBool(IS_ATTACKING,false);
            yield return new WaitForSeconds(0.5f);
            animationIsplaying = false;
        }

        private void LaunchEnemyAttackAnimation()
        {
            enemyAnimator.SetBool(IS_ATTACKING,false);
        }

        private void StopPlayerAttackAnimation()
        {
            playerAnimator.SetBool(IS_ATTACKING, false);
        }

        public void DeactivateBattleReport()
        {
            battleReports.SetActive(false);
        }

        public void ModifyPlayerUi(Tile tile)
        {
            tileType.text = tile.TileType.ToString();
            tileDefense.text = (tile.DefenseRate * 100) + "%";
            tileMouvementEffect.text = tile.CostToMove.ToString();
            if (tileMouvementEffect.text == int.MaxValue.ToString()) tileMouvementEffect.text = UNREACHABLE_TILE_TEXT;

            tileTexture.sprite = tile.GetSprite();
            

            if (tile.LinkedUnit != null && tile.LinkedUnit.UnitInfos != null)
            {
                characterName.text = tile.LinkedUnit.UnitInfos.characterName;
                characterClass.text = tile.LinkedUnit.UnitInfos.className;
                weapon.text = tile.LinkedUnit.UnitInfos.weaponName;
                mouvement.text = tile.LinkedUnit.MovesLeft.ToString();
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