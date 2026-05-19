using UnityEngine;

public class FireVFXDetroyTrigger : MonoBehaviour {
    private void FireVFXEnd() {
        GetComponentInParent<FireVFXController>().Healing();
    }
}
