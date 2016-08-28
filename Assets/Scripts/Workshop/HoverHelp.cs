using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HoverHelp : MonoBehaviour {

    [SerializeField]
    string helpTitle;

    [SerializeField]
    string helpText;

    [SerializeField]
    float hoverTimeout = 3f;

    [SerializeField]
    Text Title;

    [SerializeField]
    Text Body;

    Workshop workshop;

    bool showHelp = false;

    void Start()
    {
        Title.text = helpTitle;
        Body.text = helpText;
    }

    void Awake()
    {
        workshop = GetComponentInParent<Workshop>();
    }

    void OnEnable()
    {
        workshop.OnStoreItemAction += Workshop_OnStoreItemAction;
    }

    void OnDisable()
    {

    }

    private void Workshop_OnStoreItemAction(StoreItem item, StoreItemEvents type)
    {
        if (type == StoreItemEvents.Hover)
        {
            SetHover(item);
        } else if (type == StoreItemEvents.Slotted || type == StoreItemEvents.Return)
        {
            StartCoroutine(ShowHelpDelayed());
        }
    }

    void SetHover(StoreItem item)
    {
        Ingredient ingredient = Workshop.ingredients[item.Blueprint.identifier];
        Title.text = ingredient.name;
        Body.text = ingredient.text;
    }

    IEnumerator<WaitForSeconds> ShowHelpDelayed()
    {
        if (!showHelp)
        {
            showHelp = true;
            yield return new WaitForSeconds(hoverTimeout);
            if (showHelp)
            {
                Start();
                showHelp = false;
            }
        }
    }

}
