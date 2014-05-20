using UnityEngine;
using System.Collections;


//SCRIPT PAR A ANIMACAO DE VITORIA
public class VitAnim : MonoBehaviour 
{
    //Skin do relogio digital para a gui
    private GUISkin SkinGUIDigital;

    private System.TimeSpan tempoDisplayJogo;
    float tempoV;//tempo de vitoria.

    bool ShowTime;
    //bool testarBtn;
    private Status scriptStatus;
    
    // Use this for initialization
	void Start () 
    {
        ShowTime = false;
        //testarBtn = false;

        //Carrega a Skin da gui
        SkinGUIDigital = Resources.Load("Font/DS-DIGIISkin") as GUISkin;

     	GameObject statustemp = GameObject.Find("Status");
        if (statustemp != null)
        {
            scriptStatus = statustemp.GetComponent<Status>();
            tempoV = scriptStatus.GettempoVenceujogo();
            tempoDisplayJogo = System.TimeSpan.FromSeconds(tempoV);//Atribui o tempo
        }
        
        StartCoroutine(CoroutineDisplayVitoria());

	}

    public IEnumerator CoroutineDisplayVitoria( )
    {
        //float tempoFrameInicial = 2.5f;
        //float tempoEntreFrames = 0.9f;
        //float tempoUltimoFrame = 1.5f;

        //Carrega as imagens de vitoria
        Texture2D[] ArrayTextureVitoria; 

        //Tenho que salvar o tempo de jogo no Status, para poder, ou nao destrui o objeto gerente

        //Animacao vitoria
        bool keepAnimVrunning = true;
        //int frameCounterVitoria = 0;
        //int maxFramesVitoria = 6;//6 frames de animacao e 1 de pontuacao

        bool ModeFade = true;//true é com fade, false é com animacao

        if (!ModeFade)
        {
            ArrayTextureVitoria = new Texture2D[7];

            ArrayTextureVitoria[0] = Resources.Load("Vitoria/vitoria_loira1") as Texture2D;
            ArrayTextureVitoria[1] = Resources.Load("Vitoria/vitoria_loira2") as Texture2D;
            ArrayTextureVitoria[2] = Resources.Load("Vitoria/vitoria_loira3") as Texture2D;
            ArrayTextureVitoria[3] = Resources.Load("Vitoria/vitoria_loira4") as Texture2D;
            ArrayTextureVitoria[4] = Resources.Load("Vitoria/vitoria_loira5") as Texture2D;
            ArrayTextureVitoria[5] = Resources.Load("Vitoria/vitoria_loira6") as Texture2D;
            ArrayTextureVitoria[6] = Resources.Load("Vitoria/vitoria_pontuacao") as Texture2D;

        }
        else// com fade
        {
            ArrayTextureVitoria = new Texture2D[2];

            ArrayTextureVitoria[0] = Resources.Load("Vitoria/vitoria_loira1") as Texture2D;
            ArrayTextureVitoria[1] = Resources.Load("Vitoria/vitoria_pontuacao") as Texture2D;

            gameObject.renderer.material.mainTexture = ArrayTextureVitoria[0];
        }


        while (keepAnimVrunning)
        {

            if (ModeFade)//Com fade
            {
                if (gameObject.renderer.material.color.a >= 0)
                {
                    gameObject.renderer.material.color = new Color(1f, 1f, 1f, gameObject.renderer.material.color.a - 0.05f);//0.22
                    yield return new WaitForSeconds(0.2f);
                }
                else
                {
                    yield return new WaitForSeconds(2.0f);

                    gameObject.renderer.material.color = new Color(0f, 0f, 0f, 0f);//0.22
                    //gameObject.renderer.material.color = new Color(1f, 1f, 1f, 1f);//0.22
                    //gameObject.renderer.material.mainTexture = ArrayTextureVitoria[1];
                    
                    GameObject fundo = GameObject.Find("Animfundo");
                    fundo.renderer.material.mainTexture = ArrayTextureVitoria[1];
                   
                    ShowTime = true;

                    //Verifica o tempo
                    scriptStatus.VerificarDefinirTempo();
                        //( //tempoDisplayJogo.Seconds                        );
                        //Debug.Log("tempo na tela vit: " + tempoDisplayJogo.TotalMinutes); 
                    
                    
                    //Manda uma ordem para liberar a proxima fase
                    //scriptStatus.SetLiberarProxFase(); //DESABILITADO PARA A VERSAO BETA
                 
                    GameObject btn = GameObject.Find("BtnNext");
                    //btn.renderer.enabled = true;
                    btn.collider.enabled = true;
                    btn.renderer.enabled = true;
                    keepAnimVrunning = false;
                }

            }
            //else //Com animacao - OLDDD
            //{
            //    gameObject.renderer.material.mainTexture = ArrayTextureVitoria[frameCounterVitoria];

            //    if (frameCounterVitoria == 0)//Primeiro frame com tempo maior
            //    {
            //        frameCounterVitoria++;
            //        yield return new WaitForSeconds(tempoFrameInicial);
            //    }
            //    else if (frameCounterVitoria + 1 != maxFramesVitoria)
            //    {
            //        frameCounterVitoria++;
            //        yield return new WaitForSeconds(tempoEntreFrames);
            //    }
            //    else//Ultimo frame tem tempo maior
            //    {
            //        yield return new WaitForSeconds(tempoUltimoFrame);
            //        frameCounterVitoria++;//vai pro ultimo frame
            //        gameObject.renderer.material.mainTexture = ArrayTextureVitoria[frameCounterVitoria];
            //        keepAnimVrunning = false;
            //        ShowTime = true;


            //        //Verifica o tempo
            //        scriptStatus.VerificarDefinirTempo();

            //        //Manda uma ordem para liberar a proxima fase
            //        scriptStatus.SetLiberarProxFase();

            //        //Habilita o botão
            //        GameObject btn = GameObject.Find("BtnNext");
            //        //btn.renderer.enabled = true;
            //        btn.collider.enabled = true;

            //    }
            //}


        }

        //Verificar se o tempo que o cara teve para essa fase é melhor que o melhorTempo dele nessa fase
         

    }

    void OnGUI()
    {
        if (ShowTime)
        {
            GUI.skin = SkinGUIDigital;

            GUI.contentColor = new Color(0.23f,0.15f,0.10f);//59.40.26
            GUI.skin.label.fontSize = 75;
            

            string niceTime = string.Format("{0:D2}:{1:D2}",
                            tempoDisplayJogo.Minutes,
                            tempoDisplayJogo.Seconds);

            GUI.Label(new Rect(Screen.width /2 - 50, (Screen.height / 2 - 16), 230, 100), niceTime);
        }


    }

	// Update is called once per frame
	void Update () 
    {
        //if (testarBtn)
        //{
        //    foreach (Touch touch in Input.touches)
        //    {
        //        if (touch.phase == TouchPhase.Began)
        //        {
        //            Ray ray = Camera.main.ScreenPointToRay(touch.position);
        //            RaycastHit hit;
        //            if (Physics.Raycast(ray, out hit))
        //            {
        //                hit.transform.SendMessage("Clicked");
        //            }
        //        }
        //    }
        //}
	
	}
}
