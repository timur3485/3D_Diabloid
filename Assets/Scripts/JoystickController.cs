using UnityEngine;
using UniRx;           // Подключение UniRx
using System;          
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class JoystickController : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public RectTransform joystickBackground; // Внешний круг
    public RectTransform joystickHandle;     // Ручка джойстика
    public float joystickRadius = 100f;      // Радиус джойстика

    private Vector2 inputVector;
    private Subject<Vector2> moveDirection = new Subject<Vector2>(); // Создаем поток для направления

    public IObservable<Vector2> MoveDirection => moveDirection; // Открываем поток для подписчиков

    private void Start()
    {
        // Инициализация положения джойстика в центре фона
        joystickHandle.anchoredPosition = Vector2.zero;
        
        // При подписке на MoveDirection можно выполнять движения объекта
        MoveDirection
            .Subscribe(direction =>
            {
                // Здесь можно использовать direction для управления игроком
                Debug.Log($"Direction: {direction}");
            })
            .AddTo(this); // Подписка будет завершена при уничтожении объекта
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData); // Начинаем обработку касания
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        inputVector = Vector2.zero; // Сбрасываем значение при отпускании
        joystickHandle.anchoredPosition = Vector2.zero; // Возвращаем ручку в центр
        moveDirection.OnNext(inputVector); // Обновляем значение
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            joystickBackground, 
            eventData.position, 
            eventData.pressEventCamera, 
            out position);

        position = Vector2.ClampMagnitude(position, joystickRadius); // Ограничиваем движение в радиусе
        joystickHandle.anchoredPosition = position;

        // Нормализуем для получения направления
        inputVector = new Vector2(position.x / joystickRadius, position.y / joystickRadius);
        moveDirection.OnNext(inputVector); // Отправляем направление в поток
    }

    // Метод для доступа к направлению
    public Vector2 GetDirection()
    {
        return inputVector;
    }
}
