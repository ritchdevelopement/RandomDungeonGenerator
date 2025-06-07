using UnityEngine;

public class EnemyGenerator : DungeonTaskBase {
    [SerializeField] private GameObject enemyPrefab;

    public override void Execute() {
        if(enemyPrefab == null) {
            Debug.LogError("Enemy prefab not assigned!");
            return;
        }

        GameObject enemiesGameObject = new GameObject("Enemies");
        enemiesGameObject.transform.SetParent(context.dungeonGameObject.transform);
        EnemyManager enemyManager = enemiesGameObject.AddComponent<EnemyManager>();

        enemyManager.SetDungeonContext(context);
        enemyManager.SetEnemyPrefab(enemyPrefab);

        Debug.Log("Enemy manager initialized");
    }
}
