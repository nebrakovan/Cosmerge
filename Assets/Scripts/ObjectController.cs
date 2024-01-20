using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

public class ObjectController : MonoBehaviour
{
    [Header("Префаб объекта, который будет создаваться после комбинации")]
    [SerializeField] private GameObject combinedObjectPrefab; // Сборная конструкция объединенного объекта

    [Header("Индекс этого объекта и последнего")]
    [SerializeField] private int maxIndex; // Максимальный индекс
    public int objectIndex; // Индекс объекта

    [HideInInspector] public bool isCombining = false; // Происходит объединение

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

    private bool CanCombineWith(ObjectController otherObjectController) // Может объединиться с..
    {
        return otherObjectController != null &&
               objectIndex == otherObjectController.objectIndex &&
               objectIndex < maxIndex &&
               !isCombining &&
               !otherObjectController.isCombining;
    }

    private void StartCombining() // Начать объединение
    {
        isCombining = true;
    }

    private void CombineObjects(GameObject otherObject, ObjectController otherObjectController) // Объединить объекты
    {
        Vector3 spawnPosition = (transform.position + otherObject.transform.position) / 2;
        int nextIndex = objectIndex + 1;

        ScoreManager.Instance.AddScore(5, spawnPosition);

        GameObject combinedObject = ObjectPool.Instance.GetObject(nextIndex);
        ObjectController combinedObjectController = combinedObject.GetComponent<ObjectController>();
        combinedObjectController.isCombining = false;

        combinedObject.transform.position = spawnPosition;
        ActivateObject(combinedObject);

        gameObject.tag = "Disbled";
        otherObject.gameObject.tag = "Disabled";

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

    private void OnEnable()
    {
        gameObject.tag = "Enabled";
    }
}
