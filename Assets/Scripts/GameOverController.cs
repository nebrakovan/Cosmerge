using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverController : MonoBehaviour
{
    public static GameOverController Instance { get; private set; }

    public Transform raycastOrigin; // �����, ������ ����� �������� ���
    public float raycastLength = 10f; // ����� ����
    public float timeToLose = 5f; // �����, ����� �������� ����������� �����
    [SerializeField]
    private LayerMask objectLayer; // ����, �� ������� ��������� ������� �������
    private float timer = 0f;
    private GameObject currentObject = null;

    [SerializeField] private Vector2 currentBorders;
    [SerializeField] private Vector2 currentPosition;

    [SerializeField]private GameObject boundaryObject; // GameObject, ������� ������������ ��������� ������
    private BoxCollider2D boundaryCollider; // ��������� ������

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
            // ���� ���, ��������� ���
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
                    TriggerLoss("��������: ������ ���������� ��� ����� ������� �����!");
                    return;
                }
            }
        }
        else
        {
            ResetTimer();
        }

        Debug.DrawRay(raycastOrigin.position, Vector2.right * raycastLength, Color.red); // ��������� ���� ��� �������
    }

    private void ResetTimer()
    {
        currentObject = null;
        timer = 0f;
    }

    public void TriggerLoss(string message)
    {
        Debug.Log(message);
        // ����� ����� ���� ��� ��� ������������ ������ ��� ����������� ������ ���������
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player")) // �������� "Game Object Tag" �� ��� ������ �������� �������
        {
            TriggerLoss("��������: ������ ������� �������!");
        }
    }

    public void SetBoundaryPositionAndSize(Vector2 newPosition, Vector2 newSize)
    {
        boundaryObject.transform.position = newPosition;
        boundaryCollider.size = newSize;
        boundaryCollider.offset = Vector2.zero; // ����� ���������� ������ � ������� �������
    }
}
