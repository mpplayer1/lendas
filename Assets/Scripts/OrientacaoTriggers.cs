using UnityEngine;
using System.Collections;

public class OrientacaoTriggers : MonoBehaviour 
{
	GameObject[] pivot;// [GameObject.FindGameObjectsWithTag("PivotGira").Length] = GameObject.FindGameObjectsWithTag("PivotGira");

	// Use this for initialization
	void Start ()
	{
		pivot = GameObject.FindGameObjectsWithTag("PivotGira");
	}
	
	// Update is called once per frame
	void Update ()
	{
		for(int i=0; i < pivot.Length; i++)
		{
			pivot[i].transform.eulerAngles = GameObject.Find("Carro").transform.eulerAngles;
		}
	}
}
