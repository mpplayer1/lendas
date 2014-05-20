using UnityEngine;
using System.Collections;

public class CamJogoScript : MonoBehaviour {
	GameObject carro;
	ControleCarro cCar; // Script de carro;
	float oldAngY;
	public bool autoRot = true;
	// Use this for initialization
	void Start () {
		this.oldAngY = this.transform.eulerAngles.y;
		try{
			carro = GameObject.Find ("Carro");
			cCar = carro.GetComponent<ControleCarro>();
		} catch{
			Debug.LogError("A camera nao encontrou o objeto \"Carro!\"");
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(autoRot){
			if(this.gameObject.transform.eulerAngles.y != oldAngY){
				cCar.AtualizarLimites(this.transform);
				//Debug.Log ("!");
				this.oldAngY = this.gameObject.transform.eulerAngles.y;
			}
		}
	}
}
