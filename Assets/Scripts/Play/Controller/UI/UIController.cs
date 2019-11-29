using System;
using System.Collections;
using Harmony;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

//Author: Pierre-Luc Maltais, Antoine Lessard
namespace Game
{
    [Findable("UIController")]
    public class UIController : MonoBehaviour
    {
        private const string TURN_FORMAT_TEXT = "00";
        private const string TURN_DISPLAY_TEXT = "Turn ";
        private const string UNREACHABLE_TILE_TEXT = "X";
        private const float TIME_TO_WAIT_BETWEEN_ANIMATIONS = 0.5f;
        private const float TIME_BEFORE_HIDING_BATTLE_REPORT_AUTO = 2f;

        [Header("Canvas")] [SerializeField] private Canvas canvas;
        [Header("TileInfo")]
        [SerializeField] private TMP_Text tileDefense;
        [SerializeField] private TMP_Text tileMouvementEffect;
        [SerializeField] private Image tileTexture;

        [Header("ControlsInfo")]
        [SerializeField] private TMP_Text leftClickText;
        [SerializeField] private TMP_Text rightClickText;

        [Header("Turn")] 
        [SerializeField] private TMP_Text turnCounter;
        [SerializeField] private TMP_Text turnInfo;
        [SerializeField] private Animator playerInfoAnimator;

        [Header("Victory Condition")] [SerializeField] private TMP_Text victoryCondition;

        [Header("Battle Report")] 
        [SerializeField] private GameObject battleReports;
        
        [SerializeField] private GameObject battleReportAlly;
        [FormerlySerializedAs("playerHealthBar")] 
        [SerializeField] private LayoutGroup playerHealthBarLayout;
        [SerializeField] private GameObject playerDeathSymbol;
        [SerializeField] private TMP_Text playerOutcome;
        
        [SerializeField] private GameObject battleReportEnemy;
        [FormerlySerializedAs("enemyHealthBar")] 
        [SerializeField] private LayoutGroup enemyHealthBarLayout;
        [SerializeField] private GameObject enemyDeathSymbol;
        [SerializeField] private TMP_Text enemyOutcome;

        [Header("Battle Outcome Text")] 
        [SerializeField] private string missText;
        [SerializeField] private string hitText;
        [SerializeField] private string criticalHitText;

        private Animator playerAnimator;
        private Animator enemyAnimator;
        private static readonly int IS_ATTACKING = Animator.StringToHash("IsAttacking");
        private static readonly int IS_ENEMY_TURN = Animator.StringToHash("IsEnemyTurn");

        private bool animationIsPlaying;
        private BattleInfos playerBattleInfos;
        private BattleInfos enemyBattleInfos;

        private GameObject[] playerHealthBar;
        private GameObject[] enemyHealthBar;

        private GameSettings gameSettings;
        private GridController gridController;


        public bool IsBattleReportActive => battleReports.activeSelf;

        private void Awake()
        {
            gameSettings = Harmony.Finder.GameSettings;
        }

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
            for (int i = 0; i < healthBar.Length; i++)
            {
                if (i < maxHealthPoints) healthBar[i].SetActive(true);
                else healthBar[i].SetActive(false);
            }
        
            for (int i = 0; i < currentHealthPoint; i++)
            {
                RawImage healthBarImage = healthBar[i].GetComponentInChildren<RawImage>();
                healthBarImage.color = gameSettings.Green;
            }
            
            for (int i = maxHealthPoints; i > currentHealthPoint; i--)
            {
                if (i - 1 < 0) continue;
                RawImage healthBarImage;
                healthBarImage = healthBar[i - 1].GetComponentInChildren<RawImage>();
                healthBarImage.color = gameSettings.Gray;

            }

            if (beforeBattle == false)
            {
                for (int i = currentHealthPoint; i > currentHealthPoint - damage; i--)
                {
                    if (i <= 0) break;
                    RawImage healthBarImage = healthBar[i - 1].GetComponentInChildren<RawImage>();
                    healthBarImage.color = gameSettings.Red;
                }
            }
        }

        public void ChangeCharacterDamageTaken(int dmg, bool isEnemy, int critModifier)
        {
            if (isEnemy)
            {
                enemyBattleInfos.DamageTaken = dmg;
                enemyBattleInfos.CriticalHit = critModifier - 1 != 0;
            }
            else
            {
                playerBattleInfos.DamageTaken = dmg;
                playerBattleInfos.CriticalHit = critModifier - 1 != 0;
            }
        }

        public Coroutine LaunchBattleReport(bool isEnemy)
        {
            battleReports.SetActive(true);

            playerDeathSymbol.SetActive(false);
            enemyDeathSymbol.SetActive(false);
            playerOutcome.enabled = false;
            enemyOutcome.enabled = false;

            Coroutine handle = Harmony.Finder.CoroutineStarter.StartCoroutine(BattleReport(isEnemy));

            return handle;
        }

        private IEnumerator BattleReport(bool isEnemy)
        {
            if (isEnemy)
            {
                yield return AttackAnimation(enemyAnimator, playerBattleInfos, playerHealthBar, true);
                if (playerBattleInfos.CurrentHealth - playerBattleInfos.DamageTaken > 0)
                {
                    yield return new WaitForSeconds(TIME_TO_WAIT_BETWEEN_ANIMATIONS);
                    yield return AttackAnimation(playerAnimator, enemyBattleInfos, enemyHealthBar, false);
                }
            }
            else
            {
                yield return AttackAnimation(playerAnimator, enemyBattleInfos, enemyHealthBar, false);
                if (enemyBattleInfos.CurrentHealth - enemyBattleInfos.DamageTaken > 0)
                {
                    yield return new WaitForSeconds(TIME_TO_WAIT_BETWEEN_ANIMATIONS);
                    yield return AttackAnimation(enemyAnimator, playerBattleInfos, playerHealthBar, true);
                }
            }

            if (enemyBattleInfos.CurrentHealth <= 0)
            {
                enemyDeathSymbol.SetActive(true);
            }

            if (playerBattleInfos.CurrentHealth <= 0)
            {
                playerDeathSymbol.SetActive(true);
            }

            yield return new WaitForSeconds(TIME_BEFORE_HIDING_BATTLE_REPORT_AUTO);
            battleReports.SetActive(false);
        }

        private IEnumerator AttackAnimation(Animator animator, BattleInfos battleInfos, GameObject[] healthBar, bool isEnemy)
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
            animator.SetBool(IS_ATTACKING, true);
            yield return new WaitForSeconds(TIME_TO_WAIT_BETWEEN_ANIMATIONS);
            ModifyHealthBar(battleInfos.MaxHp, battleInfos.CurrentHealth, healthBar, false,
                battleInfos.DamageTaken);
            BattleOutcome(battleInfos, isEnemy);
            battleInfos.CurrentHealth -= battleInfos.DamageTaken;
            yield return new WaitForSeconds(TIME_TO_WAIT_BETWEEN_ANIMATIONS);
            animator.SetBool(IS_ATTACKING, false);
            animationIsPlaying = false;
        }

        private void BattleOutcome(BattleInfos battleInfos, bool isEnemy)
        {
            if (battleInfos.DamageTaken == 0)
            {
                if (isEnemy)
                {
                    enemyOutcome.text = missText;
                    enemyOutcome.enabled = true;
                }
                else
                {
                    playerOutcome.text = missText;
                    playerOutcome.enabled = true;
                }
            }
            else if (!battleInfos.CriticalHit)
            {
                if (isEnemy)
                {
                    enemyOutcome.text = hitText;
                    enemyOutcome.enabled = true;
                }
                else
                {
                    playerOutcome.text = hitText;
                    playerOutcome.enabled = true;
                }
            }
            else
            {
                if (isEnemy)
                {
                    enemyOutcome.text = criticalHitText;
                    enemyOutcome.enabled = true;
                }
                else
                {
                    playerOutcome.text = criticalHitText;
                    playerOutcome.enabled = true;
                }
            }
        }

        [UsedImplicitly]
        public void DeactivateBattleReport()
        {
            battleReports.SetActive(false);
        }

        public void ModifyTurnCounter(int turns)
        {
            turnCounter.text = TURN_DISPLAY_TEXT + turns.ToString(TURN_FORMAT_TEXT);
        }

        public void ModifyTurnInfo(UnitOwner player)
        {
            turnInfo.text = player.Name;
            playerInfoAnimator.SetBool(IS_ENEMY_TURN, player is ComputerPlayer);
        }

        public void ModifyVictoryCondition(string victoryCondition)
        {
            this.victoryCondition.text = victoryCondition;
        }
        
        public void UpdateTileInfos(Tile tile)
        {
            UpdateTileSprite(tile.GetSprite());
            UpdateTileCostToMoveText(tile.CostToMove);
            UpdateTileDefenseText(tile.DefenseRate);
        }

        private void UpdateTileSprite(Sprite tileSprite)
        {
            tileTexture.sprite = tileSprite;
        }

        private void UpdateTileCostToMoveText(int costToMove)
        {
            switch (costToMove)
            {
                case TileTypeExt.LOW_COST_TO_MOVE:
                    tileMouvementEffect.color = gameSettings.DarkGreen;
                    break;
                case TileTypeExt.MEDIUM_COST_TO_MOVE:
                    tileMouvementEffect.color = gameSettings.DarkYellow;
                    break;
                case TileTypeExt.HIGH_COST_TO_MOVE:
                    tileMouvementEffect.color = gameSettings.DarkRed;
                    break;
            }
            tileMouvementEffect.text = costToMove == TileTypeExt.HIGH_COST_TO_MOVE ? UNREACHABLE_TILE_TEXT : costToMove.ToString();
        }

        private void UpdateTileDefenseText(float defenseRate)
        {
            switch (defenseRate)
            {
                case TileTypeExt.LOW_DEFENSE_RATE:
                    tileDefense.color = gameSettings.DarkRed;
                    break;
                case TileTypeExt.MEDIUM_DEFENSE_RATE:
                    tileDefense.color = gameSettings.DarkYellow;
                    break;
                case TileTypeExt.HIGH_DEFENSE_RATE:
                    tileDefense.color = gameSettings.DarkGreen;
                    break;
            }
            tileDefense.text = defenseRate * 100 + "%";
        }

        public void UpdateLeftClickHint(ClickType leftClickType)
        {
            leftClickText.text = leftClickType.GetString(gameSettings);
        }

        public void UpdateRightClickHint(ClickType rightClickType)
        {
            rightClickText.text = rightClickType.GetString(gameSettings);
        }
    }
}