using UnityEngine;

public class CameraController : MonoBehaviour {
    private Transform playerTransform;
    private Vector3 offset = new Vector3(0, 0, -10f);

    private void LateUpdate() {
        if (playerTransform == null) {
            TryFindPlayer();
        }

        if (playerTransform == null) {
            return;
        }

        transform.position = playerTransform.position + offset;
    }

    private void TryFindPlayer() {
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) {
            return;
        }

        playerTransform = player.transform;
    }
}
