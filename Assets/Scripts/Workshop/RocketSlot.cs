using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class RocketSlot : MonoBehaviour
{

    [SerializeField]
    ItemType itemType;

    StoreItem item;

    Workshop workshop;

    bool isOver = false;

    public StoreItem Item
    {
        get
        {
            return item;
        }

        set
        {
            item = value;
        }
    }

    void Awake()
    {
        workshop = FindObjectOfType<Workshop>();
    }

    bool MouseOver
    {
        get
        {
            return RectTransformUtility.RectangleContainsScreenPoint(transform as RectTransform, Input.mousePosition);
        }
    }

    void Update()
    {
        if (MouseOver)
        {
            if (!isOver)
            {
                isOver = true;
                OnMouseEnter();
            }
        } else
        {
            if (isOver)
            {
                isOver = false;
                OnMouseExit();
            }
        }
    }

    public void OnMouseEnter()
    {
        Debug.Log("Enter " + this);
        if (item == null)
        {
            workshop.Emit(this, SlotEvent.Hover);
        }
    }

    public void OnMouseExit()
    {
        Debug.Log("Exit " + this);
        if (item == null)
        {
            workshop.Emit(this, SlotEvent.Exit);
        }
    }
}
