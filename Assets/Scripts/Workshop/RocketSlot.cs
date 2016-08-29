using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;


public class RocketSlot : MonoBehaviour
{

    [SerializeField]
    Sprite[] markings;

    [SerializeField]
    ItemType _itemType;

    [SerializeField]
    Image icon;

    [SerializeField]
    int stage;

    [SerializeField]
    int identifier;

    StoreItem item;

    Workshop workshop;

    bool isOver = false;

    StoreItem dragItem;

    public StoreItem Item
    {
        get
        {
            return item;
        }

        set
        {
            item = value;
            if (item)
            {
                World.RocketBlueprint[identifier] = new KeyValuePair<int, RocketComponent>(stage, item.Blueprint);
                workshop.Emit(this, SlotEvent.Filled);
            } else
            {
                World.RocketBlueprint.Remove(identifier);
                workshop.Emit(this, SlotEvent.Empied);
            }
        }
    }

    public int ID
    {
        get
        {
            return identifier;
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

    void Start()
    {
        icon.enabled = false;
        Image marking = GetComponent<Image>();
        if (_itemType == ItemType.None)
        {
            marking.sprite = null;
        } else if (_itemType == ItemType.Charges) {
            marking.sprite = markings[0];
            ToggleShadows(false);
        } else {
            marking.sprite = markings[1];
            ToggleShadows(true);
        }
    }

    void ToggleShadows(bool enable)
    {
        foreach (Shadow shadow in icon.GetComponents<Shadow>())
        {
            shadow.enabled = enabled;
        }
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
            Image img = item.SlottingImage;
            if (img == null)
            {
                icon.enabled = false;
            } else {
                icon.enabled = true;
                icon.sprite = img.sprite;
                icon.color = img.color;
                icon.fillMethod = img.fillMethod;
                icon.preserveAspect = img.preserveAspect;
                icon.fillAmount = icon.fillAmount;
            }
        } else if (type == StoreItemEvents.Drag)
        {
            dragItem = item;
        }

        if (type == StoreItemEvents.Slotted || type == StoreItemEvents.Return || type == StoreItemEvents.None)
        {
            dragItem = null;
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
        if (item != dragItem)
        {
            workshop.Emit(this, SlotEvent.Hover);
        }
    }

    public void OnMouseExit()
    {
        if (item != dragItem)
        {
            workshop.Emit(this, SlotEvent.Exit);
        }
    }
}
