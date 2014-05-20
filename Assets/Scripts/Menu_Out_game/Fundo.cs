using UnityEngine;
using System.Collections;
//using MDToolkit;

public class Fundo : MonoBehaviour {
	//MDBase mdb;
	// Use this for initialization
	void Start () {
		OT.inputCameras[0] = GameObject.Find ("Main Camera").camera;// Camera[]{GameObject.Find ("Main Camera").camera};
	}
	
	// Update is called once per frame
	void Update () {
//		if(OT.Clicked(this.gameObject, 0)){
//			Debug.Log ("Clicou");
//		}
//		if(MDBase.Clicked(this.gameObject, 0)){
//			Debug.Log("Clicou");
//			Debug.Break();
//		}
//		Debug.Log (".");
	}
}
