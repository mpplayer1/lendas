using UnityEngine;
using System.Collections;

public class Btn3 : MonoBehaviour {
	// Somente para classe usada no menu:
	ScriptObjGerenciador script;
    Camera camera1 = null;
    Status refScriptStatus = null;

	// Use this for initialization
	void Start () 
	{
		try
		{
			GameObject obj = GameObject.Find ("objGerenciador");// Referencia do objeto da classe ScriptObjGerenciador;
			script = obj.GetComponent<ScriptObjGerenciador>();
		} 
		catch
		{
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
                                    case "ButtonBack":
                                        {
                                            if (Application.loadedLevelName == "Jogo")//Testar se ta no jogo para...
                                            {
                                                GameObject.Destroy(GameObject.Find("Musica")); // ...parar a musica
                                            }

                                            if (script)
                                                if (script.pausedGame)
                                                    script.PauseResumeGame(true);
                                            //			// Resumir sons:
                                            //			AudioListener.pause = false;
                                            Application.LoadLevel("Tela_Principal");

                                        }
                                        break;

                                    case "ButtonPMenuPrincipal":
                                        {
                                            if (Application.loadedLevelName == "Jogo")//Testar se ta no jogo para...
                                            {
                                                GameObject.Destroy(GameObject.Find("Musica")); // ...parar a musica
                                            }

                                            if (script)
                                                if (script.pausedGame)
                                                    script.PauseResumeGame(true);
                                            //			// Resumir sons:
                                            //			AudioListener.pause = false;
                                            Application.LoadLevel("Tela_Principal");

                                        }
                                        break;

                                }
                                //hit.transform.SendMessage("Clicked");
                            }
                        }
                    }

                }
            }
        }//fim de android ou ios
        else//PC
        {
            if (OT.Clicked(this.gameObject, 0) )
            { // Verifica se colidiu com mouse ou touch;
                // Teste para quando despausar o jogo;
                if (Application.loadedLevelName == "Jogo")//Testar se ta no jogo para...
                {
                    GameObject.Destroy(GameObject.Find("Musica")); // ...parar a musica
                }
                if (script)
                    if (script.pausedGame)
                        script.PauseResumeGame(true);
    //			// Resumir sons:
    //			AudioListener.pause = false;
                Application.LoadLevel("Tela_Principal");
            }
        }




	}
}
