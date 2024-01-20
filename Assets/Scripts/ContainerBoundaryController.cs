using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class ContainerBoundaryController : MonoBehaviour
{
    public static ContainerBoundaryController Instance { get; private set; }

    [Header("Объект с границами")]
    [SerializeField] private GameObject boundaryObject; // Объект границ

    [Header("Настройки границ емкости")]
    [SerializeField] private Vector2 currentBorders; // Текущие границы
    [SerializeField] private Vector2 currentBordersPosition; // Текущая позиция границ

    [Header("Слой игрового объекта")]
    [SerializeField] private LayerMask objectLayer; // Слой объекта

    [Header("Время для проигрыша")]
    [SerializeField] private float timeToLose = 5f; // Время для проигрыша

    private HashSet<Collider2D> outsideObjects = new HashSet<Collider2D>(); // Объекты снаружи
    private BoxCollider2D boundaryCollider; // Коллайдер границ

    private float timer = 0f; // Таймер
    private bool isOutside = false; // Снаружи

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
            boundaryCollider = boundaryObject.AddComponent<BoxCollider2D>();
        }

        SetBoundaryPositionAndSize(currentBordersPosition, currentBorders);

    }

    private void Update()
    {
        if (isOutside)
        {
            timer += Time.deltaTime;
            if (timer >= timeToLose)
            {
                GameManager.Instance.GameOver();
            }
        }
        else
        {
            timer = 0f;
        }

        isOutside = outsideObjects.Count > 0;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enabled"))
        {
            isOutside = true;
            outsideObjects.Add(collision);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enabled") && outsideObjects.Contains(collision))
        {
            outsideObjects.Remove(collision);
        }
    }

    public void SetBoundaryPositionAndSize(Vector2 newPosition, Vector2 newSize) // Установить размер границ и позицию
    {
        boundaryObject.transform.position = newPosition;
        boundaryCollider.size = newSize;
        boundaryCollider.offset = Vector2.zero;
    }
}
