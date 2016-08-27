using UnityEngine;
using System.Collections;

public enum SlotEvent { Hover, Exit, Filled, Empied };
public delegate void RocketSlotAction(RocketSlot slot, SlotEvent type);


public enum StoreItemEvents {None, Hover, Drag, Return, Slotted };
public enum ItemType {None, Charges, Powders };

public delegate void StoreItemAction(StoreItem item, StoreItemEvents type);

public enum RocketEventsTypes { Incomplete, Ready};
public delegate void RocketAction(RocketEventsTypes type);

public class Workshop : MonoBehaviour {

    public event RocketSlotAction OnRocketSlotAction;
    public event StoreItemAction OnStoreItemAction;
    public event RocketAction OnRocketAction;

    [SerializeField]
    Canvas customerCanvas;

    public void ShowCustomer()
    {
        customerCanvas.enabled = true;
    }

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

    public void Emit(RocketEventsTypes type)
    {
        if (OnRocketAction != null)
        {
            OnRocketAction(type);
        }
    }
}
