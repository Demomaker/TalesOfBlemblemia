using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    //Author: Jérémie Bertrand
    [RequireComponent(typeof(Canvas), typeof(BoxCollider2D))]
    public class UnitStatus : MonoBehaviour
    {
        private const string HEALTH_TEXT_SEPARATOR = "/";
        
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private TextMeshProUGUI movementText;
        [SerializeField] private TextMeshProUGUI attackText;
        [SerializeField] private Image[] background;
        [SerializeField] private Image attackIcon;
        [SerializeField] private Sprite axeSprite;
        [SerializeField] private Sprite spearSprite;
        [SerializeField] private Sprite swordSprite;
        [SerializeField] private Sprite healingStaffSprite;

        private Unit unit;
        private Canvas canvas;
        private GameSettings gameSettings;
        private bool isActive = false;
        private bool iscanvasNull;

        private void Awake()
        {
            unit = transform.root.GetComponent<Unit>();
            canvas = GetComponent<Canvas>();
            gameSettings = Harmony.Finder.GameSettings;
            iscanvasNull = canvas == null;
        }

        private void Start()
        {
            if (canvas != null) canvas.enabled = false;
            InitUnitInfos();
        }
        private void InitUnitInfos()
        {
            nameText.text = unit.UnitInfos.CharacterName;
            attackText.text = unit.Stats.AttackStrength.ToString();
            
            switch (unit.WeaponType)
            {
                case WeaponType.Axe:
                    attackIcon.sprite = axeSprite;
                    break;
                case WeaponType.Spear:
                    attackIcon.sprite = spearSprite;
                    break;
                case WeaponType.Sword:
                    attackIcon.sprite = swordSprite;
                    break;
                case WeaponType.HealingStaff:
                    attackIcon.sprite = healingStaffSprite;
                    break;
            }
            
            foreach (var image in background)
            {
                image.color = unit.IsEnemy ? gameSettings.Red : gameSettings.Green;
            }
        }
        

        private void OnEnable()
        {
            unit.OnHealthChange.Notify += UpdateHealthText;
            unit.OnMovementChange.Notify += UpdateMovementText;
        }

        private void OnDisable()
        {
            unit.OnHealthChange.Notify -= UpdateHealthText;
            unit.OnMovementChange.Notify -= UpdateMovementText;
        }

        private void OnMouseEnter()
        {
            isActive = true;
        }

        protected void OnMouseExit()
        {
            isActive = false;
        }

        private void Update()
        {
            if (iscanvasNull || Time.timeScale == 0) return;
            if (Harmony.Finder.LevelController.CinematicController != null && Harmony.Finder.LevelController.CinematicController.IsPlayingACinematic)
            {
                if(canvas.enabled) canvas.enabled = false;
                return;
            }
            if(isActive && !canvas.enabled) canvas.enabled = true;
            else if(!isActive && canvas.enabled) canvas.enabled = false;
        }

        private void UpdateHealthText()
        {
            healthText.text = Mathf.Clamp(unit.CurrentHealthPoints, 0, unit.Stats.MaxHealthPoints) + HEALTH_TEXT_SEPARATOR + unit.Stats.MaxHealthPoints;
        }

        private void UpdateMovementText()
        {
            movementText.text = Mathf.Clamp(unit.MovesLeft, 0, unit.Stats.MoveSpeed).ToString();
        }
    }
}