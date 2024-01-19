using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

public class ObjectController : MonoBehaviour
{
    [SerializeField]
    private GameObject combinedObjectPrefab; // Префаб объединенного объекта
    public int objectIndex; // Индекс объекта
    public bool isCombining = false; // Флаг, указывающий на процесс слияния
    [SerializeField]
    private int maxIndex; // Максимальный индекс объекта, который не может быть объединен

    void OnCollisionEnter2D(Collision2D collision)
    {
        ObjectController otherObjectController = collision.gameObject.GetComponent<ObjectController>();

        if (CanCombineWith(otherObjectController))
        {
            StartCombining();
            otherObjectController.StartCombining();
            CombineObjects(collision.gameObject, otherObjectController);
        }
    }

    private bool CanCombineWith(ObjectController otherObjectController)
    {
        // Проверка, можно ли объединиться с другим объектом
        return otherObjectController != null &&
               objectIndex == otherObjectController.objectIndex &&
               objectIndex < maxIndex && // Проверка, что индекс не максимальный
               !isCombining &&
               !otherObjectController.isCombining;
    }

    private void StartCombining()
    {
        isCombining = true;
    }

    private void CombineObjects(GameObject otherObject, ObjectController otherObjectController)
    {
        Vector3 spawnPosition = (transform.position + otherObject.transform.position) / 2;
        int nextIndex = objectIndex + 1; // Определение индекса для нового объекта

        ScoreManager.Instance.AddScore(5, spawnPosition);

        GameObject combinedObject = ObjectPool.Instance.GetObject(nextIndex);
        ObjectController combinedObjectController = combinedObject.GetComponent<ObjectController>();
        combinedObjectController.isCombining = false;

        combinedObject.transform.position = spawnPosition;
        ActivateObject(combinedObject);

        DeactivateAndReturnObject(this);
        DeactivateAndReturnObject(otherObjectController);
    }

    private void ActivateObject(GameObject obj)
    {
        var rb = obj.GetComponent<Rigidbody2D>();
        var collider = obj.GetComponent<CircleCollider2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        collider.enabled = true;
        obj.SetActive(true);
    }

    private void DeactivateAndReturnObject(ObjectController objController)
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
