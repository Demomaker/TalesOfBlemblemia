using System;
using System.Collections;
using System.Threading.Tasks;
using Harmony;
using JetBrains.Annotations;
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

        [Header("Turn")] 
        [SerializeField] private TMP_Text turnCounter;
        [SerializeField] private TMP_Text turnInfo;

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


        private const string UNREACHABLE_TILE_TEXT = "Unreachable";
        private const float TIME_TO_WAIT_BETWEEN_ANIMATIONS = 0.5f;
        private const float TIME_BEFORE_HIDING_BATTLE_REPORT_AUTO = 2f;

        private Animator playerAnimator;
        private Animator enemyAnimator;
        private static readonly int IS_ATTACKING = Animator.StringToHash("IsAttacking");

        private bool animationIsPlaying = false;
        private BattleInfos playerBattleInfos;
        private BattleInfos enemyBattleInfos;

        private GameObject[] playerHealthbar;
        private GameObject[] enemyHealthbar;

        public bool IsBattleReportActive => battleReports.activeSelf;

        private void Start()
        {
            playerHealthbar = playerHealthBarLayout.Children();
            enemyHealthbar = enemyHealthBarLayout.Children();
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

            for (int i = maxHealthPoints; i > currentHealthPoint; i--)
            {
                RawImage healthBarImage = healthBar[i - 1].GetComponentInChildren<RawImage>();
                healthBarImage.color = grey;
            }

            if (beforeBattle == false)
            {
                for (int i = currentHealthPoint; i > currentHealthPoint - damage; i--)
                {
                    RawImage healthBarImage = healthBar[i - 1].GetComponentInChildren<RawImage>();
                    healthBarImage.color = red;
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
                yield return AttackAnimation(enemyAnimator, playerBattleInfos, playerHealthbar);
                if (playerBattleInfos.HealthBeforeCombat - playerBattleInfos.DamageTaken > 0)
                {
                    yield return new WaitForSeconds(TIME_TO_WAIT_BETWEEN_ANIMATIONS);
                    yield return AttackAnimation(playerAnimator, enemyBattleInfos, enemyHealthbar);
                }
                   
            }
            else
            {
                yield return AttackAnimation(playerAnimator, enemyBattleInfos, enemyHealthbar);
                if (enemyBattleInfos.HealthBeforeCombat - enemyBattleInfos.DamageTaken > 0)
                {
                    yield return new WaitForSeconds(TIME_TO_WAIT_BETWEEN_ANIMATIONS);
                    yield return AttackAnimation(enemyAnimator, playerBattleInfos, playerHealthbar);
                }
                   
            }

            if (enemyBattleInfos.HealthBeforeCombat - enemyBattleInfos.DamageTaken <= 0)
            {
                enemyDeathSymbol.SetActive(true);
            }

            if (playerBattleInfos.HealthBeforeCombat - playerBattleInfos.DamageTaken <= 0)
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
                ModifyHealthBar(playerBattleInfos.MaxHp, playerBattleInfos.HealthBeforeCombat, playerHealthbar);
                ModifyHealthBar(enemyBattleInfos.MaxHp, enemyBattleInfos.HealthBeforeCombat, enemyHealthbar);
            #endregion
            
            animationIsPlaying = true;
            animator.SetBool(IS_ATTACKING,true);
            yield return new WaitForSeconds(TIME_TO_WAIT_BETWEEN_ANIMATIONS);
            ModifyHealthBar(battleInfos.MaxHp, battleInfos.HealthBeforeCombat, healthBar, false, battleInfos.DamageTaken);
            yield return new WaitForSeconds(TIME_TO_WAIT_BETWEEN_ANIMATIONS);
            animator.SetBool(IS_ATTACKING,false);
            yield return new WaitForSeconds(TIME_TO_WAIT_BETWEEN_ANIMATIONS);
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

        public void ModifyTurnInfo(string player)
        {
            turnInfo.text = player + " turn";
        }

        public void ModifyVictoryCondition(string victoryCondition)
        {
            this.victoryCondition.text = victoryCondition;
        }
    }
}