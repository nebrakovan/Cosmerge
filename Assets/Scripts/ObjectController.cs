using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

public class ObjectController : MonoBehaviour
{
    [Header("������ �������, ������� ����� ����������� ����� �������")]
    [SerializeField] private GameObject mergedObjectPrefab; // ������� ����������� ������������� �������

    [Header("������ ����� ������� � ����������")]
    [SerializeField] private int maxIndex; // ������������ ������
    public int objectIndex; // ������ �������

    [HideInInspector] public bool isMerging = false; // ���������� �������

    void OnCollisionEnter2D(Collision2D collision)
    {
        ObjectController otherObjectController = collision.gameObject.GetComponent<ObjectController>();

        if (CanMergeWith(otherObjectController))
        {
            StartMerging();
            otherObjectController.StartMerging();
            MergeObjects(collision.gameObject, otherObjectController);
        }
    }

    private bool CanMergeWith(ObjectController otherObjectController) // ����� ������������ �..
    {
        return otherObjectController != null &&
               objectIndex == otherObjectController.objectIndex &&
               objectIndex < maxIndex &&
               !isMerging &&
               !otherObjectController.isMerging;
    }

    private void StartMerging() // ������ �������
    {
        isMerging = true;
    }

    private void MergeObjects(GameObject otherObject, ObjectController otherObjectController) // ���������� �������
    {
        Vector3 spawnPosition = (transform.position + otherObject.transform.position) / 2;
        int nextIndex = objectIndex + 1;

        ScoreManager.Instance.AddScore(5, spawnPosition);

        GameObject mergedObject = ObjectPool.Instance.GetObject(nextIndex);
        ObjectController mergedObjectController = mergedObject.GetComponent<ObjectController>();
        mergedObjectController.isMerging = false;

        mergedObject.transform.position = spawnPosition;
        ActivateObject(mergedObject);

        DeactivateAndReturnObject(this);
        DeactivateAndReturnObject(otherObjectController);
    }

    private void ActivateObject(GameObject obj) // ������������ ������
    {
        var rb = obj.GetComponent<Rigidbody2D>();
        var collider = obj.GetComponent<CircleCollider2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        collider.enabled = true;
        obj.SetActive(true);
    }

    private void DeactivateAndReturnObject(ObjectController objController) // �������������� � ������� ������
    {
        var gameObject = objController.gameObject;
        var rb = gameObject.GetComponent<Rigidbody2D>();
        var collider = gameObject.GetComponent<CircleCollider2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        collider.enabled = false;
        gameObject.SetActive(false);
        ObjectPool.Instance.ReturnObject(gameObject, objController.objectIndex);
    }
}
