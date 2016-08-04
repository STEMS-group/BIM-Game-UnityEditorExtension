using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditor;
using System.Globalization;
using System.Text;

public class ModelObject : MonoBehaviour
{
    public string Id;
    public string Name;
    public string Type;
    public string Category;
    public string Obs;
    //private string FireRating;
    //private string FeatTransferCoeficient;
    //private string ThermalResistance;
    //private string ThermalMass;
    //private string Absorptance;

    public List<ModelMaterial> Materials = new List<ModelMaterial>();

    public GameObject Object;

    //public string Id
    //{
    //    get
    //    {
    //        return _id;
    //    }

    //    set
    //    {
    //        _id = value;
    //    }
    //}

    //public string Name
    //{
    //    get
    //    {
    //        return _name;
    //    }

    //    set
    //    {
    //        _name = value;
    //    }
    //}

    //public GameObject Object
    //{
    //    get
    //    {
    //        return _object;
    //    }

    //    set
    //    {
    //        _object = value;
    //    }
    //}

    //public string Type
    //{
    //    get
    //    {
    //        return _type;
    //    }

    //    set
    //    {
    //        _type = value;
    //    }
    //}

    //public string Category
    //{
    //    get
    //    {
    //        return _category;
    //    }

    //    set
    //    {
    //        _category = value;
    //    }
    //}

    //public List<ModelMaterial> Materials
    //{
    //    get
    //    {
    //        return _objectMaterials;
    //    }

    //    set
    //    {
    //        _objectMaterials = value;
    //    }
    //}

    public void AddMaterials(string result, bool updateMaterial)
    {
        string[] materialsStr = result.Split('#');

        Object.GetComponents(Materials);

        Materials.ForEach(mat => DestroyImmediate(mat));

        foreach (string m in materialsStr)
        {
            if (m == "") continue;

            string[] parameters = m.Substring(1, m.Length - 2).Split(';');

            ModelMaterial mat = Object.AddComponent<ModelMaterial>();

            foreach (string p in parameters)
            {
                string[] keyValue = p.Split(':');

                switch (keyValue[0])
                {
                    case "id":
                        mat.Id = keyValue[1];
                        break;
                    case "name":
                        mat.Name = keyValue[1].Replace('/', '-');
                        break;
                    case "color":
                        string[] values = keyValue[1].Substring(1, keyValue[1].Length - 2).Split(',');
                        mat.Color = new Color(
                            float.Parse(values[0], CultureInfo.InvariantCulture.NumberFormat) / 255,
                            float.Parse(values[1], CultureInfo.InvariantCulture.NumberFormat) / 255,
                            float.Parse(values[2], CultureInfo.InvariantCulture.NumberFormat) / 255,
                            1 - float.Parse(values[3], CultureInfo.InvariantCulture.NumberFormat) / 100);
                        break;
                    case "shininess":
                        mat.Shininess = 1 - float.Parse(keyValue[1], CultureInfo.InvariantCulture.NumberFormat) / 255;
                        break;
                    case "smothness":
                        mat.Smothness = 1 - float.Parse(keyValue[1], CultureInfo.InvariantCulture.NumberFormat) / 100;
                        break;
                    default:
                        break;
                }
            }

            if (mat.Name != "")
            {
                Materials.Add(mat);

                if (!MaterialExists(mat) || updateMaterial)
                {
                    CreateMaterial(mat);
                }

            }
            mat.Applied = false;
        }
    }

    private bool MaterialExists(ModelMaterial mat)
    {
        return Resources.Load<Material>("Materials/" + mat.Id + "-" + mat.Name) != null;
    }

    public bool HasFrame()
    {
        return Obs != "No Frame";
    }

    public void UpdateInfo(string result)
    {
        string elementInfo = result.Split('#')[0];

        string[] parameters = elementInfo.Substring(1, elementInfo.Length - 2).Split(';');

        foreach (string p in parameters)
        {
            string[] keyValue = p.Split(':');

            switch (keyValue[0])
            {
                //case "id":
                //    Id = keyValue[1];
                //    break;
                //case "name":
                //    Name = keyValue[1].Replace('/', '-');
                //    break;
                case "category":
                    Category = keyValue[1];
                    break;
                case "type":
                    Type = keyValue[1];
                    break;
                case "obs":
                    Obs = keyValue[1];
                    break;
                default:
                    break;
            }

        }
    }

    public void ApplyMaterialsToSameTypeObjects()
    {

    }

    public void ApplyMaterial(ModelMaterial mat)
    {
        Materials.ForEach(m =>
        {
            if (m.Id == mat.Id)
            {
                m.Applied = true;

                var materialToApply = Resources.Load<Material>("Materials/" + mat.Id + "-" + mat.Name);

                MeshRenderer render = Object.GetComponent<MeshRenderer>();
                if (render == null)
                {
                    render = Object.AddComponent<MeshRenderer>();
                }
                render.material = materialToApply;

            }
            else
            {
                m.Applied = false;
            }
        });
    }

    private void CreateMaterial(ModelMaterial mat)
    {
        var material = new Material(Shader.Find("Standard"));
        AssetDatabase.CreateAsset(material, "Assets/Resources/Materials/" + mat.Id + "-" + mat.Name + ".mat");
        material.color = mat.Color;
        material.SetFloat("_Glossiness", mat.Smothness);
        material.SetFloat("_Metallic", mat.Shininess);

        if (material.color[3] < 1)
        {
            material.SetFloat("_Mode", 3);
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.DisableKeyword("_ALPHATEST_ON");
            material.EnableKeyword("_ALPHABLEND_ON");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = 3000;
        }


    }

    public void AddMaterial(ModelMaterial material)
    {
        Materials.Add(material);
    }

    public ModelMaterial GetMaterial(string id)
    {
        foreach (ModelMaterial mat in Materials)
        {
            if (mat.Id == id)
            {
                return mat;
            }
        }
        return null;
    }
}
