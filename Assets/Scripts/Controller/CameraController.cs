using UnityEngine;

public class CameraController : MonoBehaviour {
    [Header("Follow Settings")]

    [SerializeField]
    private Transform playerTransform;

    [Header("Smoothing")]

    [SerializeField]
    private bool useSmoothFollow = false;

    [SerializeField]
    private float smoothTime = 0.3f;

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
        Vector3 targetPosition = playerTransform.position;

        if(useSmoothFollow) {
            transform.position = Vector3.SmoothDamp(
                transform.position,
                targetPosition,
                ref velocity,
                smoothTime
            );
        } else {
            transform.position = targetPosition;
            velocity = Vector3.zero;
        }
    }

    public void SetPlayer(Transform newPlayer) {
        playerTransform = newPlayer;
    }
}
