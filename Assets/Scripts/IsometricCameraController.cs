using UnityEngine;
using UniRx;

public class IsometricCameraController : MonoBehaviour
{
    public Transform player;  // Ссылка на объект игрока
    public Vector3 offset = new Vector3(0f, 10f, -10f); // Смещение для изометрического вида
    public float smoothTime = 0.2f;  // Время плавного следования камеры

    private Vector3 velocity = Vector3.zero;

    private void Start()
    {
        // Наблюдаем за позицией игрока и обновляем позицию камеры реактивно
        Observable.EveryLateUpdate()
            .Where(_ => player != null) // Проверяем, что объект игрока существует
            .Subscribe(_ =>
            {
                // Рассчитываем целевую позицию камеры
                Vector3 targetPosition = player.position + offset;

                // Плавно перемещаем камеру к целевой позиции
                transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
            })
            .AddTo(this); // Автоматически отписываемся при уничтожении объекта
    }
}
