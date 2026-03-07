using UnityEngine;
using UnityEngine.UI;

public class WeaponUI : MonoBehaviour {
    [SerializeField] private Image primaryCooldownFill;
    [SerializeField] private Image secondaryCooldownFill;

    private void Update() {
        primaryCooldownFill.fillAmount = WeaponController.PrimaryCooldownFraction;
        secondaryCooldownFill.fillAmount = WeaponController.SecondaryCooldownFraction;
    }
}
