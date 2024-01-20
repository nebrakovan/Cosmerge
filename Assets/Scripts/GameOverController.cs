using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverController : MonoBehaviour
{
    public static GameOverController Instance { get; private set; }

    public Transform raycastOrigin; // Точка, откуда будет исходить луч
    public float raycastLength = 10f; // Длина луча
    public float timeToLose = 5f; // Время, после которого проигрывает игрок
    [SerializeField]
    private LayerMask objectLayer; // Слой, на котором находятся игровые объекты
    private float timer = 0f;
    private GameObject currentObject = null;

    [SerializeField] private Vector2 currentBorders;
    [SerializeField] private Vector2 currentPosition;

    [SerializeField]private GameObject boundaryObject; // GameObject, который представляет коллайдер границ
    private BoxCollider2D boundaryCollider; // Коллайдер границ

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        boundaryCollider = boundaryObject.GetComponent<BoxCollider2D>();
        if (boundaryCollider == null)
        {
            // Если нет, добавляем его
            boundaryCollider = boundaryObject.AddComponent<BoxCollider2D>();
        }
    }

    void Update()
    {
        CheckObjectUnderRay();

        SetBoundaryPositionAndSize(currentPosition, currentBorders);

    }

    private void CheckObjectUnderRay()
    {
        raycastOrigin.position = new Vector3(boundaryCollider.bounds.min.x, boundaryCollider.bounds.max.y - 1f, raycastOrigin.position.z);
        RaycastHit2D hit = Physics2D.Raycast(raycastOrigin.position, Vector2.right, raycastLength, objectLayer);

        if (hit.collider != null)
        {
            if (currentObject == null)
            {
                currentObject = hit.collider.gameObject;
            }

            if (currentObject == hit.collider.gameObject)
            {
                timer += Time.deltaTime;
                if (timer >= timeToLose)
                {
                    TriggerLoss("Проигрыш: объект задержался под лучом слишком долго!");
                    return;
                }
            }
        }
        else
        {
            ResetTimer();
        }

        Debug.DrawRay(raycastOrigin.position, Vector2.right * raycastLength, Color.red); // Отрисовка луча для отладки
    }

    private void ResetTimer()
    {
        currentObject = null;
        timer = 0f;
    }

    public void TriggerLoss(string message)
    {
        Debug.Log(message);
        // Здесь может быть код для перезагрузки уровня или отображения экрана проигрыша
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player")) // Замените "Game Object Tag" на тег вашего игрового объекта
        {
            TriggerLoss("Проигрыш: объект покинул емкость!");
        }
    }

    public void SetBoundaryPositionAndSize(Vector2 newPosition, Vector2 newSize)
    {
        boundaryObject.transform.position = newPosition;
        boundaryCollider.size = newSize;
        boundaryCollider.offset = Vector2.zero; // Центр коллайдера всегда в позиции объекта
    }
}
