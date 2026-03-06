using System.Collections.Generic;
using UnityEngine;

public class PerkManager : MonoBehaviour {
    [System.Serializable]
    private struct RarityWeight {
        public PerkRarity rarity;
        public int weight;
    }

    public static PerkManager Instance { get; private set; }

    private const int PerkChoiceCount = 3;

    [SerializeField] private PerkData[] allPerks;
    [SerializeField] private PerkSelectionUI perkSelectionUI;
    [SerializeField] private RarityWeight[] rarityWeights;

    private readonly HashSet<PerkData> chosenUniquePerks = new HashSet<PerkData>();

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    private void OnEnable() {
        PlayerController.OnDeath += ResetUniquePerks;
    }

    private void OnDisable() {
        PlayerController.OnDeath -= ResetUniquePerks;
    }

    private void ResetUniquePerks() {
        chosenUniquePerks.Clear();
    }

    public void ShowPerkSelectionUI(Room room) {
        List<PerkData> choices = PickRandomPerks(PerkChoiceCount);
        perkSelectionUI.Show(room, choices);
    }

    public void SelectPerk(PerkData perk, Room room) {
        if (perk.isUnique) {
            chosenUniquePerks.Add(perk);
        }
        ApplyPerk(perk);
        EnemyManager.Instance.FinalizeEncounter(room);
    }

    private List<PerkData> PickRandomPerks(int count) {
        HashSet<PerkType> excludedTypes = new();
        foreach (PerkData chosen in chosenUniquePerks) {
            foreach (PerkType excluded in chosen.incompatibleWith) {
                excludedTypes.Add(excluded);
            }
        }

        List<PerkData> pool = new List<PerkData>();
        foreach (PerkData perk in allPerks) {
            if (perk.isUnique && chosenUniquePerks.Contains(perk)) { continue; }
            if (excludedTypes.Contains(perk.type)) { continue; }
            pool.Add(perk);
        }

        List<PerkData> result = new List<PerkData>();
        while (result.Count < count && pool.Count > 0) {
            PerkData picked = PickWeighted(pool);
            result.Add(picked);
            pool.Remove(picked);
        }

        return result;
    }

    private PerkData PickWeighted(List<PerkData> pool) {
        int totalWeight = 0;
        foreach (PerkData perk in pool) {
            totalWeight += GetWeight(perk.rarity);
        }

        int roll = Random.Range(0, totalWeight);
        int cumulative = 0;
        foreach (PerkData perk in pool) {
            cumulative += GetWeight(perk.rarity);
            if (roll < cumulative) {
                return perk;
            }
        }

        return pool[^1];
    }

    private int GetWeight(PerkRarity rarity) {
        foreach (RarityWeight entry in rarityWeights) {
            if (entry.rarity == rarity) {
                return entry.weight;
            }
        }
        return 1;
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
            case PerkType.Multishot:
            WeaponController.Instance.AddProjectile();
            break;
            case PerkType.Ricochet:
            WeaponController.Instance.EnableRicochet();
            break;
        }
    }
}
