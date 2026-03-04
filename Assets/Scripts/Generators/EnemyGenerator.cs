using UnityEngine;

public class EnemyGenerator : DungeonTaskBase {
    [SerializeField] private EnemyData[] enemyData;

    public override void Execute() {
        if (enemyData == null || enemyData.Length == 0) {
            Debug.LogError("No EnemyData assigned to EnemyGenerator!");
            return;
        }

        GameObject enemiesGameObject = new GameObject("Enemies");
        enemiesGameObject.transform.SetParent(context.dungeonGameObject.transform);
        EnemyManager enemyManager = enemiesGameObject.AddComponent<EnemyManager>();

        enemyManager.SetDungeonContext(context);
        enemyManager.SetEnemyData(enemyData);
    }
}
