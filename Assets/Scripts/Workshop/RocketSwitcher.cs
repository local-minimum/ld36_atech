using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class RocketSwitcher : MonoBehaviour {

    [SerializeField] List<int> slotsInRocket = new List<int>();
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
        slotsInRocket.Clear();
        slotsInRocket.AddRange(GetComponentsInChildren<RocketSlot>().Where(i => i.activated).Select(i => i.ID));
    }

    void OnEnable()
    {
        workshop.OnRocketSlotAction += Workshop_OnRocketSlotAction;
    }

    void OnDisable()
    {
        workshop.OnRocketSlotAction -= Workshop_OnRocketSlotAction;
    }

    private void Workshop_OnRocketSlotAction(RocketSlot slot, SlotEvent type)
    {
        if (type == SlotEvent.Activated)
        {
            Start();
        }

        if (type == SlotEvent.Empied || type == SlotEvent.Filled)
        {
            workshop.Emit(Ready ? RocketEventsTypes.Ready : RocketEventsTypes.Incomplete);
        }
    }
}
