using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Массив для объектов, которые будут спавниться")]
    [Tooltip("На самом деле этот массив нужен только для рассчета веса и отображения следующего объекта")]
    [SerializeField] private GameObject[] objects; // Объекты

    [Header("Ограничение места спавна")]
    [SerializeField] private Vector2 objectSpawnXClamp = new Vector2(-5f, 5f); // Зажим X для создания объекта
    [SerializeField] private Vector2 objectSpawnYZ = new Vector2(0f, 0f); // Зажим Y для создания объекта

    [Header("Картинка для отображения следующего объекта для спавна")]
    [SerializeField] private Image nextObjectImage;// Изображение следующего объекта

    [Header("Настройки спавна")]
    [SerializeField] private float objectMoveSpeed = 1f; // Скорость перемещения объекта
    [SerializeField] private float spawnDelay = 1f; // Задержка создания
    [SerializeField] private int excludedObjectsToSpawn = 1; // Исключенные объекты для создания

    private GameObject currentObject; // Текущий объект
    private GameObject nextObject; // Следующий объект
    private SpriteRenderer nextObjectSpriteRenderer; // Средство визуализации спрайтов следующего объекта
    private Camera mainCamera; // Главная камера
    private Vector3 spawnPosition; // Позиция появления

    private int selectedIndex; // Выбранный индекс
    private float[] weights; // Веса
    private float totalWeight; // Общий вес
    private bool canSpawnNext = true; //Может создать следующий


    void Awake()
    {
        InitializeObjectWeights();
        SelectRandomObject();
        UpdateNextObjectView();

        mainCamera = Camera.main;
    }

    private void InitializeObjectWeights()
    {
        int halfLength = objects.Length - excludedObjectsToSpawn;
        weights = new float[halfLength];
        totalWeight = 0;

        for (int i = 0; i < halfLength; i++)
        {
            weights[i] = 1f / (i + 1);
            totalWeight += weights[i];
        }
    }

    private void Update()
    {
        if (currentObject != null)
        {
            if (Input.GetMouseButton(0))
            {
                MoveObjectToCursor();
            }

            if (Input.GetMouseButtonUp(0))
            {
                ActivateObjectPhysics();
            }
        }

        if (currentObject == null && Input.GetMouseButtonDown(0) && canSpawnNext)
        {
            SpawnObject();
        }
    }

    private void SpawnObject()
    {
        spawnPosition = GetObjectSpawnPosition();
        currentObject = ObjectPool.Instance.GetObject(selectedIndex);
        currentObject.transform.position = spawnPosition;
        currentObject.SetActive(true);

        ObjectController currentObjectController = currentObject.GetComponent<ObjectController>();
        currentObjectController.isCombining = false;

        SelectRandomObject();
        UpdateNextObjectView();

        canSpawnNext = false;
        StartCoroutine(EnableSpawnAfterDelay());
    }

    private Vector3 GetObjectSpawnPosition()
    {
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.x = Mathf.Clamp(mousePos.x, objectSpawnXClamp.x, objectSpawnXClamp.y);
        mousePos.y = objectSpawnYZ.x;
        mousePos.z = objectSpawnYZ.y;
        return mousePos;
    }

    private void MoveObjectToCursor()
    {
        Vector3 currentMousePos = GetObjectSpawnPosition();
        currentObject.transform.position = Vector3.MoveTowards(currentObject.transform.position, currentMousePos, objectMoveSpeed * Time.deltaTime);
    }

    private void ActivateObjectPhysics()
    {
        Rigidbody2D rb = currentObject.GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        CircleCollider2D collider = currentObject.GetComponent<CircleCollider2D>();
        collider.enabled = true;

        currentObject = null;
    }

    private IEnumerator EnableSpawnAfterDelay()
    {
        yield return new WaitForSeconds(spawnDelay);
        canSpawnNext = true;
    }

    private void SelectRandomObject()
    {
        float randomValue = Random.value * totalWeight;
        float currentSum = 0;

        selectedIndex = weights.TakeWhile(weight => (currentSum += weight) < randomValue).Count();

        nextObject = objects[selectedIndex];
        nextObjectSpriteRenderer = nextObject.GetComponent<SpriteRenderer>();
    }

    private void UpdateNextObjectView()
    {
        nextObjectImage.sprite = nextObjectSpriteRenderer.sprite;
        nextObjectImage.color = nextObjectSpriteRenderer.color;
    }
}
