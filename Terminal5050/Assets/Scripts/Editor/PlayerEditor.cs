using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Player))]
public class PlayerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUI.BeginDisabledGroup(true);
        
        base.OnInspectorGUI();

        EditorGUI.EndDisabledGroup();

        if (GUILayout.Button("Make Local"))
        {
            ((Player)target).MakeLocal();
        }
    }
}
