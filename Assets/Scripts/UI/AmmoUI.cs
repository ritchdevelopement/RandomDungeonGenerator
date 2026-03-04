using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HorizontalLayoutGroup))]
public class AmmoUI : MonoBehaviour {
    [SerializeField] private float iconSpacing = 4f;
    [SerializeField] private Color activeColor = Color.white;
    [SerializeField] private Color depletedColor = new Color(1f, 1f, 1f, 0.3f);

    private HorizontalLayoutGroup layoutGroup;
    private RectTransform containerRect;
    private Image[] ammoIcons;

    private void Awake() {
        layoutGroup = GetComponent<HorizontalLayoutGroup>();
        containerRect = (RectTransform) transform;
    }

    private IEnumerator Start() {
        yield return new WaitUntil(() => WeaponController.Instance != null);
        BuildIcons();
        WeaponController.OnWeaponChanged += RebuildIcons;
    }

    private void OnDestroy() {
        WeaponController.OnWeaponChanged -= RebuildIcons;
    }

    private void RebuildIcons() {
        foreach (Transform child in transform) {
            Destroy(child.gameObject);
        }

        BuildIcons();
    }

    private void BuildIcons() {
        float iconSize = containerRect.rect.height;
        ConfigureLayoutGroup();

        ammoIcons = new Image[WeaponController.MaxAmmo];
        for (int i = 0; i < WeaponController.MaxAmmo; i++) {
            ammoIcons[i] = CreateIcon(i, iconSize);
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

    private Image CreateIcon(int index, float size) {
        GameObject iconObject = new($"AmmoIcon_{index}", typeof(RectTransform), typeof(Image));
        iconObject.transform.SetParent(transform, false);

        RectTransform rectTransform = iconObject.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(size, size);

        Image icon = iconObject.GetComponent<Image>();
        icon.sprite = WeaponController.WeaponSprite;
        icon.preserveAspect = true;
        return icon;
    }

    private void FitContainerWidthToIcons(float iconSize) {
        float horizontalPadding = layoutGroup.padding.left + layoutGroup.padding.right;
        float totalSpacing = iconSpacing * (WeaponController.MaxAmmo - 1);
        float totalWidth = iconSize * WeaponController.MaxAmmo + totalSpacing + horizontalPadding;

        containerRect.pivot = new Vector2(1f, containerRect.pivot.y);
        containerRect.sizeDelta = new Vector2(totalWidth, containerRect.sizeDelta.y);
    }

    private void Update() {
        if (ammoIcons == null) {
            return;
        }

        for (int i = 0; i < ammoIcons.Length; i++) {
            bool isActive = i >= ammoIcons.Length - WeaponController.CurrentAmmo;
            ammoIcons[i].color = isActive ? activeColor : depletedColor;
        }
    }
}
