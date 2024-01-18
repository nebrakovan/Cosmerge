using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;

    [SerializeField]
    private GameObject[] objectPrefabs; // Массив префабов объектов
    [SerializeField]
    private int[] initialPoolSize; // Начальный размер для каждого пула объектов

    private Dictionary<int, Queue<GameObject>> objectPools = new Dictionary<int, Queue<GameObject>>();

    void Awake()
    {
        // Проверка на существование других экземпляров ObjectPool
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        InitializePools();
    }

    private void InitializePools()
    {
        for (int i = 0; i < objectPrefabs.Length; i++)
        {
            objectPools[i] = new Queue<GameObject>();

            int poolSize = i < initialPoolSize.Length ? initialPoolSize[i] : 10; // Установка размера пула
            AddObjectsToPool(i, poolSize);
        }
    }

    public GameObject GetObject(int index)
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

    private void AddObjectsToPool(int index, int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(objectPrefabs[index]);
            obj.SetActive(false);
            objectPools[index].Enqueue(obj);
        }
    }

    public void ReturnObject(GameObject obj, int index)
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
