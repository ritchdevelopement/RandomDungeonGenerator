using UnityEngine;

public class CameraController : MonoBehaviour {
    [Header("Follow Settings")]

    [SerializeField]
    private Transform playerTransform;
    private Vector3 offset = new Vector3(0, 0, -10f);

    private void LateUpdate() {
        if (playerTransform == null) {
            return;
        }

        FollowPlayer();
    }

    private void FollowPlayer() {
        Vector3 targetPosition = playerTransform.position + offset;
        transform.position = targetPosition;
    }

    public void SetPlayer(Transform newPlayer) {
        playerTransform = newPlayer;
    }
}
