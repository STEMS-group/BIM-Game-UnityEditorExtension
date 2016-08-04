using UnityEngine;
using System.Collections;

public class DoorScript : MonoBehaviour {

	// Smothly open a door
	private float smooth = 2.0f;
	private float DoorOpenAngle = 90.0f;
	private bool open;
	private bool enter;

	private Vector3 defaultRot;
	private Vector3 openRot;

	//private bool enter;
	//public bool open = false;

	float Angle = 0f;
	int Speed = 1;

	// Use this for initialization
	void Start(){
		defaultRot = transform.eulerAngles;


		if (this.gameObject.GetComponent<DoorPanel> ().Clockwise) {
			openRot = new Vector3 (defaultRot.x, defaultRot.y + DoorOpenAngle, defaultRot.z);
		} else {
			openRot = new Vector3 (defaultRot.x, defaultRot.y - DoorOpenAngle, defaultRot.z);
		}
	}

	//Main function
	void Update (){
		if(open){
			//Open door
			transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, openRot, Time.deltaTime * smooth);
		}else{
			//Close door
			transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, defaultRot, Time.deltaTime * smooth);
		}

		if(Input.GetKeyDown("e") && enter){
			open = !open;
		}
	}

		
	void OnTriggerEnter (Collider other){
		if (other.gameObject.tag == "Player") {
			enter = true;
		}
	}
		
	void OnTriggerExit (Collider other){
		if (other.gameObject.tag == "Player") {
			enter = false;
		}
	}

	void OnGUI(){
		if(enter && !open){
			GUI.Label(new UnityEngine.Rect(Screen.width/2 - 75, Screen.height - 100, 150, 50), "Press 'E' to open");				
		}

		if(enter && open){
			GUI.Label(new UnityEngine.Rect(Screen.width/2 - 75, Screen.height - 100, 150, 50), "Press 'E' to close");				
		}
	}

}
