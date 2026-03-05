using UnityEngine;

[CreateAssetMenu(fileName = "PerkData", menuName = "Perks/PerkData")]
public class PerkData : ScriptableObject {
    public string perkName;
    [TextArea] public string description;
    public Sprite icon;
    public PerkRarity rarity;
    public PerkType type;
    public float value;
}
