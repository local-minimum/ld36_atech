using UnityEngine;
using System.Collections;

public enum SlotEvent { Hover, Exit, Filled, Empied };
public delegate void RocketSlotAction(RocketSlot slot, SlotEvent type);


public enum StoreItemEvents { Hover, Drag, Return, Slotted };
public enum ItemType { Charges, Powders };

public delegate void StoreItemAction(StoreItem item, StoreItemEvents type);


public class Workshop : MonoBehaviour {

    public event RocketSlotAction OnRocketSlotAction;
    public event StoreItemAction OnStoreItemAction;

    public void Emit(RocketSlot slot, SlotEvent type)
    {
        if (OnRocketSlotAction != null)
        {
            OnRocketSlotAction(slot, type);
        }
    }

    public void Emit(StoreItem item, StoreItemEvents type)
    {
        if (OnStoreItemAction != null)
        {
            OnStoreItemAction(item, type);
        }
    }
}
