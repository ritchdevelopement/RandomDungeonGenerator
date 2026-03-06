using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HorizontalLayoutGroup))]
public class HealthUI : MonoBehaviour {
    [SerializeField] private Sprite heartSprite;
    [SerializeField] private float iconSpacing = 4f;
    [SerializeField] private Color activeColor = Color.white;
    [SerializeField] private Color depletedColor = new Color(1f, 1f, 1f, 0.3f);

    private HorizontalLayoutGroup layoutGroup;
    private RectTransform containerRect;
    private Image[] heartIcons;

    private void Awake() {
        layoutGroup = GetComponent<HorizontalLayoutGroup>();
        containerRect = (RectTransform) transform;
    }

    private void OnEnable() {
        PlayerController.OnHealthChanged += RefreshHeartColors;
        if (PlayerController.MaxHealth > 0) {
            RefreshHeartColors(PlayerController.CurrentHealth);
        }
    }

    private void OnDisable() {
        PlayerController.OnHealthChanged -= RefreshHeartColors;
    }

    private void CreateHeartIcons() {
        float iconSize = containerRect.rect.height;
        ConfigureLayoutGroup();

        heartIcons = new Image[PlayerController.MaxHealth];
        for (int i = 0; i < PlayerController.MaxHealth; i++) {
            heartIcons[i] = CreateHeartIcon(i, iconSize);
        }

        FitContainerWidthToIcons(iconSize);
    }

    private void ConfigureLayoutGroup() {
        layoutGroup.spacing = iconSpacing;
        layoutGroup.childControlWidth = false;
        layoutGroup.childControlHeight = false;
        layoutGroup.childForceExpandWidth = false;
        layoutGroup.childForceExpandHeight = false;
    }

    private Image CreateHeartIcon(int index, float size) {
        GameObject iconObject = new($"HeartIcon_{index}", typeof(RectTransform), typeof(Image));
        iconObject.transform.SetParent(transform, false);

        RectTransform rectTransform = iconObject.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(size, size);

        Image icon = iconObject.GetComponent<Image>();
        icon.sprite = heartSprite;
        icon.preserveAspect = true;
        return icon;
    }

    private void FitContainerWidthToIcons(float iconSize) {
        float horizontalPadding = layoutGroup.padding.left + layoutGroup.padding.right;
        float totalSpacing = iconSpacing * (PlayerController.MaxHealth - 1);
        float totalWidth = iconSize * PlayerController.MaxHealth + totalSpacing + horizontalPadding;

        containerRect.sizeDelta = new Vector2(totalWidth, containerRect.sizeDelta.y);
    }

    private void RefreshHeartColors(int currentHealth) {
        if (heartIcons == null || heartIcons.Length != PlayerController.MaxHealth) {
            RebuildHeartIcons();
        }

        for (int i = 0; i < heartIcons.Length; i++) {
            bool isHeartFilled = i < currentHealth;
            heartIcons[i].color = isHeartFilled ? activeColor : depletedColor;
        }
    }

    private void RebuildHeartIcons() {
        if (heartIcons != null) {
            foreach (Image icon in heartIcons) {
                Destroy(icon.gameObject);
            }
        }

        CreateHeartIcons();
    }
}
