using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class RocketSwitcher : MonoBehaviour {

    List<int> slotsInRocket = new List<int>();
    Workshop workshop;

    public bool Ready
    {
        get
        {
            return slotsInRocket.Select(i => World.RocketBlueprint.ContainsKey(i)).All(b => b);
        }
    }

    void Awake()
    {
        workshop = GetComponentInParent<Workshop>();

    }

    void Start()
    {
        slotsInRocket.AddRange(GetComponentsInChildren<RocketSlot>().Select(i => i.ID));
    }

    void OnEnable()
    {
        workshop.OnRocketSlotAction += Workshop_OnRocketSlotAction;
    }

    private void Workshop_OnRocketSlotAction(RocketSlot slot, SlotEvent type)
    {

        if (type == SlotEvent.Empied || type == SlotEvent.Filled)
        {
            workshop.Emit(Ready ? RocketEventsTypes.Ready : RocketEventsTypes.Incomplete);
        }
    }
}
