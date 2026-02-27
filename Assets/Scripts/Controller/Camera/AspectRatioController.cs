using UnityEngine;

public class AspectRatioController : MonoBehaviour {
    private const float TargetAspectRatio = 16f / 9f;

    private Camera targetCamera;
    private int lastScreenWidth;
    private int lastScreenHeight;

    private void Awake() {
        targetCamera = GetComponent<Camera>();

        if (targetCamera == null) {
            Debug.LogError("AspectRatioController requires a Camera component!");
            enabled = false;
            return;
        }

        UpdateScreenSize();
        EnforceAspectRatio();
    }

    private void Update() {
        if (HasScreenSizeChanged()) {
            UpdateScreenSize();
            EnforceAspectRatio();
        }
    }

    private void UpdateScreenSize() {
        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;
    }

    private bool HasScreenSizeChanged() {
        return Screen.width != lastScreenWidth || Screen.height != lastScreenHeight;
    }

    private void EnforceAspectRatio() {
        float currentAspectRatio = GetCurrentAspectRatio();

        if (IsAspectRatioCorrect(currentAspectRatio)) {
            SetFullScreenRect();
            return;
        }

        if (IsScreenWiderThanTarget(currentAspectRatio)) {
            ApplyHorizontalCrop(currentAspectRatio);
        } else {
            ApplyVerticalCrop(currentAspectRatio);
        }
    }

    private float GetCurrentAspectRatio() {
        return (float) Screen.width / Screen.height;
    }

    private bool IsAspectRatioCorrect(float currentAspectRatio) {
        return Mathf.Approximately(currentAspectRatio, TargetAspectRatio);
    }

    private bool IsScreenWiderThanTarget(float currentAspectRatio) {
        return currentAspectRatio > TargetAspectRatio;
    }

    private void SetFullScreenRect() {
        targetCamera.rect = new Rect(0f, 0f, 1f, 1f);
    }

    private void ApplyHorizontalCrop(float currentAspectRatio) {
        float widthScale = TargetAspectRatio / currentAspectRatio;
        float horizontalOffset = (1f - widthScale) * 0.5f;

        targetCamera.rect = new Rect(horizontalOffset, 0f, widthScale, 1f);
    }

    private void ApplyVerticalCrop(float currentAspectRatio) {
        float heightScale = currentAspectRatio / TargetAspectRatio;
        float verticalOffset = (1f - heightScale) * 0.5f;

        targetCamera.rect = new Rect(0f, verticalOffset, 1f, heightScale);
    }
}
