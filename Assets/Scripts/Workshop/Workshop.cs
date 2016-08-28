using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Audio;

public enum SlotEvent { Hover, Exit, Filled, Empied };
public delegate void RocketSlotAction(RocketSlot slot, SlotEvent type);


public enum StoreItemEvents {None, Hover, Drag, Return, Slotted };
public enum ItemType {None, Charges, Powders };

public delegate void StoreItemAction(StoreItem item, StoreItemEvents type);

public enum RocketEventsTypes { Incomplete, Ready};
public delegate void RocketAction(RocketEventsTypes type);

[System.Serializable]
public class Ingredient
{
    public string identifier;
    public string name;
    public string text;
    public int level;
}

public class Workshop : MonoBehaviour {

    public event RocketSlotAction OnRocketSlotAction;
    public event StoreItemAction OnStoreItemAction;
    public event RocketAction OnRocketAction;

    [SerializeField]
    AudioMixerSnapshot customerSnapshot;

    [SerializeField]
    AudioSource speakerButtons;

    [SerializeField]
    AudioClip toOrderClip;

    [SerializeField]
    AudioClip toFireworks;

    [SerializeField]
    float fadeTime;

    public static Dictionary<string, Ingredient> ingredients = new Dictionary<string, Ingredient>();

    private static Workshop _shop;

    private static Workshop shop
    {
        get
        {
            if (_shop == null)
            {
                _shop = FindObjectOfType<Workshop>();
            }
            return _shop;
        }
    }

    [SerializeField]
    List<string> json_files = new List<string>();

    [SerializeField]
    Canvas customerCanvas;

    void OnEnable()
    {
        if (_shop == null)
        {
            _shop = this;
        }
    }

    void OnDisable()
    {
        if (_shop == this)
        {
            _shop = null;
        }
    }

    public void ShowCustomer()
    {
        speakerButtons.PlayOneShot(toOrderClip);
        customerSnapshot.TransitionTo(fadeTime);
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

    public static void LoadJSON()
    {
        shop._LoadJSON();
    }

    void _LoadJSON()
    {
        for (int part_index = 0, files = json_files.Count; part_index < files; part_index++)
        {

            TextAsset asset = Resources.Load(json_files[part_index]) as TextAsset;
            if (asset == null)
            {
                Debug.LogError("Missing file: " + json_files[part_index]);
                continue;
            }
            else
            {
                Debug.Log("Parsing JSON: " + json_files[part_index]);
            }
            string json = asset.text;
            Ingredient part = JsonUtility.FromJson<Ingredient>(json);
            if (ingredients.ContainsKey(part.name))
            {
                Debug.LogError(string.Format("Duplicated ingredient '{0}' from file '{1}' was already loaded.", part.identifier, json_files[part_index]));
            }
            ingredients.Add(part.identifier, part);
        }

    }
}
