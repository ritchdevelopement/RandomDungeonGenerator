using System.Collections.Generic;
using UnityEngine;

public class PerkManager : MonoBehaviour {
    public static PerkManager Instance { get; private set; }

    private const int PerkChoiceCount = 3;

    [SerializeField] private PerkData[] allPerks;
    [SerializeField] private PerkSelectionUI perkSelectionUI;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public void ShowPerkSelectionUI(Room room) {
        List<PerkData> choices = PickRandomPerks(PerkChoiceCount);
        perkSelectionUI.Show(room, choices);
    }

    public void SelectPerk(PerkData perk, Room room) {
        ApplyPerk(perk);
        EnemyManager.Instance.FinalizeEncounter(room);
    }

    private List<PerkData> PickRandomPerks(int count) {
        List<PerkData> pool = new List<PerkData>(allPerks);
        List<PerkData> result = new List<PerkData>();

        for (int i = 0; i < count && pool.Count > 0; i++) {
            int index = Random.Range(0, pool.Count);
            result.Add(pool[index]);
            pool.RemoveAt(index);
        }

        return result;
    }

    private static void ApplyPerk(PerkData perk) {
        switch (perk.type) {
            case PerkType.MaxHealth:
            PlayerController.Instance.AddMaxHealth((int) perk.value);
            break;
            case PerkType.MoveSpeed:
            PlayerController.Instance.AddMoveSpeed(perk.value);
            break;
            case PerkType.DashCooldown:
            PlayerController.Instance.ReduceDashCooldown(perk.value);
            break;
            case PerkType.MaxAmmo:
            WeaponController.Instance.AddMaxAmmo((int) perk.value);
            break;
            case PerkType.Piercing:
            WeaponController.Instance.EnablePiercing();
            break;
        }
    }
}
