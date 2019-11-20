using System.Collections;
using Harmony;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

//Author: Pierre-Luc Maltais
namespace Game
{
    [Findable("UIController")]
    public class UIController : MonoBehaviour
    {
        private const string TURN_FORMAT_TEXT = "00";
        private const string TURN_DISPLAY_TEXT = "Turn ";
        private const string UNREACHABLE_TILE_TEXT = "\u221E";
        private const float TIME_TO_WAIT_BETWEEN_ANIMATIONS = 0.5f;
        private const float TIME_BEFORE_HIDING_BATTLE_REPORT_AUTO = 2f;
        
        [Header("Canvas")] [SerializeField] private Canvas canvas;
        [Header("TileInfo")] 
        [SerializeField] private TMP_Text tileType;
        [SerializeField] private TMP_Text tileDefense;
        [SerializeField] private TMP_Text tileMouvementEffect;
        [SerializeField] private Image tileTexture;

        [Header("Turn")] 
        [SerializeField] private TMP_Text turnCounter;
        [SerializeField] private TMP_Text turnInfo;
        [SerializeField] private Image playerTurnsLeftBackground;
        [SerializeField] private Image playerTurnsRightBackground;

        [Header("Victory Condition")] [SerializeField] private TMP_Text victoryCondition;

        [Header("Battle Report")] 
        [SerializeField] private GameObject battleReports;
        
        [SerializeField] private GameObject battleReportAlly;
        [FormerlySerializedAs("playerHealthBar")] 
        [SerializeField] private LayoutGroup playerHealthBarLayout;
        [SerializeField] private GameObject playerDeathSymbol;
        
        [SerializeField] private GameObject battleReportEnemy;
        [FormerlySerializedAs("enemyHealthBar")] 
        [SerializeField] private LayoutGroup enemyHealthBarLayout;
        [SerializeField] private GameObject enemyDeathSymbol;

        [Header("Colors")] 
        [SerializeField] private Color green;
        [SerializeField] private Color red;
        [SerializeField] private Color grey;

        private Animator playerAnimator;
        private Animator enemyAnimator;
        private static readonly int IS_ATTACKING = Animator.StringToHash("IsAttacking");

        private bool animationIsPlaying;
        private BattleInfos playerBattleInfos;
        private BattleInfos enemyBattleInfos;

        private GameObject[] playerHealthBar;
        private GameObject[] enemyHealthBar;


        public bool IsBattleReportActive => battleReports.activeSelf;
        
        private void Start()
        {
            playerHealthBar = playerHealthBarLayout.Children();
            enemyHealthBar = enemyHealthBarLayout.Children();
            playerBattleInfos = new BattleInfos();
            enemyBattleInfos = new BattleInfos();
            canvas.enabled = true;
            playerAnimator = battleReportAlly.GetComponent<Animator>();
            enemyAnimator = battleReportEnemy.GetComponent<Animator>();
            DeactivateBattleReport();
        }

        public void SetupCharactersBattleInfo(int maxHealthPoints, 
        int currentHealthPoint, int targetMaxHealthPoint, int targetCurrentHealthPoint, bool isEnemy)
        {
            if (isEnemy)
            {
                enemyBattleInfos.ChangeInfos(maxHealthPoints, currentHealthPoint);
                playerBattleInfos.ChangeInfos(targetMaxHealthPoint, targetCurrentHealthPoint);
            }
            else
            {
                playerBattleInfos.ChangeInfos(maxHealthPoints, currentHealthPoint);
                enemyBattleInfos.ChangeInfos(targetMaxHealthPoint, targetCurrentHealthPoint);
            }
        }
        
        private void ModifyHealthBar(int maxHealthPoints, int currentHealthPoint, GameObject[] healthBar, bool beforeBattle = true, int damage = 0)
        {
            for (int i = healthBar.Length; i > maxHealthPoints; i--)
            {
                healthBar[i - 1].SetActive(false);
            }
        
            for (int i = 0; i < currentHealthPoint; i++)
            {
                RawImage healthBarImage = healthBar[i].GetComponentInChildren<RawImage>();
                healthBarImage.color = green;
            }
            
            for (int i = maxHealthPoints; i > currentHealthPoint; i--)
            {
                RawImage healthBarImage = healthBar[i - 1].GetComponentInChildren<RawImage>();
                healthBarImage.color = grey;
            }

            if (beforeBattle == false)
            {
                for (int i = currentHealthPoint; i > currentHealthPoint - damage; i--)
                {
                    if (i > 0)
                    {
                        RawImage healthBarImage = healthBar[i - 1].GetComponentInChildren<RawImage>();
                        healthBarImage.color = red;
                    }
                    else
                        break;
                }
            }
        }

        public void ChangeCharacterDamageTaken(int dmg, bool isEnemy)
        {
            if (isEnemy)
            {
                enemyBattleInfos.DamageTaken = dmg;
            }
            else
            {
                playerBattleInfos.DamageTaken = dmg;
            }
        }

        public Coroutine LaunchBattleReport(bool isEnemy)
        {
            battleReports.SetActive(true);

            playerDeathSymbol.SetActive(false);
            enemyDeathSymbol.SetActive(false);

            Coroutine handle = StartCoroutine(BattleReport(isEnemy));

            return handle;
        }

        private IEnumerator BattleReport(bool isEnemy)
        {
            if (isEnemy)
            {
                yield return AttackAnimation(enemyAnimator, playerBattleInfos, playerHealthBar);
                if (playerBattleInfos.CurrentHealth - playerBattleInfos.DamageTaken > 0)
                {
                    yield return new WaitForSeconds(TIME_TO_WAIT_BETWEEN_ANIMATIONS);
                    yield return AttackAnimation(playerAnimator, enemyBattleInfos, enemyHealthBar);
                }
            }
            else
            {
                yield return AttackAnimation(playerAnimator, enemyBattleInfos, enemyHealthBar);
                if (enemyBattleInfos.CurrentHealth - enemyBattleInfos.DamageTaken > 0)
                {
                    yield return new WaitForSeconds(TIME_TO_WAIT_BETWEEN_ANIMATIONS);
                    yield return AttackAnimation(enemyAnimator, playerBattleInfos, playerHealthBar);
                }
            }

            if (enemyBattleInfos.CurrentHealth - enemyBattleInfos.DamageTaken <= 0)
            {
                enemyDeathSymbol.SetActive(true);
            }

            if (playerBattleInfos.CurrentHealth - playerBattleInfos.DamageTaken <= 0)
            {
                playerDeathSymbol.SetActive(true);
            }

            yield return new WaitForSeconds(TIME_BEFORE_HIDING_BATTLE_REPORT_AUTO);
            battleReports.SetActive(false);
        }

        private IEnumerator AttackAnimation(Animator animator, BattleInfos battleInfos, GameObject[] healthBar)
        {
            while (animationIsPlaying)
            {
                yield return null;
            }
            
            #region BeforeCombat
                ModifyHealthBar(playerBattleInfos.MaxHp, playerBattleInfos.CurrentHealth, playerHealthBar);
                ModifyHealthBar(enemyBattleInfos.MaxHp, enemyBattleInfos.CurrentHealth, enemyHealthBar);
            #endregion
            
            animationIsPlaying = true;
            animator.SetBool(IS_ATTACKING,true);
            yield return new WaitForSeconds(TIME_TO_WAIT_BETWEEN_ANIMATIONS);
            ModifyHealthBar(battleInfos.MaxHp, battleInfos.CurrentHealth, healthBar, false, battleInfos.DamageTaken);
            battleInfos.CurrentHealth -= battleInfos.DamageTaken;
            yield return new WaitForSeconds(TIME_TO_WAIT_BETWEEN_ANIMATIONS);
            animator.SetBool(IS_ATTACKING,false);
            animationIsPlaying = false;
        }

        [UsedImplicitly]
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
        }

        public void ModifyTurnCounter(int turns)
        {
            turnCounter.text = TURN_DISPLAY_TEXT + turns.ToString(TURN_FORMAT_TEXT);
        }

        public void ModifyTurnInfo(string player)
        {
            turnInfo.text = player;
            if (player == "Player")
            {
                playerTurnsLeftBackground.color = green;
                playerTurnsRightBackground.color = green;
            }
            else
            {
                playerTurnsLeftBackground.color = red;
                playerTurnsRightBackground.color = red;
            }
        }

        public void ModifyVictoryCondition(string victoryCondition)
        {
            this.victoryCondition.text = victoryCondition;
        }
    }
}