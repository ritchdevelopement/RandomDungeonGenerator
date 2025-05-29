using UnityEngine;

public abstract class DungeonTaskBase : MonoBehaviour {
    protected DungeonGenerationContext context;

    public virtual void SetContext(DungeonGenerationContext context) {
        this.context = context;
    }

    public abstract void Execute();
}
