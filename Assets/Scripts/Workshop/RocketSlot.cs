using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class RocketSlot : MonoBehaviour
{

    [SerializeField]
    ItemType _itemType;

    [SerializeField]
    Image icon;

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

    public ItemType itemType
    {
        get
        {
            return _itemType;
        }
    }

    void Awake()
    {
        workshop = FindObjectOfType<Workshop>();
    }

    void OnEnable()
    {
        workshop.OnStoreItemAction += Workshop_OnStoreItemAction;
    }

    void OnDisable()
    {
        workshop.OnStoreItemAction -= Workshop_OnStoreItemAction;
    }

    private void Workshop_OnStoreItemAction(StoreItem item, StoreItemEvents type)
    {
        if (item == this.item && type == StoreItemEvents.Slotted)
        {
            Image img = item.GetComponent<Image>();
            icon.sprite = img.sprite;
            icon.color = img.color;
            icon.fillMethod = img.fillMethod;
            icon.preserveAspect = img.preserveAspect;
            icon.fillAmount = icon.fillAmount;

        }
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
        if (item == null)
        {
            workshop.Emit(this, SlotEvent.Hover);
        }
    }

    public void OnMouseExit()
    {
        if (item == null)
        {
            workshop.Emit(this, SlotEvent.Exit);
        }
    }
}
