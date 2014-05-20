using UnityEngine;
using System.Collections;

public class SelfDestruction : MonoBehaviour {

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad(this.gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		if(!Application.isLoadingLevel){ // Destroi o objeto se a cena ja houver sido carregada;
			Destroy(this.gameObject);
		}
	}
}
