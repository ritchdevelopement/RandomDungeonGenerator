using UnityEngine;

public class StartScreenUI : MonoBehaviour {
    [SerializeField] private GameObject startPanel;
    [SerializeField] private DungeonComposer dungeonComposer;

    private void Awake() {
        Time.timeScale = 1f;
        startPanel.SetActive(true);
    }

    public void StartGame() {
        startPanel.SetActive(false);
        dungeonComposer.ComposeDungeon();
    }
}
