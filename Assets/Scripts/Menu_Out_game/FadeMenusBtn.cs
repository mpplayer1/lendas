using UnityEngine;
using System.Collections;

public class FadeMenusBtn : MonoBehaviour {

    Status refScriptStatus = null;
    Camera camera1 = null;

	// Use this for initialization
	void Start () 
    {
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
                                switch (hit.collider.tag)
                                {
                                    case "PRedBtnM"://Pressionou o Botao menu
                                        {
                                            if (Application.loadedLevelName == "Jogo")//Testar se ta no jogo para...
                                            {
                                                GameObject.Destroy(GameObject.Find("Musica")); // ...parar a musica do jogo
                                            }

                                            if (refScriptStatus == null)
                                                refScriptStatus = GameObject.FindGameObjectWithTag("Status").GetComponent<Status>();

                                            refScriptStatus.SetSelecionarBotaoFase();

                                            ScriptObjGerenciador refScriptGerente = GameObject.FindGameObjectWithTag("GameController").GetComponent<ScriptObjGerenciador>();
                                            refScriptGerente.PauseResumeGame(true);

                                            Application.LoadLevel("Tela_Fases");
                                        }
                                        break;

                                    case "PRedBtnR"://Reinicia a fase
                                        {
                                            ScriptObjGerenciador refScriptGerente = GameObject.FindGameObjectWithTag("GameController").GetComponent<ScriptObjGerenciador>();
                                            refScriptGerente.PauseResumeGame(true);

                                            Application.LoadLevel(Application.loadedLevel);
                                        }
                                        break;
                                }

                            }
                        }
                    }
                }
            }
        }
        else //PC
        {
            if (OT.Clicked(this.gameObject, 0) )
            {
                if (this.tag == "PRedBtnM")//Pressionou o Botao menu
                {
                    if (Application.loadedLevelName == "Jogo")//Testar se ta no jogo para...
                    {
                        GameObject.Destroy(GameObject.Find("Musica")); // ...parar a musica do jogo
                    }

                    if (refScriptStatus == null) 
                        refScriptStatus = GameObject.FindGameObjectWithTag("Status").GetComponent<Status>();

                    refScriptStatus.SetSelecionarBotaoFase();

                    ScriptObjGerenciador refScriptGerente = GameObject.FindGameObjectWithTag("GameController").GetComponent<ScriptObjGerenciador>();
                    refScriptGerente.PauseResumeGame(true);

                    Application.LoadLevel("Tela_Fases");

                }
                else if (this.tag == "PRedBtnR")//Reinicia a fase
                {
                    ScriptObjGerenciador refScriptGerente = GameObject.FindGameObjectWithTag("GameController").GetComponent<ScriptObjGerenciador>();
                    refScriptGerente.PauseResumeGame(true);


                    Application.LoadLevel(Application.loadedLevel);
                }
            }
        }
        
        


	}
}
