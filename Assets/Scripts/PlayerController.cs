using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameObject[] objects;
    [SerializeField] private Vector2 objectSpawnXClamp = new Vector2(-5f, 5f);
    [SerializeField] private Vector2 objectSpawnYZ = new Vector2(0f, 0f);
    [SerializeField] private Image nextObjectImage;
    [SerializeField] private float objectMoveSpeed = 1f;
    [SerializeField] private float spawnDelay = 1f; // Задержка после спавна следующего объекта

    private GameObject currentObject;
    private GameObject nextObject;
    private int selectedIndex;
    private float[] weights;
    private float totalWeight;

    private bool canSpawnNext = true;
    private Vector3 spawnPosition;
    private SpriteRenderer nextObjectSpriteRenderer;

    void Awake()
    {
        InitializeObjectWeights();
        SelectRandomObject();
        UpdateNextObjectView();
    }

    private void InitializeObjectWeights()
    {
        int halfLength = objects.Length / 2;
        weights = new float[halfLength];
        totalWeight = 0;

        for (int i = 0; i < halfLength; i++)
        {
            weights[i] = 1f / (i + 1);
            totalWeight += weights[i];
        }
    }

    private void SelectRandomObject()
    {
        float randomValue = Random.value * totalWeight;
        float currentSum = 0;

        for (int i = 0; i < weights.Length; i++)
        {
            currentSum += weights[i];
            if (randomValue <= currentSum)
            {
                selectedIndex = i;
                break;
            }
        }

        nextObject = objects[selectedIndex];
        nextObjectSpriteRenderer = nextObject.GetComponent<SpriteRenderer>();
    }

    private void UpdateNextObjectView()
    {
        nextObjectImage.sprite = nextObjectSpriteRenderer.sprite;
        nextObjectImage.color = nextObjectSpriteRenderer.color;
    }

    void Update()
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

        // Отключаем возможность спавна и запускаем корутину для задержки
        canSpawnNext = false;
        StartCoroutine(EnableSpawnAfterDelay());
    }

    private Vector3 GetObjectSpawnPosition()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
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
}
