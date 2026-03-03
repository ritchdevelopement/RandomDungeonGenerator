using UnityEngine;
using UnityEngine.UI;

public class DashCooldownUI : MonoBehaviour {
    [SerializeField] private Image cooldownFillImage;

    private void Update() {
        cooldownFillImage.fillAmount = PlayerController.DashCooldownFraction;
    }
}
