using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Customer))]
public class CustomerEditor : Editor {

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

    }
}
