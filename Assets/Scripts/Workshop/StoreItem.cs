using UnityEngine;
using System.Collections.Generic;

public class StoreItem : MonoBehaviour {

    public ItemType itemType = ItemType.Charges;

    Workshop workshop;
    RocketSlot slot;
    StoreItemEvents state = StoreItemEvents.None;

    Vector3 sourcePosition;

    [SerializeField]
    AnimationCurve retraction;

    [SerializeField] int steps = 20;
    [SerializeField] float delta = 0.01f;

    void Awake()
    {
        workshop = FindObjectOfType<Workshop>();
        sourcePosition = transform.position;
    }

    void LateUpdate () {
	
	}

    void OnEnable()
    {
        workshop.OnRocketSlotAction += Workshop_OnRocketSlotAction;
        workshop.OnStoreItemAction += Workshop_OnStoreItemAction;
    }

    void OnDisalbe()
    {
        workshop.OnRocketSlotAction -= Workshop_OnRocketSlotAction;
        workshop.OnStoreItemAction -= Workshop_OnStoreItemAction;
    }

    private void Workshop_OnStoreItemAction(StoreItem item, StoreItemEvents type)
    {
        if (item == this && type == StoreItemEvents.Slotted)
        {
            transform.position = sourcePosition;
        }
    }

    private void Workshop_OnRocketSlotAction(RocketSlot slot, SlotEvent type)
    {
        if (state == StoreItemEvents.Drag) {

            if (type == SlotEvent.Hover)
            {
                if (slot.itemType == itemType)
                {
                    this.slot = slot;
                }
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

    public void OnBeginDrag()
    {
        if (state == StoreItemEvents.None)
        {
            workshop.Emit(this, StoreItemEvents.Drag);
            state = StoreItemEvents.Drag;
        }
    }

    public void OnDrag()
    {
        if (state == StoreItemEvents.Drag)
        {
            transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag()
    {
        if (slot == null)
        {
            StartCoroutine(AnimateTo(transform.position, sourcePosition, StoreItemEvents.Return));
            slot = null;
        } else
        {
            slot.Item = this;                        
            StartCoroutine(AnimateTo(transform.position, slot.transform.position, StoreItemEvents.Slotted));
            slot = null;
        }
        
    }

    IEnumerator<WaitForSeconds> AnimateTo(Vector3 from, Vector3 to, StoreItemEvents endEvent)
    {
        for (int i = 0; i <steps; i ++)
        {
            transform.position = Vector3.LerpUnclamped(from, to, retraction.Evaluate(i / (float) steps));
            yield return new WaitForSeconds(delta);
        }
        transform.position = to;
        workshop.Emit(this, endEvent);
        state = StoreItemEvents.None;

    }
}
