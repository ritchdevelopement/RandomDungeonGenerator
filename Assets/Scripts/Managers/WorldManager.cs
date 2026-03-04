using UnityEngine;

public class WorldManager : MonoBehaviour {
    public static WorldManager Instance { get; private set; }

    [SerializeField] private int currentWorld = 1;
    [SerializeField] private int currentLevel = 1;

    public int CurrentWorld => currentWorld;
    public int CurrentLevel => currentLevel;
    public EnemyFaction ActiveFaction => WorldToFaction(currentWorld);

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public void AdvanceLevel() {
        if (currentLevel < 10) {
            currentLevel++;
        } else {
            currentLevel = 1;
            currentWorld++;
        }
    }

    private static EnemyFaction WorldToFaction(int world) => world switch {
        1 => EnemyFaction.Undead,
        2 => EnemyFaction.Orc,
        3 => EnemyFaction.Demon,
        _ => EnemyFaction.Undead
    };
}
