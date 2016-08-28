using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class StoreItem : MonoBehaviour {

    public ItemType itemType {
        get
        {
            if (blueprint == null)
            {
                return ItemType.None;
            } else if (typeof(Charge) == blueprint.GetType())
            {
                return ItemType.Charges;
            } else
            {
                return ItemType.Powders;
            }
        }
    }

    Workshop workshop;
    RocketSlot slot;
    StoreItemEvents state = StoreItemEvents.None;

    [SerializeField]
    Transform sourcePosition;

    [SerializeField]
    AnimationCurve retraction;

    [SerializeField] int steps = 20;
    [SerializeField] float delta = 0.01f;

    [SerializeField]
    RocketComponent blueprint;

    [SerializeField, Range(0, 10)]
    float positionNoise = 0.1f;

    [SerializeField]
    Transform dragParent;

    [SerializeField]
    Transform restParent;

    public RocketComponent Blueprint
    {
        get
        {
            return blueprint;
        }

        set
        {
            blueprint = value;
        }
    }

    Vector3 Noise
    {
        get
        {
            return new Vector3(Random.Range(-positionNoise, positionNoise), Random.Range(-positionNoise, positionNoise), 0);
        }
    }

    void Awake()
    {
        workshop = FindObjectOfType<Workshop>();
        transform.position = sourcePosition.position + Noise;
    }

    void Start()
    {
        if (typeof(Powder) == blueprint.GetType())
        {
            Image plate = GetComponent<Image>();
            Color color = new Color();
            Color refColor = (blueprint as Powder).color;
            color.r = refColor.r;
            color.g = refColor.g;
            color.b = refColor.b;
            color.a = 1;
            foreach(Image im in GetComponentsInChildren<Image>())
            {
                if (im != plate)
                {
                    im.color = color;
                }
            }
        }

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
            transform.position = sourcePosition.position + Noise;
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
            transform.SetParent(dragParent);
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
            StartCoroutine(AnimateTo(transform.position, sourcePosition.position + Noise, StoreItemEvents.Return));
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
        transform.SetParent(restParent);
    }
}
