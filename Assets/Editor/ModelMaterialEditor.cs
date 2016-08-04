using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Collections.Generic;


[CustomEditor(typeof(ModelMaterial))]
public class ModelMaterialEditor : Editor
{

    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Apply Material"))
        {
            ModelMaterial mat = (ModelMaterial) target;

            mat.gameObject.GetComponent<ModelObject>().ApplyMaterial(mat);
        }

        GUILayout.Space(10);

        DrawDefaultInspector();

    }
}
