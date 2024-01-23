using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("������ ��� ��������, ������� ����� ����������")]
    [SerializeField] private GameObject[] objects; // �������

    [Header("����������� ����� ������")]
    [SerializeField] private Vector2 objectSpawnXClamp = new Vector2(-5f, 5f); // ����� X ��� �������� �������
    [SerializeField] private Vector2 objectSpawnYClamp = new Vector2(-5f, 5f); // ����� Y ��� �������� �������

    [Header("�������� ��� ����������� ���������� �������")]
    [SerializeField] private Image nextObjectImage;// ����������� ���������� �������

    [Header("��������� ������")]
    [SerializeField] private float objectMoveSpeed = 1f; // �������� ����������� �������
    [SerializeField] private float spawnDelay = 1f; // �������� ��������
    [SerializeField] private int excludedObjectsToSpawn = 1; // ����������� ������� ��� ��������

    private GameObject currentObject; // ������� ������
    private GameObject nextObject; // ��������� ������
    private SpriteRenderer nextObjectSpriteRenderer; // �������� ������������ �������� ���������� �������
    private Camera mainCamera; // ������� ������
    private Vector3 spawnPosition; // ������� ���������

    private int selectedIndex; // ��������� ������
    private float[] weights; // ����
    private float totalWeight; // ����� ���
    private bool canSpawnNext = true; //����� ������� ���������


    private void Awake()
    {
        InitializeObjectWeights();
        SelectRandomObject();
        UpdateNextObjectDisplay();

        mainCamera = Camera.main;
    }

    private void InitializeObjectWeights() // ���������������� ���� ��������
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
        if (currentObject == null && Input.GetMouseButtonDown(0) && canSpawnNext)
        {
            SpawnObject();
        }

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
    }

    private void SpawnObject() // �������� �������
    {
        spawnPosition = GetObjectSpawnPosition();
        currentObject = ObjectPool.Instance.GetObject(selectedIndex);
        currentObject.transform.position = spawnPosition;
        currentObject.SetActive(true);

        ObjectController currentObjectController = currentObject.GetComponent<ObjectController>();
        currentObjectController.isCombining = false;

        SelectRandomObject();
        UpdateNextObjectDisplay();

        canSpawnNext = false;
        StartCoroutine(EnableSpawnAfterDelay());
    }

    private Vector3 GetObjectSpawnPosition() // �������� �������� ��� ��������
    {
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.x = Mathf.Clamp(mousePos.x, objectSpawnXClamp.x, objectSpawnXClamp.y);
        mousePos.y = Mathf.Clamp(mousePos.y, transform.position.y + objectSpawnYClamp.x, transform.position.y + objectSpawnYClamp.y);
        mousePos.z = transform.position.z;
        return mousePos;
    }

    private IEnumerator EnableSpawnAfterDelay() // �������� �������� ����� ��������
    {
        yield return new WaitForSeconds(spawnDelay);
        canSpawnNext = true;
    }

    private void SelectRandomObject() // ������� ��������� ������
    {
        float randomValue = Random.value * totalWeight;
        float currentSum = 0;

        selectedIndex = weights.TakeWhile(weight => (currentSum += weight) < randomValue).Count();

        nextObject = objects[selectedIndex];
        nextObjectSpriteRenderer = nextObject.GetComponent<SpriteRenderer>();
    }

    private void UpdateNextObjectDisplay() // �������� ����������� ���������� �������
    {
        nextObjectImage.sprite = nextObjectSpriteRenderer.sprite;
        nextObjectImage.color = nextObjectSpriteRenderer.color;
    }

    private void MoveObjectToCursor() // ����������� ������ � �������
    {
        Vector3 currentMousePos = GetObjectSpawnPosition();
        currentObject.transform.position = Vector3.MoveTowards(currentObject.transform.position, currentMousePos, objectMoveSpeed * Time.deltaTime);
    }

    private void ActivateObjectPhysics() // ������������ ������ �������
    {
        Rigidbody2D rb = currentObject.GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        CircleCollider2D collider = currentObject.GetComponent<CircleCollider2D>();
        collider.enabled = true;

        currentObject = null;
    }

}
