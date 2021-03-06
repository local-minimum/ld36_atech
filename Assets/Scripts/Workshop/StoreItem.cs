﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

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

    public Image SlottingImage
    {
        get
        {
            ItemType iType = itemType;
            if (iType == ItemType.Charges)
            {
                return GetComponent<Image>();
            } else if (iType == ItemType.Powders)
            {
                Image thisImg = GetComponent<Image>();
                return GetComponentsInChildren<Image>().Where(e => e != thisImg).First();

            } else
            {
                return null;
            }
        }
    }

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
        World_OnNewLevel(World.Level);
    }

    void OnEnable()
    {
        workshop.OnRocketSlotAction += Workshop_OnRocketSlotAction;
        workshop.OnStoreItemAction += Workshop_OnStoreItemAction;
        World.OnNewLevel += World_OnNewLevel;
    }

    void OnDisalbe()
    {
        workshop.OnRocketSlotAction -= Workshop_OnRocketSlotAction;
        workshop.OnStoreItemAction -= Workshop_OnStoreItemAction;
    }

    void OnDestroy() { 
        World.OnNewLevel -= World_OnNewLevel;
    }

    private void World_OnNewLevel(int lvl)
    {
        if (Workshop.ingredients[blueprint.identifier].level > World.Level)
        {
            gameObject.SetActive(false);
        } else
        {
            gameObject.SetActive(true);
        }
    }

    private void Workshop_OnStoreItemAction(StoreItem item, StoreItemEvents type)
    {
        if (item == this) {
            if (type == StoreItemEvents.Slotted)
            {
                SlottingImage.enabled = false;
                transform.position = sourcePosition.position + Noise;
            } else if (type == StoreItemEvents.Return)
            {
                SlottingImage.enabled = true;
            }
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
			} else if (type == SlotEvent.Exit && slot == this.slot)
            {
                this.slot = null;
            }
         }
    }

    public void Hover()
    {
		Debug.Log (name + ": Hover " + SlottingImage.enabled);
        if (SlottingImage.enabled)
        {
            workshop.Emit(this, StoreItemEvents.Hover);
        }
    }

    public void OnBeginDrag()
    {
		//Debug.Log (string.Format("{0}: DragStart {1} & {2}", name, SlottingImage.enabled, state));

        if (SlottingImage.enabled && state == StoreItemEvents.None)
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
		//Debug.Log (string.Format("{0}: DragEnd {1} & {2} & slot {3}", name, SlottingImage.enabled, state, slot));
		if (state == StoreItemEvents.Drag)
        {
            if (slot == null)
            {
                StartCoroutine(AnimateTo(transform.position, sourcePosition.position + Noise, StoreItemEvents.Return));
                slot = null;
            }
            else
            {
                slot.Item = this;
                StartCoroutine(AnimateTo(transform.position, slot.transform.position, StoreItemEvents.Slotted));
                slot = null;
            }
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
