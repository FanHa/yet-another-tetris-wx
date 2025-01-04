using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
public class InventoryMouseFollower : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private InventoryItem item;

    public void Awake()
    {
        canvas = transform.root.GetComponent<Canvas>();
        item = GetComponentInChildren<InventoryItem>();
    }

    public void SetData(Sprite sprite)
    {
        item.SetData(sprite);
    }

    public void Update()
    {
        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform, 
            Input.mousePosition, 
            mainCamera, 
            out position);
        transform.position = canvas.transform.TransformPoint(position);
    }

    public void Toggle(bool val)
    {
        gameObject.SetActive(val);
    }
}
}
