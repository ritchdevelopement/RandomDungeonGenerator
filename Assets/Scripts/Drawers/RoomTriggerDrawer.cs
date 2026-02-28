using UnityEngine;

public class RoomTriggerDrawer : DungeonTaskBase {
    [SerializeField] private Color triggerColor = Color.white;
    [SerializeField] private int triggerSize = 3;

    public override void Execute() {
        DoorManager.Instance.OpenRoomDoors(context.playerSpawnRoom);

        foreach (Room room in context.createdRooms.Values) {
            if (room == context.playerSpawnRoom) {
                continue;
            }

            PlaceTrigger(room);
        }
    }

    private void PlaceTrigger(Room room) {
        GameObject triggerObject = new($"RoomTrigger_{room.Center}");
        triggerObject.transform.SetParent(context.dungeonGameObject.transform);
        triggerObject.transform.position = new Vector3(room.Center.x, room.Center.y, 0);
        triggerObject.transform.localScale = new Vector3(triggerSize, triggerSize, 1);

        SpriteRenderer spriteRenderer = triggerObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = CreateSquareSprite();
        spriteRenderer.color = triggerColor;

        BoxCollider2D triggerCollider = triggerObject.AddComponent<BoxCollider2D>();
        triggerCollider.isTrigger = true;

        RoomTrigger roomTrigger = triggerObject.AddComponent<RoomTrigger>();
        roomTrigger.Initialize(room);
        RoomManager.Instance.RegisterTrigger(room, roomTrigger);
    }

    private static Sprite CreateSquareSprite() {
        Texture2D whiteTexture = new(1, 1);
        whiteTexture.SetPixel(0, 0, Color.white);
        whiteTexture.Apply();
        return Sprite.Create(whiteTexture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
    }
}
