using System.Collections;
using Harmony;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

//Author: Pierre-Luc Maltais (mainly), Antoine Lessard
namespace Game
{
    [Findable("UIController")]
    public class UIController : MonoBehaviour
    {
        [SerializeField] private string turnFormatText = "00";
        [SerializeField] private string turnDisplayText = "Turn ";
        [SerializeField] private string unreachableTileText = "X";
        [SerializeField] private float timeToWaitBetweenAnimations = 0.5f;
        [SerializeField] private float timeBeforeHidingBattleReportAuto = 2f;

        [Header("Canvas")] 
        [SerializeField] private Canvas canvas;
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

        [Header("Victory Condition")] 
        [SerializeField] private TMP_Text victoryCondition;

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
        private CoroutineStarter coroutineStarter;
        private Coroutine displayReportCoroutine;

        public bool IsBattleReportActive => battleReports.activeSelf;

        private void Awake()
        {
            gameSettings = Harmony.Finder.GameSettings;
            coroutineStarter = Harmony.Finder.CoroutineStarter;
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

        public void SetupCharactersBattleInfo(int maxHealthPoints, int currentHealthPoint, int targetMaxHealthPoint, int targetCurrentHealthPoint, bool isEnemy)
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
                healthBar[i].SetActive(i < maxHealthPoints);
            }
        
            for (int i = 0; i < healthBar.Length; i++)
            {
                var healthBarImage = healthBar[i].GetComponentInChildren<RawImage>();
                healthBarImage.color = gameSettings.Green;
            }
            
            for (int i = maxHealthPoints; i > currentHealthPoint; i--)
            {
                if (i - 1 < 0) continue;
                var healthBarImage = healthBar[i - 1].GetComponentInChildren<RawImage>();
                healthBarImage.color = gameSettings.Gray;
            }

            if (beforeBattle == false)
            {
                for (int i = currentHealthPoint; i > currentHealthPoint - damage; i--)
                {
                    if (i <= 0) break;
                    var healthBarImage = healthBar[i - 1].GetComponentInChildren<RawImage>();
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

        public void LaunchBattleReport(bool isEnemy)
        {
            battleReports.SetActive(true);

            enemyAnimator.SetBool(IS_ATTACKING, false);
            playerAnimator.SetBool(IS_ATTACKING, false);
            animationIsPlaying = false;
            playerDeathSymbol.SetActive(false);
            enemyDeathSymbol.SetActive(false);
            playerOutcome.enabled = false;
            enemyOutcome.enabled = false;
            
            if(displayReportCoroutine != null) coroutineStarter.StopCoroutine(displayReportCoroutine);

            displayReportCoroutine = coroutineStarter.StartCoroutine(BattleReport(isEnemy));
        }

        private IEnumerator BattleReport(bool isEnemy)
        {
            if (isEnemy)
            {
                yield return AttackAnimation(enemyAnimator, playerBattleInfos, playerHealthBar, true);
                if (playerBattleInfos.CurrentHealth > 0)
                {
                    yield return new WaitForSeconds(timeToWaitBetweenAnimations);
                    yield return AttackAnimation(playerAnimator, enemyBattleInfos, enemyHealthBar, false);
                }
            }
            else
            {
                yield return AttackAnimation(playerAnimator, enemyBattleInfos, enemyHealthBar, false);
                if (enemyBattleInfos.CurrentHealth > 0)
                {
                    yield return new WaitForSeconds(timeToWaitBetweenAnimations);
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

            yield return new WaitForSeconds(timeBeforeHidingBattleReportAuto);
            if(battleReports != null) battleReports.SetActive(false);
        }

        private IEnumerator AttackAnimation(Animator animator, BattleInfos battleInfos, GameObject[] healthBar, bool isEnemy)
        {
            while (animationIsPlaying)
            {
                yield return null;
            }
            
            ModifyHealthBar(playerBattleInfos.MaxHp, playerBattleInfos.CurrentHealth, playerHealthBar);
            ModifyHealthBar(enemyBattleInfos.MaxHp, enemyBattleInfos.CurrentHealth, enemyHealthBar);

            animationIsPlaying = true;
            animator.SetBool(IS_ATTACKING, true);
            yield return new WaitForSeconds(timeToWaitBetweenAnimations);
            ModifyHealthBar(battleInfos.MaxHp, battleInfos.CurrentHealth, healthBar, false,
                battleInfos.DamageTaken);
            BattleOutcome(battleInfos, isEnemy);
            battleInfos.CurrentHealth -= battleInfos.DamageTaken;
            yield return new WaitForSeconds(timeToWaitBetweenAnimations);
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

        //Author : Jérémie Bertrand from here on out
        public void ModifyTurnCounter(int turns)
        {
            turnCounter.text = turnDisplayText + turns.ToString(turnFormatText);
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
            tileMouvementEffect.text = costToMove == TileTypeExt.HIGH_COST_TO_MOVE ? unreachableTileText : costToMove.ToString();
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