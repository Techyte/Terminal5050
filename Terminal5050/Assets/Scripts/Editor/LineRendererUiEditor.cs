using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LineRendererUi))]
public class LineRendererUiEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Draw"))
        {
            ((LineRendererUi)target).DrawLine();
        }
    }
}
