using UnityEngine;
using Cinemachine;

public class CameraShake : CinemachineExtension {
    public static CameraShake Instance { get; private set; }

    private float shakeIntensity;
    private float shakeTimer;
    private float totalShakeTime;
    private float startingIntensity;

    protected override void Awake() {
        base.Awake();
        if (Instance == null) {
            Instance = this;
        }
    }

    public void Shake(float intensity, float time) {
        shakeIntensity = intensity;
        startingIntensity = intensity;
        shakeTimer = time;
        totalShakeTime = time;
    }

    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage, ref CameraState state, float deltaTime) {
        
        if (stage == CinemachineCore.Stage.Body) {
            if (shakeTimer > 0) {
                shakeTimer -= deltaTime;
                
                float currentIntensity = Mathf.Lerp(startingIntensity, 0f, 1 - (shakeTimer / totalShakeTime));
                
                // 카메라의 로컬축(가로, 세로)을 기준으로 흔들림 오프셋 계산
                Vector3 shakeOffset = (state.RawOrientation * Vector3.right * Random.Range(-1f, 1f) + 
                                       state.RawOrientation * Vector3.up * Random.Range(-1f, 1f)) * currentIntensity;

                state.RawPosition += shakeOffset;
            }
        }
    }
}
