using TMPro;
using UnityEngine;

public class EncounterInfoUI : MonoBehaviour {
    [SerializeField] private TMP_Text infoText;

    private void Awake() {
        gameObject.SetActive(false);
        EnemyManager.OnEncounterInfoChanged += UpdateDisplay;
    }

    private void OnDestroy() {
        EnemyManager.OnEncounterInfoChanged -= UpdateDisplay;
    }

    private void UpdateDisplay(string text) {
        if (string.IsNullOrEmpty(text)) {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);
        infoText.text = text;
    }
}
