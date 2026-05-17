using UnityEngine;

public class ParallaxBackground : MonoBehaviour {
    private Camera mainCamera;
    private float lastCameraPositionX;
    private float cameraHalfWidth;
    
    [SerializeField] private ParallaxLayer[] backGroundLayer;

    private void Awake() {
        mainCamera = Camera.main;
        cameraHalfWidth = mainCamera.orthographicSize * mainCamera.aspect;
        CalculateImageWidth();
    }

    private void FixedUpdate() {
        float currentCameraPositionX = mainCamera.transform.position.x;
        float distanceToMove = currentCameraPositionX - lastCameraPositionX;
        lastCameraPositionX = currentCameraPositionX;

        float cameraLeftEdge = currentCameraPositionX - cameraHalfWidth;
        float cameraRightEdge = currentCameraPositionX + cameraHalfWidth;

        foreach (ParallaxLayer layer in backGroundLayer) {
            layer.Move(distanceToMove);
            layer.LoopBackground(cameraLeftEdge, cameraRightEdge);
        }
    }

    private void CalculateImageWidth() {
        foreach (ParallaxLayer layer in backGroundLayer) {
            layer.CalculateImageWidth();
        }
    }
}
