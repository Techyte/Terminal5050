using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PowerManager))]
public class PowerManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Overload Power"))
        {
            ((PowerManager)target).SetCharge(0);
        }
        
        base.OnInspectorGUI();
    }
}
