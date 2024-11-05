using UnityEngine;
using UniRx;
using UnityEngine.UI;
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public float rotationSmoothTime = 0.12f;
    public float speedChangeRate = 10.0f;
    public Button attackButton; // Ссылка на UI-кнопку для атаки
    public Transform weaponOriginalPosition;
    private float targetRotation = 0.0f;
    private float rotationVelocity;
    private float currentSpeed = 0.0f;
    private Vector2 joystickInput = Vector2.zero;
    private Rigidbody playerRigidbody;
    private Animator animator;
    public GameObject weapon; // Оружие, которое нужно присоединить
    public Transform hand;    // Рука, к которой присоединяется оружие
    [SerializeField]
    private JoystickController joystickController; 

    private void Start()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>(); 

        Observable.EveryFixedUpdate()
            .Subscribe(_ =>
            {
                float animationSpeed = currentSpeed / moveSpeed; // Пропорция текущей скорости к максимальной скорости
                animator.SetFloat("Speed", animationSpeed); // Обновляем параметр "Speed" в аниматоре
            })
            .AddTo(this);
        joystickController.MoveDirection
            .Subscribe(direction =>
            {
                joystickInput = direction;
                UpdateAnimation(); 
            })
            .AddTo(this);
         // Подписка на событие нажатия кнопки атаки
        attackButton.OnClickAsObservable()
            .Subscribe(_ =>
            {
                animator.Play("Attack"); // Проигрываем анимацию с именем "Attack"
            })
            .AddTo(this);

       
            
    }
    
     private void Update()
    {
       

        // Проверка, воспроизводится ли анимация атаки
        bool isAttacking = animator.GetCurrentAnimatorStateInfo(0).IsName("Attack");

        if (isAttacking)
        {
            // Устанавливаем оружие в руку, если атака
            EquipWeapon();
        }
        else
        {
            // Возвращаем оружие в исходное положение, если атака завершена
            UnequipWeapon();
        }
    }
    private void FixedUpdate()
    {
        Move();
    }
  private void EquipWeapon()
    {
        if (weapon.transform.parent != hand)
        {
            weapon.transform.SetParent(hand);
            weapon.transform.localPosition = Vector3.zero;
            weapon.transform.localRotation = Quaternion.Euler(0, 180, 0); // Поворот на 180 градусов
        }
    }

    private void UnequipWeapon()
    {
        if (weapon.transform.parent != weaponOriginalPosition.parent)
        {
            weapon.transform.SetParent(weaponOriginalPosition.parent);
            weapon.transform.position = weaponOriginalPosition.position;
            weapon.transform.rotation = weaponOriginalPosition.rotation;
        }
    }
private void Move()
    {
        float targetSpeed = joystickInput.magnitude * moveSpeed;

        // Если джойстик не отклонен, мгновенно остановить игрока
        if (joystickInput == Vector2.zero)
        {
            targetSpeed = 0.0f;
            currentSpeed = 0.0f;
        }
        else
        {
            // Плавное изменение скорости при отклонении джойстика
            currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * speedChangeRate);
        }

        // Нормализуем направление движения
        Vector3 inputDirection = new Vector3(joystickInput.x, 0.0f, joystickInput.y).normalized;

        // Поворот игрока в направлении движения
        if (joystickInput != Vector2.zero)
        {
            targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }

        // Движение игрока вперед относительно текущего поворота
        Vector3 targetDirection = transform.forward * currentSpeed * Time.deltaTime;
        playerRigidbody.MovePosition(transform.position + targetDirection);
    }

    private void UpdateAnimation()
    {
        if (joystickInput.magnitude > 0.1f)
        {
           
            animator.Play("Run");
        }
        else
        {
           
            animator.Play("Idle");
        }
    }
}
