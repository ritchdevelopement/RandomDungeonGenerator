using UnityEngine;

public class RoomTriggerDrawer : DungeonTaskBase {
    [SerializeField] private float triggerInset = 1.5f;

    public override void Execute() {
        DoorManager.Instance.OpenRoomDoors(context.playerSpawnRoom);

        foreach (Room room in context.createdRooms.Values) {
            if (room == context.playerSpawnRoom) {
                continue;
            }

            PlaceTrigger(room);
        }

        RoomManager.Instance.AssignInitialEvents();
    }

    private void PlaceTrigger(Room room) {
        GameObject triggerObject = new($"RoomTrigger_{room.Center}");
        triggerObject.transform.SetParent(context.dungeonGameObject.transform);
        triggerObject.transform.position = new Vector3(room.Center.x, room.Center.y, 0);

        BoxCollider2D triggerCollider = triggerObject.AddComponent<BoxCollider2D>();
        triggerCollider.isTrigger = true;
        triggerCollider.size = new Vector2(room.Bounds.width - triggerInset * 2f, room.Bounds.height - triggerInset * 2f);

        RoomTrigger roomTrigger = triggerObject.AddComponent<RoomTrigger>();
        roomTrigger.Initialize(room);
        RoomManager.Instance.RegisterTrigger(room, roomTrigger);
    }
}
