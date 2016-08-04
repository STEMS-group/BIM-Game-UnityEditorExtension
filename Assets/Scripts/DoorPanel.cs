using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class DoorPanel : MonoBehaviour
{
    public enum RotationPivot { Left, Right };
    private static int DEFAULT_ROTATION_ANGLE = 90;
	public static float WIDTH = 0.80f;

    public RotationPivot Pivot = RotationPivot.Left;
    public bool Clockwise = true;
    private int rotationAngle = DEFAULT_ROTATION_ANGLE;

	private float _width = WIDTH;

    public bool Opened = false;

    public void OpenOnClockwise(bool direction)
    {
        Clockwise = direction;
        
    }

    public void OpenDoor()
    {
        if (!Opened)
        {
            transform.Rotate(0, 0, rotationAngle);
            Opened = true;
        }
        else
        {
            transform.Rotate(0, 0, -rotationAngle);
            Opened = false;
        }
    }

    public void UpdateOpeningDirection()
    {
		_width = Pivot == RotationPivot.Left ? -WIDTH : WIDTH;
		rotationAngle = Clockwise ? DEFAULT_ROTATION_ANGLE : -DEFAULT_ROTATION_ANGLE;

		//gameObject.GetComponentInChildren<Transform>().localPosition = new Vector3(_width, 0, 0);
		//gameObject.transform.Translate(_width/2, 0, 0);
		gameObject.transform.localPosition = new Vector3(_width/2,0,0);
		gameObject.GetComponentInParent<BoxCollider>().center = new Vector3(-_width/2,0,0);

		foreach (Transform child in transform) {
			//child.Translate(-_width/2, 0, 0);
			child.localPosition = new Vector3(-_width/2,0,0);
		}
    }
}

