using UnityEngine;

public class CameraController : MonoBehaviour {
    [Header("Follow Settings")]

    [SerializeField]
    private Transform playerTransform;
    private Vector3 offset = new Vector3(0, 0, -10f);
    private Vector3 velocity = Vector3.zero;

    private void Start() {
        if(playerTransform == null) {
            Debug.LogWarning("Player Transform nicht zugewiesen! Setze es manuell oder per SetPlayer()");
        }
    }

    private void LateUpdate() {
        if(playerTransform == null) {
            return;
        }

        FollowPlayer();
    }

    private void FollowPlayer() {
        Vector3 targetPosition = playerTransform.position + offset;
        transform.position = targetPosition;
        velocity = Vector3.zero;
    }

    public void SetPlayer(Transform newPlayer) {
        playerTransform = newPlayer;
    }
}
