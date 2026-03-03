using System.Collections;
using UnityEngine;

public class DashGhostTrail : MonoBehaviour {
    [SerializeField] private float ghostInterval = 0.03f;
    [SerializeField] private float ghostFadeDuration = 0.2f;
    [SerializeField] private Color ghostColor = new Color(0.5f, 0.8f, 1f, 0.7f);

    private SpriteRenderer playerSpriteRenderer;
    private Coroutine trailCoroutine;
    private WaitForSeconds ghostIntervalWait;

    private void Awake() {
        playerSpriteRenderer = GetComponent<SpriteRenderer>();
        ghostIntervalWait = new WaitForSeconds(ghostInterval);
    }

    public void StartTrail() {
        trailCoroutine = StartCoroutine(SpawnGhosts());
    }

    public void StopTrail() {
        if (trailCoroutine != null) {
            StopCoroutine(trailCoroutine);
            trailCoroutine = null;
        }
    }

    private IEnumerator SpawnGhosts() {
        while (true) {
            SpawnGhost();
            yield return ghostIntervalWait;
        }
    }

    private void SpawnGhost() {
        GameObject ghost = new GameObject("DashGhost");
        ghost.transform.position = transform.position;
        ghost.transform.localScale = transform.localScale;

        SpriteRenderer ghostRenderer = ghost.AddComponent<SpriteRenderer>();
        ghostRenderer.sprite = playerSpriteRenderer.sprite;
        ghostRenderer.flipX = playerSpriteRenderer.flipX;
        ghostRenderer.sortingLayerName = playerSpriteRenderer.sortingLayerName;
        ghostRenderer.sortingOrder = playerSpriteRenderer.sortingOrder - 1;
        ghostRenderer.color = ghostColor;

        StartCoroutine(FadeAndDestroyGhost(ghostRenderer));
    }

    private IEnumerator FadeAndDestroyGhost(SpriteRenderer ghostRenderer) {
        Color startColor = ghostRenderer.color;
        float elapsed = 0f;

        while (elapsed < ghostFadeDuration) {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startColor.a, 0f, elapsed / ghostFadeDuration);
            ghostRenderer.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        Destroy(ghostRenderer.gameObject);
    }
}
