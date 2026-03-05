using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PerkUI : MonoBehaviour {
    [SerializeField] private TMP_Text perkNameText;

    private Image icon;
    private Button button;

    public void Setup(PerkData data, System.Action onSelected) {
        icon ??= GetComponent<Image>();
        button ??= GetComponent<Button>();

        icon.sprite = data.icon;
        perkNameText.text = data.perkName;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onSelected());
    }
}
