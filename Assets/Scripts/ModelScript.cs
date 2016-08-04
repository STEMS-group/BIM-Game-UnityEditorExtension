using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ModelScript : MonoBehaviour
{
    public List<GameObject> ModelObjects = new List<GameObject>();
    public int NumberOfMaterials = 0;
    private Dictionary<string, ModelMaterial> ModelMaterials = new Dictionary<string, ModelMaterial>();

    public bool UpdateMaterials = false;

    public bool elementsLoaded = false;
    public bool materialsLoaded = false;
    public bool DebugMode = true;

    public void ParseElements()
    {
        ModelObjects.Clear();

        //bool elementRemoved;

        int children = transform.childCount;

        for (int i = children-1; i >=0; i--)
        {
            GameObject child = transform.GetChild(i).gameObject;

            if (!ValidObject(child))
            {
                DestroyImmediate(child);
                //elementRemoved = true;
                //break;
            }
            else if (child.name.Contains("_Light"))
            {
                var light = child.GetComponent<Light>();
                if (light == null)
                {
                    light = child.AddComponent<Light>();

                }
                light.enabled = false;
                light.type = LightType.Point;
                light.shadows = LightShadows.Soft;
                light.range = 5;

                //FIX ME: Light Shadows Two Sides
            }
            else
            {

                int startIndex = child.name.IndexOf('[') + 1;
                int endIndex = child.name.LastIndexOf(']');
                int length = endIndex - startIndex;

                if (endIndex < 0)
                {
                    DestroyImmediate(gameObject);
                    continue;
                }

                string id = child.name.Substring(startIndex, length);
                string name = child.name.Substring(0, startIndex - 1);

                ModelObject obj = child.GetComponent<ModelObject>();

                if (obj == null)
                {
                    obj = child.AddComponent<ModelObject>();
                }

                obj.Id = id;
                obj.Name = name;
                obj.Object = child;

                if (!ModelObjects.Contains(child))
                {
                    ModelObjects.Add(child);
                }

                child.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            }
        }

    }

    private bool ValidObject(GameObject obj)
    {
        if (obj.name.Contains("_Light")) return true;
        if (obj.GetComponent<MeshRenderer>() == null) return false;

        return true;
    }

    public void UpdateMaterialsList()
    {
        foreach (GameObject obj in ModelObjects)
        {
            ModelObject model_object = obj.GetComponent<ModelObject>();

            foreach (ModelMaterial mat in model_object.Materials)
            {
                ModelMaterials.Add(mat.Id, mat);
            }
        }

        NumberOfMaterials = ModelMaterials.Count;
    }

    public void AddMaterial(ModelMaterial material)
    {
        if (ModelMaterials.ContainsKey(material.Id))
        {
            ModelMaterials[material.Id] = material;
        }
        else
        {
            ModelMaterials.Add(material.Id, material);
        }

        NumberOfMaterials = ModelMaterials.Count;
    }

    public void UpdateDoorElement(GameObject obj)
    {
        ModelObject modelObject = obj.GetComponent<ModelObject>();

		if (modelObject.Name.Contains ("Double") || modelObject.Name.Contains ("Dbl"))
			return;




        int frameCounting = 0;

        if (modelObject.HasFrame())
        {
            GameObject frame = new GameObject("Door Frame [" + modelObject.Id + "]");
            frame.transform.parent = modelObject.transform;
            frame.transform.localScale = new Vector3(1, 1, 1);
            frame.transform.localPosition = new Vector3(0, 0, 0);
            frame.transform.localRotation = new Quaternion();
            frame.AddComponent<MeshFilter>();
            frame.AddComponent<MeshCollider>();
            frame.AddComponent<MeshRenderer>();
            AddFrame(modelObject, frame);
            frameCounting = 1;
        }



        GameObject panel = new GameObject("Door Panel [" + modelObject.Id + "]");
        panel.transform.parent = modelObject.transform;
        panel.transform.localScale = new Vector3(1, 1, 1);
        panel.transform.localPosition = new Vector3(0, 0, 0);
        panel.transform.localRotation = new Quaternion();

		BoxCollider box = panel.GetComponent<BoxCollider> ();
		if (box == null) {
			box = panel.AddComponent<BoxCollider> ();
		}
		box.isTrigger = true;
		box.size = new Vector3 (1, 1, 2);
		//box.center = new Vector3 (0, 0, 0);

        for (int i = 0 + frameCounting; i < modelObject.GetComponent<MeshFilter>().sharedMesh.subMeshCount; i++)
        {
            AddComponentDoorPanel(modelObject, panel, i);
        }

        DoorPanel doorPanel = panel.GetComponent<DoorPanel>();
        if (doorPanel == null)
        {
            doorPanel = panel.AddComponent<DoorPanel>();
        }


        modelObject.GetComponent<MeshRenderer>().enabled = false;
        modelObject.GetComponent<MeshCollider>().enabled = false;

		doorPanel.UpdateOpeningDirection ();

		if (panel.GetComponent<DoorScript> () == null) {
			panel.AddComponent<DoorScript> ();
		}
    }

    private void AddFrame(ModelObject modelObject, GameObject frame)
	{
		Mesh myMesh = modelObject.GetComponent<MeshFilter> ().sharedMesh;

		Mesh frameMesh = new Mesh ();

		int[] oldTrianges = myMesh.GetTriangles (0);

		int count = 0;
		Dictionary<int, int> dictionary = new Dictionary<int, int> ();
		for (int x = 0; x < oldTrianges.Length; x++) {
			int current = oldTrianges [x];

			if (!dictionary.ContainsKey (current)) {
				dictionary.Add (current, count);
				count = count + 1;
			}
		}

		int[] newTriangles = new int[oldTrianges.Length];
		for (int x = 0; x < oldTrianges.Length; x++) {
			newTriangles [x] = dictionary [oldTrianges [x]];
		}

		Vector3[] oldVerts = myMesh.vertices;
		Vector3[] newVerts = new Vector3[count];
		foreach (KeyValuePair<int, int> pair in dictionary) {
			int oldVertIndex = pair.Key;
			int newVertIndex = pair.Value;
			newVerts [newVertIndex] = oldVerts [oldVertIndex];
		}

		frameMesh.vertices = newVerts;
		frameMesh.triangles = newTriangles;

		Vector2[] uvs = new Vector2[newVerts.Length];

		for (int i = 0; i < newVerts.Length; i++) {

			uvs [i] = new Vector2 (newVerts [i].x, newVerts [i].z);
		}
		frameMesh.uv = uvs;

        frameMesh.RecalculateNormals();
        frameMesh.Optimize();

        frame.GetComponent<MeshFilter>().mesh = frameMesh;

        frame.GetComponent<MeshRenderer>().material = modelObject.GetComponent<MeshRenderer>().sharedMaterials[0];
    }

    private void AddComponentDoorPanel(ModelObject modelObject, GameObject panel, int submeshIndex)
    {
        GameObject component = new GameObject("Component " + submeshIndex);
        component.transform.parent = panel.transform;
        component.transform.localScale = new Vector3(1, 1, 1);
        component.transform.localPosition = new Vector3(0, 0, 0);
        component.transform.localRotation = new Quaternion();

        MeshFilter meshFilter = component.AddComponent<MeshFilter>();
        MeshCollider meshCollider = component.AddComponent<MeshCollider>();
        MeshRenderer meshRenderer = component.AddComponent<MeshRenderer>();

        Mesh myMesh = modelObject.GetComponent<MeshFilter>().sharedMesh;
//		List<Vector2> myUVs = new List<Vector2> ();
//		myMesh.GetUVs(submeshIndex,myUVs);
//
        Mesh panelMesh = new Mesh();

        int[] oldTrianges = myMesh.GetTriangles(submeshIndex);

        int count = 0;
        Dictionary<int, int> dictionary = new Dictionary<int, int>();
        for (int x = 0; x < oldTrianges.Length; x++)
        {
            int current = oldTrianges[x];

            if (!dictionary.ContainsKey(current))
            {
                dictionary.Add(current, count);
                count = count + 1;
            }
        }

        int[] newTriangles = new int[oldTrianges.Length];
        for (int x = 0; x < oldTrianges.Length; x++)
        {
            newTriangles[x] = dictionary[oldTrianges[x]];
        }

        Vector3[] oldVerts = myMesh.vertices;
        Vector3[] newVerts = new Vector3[count];
        foreach (KeyValuePair<int, int> pair in dictionary)
        {
            int oldVertIndex = pair.Key;
            int newVertIndex = pair.Value;
            newVerts[newVertIndex] = oldVerts[oldVertIndex];
        }

        panelMesh.vertices = newVerts;
        panelMesh.triangles = newTriangles;


//		panelMesh.SetUVs(0, myUVs);

		Vector2[] uvs = new Vector2[newVerts.Length];

		for (int i = 0; i < newVerts.Length; i++) {

			uvs [i] = new Vector2 (newVerts [i].x , newVerts [i].z);
		}

		panelMesh.uv = uvs;
		//panelMesh.RecalculateBounds();
        panelMesh.RecalculateNormals();
        panelMesh.Optimize();

        meshRenderer.material = modelObject.GetComponent<MeshRenderer>().sharedMaterials[submeshIndex];
        meshFilter.mesh = panelMesh;
        meshCollider.sharedMesh = panelMesh;
    }
}
