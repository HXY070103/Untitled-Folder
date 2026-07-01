using UnityEngine;

public class TimeController {
    public float time { get; private set; } = 0;
    public float localTimeScale { get; private set; } = 1;
    private bool isActive = true;

    public void Initialize() {
        time = 0;
        localTimeScale = 1;
    }

    public void Tick() {
        if (!isActive) {
            return;
        }

        time += Time.deltaTime * localTimeScale;
    }

    public void SetLocalTimeScale(float newTimeScale) {
        localTimeScale = newTimeScale;
    }

    public void ResetLocalTimeScale() {
        localTimeScale = 1;
    }

    public void SwitchOn() {
        isActive = true;
    }

    public void SwitchOff() {
        isActive = false;
    }
}
