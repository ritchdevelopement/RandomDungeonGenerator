using UnityEngine;

public class EnemyGenerator : DungeonTaskBase {
    [SerializeField] private EnemyData[] enemyData;

    public override void Execute() {
        if (enemyData == null || enemyData.Length == 0) {
            Debug.LogError("No EnemyData assigned to EnemyGenerator!");
            return;
        }

        if (EnemyManager.Instance == null) {
            Debug.LogError("EnemyManager not found in scene!");
            return;
        }

        EnemyManager.Instance.SetDungeonContext(context);
        EnemyManager.Instance.SetEnemyData(enemyData);
    }
}
