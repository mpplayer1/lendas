using UnityEngine;
using System.Collections;

public class PauseMenuPlane : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Vector3 vTemp = this.gameObject.transform.lossyScale;
		//Debug.Log (vTemp);
		vTemp.x = vTemp.x * Screen.width;
		vTemp.z = vTemp.z * Screen.height;
		this.gameObject.transform.localScale = vTemp;//new Vector3(Screen.width, 1f, Screen.height);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
