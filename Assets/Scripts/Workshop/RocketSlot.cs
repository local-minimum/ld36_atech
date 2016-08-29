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

    [SerializeField]
    int visibleFromLvl;
        
    StoreItem item;

    Workshop workshop;

    bool isOver = false;

    StoreItem dragItem;

    Image markingImage;

    public StoreItem Item
    {
        get
        {
            return item;
        }

        set
        {
            if (item && value != item)
            {
                workshop.Emit(item, StoreItemEvents.Return);
            }

            if (value)
            {
                item = value;
                World.RocketBlueprint[identifier] = new KeyValuePair<int, string>(stage, item.Blueprint.identifier);
                Debug.Log(string.Format("Set rocket blueprint id {0}, at stage {1} to {2}", identifier, stage, item.Blueprint));
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

    public bool activated
    {
        get
        {
            return markingImage.enabled;
        }
    }

    void Awake()
    {
        workshop = FindObjectOfType<Workshop>();
        markingImage = GetComponent<Image>();
    }

    void Start()
    {

        icon.enabled = false;

        if (_itemType == ItemType.None)
        {
            markingImage.sprite = null;
        }
        else if (_itemType == ItemType.Charges)
        {
            markingImage.sprite = markings[0];
            ToggleShadows(false);
        }
        else {
            markingImage.sprite = markings[1];
            ToggleShadows(true);
        }

        World_OnNewLevel(World.Level);
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
        World.OnNewLevel += World_OnNewLevel;
    }

    void OnDestroy()
    {
        World.OnNewLevel -= World_OnNewLevel;
    }

    void OnDisable()
    {        
        workshop.OnStoreItemAction -= Workshop_OnStoreItemAction;
    }

    private void World_OnNewLevel(int lvl)
    {
        //Debug.Log(string.Format("{0} <= {1} = {2}", visibleFromLvl, lvl, visibleFromLvl <= lvl));
        if (visibleFromLvl <= lvl)
        {
            markingImage.enabled = true;
            workshop.Emit(this, SlotEvent.Activated);

        }
        else
        {
            markingImage.enabled = false;
        }
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
        if (activated)
        {

            if (MouseOver)
            {
                if (!isOver)
                {
                    isOver = true;
                    OnMouseEnter();
                }
            }
            else
            {
                if (isOver)
                {
                    isOver = false;
                    OnMouseExit();
                }
            }
        }

    }

    public void OnMouseEnter()
    {
        if (item != dragItem)
        {
            Debug.Log("Enter " + name);
            workshop.Emit(this, SlotEvent.Hover);
        }
    }

    public void OnMouseExit()
    {
        if (item != dragItem)
        {
            Debug.Log("Exit " + name);
            StartCoroutine(_delayExit());
        }
    }

    IEnumerator<WaitForSeconds> _delayExit()
    {
        yield return new WaitForSeconds(1.5f);
        if (item != dragItem)
        {
            workshop.Emit(this, SlotEvent.Exit);
        }
    }
}
