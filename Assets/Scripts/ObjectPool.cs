using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;

    [Header("Префабы создаваемых объектов")]
    [SerializeField] private GameObject[] objectPrefabs; // Префабы объектов

    [Header("Размеры пулов каждого объекта")]
    [SerializeField] private int[] initialPoolSize; // Начальный размер пула 

    private Dictionary<int, Queue<GameObject>> objectPools = new Dictionary<int, Queue<GameObject>>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        InitializePools();
    }

    private void InitializePools() // Инициализировать пулы
    {
        for (int i = 0; i < objectPrefabs.Length; i++)
        {
            objectPools[i] = new Queue<GameObject>();

            int poolSize = i < initialPoolSize.Length ? initialPoolSize[i] : 10;
            AddObjectsToPool(i, poolSize);
        }
    }

    public GameObject GetObject(int index) // Получить объект
    {
        if (!objectPools.ContainsKey(index))
        {
            Debug.LogError("ObjectPool: Нет пула для индекса " + index);
            return null;
        }

        if (objectPools[index].Count == 0)
        {
            AddObjectsToPool(index, 1);
        }

        return objectPools[index].Dequeue();
    }

    private void AddObjectsToPool(int index, int count) // Добавить объекты в пулл
    {
        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(objectPrefabs[index], transform);
            obj.SetActive(false);
            objectPools[index].Enqueue(obj);
        }
    }

    public void ReturnObject(GameObject obj, int index) // Вернуть объект
    {
        obj.SetActive(false);
        if (!objectPools.ContainsKey(index))
        {
            Debug.LogError("ObjectPool: Нет пула для индекса " + index);
            return;
        }
        objectPools[index].Enqueue(obj);
    }
}
