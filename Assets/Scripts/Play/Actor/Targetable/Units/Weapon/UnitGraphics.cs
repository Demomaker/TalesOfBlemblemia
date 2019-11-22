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
            nameText.text = unit.UnitInfos.characterName;
            
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
            
            attackText.text = unit.Stats.AttackStrength.ToString();
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
            movementText.text = unit.MovesLeft.ToString();
            healthText.text = unit.CurrentHealthPoints.ToString();
            if (canvas != null) canvas.enabled = true;
        }

        private void DisableUnitGraphics()
        {
            if (canvas != null) canvas.enabled = false;
        }
    }
}