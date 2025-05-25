using UnityEngine;

public abstract class DungeonSubGeneratorBase : MonoBehaviour {
    protected DungeonGenerationContext context;

    public virtual void SetContext(DungeonGenerationContext context) {
        this.context = context;
    }

    public abstract void Run();
}
