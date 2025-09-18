using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Ссылка на игрока
    public Vector3 offset = new Vector3(0, 0, -10); // Смещение камеры
    public float smoothSpeed = 0.125f; // Скорость сглаживания (меньше = плавнее)
    public float speedZoomFactor = 0.1f; // Увеличение поля зрения при высокой скорости
    private Camera cam;
    private float baseOrthoSize; // Базовый размер камеры
    private Vector2 lastPlayerPosition; // Последняя позиция игрока
    private float maxSpeed = 10f; // Максимальная скорость игрока для расчета зума

    void Start()
    {
        cam = GetComponent<Camera>();
        baseOrthoSize = cam.orthographicSize; // Сохраняем базовый размер камеры
        if (target != null)
            lastPlayerPosition = target.position;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Вычисляем текущую скорость игрока
        Vector2 currentPosition = target.position;
        float playerSpeed = (currentPosition - lastPlayerPosition).magnitude / Time.deltaTime;
        lastPlayerPosition = currentPosition;

        // Плавное следование за игроком
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // Динамический зум камеры в зависимости от скорости
        float speedRatio = Mathf.Clamp(playerSpeed / maxSpeed, 0f, 1f);
        cam.orthographicSize = Mathf.Lerp(baseOrthoSize, baseOrthoSize * 1.2f, speedRatio * speedZoomFactor);
    }
}