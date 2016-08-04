using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Collections.Generic;

[CustomEditor(typeof(ModelScript))]
public class UpdateModelEditor : Editor
{
    RevitTCPClient tcpClient = RevitTCPClient.Client;

    ModelScript model;
    

    public override void OnInspectorGUI()
    {
        model = (ModelScript)target;

        if (GUILayout.Button("1. Parse Elements"))
        {
            model.ParseElements();
        }

        GUILayout.Space(10);

        if (!tcpClient.IsActive)
        {
            if (GUILayout.Button("2. Connect to TCP Server"))
            {
                tcpClient.ConnectToServer();
            }
        }
        else
        {
            GUI.enabled = tcpClient.IsActive;
            if (GUILayout.Button("2. Disconnect from TCP Server"))
            {
                tcpClient.DisconnectToServer();
            }
        }

        GUI.enabled = tcpClient.IsActive;
        if (GUILayout.Button("2.1 Stop Listening Message"))
        {
            SendStopListeningMessage();
        }

        GUILayout.Space(10);

        GUI.enabled = true;


        //if (GUILayout.Button("Update Glass Panels"))
        //{
        //    UpdatePanels();
        //}

        GUI.enabled = tcpClient.IsActive;

        EditorGUI.BeginDisabledGroup(!tcpClient.IsActive);

        if (GUILayout.Button("3. Gather Elements Info"))
        {
            GatherElementsInfo();
        }

        model.UpdateMaterials = EditorGUILayout.Toggle("Update Materials", model.UpdateMaterials);
        if (GUILayout.Button("4. Gather Materials"))
        {
            GatherMaterials();
        }

        EditorGUI.EndDisabledGroup();

        GUILayout.Space(10);

        GUI.enabled = model.materialsLoaded;

        if (GUILayout.Button("5. Apply Default Materials"))
        {
            ApplyMaterials();
        }

        GUI.enabled = model.elementsLoaded;

        if (GUILayout.Button("6. Update Door Elements"))
        {
            UpdateDoorElements();
        }

        //if (GUILayout.Button("7. Add Lights"))
        //{
        //    AddLights();
        //}

        GUILayout.Space(10);

        GUI.enabled = true;

        model.DebugMode = EditorGUILayout.Toggle("Debug Mode", model.DebugMode);

        if (model.DebugMode)
        {
            DrawDefaultInspector();
        }
    }

    private void GatherElementsInfo()
    {
        foreach (GameObject obj in model.ModelObjects)
        {
            try
            {
                ModelObject model_object = obj.GetComponent<ModelObject>();

                string result = tcpClient.sendRequest("GetElementInfo?id=" + model_object.Id + "$");

                model_object.UpdateInfo(result);
            }
            catch (Exception e)
            {
                Debug.Log("Error @" + obj.GetComponent<ModelObject>().Name + " - " + obj.GetComponent<ModelObject>().Id + "\n" + e.Message);
            }

        }

        model.elementsLoaded = true;
    }

    private void GatherMaterials()
    {
        foreach (GameObject obj in model.ModelObjects)
        {
            try
            {
                ModelObject model_object = obj.GetComponent<ModelObject>();

                string result = tcpClient.sendRequest("GetMaterials?id=" + model_object.Id + "$");

                model_object.AddMaterials(result, model.UpdateMaterials);

                AddMaterialToModel(model_object.Materials);

            }
            catch (Exception e)
            {
                Debug.Log("Error @" + obj.GetComponent<ModelObject>().Name + " - " + obj.GetComponent<ModelObject>().Id + "\n" + e.Message);
            }
        }
        model.materialsLoaded = true;
    }

    private void UpdateDoorElements()
    {
        model.ModelObjects.ForEach(obj =>
        {
            if (obj.GetComponent<ModelObject>().Type == "Door")
            {
                model.UpdateDoorElement(obj);
            }
        });
    }

    private void SendStopListeningMessage()
    {
        tcpClient.sendRequest("StopListening?" + "$");
    }

    private void AddMaterialToModel(List<ModelMaterial> materials)
    {
        materials.ForEach(mat => model.AddMaterial(mat));
    }

    private void UpdatePanels()
    {
        model.ModelObjects.ForEach(obj =>
        {
            ModelObject model_object = obj.GetComponent<ModelObject>();
            if (model_object.Name.Contains("System Panel Glazed"))
            {
                var glass = Resources.Load<Material>("Materials/26-Glass");
                obj.GetComponent<MeshRenderer>().material = glass;
            }
        });
    }

    private void ApplyMaterials()
    {
        model.ModelObjects.ForEach(obj =>
        {
            try
            {
                Material[] applyingMaterial = new Material[obj.GetComponent<MeshFilter>().sharedMesh.subMeshCount];
                ModelMaterial[] mats = obj.GetComponents<ModelMaterial>();

                if (mats.Length > 0)
                {
                    for (int i = 0; i < applyingMaterial.Length; i++)
                    {
                        applyingMaterial[i] = Resources.Load<Material>("Materials/" + mats[i % mats.Length].Id + "-" + mats[i % mats.Length].Name);
                    }
                    obj.GetComponent<MeshRenderer>().materials = applyingMaterial;
                }
                
            }
            catch (Exception e)
            {
                Debug.Log("Error @" + obj.GetComponent<ModelObject>().Name + " - " + obj.GetComponent<ModelObject>().Id + "\n" + e.Message);
            }
        });
    }

    private void AddLights()
    {
        model.ModelObjects.ForEach(obj =>
        {
            try
            {
                if (obj.name.Contains("_Light"))
                {
                    var light = obj.GetComponent<Light>();
                    if (light == null)
                    {
                        light = obj.AddComponent<Light>();

                    }
                    light.enabled = false;
                    light.type = LightType.Point;
                    light.range = 5;

                    //FIX ME: Light Shadows Two Sides

                }

            }
            catch (Exception e)
            {
                Debug.Log("Error @" + obj.GetComponent<ModelObject>().Name + " - " + obj.GetComponent<ModelObject>().Id + "\n" + e.Message);
            }
        });
    }
}
