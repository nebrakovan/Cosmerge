using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class ContainerBoundaryController : MonoBehaviour
{
    public static ContainerBoundaryController Instance { get; private set; }

    [SerializeField] private LayerMask objectLayer;

    [SerializeField] private float timeToLose = 5f;

    private float timer = 0f;

    [SerializeField] private Vector2 currentBorders;
    [SerializeField] private Vector2 currentPosition;

    [SerializeField] private GameObject boundaryObject;
    private BoxCollider2D boundaryCollider;

    [SerializeField] private bool isOutside = false;
    private HashSet<Collider2D> outsideObjects = new HashSet<Collider2D>();

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
            boundaryCollider = boundaryObject.AddComponent<BoxCollider2D>();
        }

        SetBoundaryPositionAndSize(currentPosition, currentBorders);

    }

    void Update()
    {
        if (isOutside)
        {
            timer += Time.deltaTime;
            if (timer >= timeToLose)
            {
                GameManager.Instance.GameOver();
            }
        }
        else
        {
            timer = 0f;
        }

        isOutside = outsideObjects.Count > 0;
    }

    void OnTriggerExit2D(Collider2D collision)
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

    public void SetBoundaryPositionAndSize(Vector2 newPosition, Vector2 newSize)
    {
        boundaryObject.transform.position = newPosition;
        boundaryCollider.size = newSize;
        boundaryCollider.offset = Vector2.zero;
    }
}
