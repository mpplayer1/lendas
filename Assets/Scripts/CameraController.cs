using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour 
{
	public GameObject pontoFinal;
	public GameObject pontoInicial;
	public GameObject pontoSeguir;
	public Rigidbody carro;
	public float modAlturaCam = 4.0f;
	public float modHoriCam = 4.0f;
	
	// Use this for initialization
	void Start()
	{
		// Resumir sons ou liberar AudioListener caso nao esteja:
		AudioListener.pause = false;
	}
	
	void Update()
	{
		if(carro.velocity.magnitude < 10)
		{
			pontoSeguir.transform.position = Vector3.Lerp(pontoInicial.transform.position, pontoFinal.transform.position, carro.velocity.magnitude / modHoriCam);
		}
	}
	
	// Update is called once per frame
	void FixedUpdate()
	{
		transform.position = Vector3.Lerp(transform.position, pontoSeguir.transform.position, 7*Time.deltaTime);
	}
}
