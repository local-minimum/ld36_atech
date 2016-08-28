using UnityEngine;
using UnityEngine.UI;

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

    void Start()
    {
        Title.text = helpTitle;
        Body.text = helpText;
    }
}
