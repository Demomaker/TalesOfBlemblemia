using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    [RequireComponent(typeof(Canvas), typeof(BoxCollider2D))]
    public class UnitGraphics : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private TextMeshProUGUI movementText;
        [SerializeField] private TextMeshProUGUI attackText;
        [SerializeField] private Image[] nameBackground;
        [SerializeField] private Image attackIcon;
        [SerializeField] private Sprite axeSprite;
        [SerializeField] private Sprite spearSprite;
        [SerializeField] private Sprite swordSprite;
        [SerializeField] private Sprite healingStaffSprite;

        private Unit unit;
        private Canvas canvas;
        private GameSettings gameSettings;

        private void Awake()
        {
            unit = transform.root.GetComponent<Unit>();
            canvas = GetComponent<Canvas>();
            gameSettings = Harmony.Finder.GameSettings;
        }

        private void Start()
        {
            DisableUnitGraphics();
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
            
            foreach (var image in nameBackground)
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
            EnableUnitGraphics();
        }

        protected void OnMouseExit()
        {
            DisableUnitGraphics();
        }

        private void EnableUnitGraphics()
        {
            if (canvas != null) canvas.enabled = true;
        }

        private void DisableUnitGraphics()
        {
            if (canvas != null) canvas.enabled = false;
        }

        private void UpdateHealthText()
        {
            healthText.text = Mathf.Clamp(unit.CurrentHealthPoints, 0, unit.Stats.MaxHealthPoints).ToString();
        }

        private void UpdateMovementText()
        {
            movementText.text = Mathf.Clamp(unit.MovesLeft, 0, unit.Stats.MoveSpeed).ToString();
        }
    }
}