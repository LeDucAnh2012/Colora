using UnityEngine;

public class UIParticleScaler : MonoBehaviour
{
    public Camera uiCamera;
    private ParticleSystem uiParticleSystem;

    private Vector3 initialScale= Vector2.one;
    private float initialCameraSize=10;

    void Awake()
    {
        if (uiCamera == null)
        {
            uiCamera = Camera.main;
        }

        if (uiParticleSystem == null)
        {
            uiParticleSystem = GetComponent<ParticleSystem>();
        }

    }

    void Update()
    {
        // Tính toán hệ số scale dựa trên kích thước camera hiện tại so với ban đầu
        float scaleFactor = initialCameraSize / uiCamera.orthographicSize;
        uiParticleSystem.transform.localScale = initialScale * scaleFactor;
    }
}
