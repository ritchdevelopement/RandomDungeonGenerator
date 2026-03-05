using System.Collections.Generic;
using UnityEngine;

public class PerkSelectionUI : MonoBehaviour {
    [SerializeField] private GameObject panel;

    private PerkUI[] perkCards;
    private Room currentRoom;

    private void Awake() {
        panel.SetActive(false);
    }

    public void Show(Room room, List<PerkData> perks) {
        perkCards ??= panel.GetComponentsInChildren<PerkUI>(true);
        currentRoom = room;

        for (int i = 0; i < Mathf.Min(perkCards.Length, perks.Count); i++) {
            int index = i;
            perkCards[i].Setup(perks[i], () => OnPerkSelected(perks[index]));
        }

        panel.SetActive(true);
        Time.timeScale = 0f;
    }

    private void OnPerkSelected(PerkData perkData) {
        panel.SetActive(false);
        Time.timeScale = 1f;
        PerkManager.Instance.SelectPerk(perkData, currentRoom);
    }
}
