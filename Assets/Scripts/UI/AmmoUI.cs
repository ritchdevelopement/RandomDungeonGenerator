using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HorizontalLayoutGroup))]
public class AmmoUI : MonoBehaviour {
    [SerializeField] private float iconSpacing = 4f;
    [SerializeField] private Color activeColor = Color.white;
    [SerializeField] private Color depletedColor = new Color(1f, 1f, 1f, 0.3f);

    private Image[] ammoIcons;

    private IEnumerator Start() {
        yield return new WaitUntil(() => ThrowController.Instance != null);
        CreateIcons();
    }

    private void CreateIcons() {
        float iconSize = GetContainerHeight();
        ApplySpacing();

        ammoIcons = new Image[ThrowController.MaxAmmo];
        for (int i = 0; i < ThrowController.MaxAmmo; i++) {
            ammoIcons[i] = CreateIcon(i, iconSize);
        }

        FitContainerWidthToIcons(iconSize);
    }

    private float GetContainerHeight() {
        return ((RectTransform) transform).rect.height;
    }

    private void ApplySpacing() {
        HorizontalLayoutGroup hlg = GetComponent<HorizontalLayoutGroup>();
        hlg.spacing = iconSpacing;
        hlg.childControlWidth = false;
        hlg.childControlHeight = false;
        hlg.childForceExpandWidth = false;
        hlg.childForceExpandHeight = false;
    }

    private Image CreateIcon(int index, float size) {
        GameObject iconObject = new($"AmmoIcon_{index}", typeof(RectTransform), typeof(Image));
        iconObject.transform.SetParent(transform, false);

        RectTransform rectTransform = iconObject.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(size, size);

        Image icon = iconObject.GetComponent<Image>();
        icon.sprite = ThrowController.WeaponSprite;
        icon.preserveAspect = true;
        return icon;
    }

    private void FitContainerWidthToIcons(float iconSize) {
        HorizontalLayoutGroup hlg = GetComponent<HorizontalLayoutGroup>();
        float horizontalPadding = hlg.padding.left + hlg.padding.right;
        float totalSpacing = iconSpacing * (ThrowController.MaxAmmo - 1);
        float totalWidth = iconSize * ThrowController.MaxAmmo + totalSpacing + horizontalPadding;

        RectTransform containerRt = (RectTransform) transform;
        containerRt.pivot = new Vector2(1f, containerRt.pivot.y);
        containerRt.sizeDelta = new Vector2(totalWidth, containerRt.sizeDelta.y);
    }

    private void Update() {
        if (ammoIcons == null) {
            return;
        }

        for (int i = 0; i < ammoIcons.Length; i++) {
            bool isActive = i >= ammoIcons.Length - ThrowController.CurrentAmmo;
            ammoIcons[i].color = isActive ? activeColor : depletedColor;
        }
    }
}
