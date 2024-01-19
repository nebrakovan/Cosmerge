using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

public class ObjectController : MonoBehaviour
{
    [Header("Префаб объекта, который будет создаваться после слияния")]
    [SerializeField] private GameObject mergedObjectPrefab; // Сборная конструкция объединенного объекта

    [Header("Индекс этого объекта и последнего")]
    [SerializeField] private int maxIndex; // Максимальный индекс
    public int objectIndex; // Индекс объекта

    [HideInInspector] public bool isMerging = false; // Происходит слияние

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

    private bool CanMergeWith(ObjectController otherObjectController) // Может объединяться с..
    {
        return otherObjectController != null &&
               objectIndex == otherObjectController.objectIndex &&
               objectIndex < maxIndex &&
               !isMerging &&
               !otherObjectController.isMerging;
    }

    private void StartMerging() // Начать слияние
    {
        isMerging = true;
    }

    private void MergeObjects(GameObject otherObject, ObjectController otherObjectController) // Объединить объекты
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

    private void ActivateObject(GameObject obj) // Активировать объект
    {
        var rb = obj.GetComponent<Rigidbody2D>();
        var collider = obj.GetComponent<CircleCollider2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        collider.enabled = true;
        obj.SetActive(true);
    }

    private void DeactivateAndReturnObject(ObjectController objController) // Деактивировать и вернуть объект
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
