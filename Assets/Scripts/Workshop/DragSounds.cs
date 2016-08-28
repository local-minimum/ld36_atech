using UnityEngine;


public class DragSounds : MonoBehaviour {

    Workshop workshop;

    [SerializeField]
    AudioClip defaultPowder;

    [SerializeField]
    AudioClip defaultCharge;

	void Awake () {
        workshop = GetComponentInParent<Workshop>();
	}

    void OnEnable()
    {
        workshop.OnStoreItemAction += Workshop_OnStoreItemAction;
    }

    void OnDisable()
    {
        workshop.OnStoreItemAction -= Workshop_OnStoreItemAction;
    }

    private void Workshop_OnStoreItemAction(StoreItem item, StoreItemEvents type)
    {
        if (type == StoreItemEvents.Drag || type == StoreItemEvents.Slotted)
        {
            if (typeof(Powder) == item.Blueprint.GetType())
            {

                AudioClip snd = (item.Blueprint as Powder).audio;

                if (snd != null)
                {
                    SingleCam.ButtonSpeaker.PlayOneShot(snd);
                } else if (defaultPowder != null)
                {
                    SingleCam.ButtonSpeaker.PlayOneShot(defaultPowder);
                }
            } else if (defaultCharge != null)
            {
                SingleCam.ButtonSpeaker.PlayOneShot(defaultCharge);
            }
        }
    }
}
