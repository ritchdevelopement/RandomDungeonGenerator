using System.Collections.Generic;
using UnityEngine;

public class RoomFog {
    public Room Room { get; }
    public bool IsRevealed { get; private set; }

    private readonly List<GameObject> overlays;

    public RoomFog(Room room, List<GameObject> overlays) {
        Room = room;
        this.overlays = overlays;
    }

    public void Reveal() {
        if (IsRevealed) {
            return;
        }
        IsRevealed = true;
        foreach (GameObject overlay in overlays) {
            overlay.SetActive(false);
        }
    }
}
