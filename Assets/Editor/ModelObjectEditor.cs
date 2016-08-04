using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Collections.Generic;

[CustomEditor(typeof(ModelObject))]
public class ModelObjectEditor : Editor {

    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Replicate materials in objects of same type"))
        {
            ModelObject gameObject = (ModelObject) target;

            Material[] mats = gameObject.GetComponent<MeshRenderer>().sharedMaterials;

            List<ModelObject> objs = new List<ModelObject>(FindObjectsOfType<ModelObject>());

            objs.ForEach(o => {
				if (o.GetComponent<ModelObject>().Name == gameObject.Name &&
					o.GetComponent<MeshRenderer>().sharedMaterials.Length == mats.Length)
                {
                    o.GetComponent<MeshRenderer>().materials = mats;
                }
            });

        }
        GUILayout.Space(10);

        DrawDefaultInspector();
    }
}
