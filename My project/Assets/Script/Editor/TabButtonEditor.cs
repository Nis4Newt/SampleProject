using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(TabButton), true)]
public class TabButtonEditor : ButtonEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_text"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_noti"));
        serializedObject.ApplyModifiedProperties();
    }
}
