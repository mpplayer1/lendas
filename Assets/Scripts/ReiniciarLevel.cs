using UnityEngine;
using System.Collections;

public class ReiniciarLevel : MonoBehaviour {
	ScriptObjGerenciador script;
	public Texture2D imgLoading;
	bool desenhaLoading = false;
    Status refScriptStatus = null;

    Camera camera1 = null;

	// Use this for initialization
	void Start () {
		try{
			GameObject obj = GameObject.Find ("objGerenciador");// Referencia do objeto da classe ScriptObjGerenciador;
			script = obj.GetComponent<ScriptObjGerenciador>();
		} catch{
			//Application.Quit(); // Sai da aplicaçao pois nao conseguiu achar o objeto;
		}

        if (camera1 == null)
            camera1 = GameObject.Find("Main Camera").GetComponent<Camera>();

        if (refScriptStatus == null)
            refScriptStatus = GameObject.Find("Status").GetComponent<Status>();
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (refScriptStatus.isAndroid
          || refScriptStatus.isiOS)
        {

            if (this.renderer.enabled)//Se o botao estiver visivel, dai faz a checagem, caso contrario vai ficar tocando raycast quando o jogador tocar na tela para mover o carro
            {
                if (Input.touchCount >= 1)
                {
                    foreach (Touch touch in Input.touches)
                    {
                        if (touch.phase == TouchPhase.Began)
                        {
                            Ray ray = camera1.ScreenPointToRay(touch.position);
                            RaycastHit hit;

                            if (Physics.Raycast(ray, out hit))
                            {
                                switch (hit.collider.name)
                                {
                                    case "ButtonPReiniciar":
                                        {
                                            this.desenhaLoading = true;
                                            if (script)
                                                if (script.pausedGame)
                                                    script.PauseResumeGame(true);
                                            //			// Resumir sons:
                                            //			AudioListener.pause = false;
                                            Application.LoadLevel(Application.loadedLevel);
                                        }
                                        break;
                                }
                                //hit.transform.SendMessage("Clicked");
                            }
                        }
                    }
                }
            }
        }
        else//PC
        {
            if (OT.Clicked(this.gameObject) )
            {
                this.desenhaLoading = true;
                if (script)
                    if (script.pausedGame)
                        script.PauseResumeGame(true);
                //			// Resumir sons:
                //			AudioListener.pause = false;
                Application.LoadLevel(Application.loadedLevel);
            }
        }




	}

	void OnGUI(){
		if(desenhaLoading)
			GUI.DrawTexture(new Rect(0,0, Screen.width, Screen.height), imgLoading);
	}
}
