using UnityEngine;
using System.Collections;

public class StoreItem : MonoBehaviour {

    public ItemType itemType = ItemType.Charges;

    Workshop workshop;
    RocketSlot slot;
    StoreItemEvents state;

    void Awake()
    {
        workshop = FindObjectOfType<Workshop>();
    }

    void LateUpdate () {
	
	}

    void OnEnable()
    {
        workshop.OnRocketSlotAction += Workshop_OnRocketSlotAction;
    }

    void OnDisalbe()
    {
        workshop.OnRocketSlotAction -= Workshop_OnRocketSlotAction;
    }

    private void Workshop_OnRocketSlotAction(RocketSlot slot, SlotEvent type)
    {
        if (state == StoreItemEvents.Drag) {
            if (type == SlotEvent.Hover)
            {
                this.slot = slot;
            } else if (type == SlotEvent.Exit)
            {
                this.slot = null;
            }
         }
    }

    public void Hover()
    {
        workshop.Emit(this, StoreItemEvents.Hover);        
    }

    public void Drag()
    {
        workshop.Emit(this, StoreItemEvents.Drag);
    }

    public void Release()
    {
        if (slot == null)
        {
            workshop.Emit(this, StoreItemEvents.Return);
        } else
        {
            slot.Item = this;
            workshop.Emit(this, StoreItemEvents.Slotted);

            slot = null;
        }

        
    }
}
