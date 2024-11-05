using UnityEngine;

public class DefeatOnCollision : MonoBehaviour
{
    public Animator animator; // Ссылка на компонент анимации

    private void OnTriggerEnter(Collider other)
    {
        // Проверка, что триггер активирован объектом с тегом "Weapon"
        if (other.gameObject.tag.Contains("Weapon"))
        {
            // Проигрываем анимацию "Defeat"
            animator.Play("Defeat");
Debug.Log("d");
            // Запускаем задержку для уничтожения объекта
            Invoke("DestroyObject", 1f); // Уничтожаем объект через 1 секунду
        }
    }

    // Метод для уничтожения объекта
    private void DestroyObject()
    {
        Destroy(gameObject);
    }
}
