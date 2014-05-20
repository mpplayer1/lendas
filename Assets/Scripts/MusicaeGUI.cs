using UnityEngine;
using System.Collections;

public class MusicaeGUI : MonoBehaviour {

	private static MusicaeGUI instance = null;
	// Use this for initialization
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	public static MusicaeGUI Instance 
	{
		get { return instance; }
	}
	void Awake() 
	{
		if (instance != null && instance != this) 
		{
			Destroy(this.gameObject);
			return;
		}
		else
		{
			instance = this;
		}

		DontDestroyOnLoad(this.gameObject);
	}
}
