using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapUI : MonoBehaviour {
    [SerializeField] private RectTransform mapContainer;

    [Header("Layout")]
    [SerializeField] private int cellSize = 12;
    [SerializeField] private int connectorThickness = 3;
    [SerializeField] private int connectorLength = 5;

    [Header("Colors")]
    [SerializeField] private Color currentRoomColor = new Color(1f, 0.95f, 0.5f, 1f);
    [SerializeField] private Color visitedRoomColor = new Color(0.55f, 0.55f, 0.55f, 1f);
    [SerializeField] private Color discoveredRoomColor = new Color(0.25f, 0.25f, 0.25f, 0.9f);
    [SerializeField] private Color connectorColor = new Color(0.45f, 0.45f, 0.45f, 1f);

    private Dictionary<Room, Vector2Int> gridCoords = new();
    private Dictionary<Room, Image> roomIcons = new();
    private Dictionary<(Room, Room), Image> connectorIcons = new();
    private Room currentRoom;

    private void OnEnable() {
        RoomManager.OnDungeonLoaded += InitializeMinimap;
        RoomManager.OnRoomEntered += EnterRoom;
    }

    private void OnDisable() {
        RoomManager.OnDungeonLoaded -= InitializeMinimap;
        RoomManager.OnRoomEntered -= EnterRoom;
    }

    private void InitializeMinimap(Room spawnRoom) {
        ClearMinimap();
        BuildGridCoords(spawnRoom);
        CreateIcons();
        EnterRoom(spawnRoom);
    }

    private void EnterRoom(Room room) {
        if (currentRoom != null && roomIcons.TryGetValue(currentRoom, out Image previousIcon)) {
            previousIcon.color = visitedRoomColor;
        }

        currentRoom = room;

        if (roomIcons.TryGetValue(room, out Image icon)) {
            icon.gameObject.SetActive(true);
            icon.color = currentRoomColor;
            mapContainer.anchoredPosition = -icon.rectTransform.anchoredPosition;
        }

        foreach (Room neighbor in room.Neighbors.Values) {
            if (roomIcons.TryGetValue(neighbor, out Image neighborIcon) && !neighborIcon.gameObject.activeSelf) {
                neighborIcon.gameObject.SetActive(true);
                neighborIcon.color = discoveredRoomColor;
            }

            RevealConnector(room, neighbor);
        }
    }

    private void ClearMinimap() {
        for (int i = mapContainer.childCount - 1; i >= 0; i--) {
            Destroy(mapContainer.GetChild(i).gameObject);
        }

        gridCoords.Clear();
        roomIcons.Clear();
        connectorIcons.Clear();
        currentRoom = null;
    }

    private void BuildGridCoords(Room spawnRoom) {
        Queue<(Room room, Vector2Int coord)> queue = new();
        queue.Enqueue((spawnRoom, Vector2Int.zero));
        gridCoords[spawnRoom] = Vector2Int.zero;

        while (queue.Count > 0) {
            (Room current, Vector2Int coord) = queue.Dequeue();
            foreach (KeyValuePair<Direction, Room> pair in current.Neighbors) {
                if (pair.Value == null || gridCoords.ContainsKey(pair.Value)) {
                    continue;
                }

                Vector2Int neighborCoord = coord + DirectionToGridOffset(pair.Key);
                gridCoords[pair.Value] = neighborCoord;
                queue.Enqueue((pair.Value, neighborCoord));
            }
        }
    }

    private void CreateIcons() {
        int step = cellSize + connectorLength;

        foreach (KeyValuePair<Room, Vector2Int> pair in gridCoords) {
            Room room = pair.Key;
            Vector2 position = new(pair.Value.x * step, pair.Value.y * step);
            CreateRoomIcon(room, position);
            CreateConnectorIcons(room, pair.Value, step, Vector2.zero);
        }
    }

    private void CreateRoomIcon(Room room, Vector2 position) {
        GameObject iconObject = new("RoomIcon");
        iconObject.transform.SetParent(mapContainer, false);

        Image image = iconObject.AddComponent<Image>();
        image.color = discoveredRoomColor;

        RectTransform rectTransform = iconObject.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(cellSize, cellSize);
        rectTransform.anchoredPosition = position;

        iconObject.SetActive(false);
        roomIcons[room] = image;
    }

    private void CreateConnectorIcons(Room room, Vector2Int coord, int step, Vector2 center) {
        foreach (KeyValuePair<Direction, Room> pair in room.Neighbors) {
            // Only create connector once per pair (East and North side only)
            if (pair.Key != Direction.East && pair.Key != Direction.North) {
                continue;
            }

            if (!gridCoords.ContainsKey(pair.Value)) {
                continue;
            }

            Room neighbor = pair.Value;
            Vector2Int neighborCoord = gridCoords[neighbor];
            Vector2 posA = new Vector2(coord.x * step, coord.y * step) - center;
            Vector2 posB = new Vector2(neighborCoord.x * step, neighborCoord.y * step) - center;

            GameObject connObject = new("Connector");
            connObject.transform.SetParent(mapContainer, false);

            Image connImage = connObject.AddComponent<Image>();
            connImage.color = connectorColor;

            RectTransform connRect = connObject.GetComponent<RectTransform>();
            connRect.anchoredPosition = (posA + posB) * 0.5f;

            bool isHorizontal = pair.Key == Direction.East;
            connRect.sizeDelta = isHorizontal
                ? new Vector2(connectorLength, connectorThickness)
                : new Vector2(connectorThickness, connectorLength);

            connObject.SetActive(false);
            connectorIcons[(room, neighbor)] = connImage;
        }
    }

    private void RevealConnector(Room roomA, Room roomB) {
        if (connectorIcons.TryGetValue((roomA, roomB), out Image connector)
            || connectorIcons.TryGetValue((roomB, roomA), out connector)) {
            connector.gameObject.SetActive(true);
            connector.color = connectorColor;
        }
    }

    private static Vector2Int DirectionToGridOffset(Direction direction) {
        return direction switch {
            Direction.North => new Vector2Int(0, 1),
            Direction.South => new Vector2Int(0, -1),
            Direction.East => new Vector2Int(1, 0),
            Direction.West => new Vector2Int(-1, 0),
            _ => Vector2Int.zero,
        };
    }
}
