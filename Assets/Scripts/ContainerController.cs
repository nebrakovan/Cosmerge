using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;

public class ContainerController : MonoBehaviour
{
    public static ContainerController Instance { get; private set; }

    [Header("Объект емкости")]
    [SerializeField] private GameObject containerObject; // Объект емкости

    [Header("Слой игрового объекта")]
    [SerializeField] private LayerMask objectLayer; // Слой объекта

    [Header("Время до проигрыша и текст для его отображения")]
    [SerializeField] private TMP_Text timeToLoseText;
    [SerializeField] private float timeToLose = 5f; // Время для проигрыша

    private bool isOutside = false; // Снаружи

    private HashSet<Collider2D> outsideObjects = new HashSet<Collider2D>(); // Объекты снаружи

    private float timer = 0f; // Таймер

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        if (isOutside)
        {
            timeToLoseText.enabled = true;
            timeToLoseText.text = "" + Mathf.Round(timer);

            timer += Time.deltaTime;
            if (timer >= timeToLose)
            {
                GameManager.Instance.GameOver();
                return;
            }
        }
        else
        {
            timer = 0f;
            timeToLoseText.enabled = false;
        }

        isOutside = outsideObjects.Count > 0;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enabled"))
        {
            isOutside = true;
            outsideObjects.Add(collision);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enabled") && outsideObjects.Contains(collision))
        {
            outsideObjects.Remove(collision);
        }
    }

    public void AddObjectOutside(GameObject currentObject)
    {
        Collider2D collision = currentObject.GetComponent<Collider2D>();
        outsideObjects.Add(collision);
    }

    public void RemoveObjectOutside(GameObject currentObject)
    {
        Collider2D collider = currentObject.GetComponent<Collider2D>();
        if (collider != null && outsideObjects.Contains(collider))
        {
            outsideObjects.Remove(collider); // Remove the collider from the HashSet
        }
    }
}
