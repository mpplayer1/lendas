using UnityEngine;
using System.Collections;

public class Btn4 : MonoBehaviour {
    Status refScriptStatus = null;// Referencia para status;
	// Use this for initialization

    Camera camera1 = null;

	void Start () 
    {
		try
        {
            refScriptStatus = GameObject.Find("Status").GetComponent<Status>();
		}
        catch
        {
            refScriptStatus = null;
		}

        if (camera1 == null)
            camera1 = GameObject.Find("Main Camera").GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (!refScriptStatus.carregando())
        {
			this.renderer.enabled = true;

            if (refScriptStatus.isAndroid
            || refScriptStatus.isiOS)
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
                                    case "ButtonPlay":
                                        {
                                            //refScriptStatus = GameObject.FindGameObjectWithTag("Status").GetComponent<Status>();

                                            refScriptStatus.SetSelecionarBotaoFase();

                                            Application.LoadLevel("Tela_Fases");

                                        }
                                        break;

                                    case "ButtonBack":
                                        {
                                            //refScriptStatus = GameObject.FindGameObjectWithTag("Status").GetComponent<Status>();
                                            refScriptStatus.SetSelecionarBotaoFase();

                                            Application.LoadLevel("Tela_Fases");

                                        }
                                        break;
                                }
                                //hit.transform.SendMessage("Clicked");
                            }
                        }
                    }

                }
            }
            else//PC
            {
                if (OT.Clicked(this.gameObject, 0))
                { // Verifica se colidiu com mouse ou touch;
                    //botao proximo, que leva a seleção de fase, e tambem o botao que volta
                    if (refScriptStatus == null)
                           refScriptStatus = GameObject.FindGameObjectWithTag("Status").GetComponent<Status>();

                    refScriptStatus.SetSelecionarBotaoFase();

                    Application.LoadLevel("Tela_Fases");
                }
            }

            
		} 
        else
        {
			this.renderer.enabled = false;
		}
	}
}
