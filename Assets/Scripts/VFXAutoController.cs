using UnityEngine;

public class VFXAutoController : MonoBehaviour {
    [Header("Destroy info")]
    [SerializeField] private bool autoDestroy = true;
    [SerializeField] private float destroyDelay = 1;

    [Header("Random offset info")]
    [SerializeField] private bool randomOffset = true;
    [Space]
    [SerializeField] private float xMinOffset = -0.5f;
    [SerializeField] private float xMaxOffset = 0.5f;
    [Space]
    [SerializeField] private float yMinOffset = -0.5f;
    [SerializeField] private float yMaxOffset = 0.5f;

    [Header("Random rotation info")]
    [SerializeField] private bool randomRotation = true;
    [Space]
    [SerializeField] private float zMinRotation = -180;
    [SerializeField] private float zMaxRotation = 180;


    private void Start() {
        ApplyRandomOffset();
        ApplyRandomRotation();

        if (autoDestroy) {
            Destroy(gameObject, destroyDelay);
        }
    }

    private void ApplyRandomOffset() {
        if (!randomOffset) {
            return;
        }

        float xOffset = Random.Range(xMinOffset, xMaxOffset);
        float yOffset = Random.Range(yMinOffset, yMaxOffset);
        transform.position += new Vector3(xOffset, yOffset, 0);
    }

    private void ApplyRandomRotation() {
        if (!randomRotation) {
            return;
        }

        float zRotation = Random.Range(zMinRotation, zMaxRotation);
        transform.Rotate(0, 0, zRotation);
    }
}
