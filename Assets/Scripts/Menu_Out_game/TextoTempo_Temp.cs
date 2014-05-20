using UnityEngine;
using System.Collections;

public class TextoTempo_Temp : MonoBehaviour {
	Status script;
	string tmp;
	// Use this for initialization
	void Start () 
    {
        

//		GameObject obj = GameObject.Find("Status");
//		this.script = obj.GetComponent<Status>();
//		try{
//			tmp = "TEMPO: ";
//			if(script.tempoFase != -1f)//goto TEMPO_DEFAULT;
//				tmp += script.tempoFase.ToString();
//			else
//				tmp += "--.--";
//		} catch{
//			Debug.LogError("TextoTempo_Temp: valor de tempo nao pode ser atribuido");
//		}
		//tmp = "TEMPO: --.--";//"TEMPO: --/--/--";
	}
	
	// Update is called once per frame
	void Update () {
	
	}

//	void OnGUI()
//    {
//        GUI.skin.label.fontSize = 12;
//
//		GUI.Label(new Rect(0f, 0f,100,100), this.tmp);
//		//GUI.Label(new Rect(this.gameObject.transform.position.x, this.gameObject.transform.position.y,100,100), "TEMPO:");
//	}
}
