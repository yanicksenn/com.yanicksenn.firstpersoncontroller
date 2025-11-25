using UnityEditor;
using UnityEngine;
using YanickSenn.Controller.FirstPerson.Hand;

[CustomEditor(typeof(Grabbable))]
public class GrabbableEditor : Editor {
    private Mesh _handMesh;
    private Material _handMaterial;
    private const string HandMeshPath = "Packages/com.yanicksenn.firstpersoncontroller/Assets/model_gizmo_hand.mesh";
    private const string HandMaterialPath = "Packages/com.yanicksenn.firstpersoncontroller/Assets/material_gizmo_hand.mat";

    private void OnEnable()
    {
        _handMesh = AssetDatabase.LoadAssetAtPath<Mesh>(HandMeshPath);
        if (_handMesh == null)
        {
            Debug.LogWarning($"GrabbableEditor: Could not load hand mesh at '{HandMeshPath}'. Please ensure the mesh exists at this path.");
        }

        _handMaterial = AssetDatabase.LoadAssetAtPath<Material>(HandMaterialPath);
        if (_handMaterial == null)
        {
            Debug.LogWarning($"GrabbableEditor: Could not load hand material at '{HandMaterialPath}'. Falling back to default gizmo color.");
        }
    }

    public override void OnInspectorGUI() {
        SerializedProperty useCustomHoldingConfigProp = serializedObject.FindProperty("useCustomHoldingConfig");
        EditorGUILayout.PropertyField(useCustomHoldingConfigProp);

        if (useCustomHoldingConfigProp.boolValue)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("holdingPosition"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("holdingRotation"));
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void OnSceneGUI() {
        var grabbable = (Grabbable)target;
        var useCustomHoldingConfigProp = serializedObject.FindProperty("useCustomHoldingConfig");

        if (!useCustomHoldingConfigProp.boolValue) return;
        var holdingPositionProp = serializedObject.FindProperty("holdingPosition");
        var holdingRotationProp = serializedObject.FindProperty("holdingRotation");

        var worldPosition = grabbable.transform.TransformPoint(holdingPositionProp.vector3Value);
        var worldRotation = grabbable.transform.rotation * holdingRotationProp.quaternionValue;

        EditorGUI.BeginChangeCheck();
        var newHoldingPosition = Handles.PositionHandle(worldPosition, worldRotation);
        if (EditorGUI.EndChangeCheck())
        {
            holdingPositionProp.vector3Value = grabbable.transform.InverseTransformPoint(newHoldingPosition);
            serializedObject.ApplyModifiedProperties();
        }

        EditorGUI.BeginChangeCheck();
        var newHoldingRotation = Handles.RotationHandle(worldRotation, worldPosition);
        if (EditorGUI.EndChangeCheck()) {
            holdingRotationProp.quaternionValue = Quaternion.Inverse(grabbable.transform.rotation) * newHoldingRotation;
            serializedObject.ApplyModifiedProperties();
        }

        if (_handMesh == null || _handMaterial == null) return;
        // Push the current matrix to save it
        var oldMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(worldPosition, worldRotation, Vector3.one);

        // Draw the mesh with the material
        Graphics.DrawMesh(_handMesh, worldPosition, worldRotation, _handMaterial, 0);

        // Pop the old matrix
        Gizmos.matrix = oldMatrix;
    }
}
