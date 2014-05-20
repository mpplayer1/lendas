using UnityEngine;
using System.Collections;

public class BtnPause : MonoBehaviour
{
    // Atributos:
    ScriptObjGerenciador scriptRef;
    ControleCarro scriptRef2;

    Camera camera1 = null;
    Status refScriptStatus = null;

    // Use this for initialization
    void Start()
    {
        try
        {
            GameObject obj = GameObject.Find("objGerenciador");// Referencia do objeto da classe ScriptObjGerenciador;
            scriptRef = obj.GetComponent<ScriptObjGerenciador>();
            obj = GameObject.Find("Carro");
            scriptRef2 = obj.GetComponent<ControleCarro>();
        }
        catch
        {
            Application.Quit(); // Sai da aplicaçao pois nao conseguiu achar o objeto;
        }

        if (camera1 == null)
            camera1 = GameObject.Find("Main Camera").GetComponent<Camera>();

        if (refScriptStatus == null)
            refScriptStatus = GameObject.Find("Status").GetComponent<Status>();
    }

    // Update is called once per frame
    void Update()
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

                                // Menu de pausa e pausar o jogo:
                                GameObject[] objetos;

                                switch (hit.collider.name)
                                {
                                    case "ButtonPause":
                                        {
                                            if (this.scriptRef.pausedGame)//se o jogo tiver pausado -- quando aperta o voltar
                                            {
                                                new WaitForSeconds(1); // Subroutine;

                                                this.scriptRef.PauseResumeGame(true);//this.scriptRef.btnPausePressed = !this.scriptRef.btnPausePressed;// Pausar jogo;
                                                // Resumir sons:
                                                AudioListener.pause = false;

                                                GameObject.Find("FundoTelaPause").gameObject.GetComponent<MeshRenderer>().enabled = false;

                                                GameObject btnPause = GameObject.Find("ButtonPause").gameObject;

                                                btnPause.GetComponent<MeshRenderer>().enabled = true;
                                                btnPause.GetComponent<BoxCollider>().enabled = true;

                                                objetos = GameObject.FindGameObjectsWithTag("pauseMenu");

                                                foreach (GameObject element in objetos)
                                                {
                                                    element.gameObject.GetComponent<MeshRenderer>().enabled = false;//element.gameObject.SetActive(false);
                                                    element.gameObject.GetComponent<BoxCollider>().enabled = false;
                                                }
                                            }
                                            else
                                            {
                                                this.scriptRef.PauseResumeGame(false);
                                                // Parar sons:
                                                AudioListener.pause = true;

                                                GameObject.Find("FundoTelaPause").gameObject.GetComponent<MeshRenderer>().enabled = true;
                                                //this.gameObject.SetActive(false); // Desativa este objeto;

                                                GameObject btnPause = GameObject.Find("ButtonPause").gameObject;

                                                btnPause.GetComponent<MeshRenderer>().enabled = false;
                                                btnPause.GetComponent<BoxCollider>().enabled = false;

                                                // Ativa os botoes e o fundo:
                                                //try{
                                                objetos = GameObject.FindGameObjectsWithTag("pauseMenu");
                                                foreach (GameObject element in objetos)
                                                {
                                                    element.gameObject.GetComponent<MeshRenderer>().enabled = true;
                                                    element.gameObject.GetComponent<BoxCollider>().enabled = true;
                                                }

                                            }

                                            break;
                                        }//fim do case

                                    case "ButtonPVoltar":
                                        {
                                            if (this.scriptRef.pausedGame)//se o jogo tiver pausado -- quando aperta o voltar
                                            {
                                                new WaitForSeconds(1); // Subroutine;

                                                this.scriptRef.PauseResumeGame(true);//this.scriptRef.btnPausePressed = !this.scriptRef.btnPausePressed;// Pausar jogo;
                                                // Resumir sons:
                                                AudioListener.pause = false;

                                                GameObject.Find("FundoTelaPause").gameObject.GetComponent<MeshRenderer>().enabled = false;

                                                GameObject btnPause = GameObject.Find("ButtonPause").gameObject;

                                                btnPause.GetComponent<MeshRenderer>().enabled = true;
                                                btnPause.GetComponent<BoxCollider>().enabled = true;

                                                objetos = GameObject.FindGameObjectsWithTag("pauseMenu");

                                                foreach (GameObject element in objetos)
                                                {
                                                    element.gameObject.GetComponent<MeshRenderer>().enabled = false;//element.gameObject.SetActive(false);
                                                    element.gameObject.GetComponent<BoxCollider>().enabled = false;
                                                }
                                            }
                                            else
                                            {
                                                this.scriptRef.PauseResumeGame(false);
                                                // Parar sons:
                                                AudioListener.pause = true;

                                                GameObject.Find("FundoTelaPause").gameObject.GetComponent<MeshRenderer>().enabled = true;
                                                //this.gameObject.SetActive(false); // Desativa este objeto;

                                                GameObject btnPause = GameObject.Find("ButtonPause").gameObject;

                                                btnPause.GetComponent<MeshRenderer>().enabled = false;
                                                btnPause.GetComponent<BoxCollider>().enabled = false;

                                                // Ativa os botoes e o fundo:
                                                //try{
                                                objetos = GameObject.FindGameObjectsWithTag("pauseMenu");
                                                foreach (GameObject element in objetos)
                                                {
                                                    element.gameObject.GetComponent<MeshRenderer>().enabled = true;
                                                    element.gameObject.GetComponent<BoxCollider>().enabled = true;
                                                }

                                            }
                                            break;
                                        }


                                }


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
                // Menu de pausa e pausar o jogo:
                GameObject[] objetos;
                if (this.scriptRef.pausedGame)
                {
                    new WaitForSeconds(1); // Subroutine;

                    this.scriptRef.PauseResumeGame(true);//this.scriptRef.btnPausePressed = !this.scriptRef.btnPausePressed;// Pausar jogo;
                    // Resumir sons:
                    AudioListener.pause = false;

                    GameObject.Find("FundoTelaPause").gameObject.GetComponent<MeshRenderer>().enabled = false;
                    //try{
                    //GameObject.Find("ButtonPause").gameObject.SetActive(true);
                    //GameObject.FindGameObjectWithTag("botaoPausaInGame").gameObject.SetActive(true);
                    //GameObject objPausa = GameObject.Find("ButtonPause");
                    //objPausa.gameObject.SetActive(true);
                    //} catch{Debug.LogError("Nope");}
                    GameObject.Find("ButtonPause").gameObject.GetComponent<MeshRenderer>().enabled = true;
                    GameObject.Find("ButtonPause").gameObject.GetComponent<BoxCollider>().enabled = true;

                    objetos = GameObject.FindGameObjectsWithTag("pauseMenu");
                    foreach (GameObject element in objetos)
                    {
                        element.gameObject.GetComponent<MeshRenderer>().enabled = false;//element.gameObject.SetActive(false);
                        element.gameObject.GetComponent<BoxCollider>().enabled = false;
                    }
                }
                else
                {
                    this.scriptRef.PauseResumeGame(false);
                    // Parar sons:
                    AudioListener.pause = true;

                    GameObject.Find("FundoTelaPause").gameObject.GetComponent<MeshRenderer>().enabled = true;
                    //this.gameObject.SetActive(false); // Desativa este objeto;
                    GameObject.Find("ButtonPause").gameObject.GetComponent<MeshRenderer>().enabled = false;
                    GameObject.Find("ButtonPause").gameObject.GetComponent<BoxCollider>().enabled = false;

                    // Ativa os botoes e o fundo:
                    //try{
                    objetos = GameObject.FindGameObjectsWithTag("pauseMenu");
                    foreach (GameObject element in objetos)
                    {
                        element.gameObject.GetComponent<MeshRenderer>().enabled = true;
                        element.gameObject.GetComponent<BoxCollider>().enabled = true;
                    }
                    //				for(int i=0; i<objetos.Length; i++){
                    ////					Debug.Log (i);
                    ////					Debug.Break();
                    //					objetos[i].gameObject.SetActive(true);
                    //				}
                    //} catch{Debug.LogError ("Falhou");}
                }
            }
        }

    }//fim update



}
        
    
       






////		if (pausado) {
////			// cria botoes e imagem de fundo que faz a tela parecer uma pop-up;
////		}

/*
if (OT.Clicked (this.gameObject) || OT.Touched(this.gameObject)) {
			// Menu de pausa e pausar o jogo:
			GameObject[] objetos;
			if(this.scriptRef.pausedGame){
				this.scriptRef.PauseResumeGame(true);//this.scriptRef.btnPausePressed = !this.scriptRef.btnPausePressed;// Pausar jogo;
				try{
					//GameObject.Find("ButtonPause").gameObject.SetActive(true);
					GameObject.FindGameObjectWithTag("botaoPausaInGame").gameObject.SetActive(true);
					//GameObject objPausa = GameObject.Find("ButtonPause");
					//objPausa.gameObject.SetActive(true);
				} catch{Debug.LogError("Nope");}
				objetos = GameObject.FindGameObjectsWithTag("pauseMenu");
				foreach(GameObject element in objetos){
					element.gameObject.SetActive(false);
				}
			} else{
				this.scriptRef.PauseResumeGame(false);
				this.gameObject.SetActive(false); // Desativa este objeto;

				// Ativa os botoes e o fundo:
				//try{
				objetos = GameObject.FindGameObjectsWithTag("pauseMenu");
				foreach(GameObject element in objetos){
					element.gameObject.SetActiveRecursively(true);
				}
//				for(int i=0; i<objetos.Length; i++){
////					Debug.Log (i);
////					Debug.Break();
//					objetos[i].gameObject.SetActive(true);
//				}
				//} catch{Debug.LogError ("Falhou");}
			}
 */