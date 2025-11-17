using UnityEditor;
using YanickSenn.Controller.FirstPerson.Hand;

namespace YanickSenn.Controller.FirstPerson.Editor {
    [CustomEditor(typeof(Interactable), true)]
    public class InteractableEditor : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            serializedObject.Update();
            
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
            EditorGUI.EndDisabledGroup();
            
            DrawPropertiesExcluding(serializedObject, "m_Script", "onInteractEvent");
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onInteractEvent"), true);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
