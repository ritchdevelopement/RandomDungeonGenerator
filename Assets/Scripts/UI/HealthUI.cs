using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HorizontalLayoutGroup))]
public class HealthUI : MonoBehaviour {
    [SerializeField] private Sprite heartSprite;
    [SerializeField] private float iconSpacing = 4f;
    [SerializeField] private Color activeColor = Color.white;
    [SerializeField] private Color depletedColor = new Color(1f, 1f, 1f, 0.3f);

    private Image[] heartIcons;

    private IEnumerator Start() {
        yield return new WaitUntil(() => PlayerController.MaxHealth > 0);
        CreateHeartIcons();
    }

    private void CreateHeartIcons() {
        float iconSize = GetContainerHeight();
        ApplyLayoutGroupSettings(iconSize);

        heartIcons = new Image[PlayerController.MaxHealth];
        for (int i = 0; i < PlayerController.MaxHealth; i++) {
            heartIcons[i] = CreateHeartIcon(i, iconSize);
        }

        FitContainerWidthToIcons(iconSize);
    }

    private float GetContainerHeight() {
        return ((RectTransform) transform).rect.height;
    }

    private void ApplyLayoutGroupSettings(float iconSize) {
        HorizontalLayoutGroup hlg = GetComponent<HorizontalLayoutGroup>();
        hlg.spacing = iconSpacing;
        hlg.childControlWidth = false;
        hlg.childControlHeight = false;
        hlg.childForceExpandWidth = false;
        hlg.childForceExpandHeight = false;
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
        HorizontalLayoutGroup hlg = GetComponent<HorizontalLayoutGroup>();
        float horizontalPadding = hlg.padding.left + hlg.padding.right;
        float totalSpacing = iconSpacing * (PlayerController.MaxHealth - 1);
        float totalWidth = iconSize * PlayerController.MaxHealth + totalSpacing + horizontalPadding;

        RectTransform containerRt = (RectTransform) transform;
        containerRt.sizeDelta = new Vector2(totalWidth, containerRt.sizeDelta.y);
    }

    private void Update() {
        if (heartIcons == null) {
            return;
        }

        for (int i = 0; i < heartIcons.Length; i++) {
            bool isAlive = i < PlayerController.CurrentHealth;
            heartIcons[i].color = isAlive ? activeColor : depletedColor;
        }
    }
}
