using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DoorPanel))]
class DoorPanelEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Space(10);

        DoorPanel door = (DoorPanel)target;

        if (!door.Opened)
        {
            if (GUILayout.Button("Open Door"))
            {
                ((DoorPanel)target).OpenDoor();
            }
        }
        else
        {
            if (GUILayout.Button("Close Door"))
            {
                ((DoorPanel)target).OpenDoor();
            }

        }
        
		if (GUILayout.Button("Update Opening Door Settings"))
		{
			((DoorPanel)target).UpdateOpeningDirection();
		}

        if (GUILayout.Button("Repeat to Doors of same type"))
        {
            Debug.Log("TODO....");
        }
        

    }
}

