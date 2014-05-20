//#define DEBUG_MD // Para teste dos botoes;

using UnityEngine;
using System.Collections;

public class ScriptObjGerenciador : MonoBehaviour
{
    //Skin do relogio digital para a gui
    private GUISkin SkinGUIDigital;

	//Animacao retrovisor loira
	private int frameAtualAnimLoira;//Frame que a loira se encontra no momento
	private int maxFrameEstado;//Limite de frames para cada animaçao da loira
	private bool displayGood, displayBad; //Define se deve ou nao mostrar as imagens de good ou bad da loira
    private bool trocouState;
	public int estadoAtualLoira;//Nivel que a loira se encontra, começa em 0
	private GameObject imgLoiraAtual;//Objeto que mostra o espelho retrovisor
	private float tempoEachPhase;//Tempo de cada estado da loira, variavel conforme o tempo total de jogo

    
	private bool keepUpdatingTimer;//Se deve continuar atualizando o contador de tempo dentro da coroutine
	private bool keepCounterLoop;//Se deve continuar com a funçao de coroutine atualizando o timer

	//Controladoras globais -- Tempo de jogo
	public float tempoJogo;//tempo de jogo por fase, em segundos
	public float contadorPrincipal;
	
	//Timer
	private System.TimeSpan fatiaTempoJogo;//Timer de cada fase
	private Rect retanguloTimer;//Retangulo de display do timer
	private bool showTimer;//Mostra ou nao o timer da gui

    private bool podeDecrementarTempo;

	private bool endGame;
	private bool venceuGame;
	private bool enchendoIndicadorCombustivel;
	private bool diminuirFuelAfterFill;
	private int quantVezesDimFuelAfterFill;//Quantas vezes deve diminuir o combustivel caso ainda esteja enchendo

	public bool pausedGame;//Indica se o jogo esta pausado

	GameObject fuelMarker;

	private Transform InterfaceBussolaT;
	private Transform PontoCemiterioT;

  
	private GameObject refCarro;
	private ControleCarro scriptCarro;

	public float fuelMaximo;

	private Texture2D[] ArrayTextureLoira = new Texture2D[6];
  
    private Status refStatus = null;

    public GameObject displayEventsPlane;//Plano que mostra os eventos (velho,gato,policial ) na tela
    Texture2D[] arrayEventsPlane;
    private int planoexibidoporultimo;//indica quem foi o ultimo a utilizar do plano ( 0 velho, 1 gato, 2 policial )

	// Som
	public AudioClip somGood1, somGood2, somBad1, somBad2, somBad3;
	private EfeitosSonoros scriptSom;
	private bool somTocando;

#if DEBUG_MD
	//Testes
	public float x=0.5f, y = 0.5f, xOld, yOld;
#endif

	void Start () 
	{
		Random.seed=45;
		somTocando = false;

        refCarro = GameObject.Find("Carro");
		//Interface
		imgLoiraAtual = GameObject.FindGameObjectWithTag("ImgLoiraD");//Retrovisor loira
		
		InterfaceBussolaT = GameObject.FindGameObjectWithTag("InterBussola").transform;
		PontoCemiterioT = GameObject.FindGameObjectWithTag("PontoCemiterio").transform;
        
        fuelMarker = GameObject.FindGameObjectWithTag("PaiInterFuelMark");//Marcador de combustivel
       
		scriptSom = GameObject.Find("FonteEfeitos").GetComponent<EfeitosSonoros>();
		scriptCarro = GameObject.Find("Carro").GetComponent<ControleCarro>();

		try
		{
			refStatus = GameObject.Find("Status").GetComponent<Status>();
        }
        catch { refStatus = null; }

        if (refStatus != null)
            tempoJogo = refStatus.tempoDuracaoFaseAtual;//Atribui o tempo da fase no gerente baseado no que tempo presente no objeto status
		else
			tempoJogo = 180f;

		tempoEachPhase = (float)tempoJogo/4.1f;//44 segundos e pouco para cada fase

		estadoAtualLoira = -1;
		maxFrameEstado = 0;
		displayGood = false;
		displayBad = false;
        trocouState = false;
        podeDecrementarTempo = true;
		ChangeLoiraState(false);//Carrega o array de texturas da loira

		keepUpdatingTimer = true;
		keepCounterLoop = true;

		endGame = false;
		venceuGame = false;

		showTimer = true;
		Camera cameraT = GameObject.Find("Main Camera").GetComponent<Camera>();
#if DEBUG_MD
		xOld = x; yOld = y;
		Vector3 vec3RetT = cameraT.ViewportToScreenPoint(new Vector3(x, y, 1f)); // Viewport ali e normalizada de 0f a 1f do canto inverior esquerdo ate o canto superior direito;
#else
		Vector3 vec3RetT = cameraT.ViewportToScreenPoint(new Vector3(0.095f, 0.955f, 1f));
		retanguloTimer = new Rect(vec3RetT.x, vec3RetT.y, 80,40);//new Rect( Screen.width*0.11f , Screen.height * 0.96f, 80,40);
#endif
		enchendoIndicadorCombustivel = false;
		diminuirFuelAfterFill = false;
		quantVezesDimFuelAfterFill = 0;

        //Carrega a Skin da gui
        SkinGUIDigital = Resources.Load("Font/DS-DIGIISkin") as GUISkin;

        LoadEventsPlane();


     	StartCoroutine(AtualizadorContador());//Inicializa os contadores de tempo
	}
#if DEBUG_MD
	void TesteAtualiza(){
		Camera cameraT = GameObject.Find("Main Camera").GetComponent<Camera>();
		Vector3 vec3RetT = cameraT.ViewportToScreenPoint(new Vector3(x, y, 1f)); // Viewport ali e normalizada de 0f a 1f do canto inverior esquerdo ate o canto superior direito;
		retanguloTimer = new Rect(vec3RetT.x, vec3RetT.y, 80,40);

		xOld = x;
		yOld = y;
	}
#endif

	public void DiminuirPonteiroFuel()
	{
		if (!enchendoIndicadorCombustivel)//Se nao esta aumentando o combustivel
		{
            //120 - graus  
            float quantoDiminuir = 120 / fuelMaximo;

            //Diferença de rotaçao de angular de 100
            Vector3 angulo = fuelMarker.transform.eulerAngles;
            angulo.z -= quantoDiminuir;
            fuelMarker.transform.eulerAngles = angulo;

		}
		else
		{
			diminuirFuelAfterFill = true;
			quantVezesDimFuelAfterFill++;//Adiciona uma "ordem" para baixar combustivel
		}

	}

    IEnumerator AumentarCombustivelGradual(float quantoAumentou)
	{
		enchendoIndicadorCombustivel = true;
		//100 e a angulaçao total, 50 pra dir e 50 pra esquerda
        float anguloAumentar = (120 / fuelMaximo) * quantoAumentou;//cada vez que aumenta, deve aumentar em 12 graus - p cada ponto de combustivel que aumentou
            
		float quantSegundosGradual = 3f;//em 3 segundos e pra encher

		float quantAumentarEachTime = (float)anguloAumentar/quantSegundosGradual;

		bool continuar = true;
		Vector3 angulo;

		while (continuar)
		{
			angulo = fuelMarker.transform.eulerAngles;
			quantSegundosGradual--;//reduz 1 segundo
			angulo.z += quantAumentarEachTime;

            //em euler, o limite maximo é 359 em Z
            //limite minimo (tanque vazio) é no 239
            if ((angulo.z + quantAumentarEachTime) >= 359)
            {
                angulo.z = 359;
            }

			fuelMarker.transform.eulerAngles = angulo;


			if (quantSegundosGradual == 0)//Se acabou o tempo em segundos desse aumento gradual
			{
				continuar = false;//deve sair do laço
				enchendoIndicadorCombustivel = false;//Nao esta mais enchendo combustivel

				//se quando acabar de encher, tiver uma "ordem para baixar o combustivel
				if (diminuirFuelAfterFill)
				{
					bool continuar2 = true;

					while (continuar2)
					{
						DiminuirPonteiroFuel();//problema e se demorar tanto tempo para encher que diminua o combustivel 2 vezes
						quantVezesDimFuelAfterFill--;//1 vez a menos que precisa

						yield return new WaitForSeconds(1f);//espera 1 segundo e dai diminui o combustivel denovo

						if (quantVezesDimFuelAfterFill == 0)
						{
							continuar2 = false;
						}


					}


					diminuirFuelAfterFill = false;
				}
			}
			else
			{
				yield return new WaitForSeconds(1f);
			}
		}
		
	}

	public void AumentarCombustivel(float quantoAumentou)
	{
		StartCoroutine(AumentarCombustivelGradual(quantoAumentou) );
	}
	
	//Inicia o contador de fim de jogo
	IEnumerator IniciarFimdeJogo(bool Vitoria, bool acabouFuel)
	{

        //if (!Vitoria)
        //{
        //    if (acabouFuel)
        //        Debug.Log("Acabou combustivel");
        //    else
        //        Debug.Log("Perdeu por tempo");
        //}
        
        
        bool continuar = true;

     	//tempo inicial menos tempo que tinha quando acabou jogo em segundos
		float t;
		try {
            t = refStatus.tempoDuracaoFaseAtual;
		}catch { t = 180f; }

		keepUpdatingTimer = false;//Para de atualizar o contador
		keepCounterLoop = false;//Para de atualizar o contador
		showTimer = false;//para de mostrar o timer

		float tempoJogoScore = ( t - tempoJogo);//tempo em segundos que vai ser usado no score


		while (continuar)
		{
			if (Vitoria)
			{
				if (!venceuGame)
				{
					if(!somTocando)
					{
						scriptSom.TocaSom("vitoria");
						somTocando = true;
					}
					venceuGame = true;
                    
                    refCarro.GetComponent<ControleCarro>().SetChegouDestino();
                    yield return new WaitForSeconds(2.5f);//espera o carro parar

                    //refStatusO.GetComponent<Status>().DisplayVitoria(tempoJogoScore);
                    //Debug.Log("tempo que venceu gerente: " + tempoJogoScore);
                    refStatus.DisplayVitoria(tempoJogoScore);

                    continuar = false;


					// Game Analytics:
					string driverName = "";
					switch(refStatus.IDPersonagem){
					case 0:
						driverName = "Silvio";
						break;
					case 1:
						driverName = "Clara";
						break;
					case 2:
						driverName = "Ary";
						break;
					}
					short fase = 0;
					switch(refStatus.IDFase){
					case 1:
						fase = 1;
						break;
					case 2:
						fase = 2;
						break;
					case 3:
						fase = 3;
						break;
					case 4:
						fase = 4;
						break;
					case 5:
						fase = 5;
						break;
					case 6:
						fase = 6;
						break;
					}
					GA.API.Design.NewEvent("Level"+fase+":Driver:"+driverName+":Success", tempoJogoScore, refCarro.gameObject.transform.position);
				}
			}
			else //jogador perdeu o jogo - 
			{
				if (endGame)
				{
                    GameObject btnPausa = GameObject.FindGameObjectWithTag("botaoPausaInGame");
                    btnPausa.renderer.enabled = false;
                    DisplayEventsPlane(false, planoexibidoporultimo);
                    
                    if(!somTocando)
					{
						scriptSom.TocaSom("perdeu");
						somTocando = true;
					}
					continuar = false;

					//Toca anim da loira atacando
					//espera ate começar o fade
					yield return new WaitForSeconds(1.5f);//tempo para acabar a animaçao, dai começa a deixar vermelho -- ou deixa vermelho enquanto anima

                    GameObject objRed = GameObject.FindGameObjectWithTag("PRed");
				    objRed.renderer.enabled = true;
                     
                	bool keepLoop = true;
					float durTotal = 1.5f;
					
					while (keepLoop)
					{
						if (durTotal > 0)
						{
						    if (objRed.renderer.material.color.a <= 0.99f)
							{
                               objRed.renderer.material.color = new Color(0.55f, 1f, 1f, objRed.renderer.material.color.a + 0.05f);//0.22
                        	}
							yield return new WaitForSeconds(0.15f);
							durTotal -= 0.1f;
						}
						else 
						{
                            DisplayEventsPlane(false, planoexibidoporultimo);
                            yield return new WaitForSeconds(0.3f);

                            
                            PauseResumeGame(false);

                            //Habilita os menus
                            GameObject btnMenu = GameObject.FindGameObjectWithTag("PRedBtnM");
                            GameObject btnReiniciar = GameObject.FindGameObjectWithTag("PRedBtnR");

                            btnMenu.renderer.enabled = true;
                            btnMenu.collider.enabled = true;

                            btnReiniciar.renderer.enabled = true;
                            btnReiniciar.collider.enabled = true;
                            
                            //yield return new WaitForSeconds(1.5f);
                            keepLoop = false;
                            //refStatus.PerdeuJogo();
                            
						}
						
					}

					// Game Analytics:
					string driverName = "";
					switch(refStatus.IDPersonagem){
					case 0:
						driverName = "Silvio";
						break;
					case 1:
						driverName = "Clara";
						break;
					case 2:
						driverName = "Ary";
						break;
					}
					short fase = 0;
					switch(refStatus.IDFase){
					case 1:
						fase = 1;
						break;
					case 2:
						fase = 2;
						break;
					case 3:
						fase = 3;
						break;
					case 4:
						fase = 4;
						break;
					case 5:
						fase = 5;
						break;
					case 6:
						fase = 6;
						break;
					}
					GA.API.Design.NewEvent("Level"+fase+":Driver:"+driverName+":Fail", tempoJogoScore, refCarro.gameObject.transform.position);
				}

			}


		}

	}

	IEnumerator AtualizadorContador()
	{
        int temporizadorHabDecremento = 0;

		while (keepCounterLoop)
		{
			if (keepUpdatingTimer)//Se deve manter o contador atualizando
			{
				if ( (tempoJogo - 1f) >= 0 )
				{
					tempoJogo -= 1f;
					contadorPrincipal += 1f;
				}
				else //Se acabou o tempo de jogo daquela fase, o jogador perdeu -- faz algo
				{
                    ChangeLoiraState(true);//troca para o ultimo estado - pula do estado 3 pro 4, ou seja, o ataque da loira

                    tempoJogo = 0f;
					endGame = true;
					keepUpdatingTimer = false;
					keepCounterLoop = false;

                    refCarro.GetComponent<ControleCarro>().SetChegouDestino();

                    //Debug.Log("Atualizador contador, tempo de jogo chegou a zero:");

					StartCoroutine( IniciarFimdeJogo( false,false ) );
				}

                if (!podeDecrementarTempo)
                {
                    if (temporizadorHabDecremento == 3)//a cada 3 segundos pode usar diminuir o tempo caso haja uma batida
                    {
                        podeDecrementarTempo = true;
                        temporizadorHabDecremento = 0;
                    }
                    else
                    {
                        temporizadorHabDecremento += 1;
                        
                    }
                }

				
				//tempoJogo -= Time.deltaTime;//Sera exibido na tela
				//contadorPrincipal += Time.deltaTime;

                if (estadoAtualLoira <= 2)//4 é o ultimo - ou seja, vai trocando de estado até o penultimo, o ultimo estado é definido manualmente quando o tempo chega a zero
                {
                    if (contadorPrincipal >= tempoEachPhase)//Significa que passou tempo suficiente para trocar de fase da loira
                    {
                        contadorPrincipal = 0;
                        ChangeLoiraState(false);
                    }
                }
				
			}
			
			yield return new WaitForSeconds(1f);
			
		}
		
		
	}

    public void DecrementarTempoBatida()
    {
        if (podeDecrementarTempo)
        {
            //reduz o tempo de jogo em 2sec
            tempoJogo -= 2f;
            contadorPrincipal += 2f;//vai fazer com que a loira se tranforme mais rapido a cada batida

            ChangeLoiraReaction(false);//mostra imagem bad

            podeDecrementarTempo = false;//so vira true denovo caso tenha passado certo tempo no Atualizar Contador
        }

    }

    public IEnumerator AnimPolicial( GameObject qualPolicial )
    {
        float tempoDurAnim = 3.0f;

        Texture2D[] texturasPolicial = new Texture2D[2];
        texturasPolicial[0] = qualPolicial.renderer.material.mainTexture as Texture2D;
        texturasPolicial[1] = Resources.Load("Elementos/policia2l") as Texture2D;

        qualPolicial.renderer.material.mainTexture = texturasPolicial[1];


        yield return new WaitForSeconds(tempoDurAnim);

        qualPolicial.renderer.material.mainTexture = texturasPolicial[0];//volta ao normal;
        
    }

    //Realiza a animaçao da loira
    IEnumerator ControllerforSwitchLoiraAnim()
    {
        float tempoEntreFrames = 0.15f;
        float tempoGoodBad = 1.2f;
        
        int quantasVezes0 = 0;//Quantas vezes passou pelo if de 0
        bool keepAnimrunning = true;
        bool mostrarFrame0 = false;//Se teve um evento good ou bad e estava no frame 0 
        bool diminuirFrame = false;
        bool primeiraVez = true;

        while (keepAnimrunning)
        {
            if (trocouState)
            {
                frameAtualAnimLoira = 0;//Vai para o frame 0 de animacao ja que trocou todas imagens
                quantasVezes0 = 0;
                diminuirFrame = false;
                primeiraVez = false;

                trocouState = false;
            }

            if (displayGood && estadoAtualLoira != 4)
            {
                imgLoiraAtual.renderer.material.mainTexture = ArrayTextureLoira[4];//Todos frames de good são na pos 4
                yield return new WaitForSeconds(tempoGoodBad);
            
                displayGood = false;

                if (frameAtualAnimLoira == 0)//Se estava no frame 0 quando mostrou um GOOD
                {
                    mostrarFrame0 = true;
                }
            }
            else if (displayBad && estadoAtualLoira != 4)//Enquanto esta mostrando o bad, n roda a animaçao convencional
            {
               
                imgLoiraAtual.renderer.material.mainTexture = ArrayTextureLoira[5];
                yield return new WaitForSeconds(tempoGoodBad);
             
                displayBad = false;

                if (frameAtualAnimLoira == 0)
                {
                    mostrarFrame0 = true;
                }
            }
            else//Se nao e nenhum dos dois roda a animaçao normal
            {

                if (estadoAtualLoira == 4)//Se e o ultimo estado -- loira atacando
                {
                    imgLoiraAtual.renderer.material.mainTexture = ArrayTextureLoira[0] as Texture2D;
                   
                    //Mostra a imagem dela atacando o player
                    //endGame = true;
                    //StartCoroutine(IniciarFimdeJogo(false, false));

                    keepAnimrunning = false;//para de tocar a animaçao
                    frameAtualAnimLoira = -1;//Skipa as proximas verificaçoes e para a coroutine
                }


                if (frameAtualAnimLoira == 0)//Se for o primeiro frame
                {
                    //Poe Imagem 0
                    if (quantasVezes0 == 0 || mostrarFrame0)//Primeira vez que entrou aqui ou seja teve good ou bad quando estava no frame 0 
                    {
                        imgLoiraAtual.renderer.material.mainTexture = ArrayTextureLoira[frameAtualAnimLoira] as Texture2D;
                        mostrarFrame0 = false;
                    }
                    if (quantasVezes0 < 4)
                    {
                        yield return new WaitForSeconds(0.5f);//Primeiro frame dura dois segundos, logo roda 4 vezes esse yield
                        quantasVezes0++;
                    }
                    else//Acabou a animacao
                    {
                        quantasVezes0 = 0;

                        if (diminuirFrame)//se quando executou o frame inicial 4 vezes estava marcado para diminuir
                        {
                            diminuirFrame = false;
                        }

                        frameAtualAnimLoira++;//se executou a função 4 vezes de 0.5 - foi 2 segundos -- troca de frame

                    }

                }
                else if (frameAtualAnimLoira == 1)//Os outros tres frames duram 0.5
                {
                    //Troca para proxima imagem
                    imgLoiraAtual.renderer.material.mainTexture = ArrayTextureLoira[frameAtualAnimLoira] as Texture2D;
                    yield return new WaitForSeconds(tempoEntreFrames);

                    if (diminuirFrame)
                        frameAtualAnimLoira--;
                    else
                        frameAtualAnimLoira++;

                }
                else if (frameAtualAnimLoira == 2)//Os outros tres frames duram 0.5
                {
                    //Troca para proxima imagem
                    imgLoiraAtual.renderer.material.mainTexture = ArrayTextureLoira[frameAtualAnimLoira] as Texture2D;
                    yield return new WaitForSeconds(tempoEntreFrames);


                    if (diminuirFrame)
                        frameAtualAnimLoira--;
                    else
                        frameAtualAnimLoira++;

                }
                else if (frameAtualAnimLoira == 3 && frameAtualAnimLoira != maxFrameEstado)//Os outros tres frames duram 0.5
                {
                    //Troca para proxima imagem
                    imgLoiraAtual.renderer.material.mainTexture = ArrayTextureLoira[frameAtualAnimLoira] as Texture2D;
                    yield return new WaitForSeconds(tempoEntreFrames);

                    if (diminuirFrame)
                        frameAtualAnimLoira--;
                    else
                        frameAtualAnimLoira++;
                }


                if (frameAtualAnimLoira == maxFrameEstado)//Volta para o primeiro frame
                {
                    //Ordem de frames -- 0 1 2 3 2 1 -- volta pro 0
                    //Primeira vez que chega no ultimo frame, volta pra um anterior
                    if (primeiraVez)//Primeira vez que entrou aqui por ciclo de animação
                    {
                        frameAtualAnimLoira -= 2;//Reduz 2 frames, ja que quando foi o ultimo teve um incremento adicional

                        diminuirFrame = true;
                        primeiraVez = false;
                    }
                    else
                    {
                        primeiraVez = true;
                    }

                }

            }



        }


    }

    //Carrega as proximas imagens da loira
    private void ChangeLoiraState(bool acabouFuel)
    {
        if (!acabouFuel)
        {
            if (estadoAtualLoira == 4)//ja esta no estado de ataque, nao deve trocar de textura
                return;

            estadoAtualLoira++;//Começa em -1
        }
        else
            estadoAtualLoira = 4;//Acabou o combustivel, logo a loira ataca o player


        if (estadoAtualLoira == 0)//Carrega imagens para o estado 0 - pose 1
        {
            ArrayTextureLoira[0] = Resources.Load("Loira/Pose1/seqPose1_1") as Texture2D;
            ArrayTextureLoira[1] = Resources.Load("Loira/Pose1/seqPose1_2") as Texture2D;
            ArrayTextureLoira[2] = Resources.Load("Loira/Pose1/seqPose1_3") as Texture2D;
            ArrayTextureLoira[3] = Resources.Load("Loira/Pose1/seqPose1_4") as Texture2D;
            ArrayTextureLoira[4] = Resources.Load("Loira/Pose1/seqPose1_GOOD") as Texture2D;
            ArrayTextureLoira[5] = Resources.Load("Loira/Pose1/seqPose1_BAD") as Texture2D;
        }
        else if (estadoAtualLoira == 1)//Carrega imagens para o estado 1 - pose 2
        {
            ArrayTextureLoira[0] = Resources.Load("Loira/Pose2/seqPose2_1") as Texture2D;
            ArrayTextureLoira[1] = Resources.Load("Loira/Pose2/seqPose2_2") as Texture2D;
            ArrayTextureLoira[2] = Resources.Load("Loira/Pose2/seqPose2_3") as Texture2D;
            ArrayTextureLoira[3] = Resources.Load("Loira/Pose2/seqPose2_4") as Texture2D;
            ArrayTextureLoira[4] = Resources.Load("Loira/Pose2/seqPose2_GOOD") as Texture2D;
            ArrayTextureLoira[5] = Resources.Load("Loira/Pose2/seqPose2_BAD") as Texture2D;
        }
        else if (estadoAtualLoira == 2)//Carrega imagens para o estado 2 - pose 3
        {
            ArrayTextureLoira[0] = Resources.Load("Loira/Pose3/seqPose3_1") as Texture2D;
            ArrayTextureLoira[1] = Resources.Load("Loira/Pose3/seqPose3_2") as Texture2D;
            ArrayTextureLoira[2] = Resources.Load("Loira/Pose3/seqPose3_3") as Texture2D;
            ArrayTextureLoira[3] = Resources.Load("Loira/Pose3/seqPose3_4") as Texture2D;
            ArrayTextureLoira[4] = Resources.Load("Loira/Pose3/seqPose3_GOOD") as Texture2D;
            ArrayTextureLoira[5] = Resources.Load("Loira/Pose3/seqPose3_BAD") as Texture2D;
        }
        else if (estadoAtualLoira == 3)//Carrega imagens para o estado 3 - pose 4
        {
            ArrayTextureLoira[0] = Resources.Load("Loira/Pose4/seqPose4_1") as Texture2D;
            ArrayTextureLoira[1] = Resources.Load("Loira/Pose4/seqPose4_2") as Texture2D;
            ArrayTextureLoira[2] = Resources.Load("Loira/Pose4/seqPose4_3") as Texture2D;
            ArrayTextureLoira[3] = Resources.Load("Loira/Pose4/seqPose4_4") as Texture2D;
            ArrayTextureLoira[4] = Resources.Load("Loira/Pose4/seqPose4_GOOD") as Texture2D;
            ArrayTextureLoira[5] = Resources.Load("Loira/Pose4/seqPose4_BAD") as Texture2D;

        }
        else if (estadoAtualLoira == 4)//Carrega imagens para o estado 4 - pose 5
        {
            ArrayTextureLoira[0] = Resources.Load("Loira/Pose5/seqPose5_1") as Texture2D;
            ArrayTextureLoira[1] = null;
            ArrayTextureLoira[2] = null;
            ArrayTextureLoira[3] = null;
            ArrayTextureLoira[4] = null;
            ArrayTextureLoira[5] = null;
        }

        trocouState = true;//Avisa para a função de animacao que houve troca de estado

        if (estadoAtualLoira != 4)//Pose 1,2 e 3 tem 4 imagens cd e 2 p good e bad
        {
            maxFrameEstado = 4;//4 imagens de frame

			if(estadoAtualLoira != 0)
				scriptSom.TocaSom("zumbi"); //Toca o som de zumbi de troca de estado
        }
        else if (estadoAtualLoira == 4)//Pose 5- so possui 1 imagem
        {
			if(!somTocando)
			{
				scriptSom.TocaSom("ataque"); //Toca o som de zumbi atacando
				
                switch(scriptCarro.GetTipoCarro())
				{
				case 0: // Som do guri gritando
					scriptSom.TocaSom("guri");
					break;
				case 1: // Som da mulher gritando
					scriptSom.TocaSom("mulher");
					break;
				case 2: // Som do idoso gritando
					scriptSom.TocaSom("homem");
					break;
				}

				somTocando = true;
			}
            maxFrameEstado = 1;//3 imagens de frame - constantes
        }

        if (estadoAtualLoira == 0)
            StartCoroutine(ControllerforSwitchLoiraAnim());
    }

    //troca para a imagem boa ou ruim baseado na estadoAtual atual da loira
    public void ChangeLoiraReaction(bool EventoBom)//Incomplete
    {

        //StartCoroutine( TemporizadorLoiraReaction(EventoBom) );

        if (EventoBom)
        {
            displayGood = true;

			int temp = Random.Range (0,2); //Random pro som
			if(temp == 0)
				audio.PlayOneShot(somGood1, 2);
			else if(temp == 1)
				audio.PlayOneShot(somGood2, 2);
        }
        else
        {
            displayBad = true;

			int temp = Random.Range (0,3); //Random pro som
			if(temp == 0)
				audio.PlayOneShot(somBad1, 2);
			else if(temp == 1)
				audio.PlayOneShot(somBad2, 2);
			else if(temp == 2)
				audio.PlayOneShot(somBad3, 2);

            //FAZER PERDER TEMPO DE JOGO QUANDO ACONTECE UM EVENTO RUIM
        }


    }

	//Se deve ou nao parar o temporizador da loira
	public void ToggleUpdateTimerLoira(bool newState)
	{
		if (newState)
		{
			keepUpdatingTimer = true;
		}
		else
		{
			keepUpdatingTimer = false;
		}

	}

	public void AcabouCombustivel()
	{
		//Acabou combustivel e o carro parou
		ChangeLoiraState(true);//Passando true ele troca direto para o ultimo estado da imagem
		endGame = true;//Diz que o jogo acabou e faz algo
		StartCoroutine( IniciarFimdeJogo(false,true) );

        //desliga o atualizador de tempo
        keepUpdatingTimer = false;
        keepCounterLoop = false;

        refCarro.GetComponent<ControleCarro>().SetChegouDestino();//faz com que o carro freie

	}

	public void AlcancouCemiterio()
	{
		//Ganhou essa fase - Alcanou o cemiterio
		//Substituir a tela inteira por uma imagem?? mudar de cena??? talvez uma pra vitoria e uma com derrota - e tocar animaçoes la dentro
		StartCoroutine( IniciarFimdeJogo(true,false) );
	}

    public void PauseResumeGame(bool resume)
    {
        if (resume)
        {
            Time.timeScale = 1.0f;
            pausedGame = false;
        }
        else
        {
            Time.timeScale = 0.0f;
            pausedGame = true;
        }

        
    }

	void Update () 
	{
		//Faz a bussola olhar para o cemiterio
		InterfaceBussolaT.LookAt(PontoCemiterioT.position);
		InterfaceBussolaT.eulerAngles = new Vector3(0, InterfaceBussolaT.eulerAngles.y, 0);


        //DEBUG
        //if (Input.GetKeyDown(KeyCode.K))
        //{
        //    ChangeLoiraReaction(true);
        //}

        //if (Input.GetKeyDown(KeyCode.J))
        //{
        //    ChangeLoiraReaction(false);
        //}

        //if (Input.GetKeyDown(KeyCode.N))
        //{
        //    ChangeLoiraState(false);
        //}

        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    refCarro.GetComponent<ControleCarro>().SetChegouDestino();
        //    AcabouCombustivel();
        //}

        if (Input.GetKeyDown(KeyCode.O))
        {
            refCarro.GetComponent<ControleCarro>().SetChegouDestino();
            AlcancouCemiterio();
        }

//        if (btnPausePressed)
//        {
//            if (!pausedGame)
//                PauseResumeGame(false);
//            else
//                PauseResumeGame(true);
//        }
#if DEBUG_MD
		if(x > xOld || y > yOld || x < xOld || y < yOld)
			TesteAtualiza();
#endif
	}

	void OnGUI()
	{
		if (showTimer)
		{
            GUI.skin = SkinGUIDigital;

			GUI.contentColor = Color.white;
            
            GUI.skin.label.fontSize = 16;

			fatiaTempoJogo = System.TimeSpan.FromSeconds( tempoJogo );

			string niceTime = string.Format("{0:D2}:{1:D2}", 
			    			fatiaTempoJogo.Minutes, 
			    			fatiaTempoJogo.Seconds);
			
			GUI.Label(retanguloTimer, niceTime);
			

		}
	}

    private void LoadEventsPlane()
    {
        //Carrega as imagens do plano de eventos -- 0 é o velho, 1 - gato, 2 - polcial
        arrayEventsPlane = new Texture2D[3];
        arrayEventsPlane[0] = Resources.Load("Elementos/velhodisplay") as Texture2D;
        arrayEventsPlane[1] = Resources.Load("Elementos/gatodisplay") as Texture2D;
        arrayEventsPlane[2] = Resources.Load("Elementos/policialdisplay") as Texture2D;
    }

    /// <summary>
    /// Troca a imagem que é mostrada no plano- 0 é o velho, 1 - gato, 2 - polcial
    /// </summary>
    public void ChangeImgEventsPlane(int qual)
    {
        if (qual >= 0 && qual <= 2
            && !endGame)
        {
            displayEventsPlane.renderer.material.mainTexture = arrayEventsPlane[qual];
            planoexibidoporultimo = qual;
            DisplayEventsPlane(true);
        }
        else
            return;
  
    }

    /// <summary>
    /// Se deve tornar invisivel ou não o plano para eventos, 0 é o velho, 1 - gato, 2 - polcial 
    /// </summary>
    public void DisplayEventsPlane( bool showEventsPlane, int quempediu = 0)
    {

        if (showEventsPlane)
        {
            if (!endGame)//se o jogo nao esta acabando
               displayEventsPlane.renderer.enabled = true;
        }
        else//se for para desligar o plano - verifica quem o solicitou
        {
            //Debug.Log("Quem desligou plane: " + quempediu);
            if (planoexibidoporultimo == quempediu)//se quem pediu foi o mesmo power up do ultimo plano em exibição, dai tu desativa (assim impede que um gato desligue a img de um policial ou velho)
                displayEventsPlane.renderer.enabled = false;
        }
    }

    public void InverterTriggerGato(GameObject gatoInverter)
    {
        BoxCollider colisortemp = gatoInverter.collider as BoxCollider;
        colisortemp.center = new Vector3(colisortemp.center.x, colisortemp.center.y, (colisortemp.center.z)/2f );
    

        //pega o pai desse objeto e atualiza o centro dele tb

        colisortemp = gatoInverter.transform.parent.collider as BoxCollider;
        colisortemp.center = new Vector3(colisortemp.center.x, (colisortemp.center.y) * -1, colisortemp.center.z);

    }

    public void InverterTriggerVelho(GameObject velhoInverter)
    {
        BoxCollider colisortemp = velhoInverter.collider as BoxCollider;
        colisortemp.center = new Vector3(colisortemp.center.x, colisortemp.center.y, (colisortemp.center.z) / 2f);


        //pega o pai desse objeto e atualiza o centro dele tb

        colisortemp = velhoInverter.transform.parent.collider as BoxCollider;
        colisortemp.center = new Vector3((colisortemp.center.x) * -1, colisortemp.center.y, colisortemp.center.z);

    }
	
}
