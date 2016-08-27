using UnityEngine;


public class RocketSlot : MonoBehaviour {

    [SerializeField]
    ItemType itemType;

    StoreItem item;

    Workshop workshop;

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

    void OnMouseEnter()
    {
        if (item == null)
        {
            workshop.Emit(this, SlotEvent.Hover);
        }
    }

    void OnMouseExit()
    {
        if (item == null)
        {
            workshop.Emit(this, SlotEvent.Exit);
        }
    }
}
