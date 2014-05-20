using UnityEngine;
using System.Collections;

public class ControleCarro : MonoBehaviour 
{
	
	ScriptObjGerenciador refScriptObjGerente;//Script do objeto gerente
    GameObject camT;//Camera

    // Testes:
    bool b1 = true;

	//Taxi
	public Vector3 orientacao;
	//float speed;
	//float sensibilidade;//Controle
	//private float accel;
	//private float desacel;
	//private int maxSpeed;
	bool controleDoJogador = true, trocandoRua = false, finalizouRotacaoCam = true; // Troca de ruas;
	private short fatorMC = 2/*, fatorMCar = 2*/; // Fator de incrementaçao na rotaçao de camera e carro respectivamente;
	bool faltaAjustar; // Auxiliar para ajustar câmera;
	//Vector3 orintacaoAlinhar; // Quando o jogo tiver o controle do carro este será o guia de direção para o carro e câmera;
	private float fatorLimite = 90f; // Por padrao era 45º;
	private float anguloLimiteD;
	private float anguloLimiteE;
    private short orientacaoAtual; // Orientação oposta da câmera, para o carro andar para cima sempre, sendo assim o valor e a rotaçao da camera + 180º;

    // Botões:
	bool btnDir = false, btnEsq = false; // Marca se botão foi pressionado, fica true enquanto o botão tiver que ser mantido na tela;
	Vector2 posBtnE, posBtnD; // Marca a posiçao dos botoes que aparecem na tela quando jogador dirige; //float tempoBtnDir = 0f, tempoBtnEsq = 0f, tempoMaxBtn = 1f, alpha = 1; // Respectivamente: tempo do botão direito pressionado, tempo do botão esquerdo pressionado e por fim tempo máximo que um botão deve aparecer na tela;
	public Texture2D sprite/*spriteDir, spriteEsq*/;

	//Dispositivo
	private bool android = false;
	private int halfscreenWidth;

	// Game Analitics:
	Status statusTmp;
	float tempoSistema = 0f;
	
	//-------------------------------------
    
    
    private Velho reftolastOldMan;//Referencia ao script do ultimo velho encontrado pelo carro, é atualizado cada vez que encontra outro velho
    private Gato reftolastCat;
    private bool barredbyOldMan; //Se o carro foi interrompido pelo velho - ou seja, entrou na area do velho
    private bool barredbyCat;

	private bool chegouDestino;
	private bool imuneEfeitos; //Indica se o jogador esta imune a efeitos de desaceleraçao
	private bool escorregandoD, escorregandoE; //Indica se o jogador passou por alguma poça e esta impossibilitado de mudar a direçao
	private bool agua = false; // Marca se passou ou nao em uma poça de agua;

	private StatsCarro AtributosCarro;//Struct para os atributos do carro
	
	//Lista somente para power Ups que possuem uma duracao
	private System.Collections.Generic.List<PowerUps> listaPowerUpsAtivos = new System.Collections.Generic.List<PowerUps>();

	//Sons
	public AudioSource somMotorPuma;
	public AudioSource somMotorFusca;
	public AudioSource somMotorOpala;

	private EfeitosSonoros scriptSom;
	private bool somTocando;

	private GameObject particulas;

	private struct StatsCarro
	{
		public int tipoCarro;
		public float sensibilidade;
		public float sensibilidadePadrao;
		public float speed;
		public float accel;
		public float desacel; // Quantidade de desaceleraçao ao frear (Desaceleraçao progressiva)
		public float desacelPadrao; 
		public float batida; // velocidade que chega quando bater (Desaceleraçao instantanea)
		public float batidaPadrao;
		public float maxSpeed;
		public float maxSpeedPadrao;
		public int fuel;// Quantidade de fuel atual
		public float tempoFuel;// Tempo (em segundos) que vai diminuir 1 ponto de fuel
		public int fuelLimite;
		public float tempoRelativoFuel;

        public int intensidadeSensibilidade;//Intensidade dos powers ups de invencibilidade (são stackaveis)
        public bool boostVelocidade;//Indica se o carro esta com um power up de velocidade

		//Retorna a posicao inicial do carro e inicializa o carro
		public int SetStatsCarro(int tipoCarrot)
		{
			int faseRetornar = 1;
			Status scriptStatus;
			try
			{
				GameObject statustemp = GameObject.Find("Status");
				scriptStatus = statustemp.GetComponent<Status>();
				faseRetornar = scriptStatus.IDFase;
				tipoCarro = scriptStatus.IDPersonagem;//Tipo do carro que vem da selaçao de tela
			}
			catch
			{
				tipoCarro = tipoCarrot;
			}
			
			if (tipoCarro == 0)//Botar parametros do carro do guri
			{
				maxSpeedPadrao = 5.2f;//Velocidade limite
				accel = 0.5f;
				desacelPadrao = 0.6f;//reduçao?
				batidaPadrao = 1f;
				
				sensibilidadePadrao = 60;//Controle do carro
				
				//Combustivel
				fuelLimite = 10;
				
			}
			else if (tipoCarro == 1)//Botar parametros do carro da mulher - fusca
			{
				maxSpeedPadrao = 4.8f;//Velocidade limite
				accel = 0.3f;
				desacelPadrao = 0.90f;//Nivel 2
				batidaPadrao = 1f;
				
				sensibilidadePadrao = 90;//Controle do carro
				
				//Combustivel
				
				fuelLimite = 10;
			}
			else if (tipoCarro == 2)//Botar parametros do carro do velho
			{
				maxSpeedPadrao = 4.4f;//Velocidade limite
				accel = 0.75f;
				desacelPadrao = 1.1f;
				batidaPadrao = 1f;
				
				sensibilidadePadrao = 75;//Controle do carro
				
				//Combustivel
				fuelLimite = 10;
			}
			
			//Inicializa dependendo do tipo de carro
			speed = 0;//Velocidade inicial
			maxSpeed = maxSpeedPadrao;
			sensibilidade = sensibilidadePadrao;
			desacel = desacelPadrao;
			batida = batidaPadrao;
			fuel = fuelLimite;// Fuel inicial
			tempoRelativoFuel = 0;//Temporizador para controle de combustivel
			tempoFuel = 10f;//tempo de consumo de fuel -- definido posteriormente baseado no tempo de jogo
            intensidadeSensibilidade = 0;
            boostVelocidade = false;

         	return faseRetornar;
			
		}
		
	};
	
	
	
	private struct PowerUps
	{
		public int tipoPowerUp;
		public float duracaoPowerUp;// Qual a duraçao dos PowerUps
		public float temporizador;// Contador de tempo para começar a contar apenas quando for necessario
		public bool aindaAtivo;
		
		//Construtores
		public PowerUps(int TipoPu)
		{
			tipoPowerUp = TipoPu;
			duracaoPowerUp = 0;
			temporizador = 0;
			aindaAtivo = true;
			
			if (tipoPowerUp == 1)//Velocidade
			{
				duracaoPowerUp = 8f;
			}
			else if (tipoPowerUp == 2)//Invencibilidade -- imune a slow effects e temporizador da loira para
			{
				duracaoPowerUp = 10f;
			}
			else if (tipoPowerUp == 3)//Down Sensibilidade -- mais lento para fazer a curva
			{
				duracaoPowerUp = 7f;
			}
		
			
		}
		//DuracaoCustom
		public PowerUps(int TipoPu, float DuracaoPuCustom)
		{
			tipoPowerUp = TipoPu;
			duracaoPowerUp = DuracaoPuCustom;
			temporizador = 0;
			aindaAtivo = true;
			
			
		}
		
		public void SetTemporizador(float newTemporizador)
		{
			temporizador = newTemporizador;
		}
		
		public void SetAindaAtivo(bool newAindaAtivo)
		{
			aindaAtivo = newAindaAtivo;
		}
		
		
	}
	
	public void AtualizarLimites(Transform transformDaCameraDaCena)
	{ // Atualiza os limítes de rotação em relação a câmera; OBS: a própria câmera chama esta função quando sua orientação for alterada (Ex: 0º (Cima), 90º (Direita), 180º (Baixo) e 270º (Esquerda));

		//Limitadores de angulo de rotação
		// Limitador direita:
		anguloLimiteD = transformDaCameraDaCena.eulerAngles.y + fatorLimite; // São os valores normalizados;
		if(transformDaCameraDaCena.eulerAngles.y - fatorLimite < 0) // Se negativo;
			anguloLimiteE = 360 + (transformDaCameraDaCena.eulerAngles.y - fatorLimite); // São os valores normalizados;
		else
			anguloLimiteE = transformDaCameraDaCena.eulerAngles.y - fatorLimite;
		
		// Pegar o ângulo de orientação da câmera e atualizar a variável orientacaoAtual;
		orientacaoAtual = (short)(transformDaCameraDaCena.eulerAngles.y + 180); // Sempre 180º da direçao da camera, pois ela esta apontando para baixo;
		if(orientacaoAtual >= 360)
			orientacaoAtual -= 360;
	
	}

	void Start() 
	{
		Random.seed = 50;

		//Inicializaçao
		refScriptObjGerente = GameObject.Find("objGerenciador").GetComponent<ScriptObjGerenciador>();
		scriptSom = GameObject.Find("FonteEfeitos").GetComponent<EfeitosSonoros>();

		//Inicializador do carro
		int qualPosInicial = AtributosCarro.SetStatsCarro(0);
		InitCarroPlayer(qualPosInicial);
		
		refScriptObjGerente.fuelMaximo = AtributosCarro.fuelLimite;//preenche o combustivel maximo no gerente
		
		// Atualiza os limites da câmera:
		camT = GameObject.Find ("Main Camera");
		AtualizarLimites(camT.transform);

        //init de variavei
        barredbyOldMan = false;		
		chegouDestino = false;
		imuneEfeitos = false;
		escorregandoD = escorregandoE = false;

        //som 
        somTocando = false;

        //Particulas do power up de invencibilidade
        particulas = GameObject.Find("ParticulasInvencivel"); // Objeto das particulas para a invencibilidade
        particulas.particleEmitter.emit = false; // Desabilita o emissor de particulas


		if (Application.platform == RuntimePlatform.Android)
		{
			android = true;
		}
		
		halfscreenWidth = (int)Screen.width/2;

		// Ajuste dos botoes na tela:
		Vector3 vec3RetT = camT.camera.ViewportToScreenPoint(new Vector3(0.18f, 0.7f, 1f)); // Para alterar a posiçao do botao;
		posBtnE = new Vector2(vec3RetT.x, vec3RetT.y);
		vec3RetT = camT.camera.ViewportToScreenPoint(new Vector3(0.82f, 0.7f, 1f));
		posBtnD = new Vector2(vec3RetT.x, vec3RetT.y);

		// Game Analitics:
		GameObject temporario = GameObject.Find("Status");
		this.statusTmp = temporario.GetComponent<Status>();
		string driverName = "";
		switch(statusTmp.IDPersonagem){
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
		switch(statusTmp.IDFase){
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
		GA.API.Design.NewEvent("Selection:Level"+fase+":Driver:"+driverName); // Guarda a seleçao da fase com o nome do personagem selecionado;
	}

    //Inicializa coisas que nao podem ser feitas dentro da struct
    private void InitCarroPlayer(int qualPosInicialtemp)
    {
        GameObject tempC = GameObject.Find("Main Camera");

        //POSICAO INICIAL CARRO, baseado na fase escolhida
        if (qualPosInicialtemp == 1)//Posicao da fase 1
        {
            //original
            transform.position = GameObject.Find("Pt1").transform.position;
            //new Vector3(24.5f,0.6f,15.3f);
            transform.eulerAngles = new Vector3(0, 270, 0);

          
            tempC.transform.position = new Vector3(transform.position.x, tempC.transform.position.y, transform.position.z);
            tempC.transform.eulerAngles = new Vector3(tempC.transform.eulerAngles.x, 270, 0);


        }
        else if (qualPosInicialtemp == 2)//Posicao da fase 2
        {
            transform.position = GameObject.Find("Pt2").transform.position;
            //new Vector3(181f,0.6f,78f);
            transform.eulerAngles = new Vector3(0, 90, 0);

            tempC.transform.position = new Vector3(transform.position.x, tempC.transform.position.y, transform.position.z);
            tempC.transform.eulerAngles = new Vector3(tempC.transform.eulerAngles.x, 90, 0);//p baixo
        }
        else if (qualPosInicialtemp == 3)//Posicao da fase 3
        {
            transform.position = GameObject.Find("Pt3").transform.position;
            //new Vector3(44f,0.6f,62.20f);
            transform.eulerAngles = new Vector3(0, 90, 0);

            tempC.transform.position = new Vector3(transform.position.x, tempC.transform.position.y, transform.position.z);
            tempC.transform.eulerAngles = new Vector3(tempC.transform.eulerAngles.x, 90, 0);//p baixo
        }
        else if (qualPosInicialtemp == 4)//Posicao da fase 4
        {
            transform.position = GameObject.Find("Pt4").transform.position;
            //new Vector3(234f,0.6f,129.45f);
            transform.eulerAngles = new Vector3(0, 180, 0);//P baixo
            //camera tb
            tempC.transform.position = new Vector3(transform.position.x, tempC.transform.position.y, transform.position.z);
            tempC.transform.eulerAngles = new Vector3(tempC.transform.eulerAngles.x, 180, 0);//p baixo
        }
        else if (qualPosInicialtemp == 5)//Posicao da fase 5
        {

            transform.position = GameObject.Find("Pt5").transform.position;
            //new Vector3(291f,0.6f,246f);
            transform.eulerAngles = new Vector3(0, 270, 0);//P esquerda

            tempC.transform.position = new Vector3(transform.position.x, tempC.transform.position.y, transform.position.z);
            tempC.transform.eulerAngles = new Vector3(tempC.transform.eulerAngles.x, 270, 0);
        }
        else if (qualPosInicialtemp == 6)//Posicao da fase 6
        {
            transform.position = GameObject.Find("Pt6").transform.position;
            //new Vector3(166f,0.6f,315f);
            transform.eulerAngles = new Vector3(0, 90, 0);//direita

            tempC.transform.position = new Vector3(transform.position.x, tempC.transform.position.y, transform.position.z);
            tempC.transform.eulerAngles = new Vector3(tempC.transform.eulerAngles.x, 90, 0);
        }
		
			
		//Define o consumo maximo do carro baseado no tempo daquela fase
		float durCdFase;
		Status scriptStatust;
		try
		{
			GameObject statsobj = GameObject.Find("Status");
			scriptStatust = statsobj.GetComponent<Status>();
			//durCdFase = scriptStatust.tempoDuracaoCadaFase;
            durCdFase = scriptStatust.GetTempoJogopelaFase(scriptStatust.IDFase);//pega o tempo da fase atual
            //Debug.Log("pegou tempo certo, no carro");
		}
		catch
		{
			durCdFase = 180f; 
		}
		
		//Assim obriga os jogadores a pegarem no minimo um power up de combustivel para nao perderem o jogo esgotando o tempo total
		if (AtributosCarro.tipoCarro == 0)//Guri
		{
			AtributosCarro.tempoFuel = (durCdFase - 55)/AtributosCarro.fuelLimite;

            //sons de motor
            somMotorPuma.enabled = true;
            somMotorFusca.enabled = false;
            somMotorOpala.enabled = false;
            
		}
		else if (AtributosCarro.tipoCarro == 1)//Mulher - //Se nao e o carro 0, tenque atualizar o material
		{
			AtributosCarro.tempoFuel = (durCdFase - 40)/AtributosCarro.fuelLimite;

			Renderer tempM = gameObject.GetComponentInChildren<Renderer>();
			tempM.material = Resources.Load("Materials/CarrosPlayer/carro1M") as Material;

            //sons de motor
            somMotorPuma.enabled = false;
            somMotorFusca.enabled = true;
            somMotorOpala.enabled = false;
       
        }
		else if (AtributosCarro.tipoCarro == 2)//Velho
		{
			AtributosCarro.tempoFuel = (durCdFase - 70)/AtributosCarro.fuelLimite;
			Renderer tempM = gameObject.GetComponentInChildren<Renderer>();
			tempM.material = Resources.Load("Materials/CarrosPlayer/carro2M") as Material;//tempM.material = Resources.Load("Materials/Carros/carro2M") as Material;

            //sons de motor
            somMotorPuma.enabled = false;
            somMotorFusca.enabled = false;
            somMotorOpala.enabled = true;
    	}
		
		
	}

	

//	private void AjustarOrientacao(){
//		// Código para rotacionar a câmera:
//		GameObject camA = GameObject.Find("Main Camera");
//		var vec = camA.transform.eulerAngles;
//		vec.y = this.orientacaoAtual;
//		camA.transform.eulerAngles = vec;
////		var vec = camA.transform.eulerAngles;
////		Vector3 vt = new Vector3(vec.x, this.orientacaoAtual, vec.z);
////		camA.transform.eulerAngles = vt; // Câmera recebe a rotação. OBS: ela fica com a mesma orientação do carro, isto seria um erro, a orientaçãoAtual será atualizada automaticamente quando a câmera perceber a alteração em sua rotação;
//		// IMPORTANTE: o código acima só deve ser executado uma única vez que é quando o jogador entra em uma nova rua;
//	}

	// Ajusta o ângulo para que fique dentro dos limítes padrões:
	private static float AjustarAngulo(float ang){
		if(ang > 45 && ang < 135)
			ang = 90.0f;
		else if(ang > 135 && ang < 225)
			ang = 180.0f;
		else if(ang > 225 && ang < 315)
			ang = 270.0f;
		else
			ang = 0.0f;
		return ang;
	}
	//	private static void AjustarAngulo(ref float ang){
	//		if(ang > 45 && ang < 135)
	//			ang = 90;
	//		else if(ang > 135 && ang < 225)
	//			ang = 180;
	//		else if(ang > 225 && ang < 315)
	//			ang = 270;
	//		else
	//			ang = 0;
	//	}
	private static int AjustarAngulo(int ang){
		if(ang > 45 && ang < 135)
			ang = 90;
		else if(ang > 135 && ang < 225)
			ang = 180;
		else if(ang > 225 && ang < 315)
			ang = 270;
		else
			ang = 0;
		return ang;
	}
	private static short AjustarAngulo(short ang){
		if(ang > 45 && ang < 135)
			ang = 90;
		else if(ang > 135 && ang < 225)
			ang = 180;
		else if(ang > 225 && ang < 315)
			ang = 270;
		else
			ang = 0;
		return ang;
	}
	
	void DC(){
		GameObject camA = GameObject.Find("Main Camera");
		var scriptCamA = camA.GetComponent<CamJogoScript>();
		scriptCamA.autoRot = !scriptCamA.autoRot; // Desativa a rotaçao automatica do carro;
		//		if(camA.transform.parent != this.transform){
		//			camA.transform.parent = this.transform;
		//		} else{
		//			camA.transform.parent = null; // Volta para a raiz;
		//
		Vector3 v = new Vector3();// = new Vector3(8,8,8);
		//			//Debug.Log (v.y);
		v.y = AjustarAngulo(camA.transform.eulerAngles.y); // Retorna o ângulo final; AjustarAngulo(ref v.y);
		//			//Debug.Log (v.y);
		//			//AjustarAngulo(ref v.y);
		//			//Debug.Log (v.y);
		//			//Debug.Break ();
		v.x = camA.transform.eulerAngles.x;
		v.z = camA.transform.eulerAngles.z;
		camA.transform.eulerAngles = v;
		//		}
	}

	private void AutoMove(){ // Código para mover o carro para a nova rua;
		// Ajustar rotação da câmera:
		if(faltaAjustar){
			//AjustarOrientacao();
			DC();
			faltaAjustar = false;
            this.finalizouRotacaoCam = false; // Sinaliza que falta ajustar a rotaçao da câmera;
		}
		
		
		// Andar pela rua:
		orientacao = transform.eulerAngles;
		
		// Gambiarra:
		orientacaoAtual = AjustarAngulo(orientacaoAtual);
		
		int oTemp = (int)orientacaoAtual + 180;
		if(oTemp >= 360){
			oTemp -= 360;
		}

//		// Ajustar fator de rotaçao do carro:
//		if((orientacao.y <= oTemp+10) && (orientacao.y >= oTemp-10))
//			this.fatorMCar = 1;
//		else
//			this.fatorMCar = 2;

        if ((orientacao.y <= oTemp + 5) && (orientacao.y >= oTemp - 5) && this.finalizouRotacaoCam){ // Se a orientação estiver quase correta, igualar ela com a orientação Atual;
			orientacao.y = oTemp;

			//if(!faltaAjustar) // Para garantir que nao libere o controle do jogador quando pegar duas trocas de rua;
			//{
			if(!escorregandoD || !escorregandoE || !agua){ // Garante que o controle so volta para o jogador se um destes nao estiver sendo usado;
				this.controleDoJogador = true; // Retorna o controle ao jogador;
			}


			DC ();
            this.finalizouRotacaoCam = false;

			this.trocandoRua = false; // Flag para avisar a oleo e agua que carro esta trocando de rua;

			return; // Sair da funçao;
			//Debug.Log ("controleDoJogador: "+controleDoJogador);
			//Debug.Break();
			//}
		}
		else
		{ 
			float orietemp = orientacao.y;
			if(orientacaoAtual == 0){ // Baixo;
				if(orietemp >= orientacaoAtual && orietemp <= 180){
					orietemp = orientacao.y + ((1 * Time.deltaTime) * AtributosCarro.sensibilidade);
					
				} else{ // Estando nos quadrantes opostos, entre 180 e 360;
					orietemp = orientacao.y + ((-1 * Time.deltaTime) * AtributosCarro.sensibilidade);
				}
			} else if(orientacaoAtual == 90){ // Esquerda;
				if(orietemp >= orientacaoAtual && orietemp <= 270){
					orietemp = orientacao.y + ((1 * Time.deltaTime) * AtributosCarro.sensibilidade);
				} else{
					orietemp = orientacao.y + ((-1 * Time.deltaTime) * AtributosCarro.sensibilidade);
				}
			} else if(orientacaoAtual == 180){ // Cima;
				if(orietemp >= orientacaoAtual && orietemp <= 360){
					orietemp = orientacao.y + ((1 * Time.deltaTime) * AtributosCarro.sensibilidade);
				} else{
					orietemp = orientacao.y + ((-1 * Time.deltaTime) * AtributosCarro.sensibilidade);
				}
			} else if(orientacaoAtual == 270){ // Direita;
				if(orietemp <= orientacaoAtual && orietemp >= 90){
					orietemp = orientacao.y + ((-1 * Time.deltaTime) * AtributosCarro.sensibilidade);
				} else{
					orietemp = orientacao.y + ((1 * Time.deltaTime) * AtributosCarro.sensibilidade);
				}
			}
			
			
			orientacao.y = orietemp;
		}
		
		transform.eulerAngles = orientacao;//Aplica a rotacao
		//-----------------------------------------------------------
		
		
		rigidbody.velocity = transform.forward * AtributosCarro.speed; //Aplica a velocidade
		
		if(rigidbody.velocity.magnitude > 0.1f)
		{
			transform.forward = new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z);
		}
		
		//-----------------------------------------------------------------------------------------------------------------------------------
		
		// Rotacionar câmera:
		GameObject camA = GameObject.Find("Main Camera");
		var vec = camA.transform.eulerAngles;
		//Vector3 vt = new Vector3(vec.x, this.orientacaoAtual, vec.z);
		//camA.transform.eulerAngles = vt;
        int oriT = (int)orientacaoAtual, oriT2 = (int)orientacaoAtual + 180;
        if (oriT >= 360)
            oriT -= 360;
        if (oriT2 >= 360)
            oriT2 -= 360;

        //Debug.Log ("");
        if ((vec.y <= oriT2 + 5) && (vec.y >= oriT2 - 5)){ // Se a orientação estiver quase correta, igualar ela com a orientação Atual;
            vec.y = oriT2;
            this.finalizouRotacaoCam = true;
        }
		else
		{
			// Condiçao apra evitar que o carro fique chaqualhando:
			if((vec.y <= oriT+10) && (vec.y >= oriT-10))
				this.fatorMC = 1;
			else
				this.fatorMC = 2;

			float orietemp = vec.y;
			
			if(oriT == 0){ // Baixo;
				if(orietemp >= oriT && orietemp <= 180){
					orietemp = vec.y + ((this.fatorMC * Time.deltaTime) * AtributosCarro.sensibilidade);
					
				} else{ // Estando nos quadrantes opostos, entre 180 e 360;
					orietemp = vec.y + ((-this.fatorMC * Time.deltaTime) * AtributosCarro.sensibilidade);
					
				}
			} else if(oriT == 90){ // Esquerda;
				if(orietemp >= oriT && orietemp <= 270){
					orietemp = vec.y + ((this.fatorMC * Time.deltaTime) * AtributosCarro.sensibilidade);
				} else{
					orietemp = vec.y + ((-this.fatorMC * Time.deltaTime) * AtributosCarro.sensibilidade);
				}
			} else if(oriT == 180){ // Cima;
				if(orietemp >= oriT && orietemp <= 360){
					orietemp = vec.y + ((this.fatorMC * Time.deltaTime) * AtributosCarro.sensibilidade);
				} else{
					orietemp = vec.y + ((-this.fatorMC * Time.deltaTime) * AtributosCarro.sensibilidade);
				}
			} else if(oriT == 270){ // Direita;
				if(orietemp <= oriT && orietemp >= 90){
					orietemp = vec.y + ((-this.fatorMC * Time.deltaTime) * AtributosCarro.sensibilidade);
				} else{
					orietemp = vec.y + ((this.fatorMC * Time.deltaTime) * AtributosCarro.sensibilidade);
				}
			}
			
			
			vec.y = orietemp;
		}
		
		camA.transform.eulerAngles = vec;//Aplica a rotacao
		// Código abaixo serve somente para testes:
		//Debug.Log ("Andando Automaticamente!");
		//Debug.Break ();
	}

	//Funcao para rotacionar o carro e aplicar o movimento
	private void RotAndMove()
	{
		Vector2 positionTouch;
		bool clickEsq = false;
		bool clickDir = false;
		
		if (android)
		{
			positionTouch = Input.GetTouch(0).position;
			
			if (positionTouch.x >= halfscreenWidth)//Se o touch for na direita da tela
				clickDir = true;
			else 
				clickEsq = true;
		}
		else
		{
			//positionTouch = Input.mousePosition;
			clickDir = Input.GetMouseButton(1);
			clickEsq = Input.GetMouseButton(0);
		}
		
		//----Orientacao-------------------------------------------------
		orientacao = transform.eulerAngles;

		//		// Testes com RAYCAST:
		//		var forward = this.transform.TransformDirection(new Vector3(-1,0,1)) * 5; // Agora sim!
		//		Debug.DrawRay(this.transform.position, forward);//Debug.DrawLine(this.transform.position, new Vector3(this.transform.position.x, this.transform.position.y, (this.transform.position.z + 10)));
		//		RaycastHit rayT;
		//		if(Physics.Raycast(this.transform.position, new Vector3(-1,0,1), out rayT, 5)){ // a palavra chave "out" é semelhante a "ref";
		//			Debug.Log ("Raycast: "+rayT.transform.name); // Retorna o nome do objeto em que colidiu;
		//			Debug.Break();
		//		}
		
		//		RaycastHit ray;
		//		if(Physics.Raycast(this.transform.position, /*Vector3.forward*/new Vector3(0,0,1), out ray, 10)){ // Raycast que impede giro pois já está na calçada; // Se colidindo com prédios;
		//			Debug.Log ("Direita!!! Raycast");
		//			Debug.Break();
		//		}
		
		//if (positionTouch.x >= halfscreenWidth)//Toque na direita da tela
		if (clickDir)
		{
			float orietemp = orientacao.y + ((1 * Time.deltaTime) * AtributosCarro.sensibilidade);
			
			// Limitador:
			if (orietemp > anguloLimiteD && orietemp < anguloLimiteD + 5)
			{
				orietemp = anguloLimiteD;
			}
			
			orientacao.y = orietemp;
			//orientacao.y += (1 * Time.deltaTime) * sensibilidade;
			this.btnDir = true;
            this.btnEsq = false;
		}
		else if (clickEsq)//Esquerda
		{
			float orietemp = orientacao.y + ((-1 * Time.deltaTime) * AtributosCarro.sensibilidade);
			
			// Limitador:
			if (orietemp < anguloLimiteE && orietemp > anguloLimiteE - 5)
			{
				orietemp = anguloLimiteE;
			}
			
			orientacao.y = orietemp;
			
			//orientacao.y += (-1 * Time.deltaTime) * sensibilidade;
			this.btnEsq = true;
            this.btnDir = false;
		}
		else // Alinhar com a rua;
		{
			//Debug.Log("orientacao.y="+orientacao.y+"; orientacaoAtual="+orientacaoAtual);
			short oTemp = (short)(orientacaoAtual + 180);
			if(oTemp >= 360){
				oTemp -= 360;
			}
			if((orientacao.y <= oTemp+5) && (orientacao.y >= oTemp-5)){ // Se a orientação estiver quase correta, igualar ela com a orientação Atual;
				orientacao.y = oTemp;
			}
			else
			{
				float orietemp = orientacao.y;

				if(orientacaoAtual == 0){ // Baixo;
					if(orietemp >= orientacaoAtual && orietemp <= 180){
						orietemp = orientacao.y + ((1 * Time.deltaTime) * AtributosCarro.sensibilidade);
					
					} else{ // Estando nos quadrantes opostos, entre 180 e 360;
						orietemp = orientacao.y + ((-1 * Time.deltaTime) * AtributosCarro.sensibilidade);
				
					}
				} else if(orientacaoAtual == 90){ // Esquerda;
					if(orietemp >= orientacaoAtual && orietemp <= 270){
						orietemp = orientacao.y + ((1 * Time.deltaTime) * AtributosCarro.sensibilidade);
					} else{
						orietemp = orientacao.y + ((-1 * Time.deltaTime) * AtributosCarro.sensibilidade);
					}
				} else if(orientacaoAtual == 180){ // Cima;
					if(orietemp >= orientacaoAtual && orietemp <= 360){
						orietemp = orientacao.y + ((1 * Time.deltaTime) * AtributosCarro.sensibilidade);
					} else{
						orietemp = orientacao.y + ((-1 * Time.deltaTime) * AtributosCarro.sensibilidade);
					}
				} else if(orientacaoAtual == 270){ // Direita;
					if(orietemp <= orientacaoAtual && orietemp >= 90){
						orietemp = orientacao.y + ((-1 * Time.deltaTime) * AtributosCarro.sensibilidade);
					} else{
						orietemp = orientacao.y + ((1 * Time.deltaTime) * AtributosCarro.sensibilidade);
					}
				}
				

				orientacao.y = orietemp;
			}

			this.btnDir = false;
			this.btnEsq = false;
		}

		transform.eulerAngles = orientacao;//Aplica a rotacao
		//-----------------------------------------------------------

        AplicarSpeedCarro();
	}

    void OnGUI()
    {
        // Logica de desenhar botões:

        // Gerando a transparência:
        //Color c = GUI.color;
        //c.a = 0.5f; // Quanto maior o valor neste caso, menos transparente;
        //GUI.color = new Color(c.r, c.g, c.b, c.a); // Nova cor para a GUI; Neste caso estou só alterando o alpha;

        if (this.btnDir)
        {
			GUI.DrawTexture(new Rect(posBtnD.x, posBtnD.y, Screen.width * 0.1f, Screen.height * 0.1f), this.sprite);
        }
        else if (this.btnEsq)
        {
			GUI.DrawTexture(new Rect(posBtnE.x, posBtnE.y, -(Screen.width * 0.1f), Screen.height * 0.1f), this.sprite); // Um dos valores e transformado para negativo, para que a imagem fique ao contrario na horizontal;
        }
        //GUI.color = new Color(c.r, c.g, c.b, 1f); // Voltando para o default;
    }

    private void AplicarSpeedCarro()
    {
        rigidbody.velocity = transform.forward * AtributosCarro.speed; //Aplica a velocidade
		
		if(rigidbody.velocity.magnitude > 0.1f)
		{
			transform.forward = new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z);
		}	
    }

	private void AtualizarCombustivel()
	{
		//Velocidade -- Andar apenas quando tiver combustivel
		
		if (!chegouDestino)//Se ainda nao chegou ao destino, continua acelerando
		{
			if(AtributosCarro.fuel > 0)
			{

				if(Mathf.Floor(AtributosCarro.speed) == AtributosCarro.maxSpeed && AtributosCarro.maxSpeed == 0)
				{
					AtributosCarro.speed = 0;
				}
				else
				{
					if(AtributosCarro.speed < AtributosCarro.maxSpeed)
						AtributosCarro.speed += AtributosCarro.accel/10;//Vai acelerando
                    else if (AtributosCarro.speed > AtributosCarro.maxSpeed)
                    {
                        AtributosCarro.speed -= AtributosCarro.desacel / 10;

                    }
				}

                if (AtributosCarro.speed <= 0
                    && barredbyOldMan)
                {
                    //chama funcao do velho para ele parar e tocar animacao
                    reftolastOldMan.StopAndComplain();
                    barredbyOldMan = false;
                }

                if (AtributosCarro.speed <= 0
                    && barredbyCat)
                {
                    //chama funcao do gato para ele parar e tocar animacao
                    reftolastCat.StopAndComplain();
                    barredbyCat = false;
                }

			}
			else // Se acabar o combustivel, diminuir a velocidade ate 0
			{
				if(!somTocando)//Pra nao ficar tocando repetidamente
				{
					scriptSom.TocaSom("fuelOver");
					somTocando = true;
				}
				if(AtributosCarro.speed > 0)
				{
					AtributosCarro.speed -= 0.05f;
					somMotorPuma.volume -= 0; //Corta o som do motor
					somMotorFusca.volume -= 0;
					somMotorOpala.volume -= 0;
				}
				
				if (AtributosCarro.speed <= 0)//Quando ele parou de andar, deve acabar o jogo
				{
					refScriptObjGerente.AcabouCombustivel();
				}
			}
		}
		else//Se chegou ao cemiterio, vai reduzindo a velocidade
		{
			if(AtributosCarro.speed > 0)
			{
                if (!AtributosCarro.boostVelocidade)//se nao estiver com boost de velocidade
                {
                    if (AtributosCarro.tipoCarro == 0)
                        AtributosCarro.speed -= 0.05f;
                    else if (AtributosCarro.tipoCarro == 1)
                        AtributosCarro.speed -= 0.046f;
                    else if (AtributosCarro.tipoCarro == 2)
                        AtributosCarro.speed -= 0.042f;

                    camT.transform.position = new Vector3(camT.transform.position.x, camT.transform.position.y + 0.1f, camT.transform.position.z);
                }
                else
                {
                    if (AtributosCarro.tipoCarro == 0)
                        AtributosCarro.speed -= 0.06f;
                    else if (AtributosCarro.tipoCarro == 1)
                        AtributosCarro.speed -= 0.055f;
                    else if (AtributosCarro.tipoCarro == 2)
                        AtributosCarro.speed -= 0.048f;

                    camT.transform.position = new Vector3(camT.transform.position.x, camT.transform.position.y + 0.1f, camT.transform.position.z);

                }
			}
			else if (AtributosCarro.speed < 0)
			{
				AtributosCarro.speed = 0;

			}
			
		}
		
		//-----Controle de gasto do fuel-------
		AtributosCarro.tempoRelativoFuel += Time.deltaTime; // Contando os segundos do fuel
		
		if(AtributosCarro.tempoRelativoFuel >= AtributosCarro.tempoFuel) // Tirar 1 ponto de fuel caso chegue/passe do tempo em (tempoFuel)
		{
			if (AtributosCarro.fuel > 0)//se ainda tiver combustivel, diminui
			{
				AtributosCarro.fuel--;
				refScriptObjGerente.DiminuirPonteiroFuel();
                //Debug.Log("Diminuiu fuel: atual " + AtributosCarro.fuel);
			}
			AtributosCarro.tempoRelativoFuel = 0;
		}
		//-------------------------------------
		
	}

    //Programem o comportamento dos power ups aqui, baseado no tipo
    //IMPORTANTE: todas modificaçoes de valores devem ser feitos na struct temporaria e dps atribuida a posicao da lista
    private void ComportamentoPowerUps(int PosnaLista)//Manda a posicao da lista daquele power up
    {
        if (listaPowerUpsAtivos[PosnaLista].tipoPowerUp == 1)//Power up velocidade
        {
            if (listaPowerUpsAtivos[PosnaLista].temporizador < listaPowerUpsAtivos[PosnaLista].duracaoPowerUp)
            {
                AtributosCarro.maxSpeed = AtributosCarro.maxSpeedPadrao * 1.4f;
                AtributosCarro.boostVelocidade = true;

                //Cria uma struct de power ups temporaria
                PowerUps temp = listaPowerUpsAtivos[PosnaLista];
                temp.temporizador += Time.deltaTime;//Modifica os valores

                listaPowerUpsAtivos[PosnaLista] = temp;//Atribui o objeto temporario a lista - sobreescrevendo

            }
            else // Acabou o tempo do PowerUp, volta ao normal
            {
                AtributosCarro.maxSpeed = AtributosCarro.maxSpeedPadrao;
                AtributosCarro.boostVelocidade = false;

                PowerUps temp = listaPowerUpsAtivos[PosnaLista];
                temp.temporizador = 0;
                temp.aindaAtivo = false;

                listaPowerUpsAtivos[PosnaLista] = temp;//Atribui o objeto temporario a lista - sobreescrevendo

            }
        }
        else if (listaPowerUpsAtivos[PosnaLista].tipoPowerUp == 2)// PowerUp de Invencibilidade
        {
            if (listaPowerUpsAtivos[PosnaLista].temporizador < listaPowerUpsAtivos[PosnaLista].duracaoPowerUp)
            {
                AtributosCarro.desacel = AtributosCarro.speed; // Vai "desacelerar" ate a velocidade atual. Em outras palavras, nada acontece
                AtributosCarro.batida = AtributosCarro.speed; // Ao bater fica na mesma velocidade

                PowerUps temp = listaPowerUpsAtivos[PosnaLista];
                temp.temporizador += Time.deltaTime;//Atualiza o temporizador daquele power up

                listaPowerUpsAtivos[PosnaLista] = temp;

                imuneEfeitos = true;//Carro fica imune a efeitos de desaceleraçao
                particulas.particleEmitter.emit = true; // Abilita o emissor de particulas

            }
            else // Acabou o tempo do PowerUp, volta ao normal
            {
                AtributosCarro.desacel = AtributosCarro.desacelPadrao;
                AtributosCarro.batida = AtributosCarro.batidaPadrao;

                PowerUps temp = listaPowerUpsAtivos[PosnaLista];
                temp.temporizador = 0;
                temp.aindaAtivo = false;//Desliga o power up

                listaPowerUpsAtivos[PosnaLista] = temp;

                refScriptObjGerente.ToggleUpdateTimerLoira(true);//Ativa novamente o contador da loira e do jogo
                imuneEfeitos = false;
                particulas.particleEmitter.emit = false; // Desabilita o emissor de particulas
            }
        }
        else if (listaPowerUpsAtivos[PosnaLista].tipoPowerUp == 3)// PowerDown de Sensibilidade (Dirigibilidade)
        {
            if (listaPowerUpsAtivos[PosnaLista].temporizador < listaPowerUpsAtivos[PosnaLista].duracaoPowerUp)
            {
                //aumenta a sensibilidade baseado na intensidade (cada Pu de sensibilidade adiciona + 15, quando um acaba ele reduz a intensidade
                AtributosCarro.sensibilidade = AtributosCarro.sensibilidadePadrao + (15 * AtributosCarro.intensidadeSensibilidade);

                //Cria uma struct de power ups temporaria
                PowerUps temp = listaPowerUpsAtivos[PosnaLista];
                temp.temporizador += Time.deltaTime;//Modifica os valores

                listaPowerUpsAtivos[PosnaLista] = temp;//Atribui o objeto temporario a lista - sobreescrevendo

            }
            else // Acabou o tempo do PowerUp, volta ao normal
            {
                //Volta ao normal
                AtributosCarro.sensibilidade = AtributosCarro.sensibilidadePadrao;
                AtributosCarro.intensidadeSensibilidade--;

                if (AtributosCarro.intensidadeSensibilidade < 0)
                    AtributosCarro.intensidadeSensibilidade = 0;

                PowerUps temp = listaPowerUpsAtivos[PosnaLista];
                temp.temporizador = 0;
                temp.aindaAtivo = false;

                listaPowerUpsAtivos[PosnaLista] = temp;//Atribui o objeto temporario a lista - sobreescrevendo

            }
        }


    }

    private void UpdatePowerUpsList()
    {
        bool deveRemoverAlgumPu = false;

        if (listaPowerUpsAtivos.Count > 0)
        {
            for (int i = 0; i < listaPowerUpsAtivos.Count; i++)
            {
                if (listaPowerUpsAtivos[i].aindaAtivo)//Se o power up ainda esta ativo
                {
                    ComportamentoPowerUps(i);//Atualiza o comportamento
                }
                else
                {
                    deveRemoverAlgumPu = true;
                }

            }

        }

        if (deveRemoverAlgumPu)//pelo menos um power up deve ser removido
        {
            for (int i = listaPowerUpsAtivos.Count - 1; i >= 0; i--)//For ao contrario
            {
                if (!listaPowerUpsAtivos[i].aindaAtivo)//Se o power up nao estiver mais ativo
                {
                    listaPowerUpsAtivos.RemoveAt(i);//remove da lista
                }
            }

        }



    }

    private void MoverDir()
    {
        int fator = 1;
        float rotCemY = camT.transform.eulerAngles.y;

        if (rotCemY == 180
            || rotCemY == 270)
        { 
            fator = -1;
        }

        float orietemp = orientacao.y + ((fator * Time.deltaTime) * AtributosCarro.sensibilidade);

        // Limitador:
        if (orietemp > anguloLimiteD && orietemp < anguloLimiteD + 5)
        {
            orietemp = anguloLimiteD;
        }

        orientacao.y = orietemp;


        transform.eulerAngles = orientacao;//Aplica a rotacao


        rigidbody.velocity = transform.forward * AtributosCarro.speed; //Aplica a velocidade

        if (rigidbody.velocity.magnitude > 0.1f)
        {
            transform.forward = new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z);
        }

    }

    private void MoverEsq()
    {
        int fator = -1;
        float rotCemY = camT.transform.eulerAngles.y;

        if (rotCemY == 180
            || rotCemY == 270)
        {
            fator = 1;
        }
        
        float orietemp = orientacao.y + ((fator * Time.deltaTime) * AtributosCarro.sensibilidade);

        // Limitador:
        if (orietemp < anguloLimiteE && orietemp > anguloLimiteE - 5)
        {
            orietemp = anguloLimiteE;
        }

        orientacao.y = orietemp;


        transform.eulerAngles = orientacao;//Aplica a rotacao


        rigidbody.velocity = transform.forward * AtributosCarro.speed; //Aplica a velocidade

        if (rigidbody.velocity.magnitude > 0.1f)
        {
            transform.forward = new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z);
        }
    
    }

    IEnumerator OleoEEfeito(float tempo)
    {
        //Oleo da esquerda faz escorregar p a direita
        
        this.controleDoJogador = false;
        escorregandoD = true;

//        fatorLimite = 45f;
//        Transform Camerat = GameObject.Find("Main Camera").transform;
//        AtualizarLimites(Camerat);

		//Debug.Log (Camerat.transform.eulerAngles.y);

        yield return new WaitForSeconds(tempo);

		if(!trocandoRua){//if(faltaAjustar || !finalizouRotacaoCam){
        	this.controleDoJogador = true;
			//Debug.Log ("LiberouJogador");
		}
        escorregandoD = false;

//        fatorLimite = 90f;
//        AtualizarLimites(Camerat);
    }

    IEnumerator OleoDEfeito(float tempo)
    {
        //Oleo da direita faz escorregar p a esquerda
        
        this.controleDoJogador = false;
        escorregandoE = true;
        
//        fatorLimite = 45f;
//        Transform Camerat = GameObject.Find("Main Camera").transform;
//        AtualizarLimites( Camerat);

		//Debug.Log (Camerat.transform.eulerAngles.y);

        yield return new WaitForSeconds(tempo);

		if(!trocandoRua){//if(faltaAjustar || !finalizouRotacaoCam){
        	this.controleDoJogador = true;
			//Debug.Log ("LiberouJogador");
		}
        escorregandoE = false;
        
//        fatorLimite = 90f;
//        AtualizarLimites(Camerat);
    }

	IEnumerator AguaEfeito(float tempo)
	{
		//Oleo da esquerda faz escorregar p a direita
		
		this.controleDoJogador = false;
		//escorregandoD = true;
		this.agua = true;
		
		//		fatorLimite = 45;
		//		Transform Camerat = GameObject.Find("Main Camera").transform;
		//		AtualizarLimites(Camerat);
		
		yield return new WaitForSeconds(tempo);
		
		if(!trocandoRua)
			this.controleDoJogador = true;
		this.agua = false;
		//escorregandoD = false;
		
		//		fatorLimite = 85;
		//		AtualizarLimites(Camerat);
	}

	// Update is called once per frame
	void FixedUpdate ()
	{
		AtualizarCombustivel();
		UpdatePowerUpsList();
        if (this.controleDoJogador)
            RotAndMove();
        else
        {
            if (escorregandoD)
                MoverDir();
            else if (escorregandoE)
                MoverEsq();
			else if (agua){
				// nada...
			} else
				AutoMove();
        }

		LoopDoSom();

		// Game Analytics: Mostrar posiçao do carro a cada segundo;
		if(tempoSistema > 1){// Para garantir uma atualizaçao apos cada segundo;
			string driverName = "";
			switch(statusTmp.IDPersonagem){
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
			switch(statusTmp.IDFase){
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
			GA.API.Design.NewEvent("Level"+fase+":Driver:"+driverName+":Position", gameObject.transform.position);
            this.tempoSistema = .0f;
		}
		this.tempoSistema += Time.deltaTime;
	}
	
	void OnTriggerEnter(Collider objColisor)
	{
		//Colidiu com os power ups
		if(objColisor.tag == "PuFuel") 
		{
			AumentarFuel();
			scriptSom.TocaSom("fuelUp");
			Destroy(objColisor.gameObject);
			refScriptObjGerente.ChangeLoiraReaction(true);//Sinaliza um evento bom para a loira

			// Game Analytics:
			string driverName = "";
			switch(statusTmp.IDPersonagem){
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
			switch(statusTmp.IDFase){
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
            GA.API.Design.NewEvent("Level" + fase + "Power:Fuel:Driver:" + driverName, gameObject.transform.position);//GA.API.Design.NewEvent("Level"+fase+":Driver:Collision"+driverName, 1, gameObject.transform.position); // Game Analytics;
		}

		if (objColisor.tag=="PuVel")//power up velocidade
		{
			PowerUps PuVel = new PowerUps(1);
			listaPowerUpsAtivos.Add(PuVel);

			scriptSom.TocaSom("acelera");

			//destroi objeto
			Destroy(objColisor.gameObject);

			refScriptObjGerente.ChangeLoiraReaction(true);//Sinaliza um evento bom para a loira

			// Game Analytics:
			string driverName = "";
			switch(statusTmp.IDPersonagem){
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
			switch(statusTmp.IDFase){
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
			GA.API.Design.NewEvent("Level"+fase+"Power:Velocity:Driver:"+driverName, gameObject.transform.position);//GA.API.Design.NewEvent("Level"+fase+":Driver:Collision"+driverName, 2, gameObject.transform.position); // Game Analytics;
		}
		else if (objColisor.tag == "PuInv")//Power up invencibilidade
		{
			refScriptObjGerente.ToggleUpdateTimerLoira(false);//Para o contador da loira
			PowerUps PuInv = new PowerUps(2);//Cria o power up de invencibilidade
			listaPowerUpsAtivos.Add(PuInv);//Adiciona na lista
			//destroi objeto
			Destroy(objColisor.gameObject);

			refScriptObjGerente.ChangeLoiraReaction(true);//Sinaliza um evento bom para a loira


			// Game Analytics:
			string driverName = "";
			switch(statusTmp.IDPersonagem){
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
			switch(statusTmp.IDFase){
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
            GA.API.Design.NewEvent("Level" + fase + ":Power:Invulnerability:Driver:" + driverName, gameObject.transform.position);
            //GA.API.Design.NewEvent("Level"+fase+":Driver:Collision"+driverName, 3, gameObject.transform.position); // Game Analytics;
		}
		else if (objColisor.tag == "PdSen")//power down sensibilidade
		{
            AtributosCarro.intensidadeSensibilidade++;//Aumenta a intensidade em 1
			PowerUps PdSen = new PowerUps(3);
			listaPowerUpsAtivos.Add(PdSen);
			//destroi objeto
			Destroy(objColisor.gameObject);

			refScriptObjGerente.ChangeLoiraReaction(false);//Sinaliza um evento ruim para a loira

			// Game Analytics:
			string driverName = "";
			switch(statusTmp.IDPersonagem){
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
			switch(statusTmp.IDFase){
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
            
            GA.API.Design.NewEvent("Level" + fase + ":Power:Sensibility:Driver:" + driverName, gameObject.transform.position);
			//GA.API.Design.NewEvent("Level"+fase+":Driver:Collision"+driverName, 4, gameObject.transform.position); // Game Analytics;
		}
		else if (objColisor.tag == "PdAgua")//power down - agua
		{
			if (!imuneEfeitos)
			{
				//oleo que direciona para a direita
				StartCoroutine(AguaEfeito(1.0f));
				
				scriptSom.TocaSom("agua");
				
				refScriptObjGerente.ChangeLoiraReaction(false);//Sinaliza um evento ruim para a loira

				// Game Analytics:
				string driverName = "";
				switch(statusTmp.IDPersonagem){
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
				switch(statusTmp.IDFase){
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
                
                GA.API.Design.NewEvent("Level" + fase + ":Power:Water:Driver:" + driverName, gameObject.transform.position);
                //GA.API.Design.NewEvent("Level"+fase+":Driver:Collision"+driverName, 5, gameObject.transform.position); // Game Analytics;
			}
		}
		else if (objColisor.tag == "PdOleoE")//power down  - oleo
		{
            if (!imuneEfeitos)
            {
                //oleo que direciona para a direita
                StartCoroutine(OleoEEfeito(1.2f));

                scriptSom.TocaSom("oleo");

                refScriptObjGerente.ChangeLoiraReaction(false);//Sinaliza um evento ruim para a loira

				// Game Analytics:
				string driverName = "";
				switch(statusTmp.IDPersonagem){
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
				switch(statusTmp.IDFase){
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
                GA.API.Design.NewEvent("Level" + fase + ":Power:Oil:Driver:" + driverName, gameObject.transform.position);
				//GA.API.Design.NewEvent("Level"+fase+":Driver:Collision"+driverName, 6, gameObject.transform.position); // Game Analytics;
            }
		}
        else if (objColisor.tag == "PdOleoD")//power down  - oleo
        {

            if (!imuneEfeitos)
            {

                //oleo que direciona para a esquerda - já que o oleo se encontra na direita da pista
                StartCoroutine(OleoDEfeito(1.2f));

                scriptSom.TocaSom("oleo");

                refScriptObjGerente.ChangeLoiraReaction(false);//Sinaliza um evento ruim para a loira

				// Game Analytics:
				string driverName = "";
				switch(statusTmp.IDPersonagem){
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
				switch(statusTmp.IDFase){
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

                GA.API.Design.NewEvent("Level" + fase + ":Power:Oil:Driver:" + driverName, gameObject.transform.position);
				//GA.API.Design.NewEvent("Level"+fase+":Driver:Collision"+driverName, 7, gameObject.transform.position); // Game Analytics;
            }
        }
		
		
		if (objColisor.tag == "CemiterioTrigger")
		{
			chegouDestino = true;//Faz com que reduza a velocidade
            camT.GetComponent<CameraController>().enabled = false;
            refScriptObjGerente.AlcancouCemiterio();
           
/*
			// Game Analytics:
			string driverName = "";
			switch(statusTmp.IDPersonagem){
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
			switch(statusTmp.IDFase){
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
			GA.API.Design.NewEvent("Level"+fase+":Driver:"+driverName+":Success", this.statusTmp.GetMelhorTempo(this.statusTmp.IDPersonagem), gameObject.transform.position);
*/
		}

		// Troca de ruas:
        if (objColisor.tag == "TriggerRua")
        {
			if(!this.trocandoRua){ // Se nao estiver trcando de rua;
				//Debug.Break ();
				//				float tempR = objColisor.transform.eulerAngles.y + 180;
	            //				if(tempR >= 360)
	            //					tempR -= 360;
	            //Debug.Log ("tempR: "+tempR+"; orientacaoAtual: "+orientacaoAtual+"; objColisor: "+objColisor.transform.eulerAngles.y);
	            int inversa = (int)(objColisor.transform.eulerAngles.y + 180);
	            if (inversa >= 360)
	                inversa -= 360;

	            if ((int)this.orientacaoAtual != (int)objColisor.transform.eulerAngles.y && (int)this.orientacaoAtual != inversa)
	            { // Testando só a última; //if((this.orientacaoAtual != objColisor.transform.eulerAngles.y) && (this.orientacaoAtual != tempR)){ // Este teste é feito para garantir que não foi atingido o trigger superior e inferior;
					// Desativar oleo ou agua que possam estar ativos:
	//				this.escorregandoE = false;
	//				this.escorregandoD = false;
	//				this.agua = false;
	//				fatorLimite = 90;
	//				AtualizarLimites(camT.transform);
					// Fim da destivaçao do oleo, agua, etc;

					this.controleDoJogador = false; // Jogador perde o controle e jogo controla o carro para trocar de rua;
	                this.orientacaoAtual = (short)objColisor.transform.eulerAngles.y;//this.orientacaoAtual = objColisor.transform.eulerAngles.y + 180; // orientacaoAtual recebe o valor da rotação mais 180, pois ele deve apontar para a extremidade oposta do original, pois esta mesma var será usada depois para atualizar a câmera. IMPORTANTE: esta variável normalmente indica a posição oposta a câmera, mas esta lógica reaproveita variáveis, evitando o disperdício de memória, por isto neste contexto ela está funcionando de maneira oposta a sua funcionalidade padrão;
	                //					if(this.orientacaoAtual >= 360)
	                //						this.orientacaoAtual -= 360;
	                this.faltaAjustar = true;

					this.trocandoRua = true; // Avisa para os metodos que tratam o movimento de oleo e agua que carro esta trocando de rua;
	            }
	            //Debug.Log ("tempR: "+tempR+"; orientacaoAtual: "+orientacaoAtual+"; objColisor: "+objColisor.transform.eulerAngles.y);
			}
        }


        if (objColisor.tag == "PdPoli")
        {
            //mostra o plano de eventos com o policial       
            refScriptObjGerente.ChangeImgEventsPlane(2);

            scriptSom.TocaSom("policia");

            StartCoroutine(refScriptObjGerente.AnimPolicial( objColisor.gameObject ));

			// Game Analytics:
			string driverName = "";
			switch(statusTmp.IDPersonagem){
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
			switch(statusTmp.IDFase){
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
            GA.API.Design.NewEvent("Level" + fase + ":Enemy:Police:Driver:" + driverName, gameObject.transform.position);
			
        }


        //Velho Related
        if (objColisor.tag == "PdVelhoC"
           || objColisor.tag == "PdVelhoD"
           || objColisor.tag == "PdVelhoE"
           || objColisor.tag == "PdVelhoB")
        {
            if (!imuneEfeitos)
            {
                barredbyOldMan = true;//Se tiver imune a efeitos o carro nao irá parar, logo deve continuar false
            }

            //Pega a referencia ao ultimo velho que o player entrou na área
            reftolastOldMan = objColisor.GetComponent<Velho>();

			// Game Analytics:
			string driverName = "";
			switch(statusTmp.IDPersonagem){
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
			switch(statusTmp.IDFase){
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
            
            GA.API.Design.NewEvent("Level" + fase + ":Enemy:OldMan:Driver:" + driverName, gameObject.transform.position);
			
        }


        if (objColisor.tag == "PdVelhoSelf" && !imuneEfeitos)//se esta imune a efeitos passa reto
        { 
            //Entrou mesmo na area do velho, da uma parada brusca no carro
            barredbyOldMan = false;
            if (AtributosCarro.boostVelocidade)//se esta com pu de velocidade freia mais forte ainda
                AtributosCarro.desacel = 2.0f;
            else
                AtributosCarro.desacel = 1.2f;//freia mais bruscamente
            AtributosCarro.maxSpeed = 0;
            reftolastOldMan.StopAndComplain();
        }


        //Gato Related
        if ( objColisor.tag == "PdGato" )
        {
            if (!imuneEfeitos)
            {
                barredbyCat = true;//Se tiver imune a efeitos o carro nao irá parar, logo deve continuar false
            }

            reftolastCat = objColisor.GetComponent<Gato>();//Salva a referencia ao ultimo gato
            
			// Game Analytics:
			string driverName = "";
			switch(statusTmp.IDPersonagem){
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
			switch(statusTmp.IDFase){
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

            GA.API.Design.NewEvent("Level" + fase + ":Enemy:Cat:Driver:" + driverName, gameObject.transform.position);
			
        }

        if (objColisor.tag == "PdGatoSelf" && !imuneEfeitos)//se esta imune a efeitos passa reto
        {
            //Entrou mesmo na area do gato, da uma parada brusca no carro
            if (AtributosCarro.boostVelocidade)//se esta com pu de velocidade freia mais forte ainda
                AtributosCarro.desacel = 2.0f;
            else
                AtributosCarro.desacel = 1.2f;//freia mais bruscamente

            AtributosCarro.maxSpeed = 0;
            barredbyCat = false;
            reftolastCat.StopAndComplain();

        }

        if (objColisor.tag == "PdGatoInvT" && !imuneEfeitos)
        { 
            //Carro colidiu com um inversor de trigger de gato
            refScriptObjGerente.InverterTriggerGato(objColisor.gameObject.transform.parent.gameObject);//passa o pai do objeto
            //desliga o trigger inverso
            objColisor.collider.enabled = false;
        }

        if (objColisor.tag == "PdVelhoInvT" && !imuneEfeitos)
        {
            //Carro colidiu com um inversor de trigger de velho
            refScriptObjGerente.InverterTriggerVelho(objColisor.gameObject.transform.parent.gameObject);//passa o pai do objeto
            //desliga o trigger inverso
            objColisor.collider.enabled = false;
        }


	}
	
	void OnTriggerStay(Collider objColisor)
	{
		if (!imuneEfeitos)//Se nao estiver invencivel
		{
			if (objColisor.tag == "PdPoli")
				AtributosCarro.maxSpeed = 2;
			
            if (objColisor.tag == "PdVelhoC"
                || objColisor.tag == "PdVelhoD"
                || objColisor.tag == "PdVelhoE"
                || objColisor.tag == "PdVelhoB")
            {
                AtributosCarro.maxSpeed = 0;
            }

            if (objColisor.tag == "PdGato")
            {
                AtributosCarro.maxSpeed = 0;
            }
			
		}

  
	}
	
	void OnTriggerExit(Collider objColisor)
	{
        
        if (objColisor.tag == "PdPoli" && !imuneEfeitos)
        {
            AtributosCarro.maxSpeed = AtributosCarro.maxSpeedPadrao;
            refScriptObjGerente.DisplayEventsPlane(false,2);
        }
        
        if ((objColisor.tag == "PdVelhoC"
            || objColisor.tag == "PdVelhoD"
            || objColisor.tag == "PdVelhoE"
            || objColisor.tag == "PdVelhoB") && !imuneEfeitos) // Retorna ao normal ao sair dos Pd de area
        {
            AtributosCarro.maxSpeed = AtributosCarro.maxSpeedPadrao;

            if (barredbyOldMan)
                barredbyOldMan = false;
            
            refScriptObjGerente.DisplayEventsPlane(false,0);//velho
        }

        if ((objColisor.tag == "PdGato") && !imuneEfeitos) // Retorna ao normal ao sair dos Pd de area
        {
            AtributosCarro.maxSpeed = AtributosCarro.maxSpeedPadrao;

            if (barredbyCat)
                barredbyCat = false;

            refScriptObjGerente.DisplayEventsPlane(false,1);
        }

        if (objColisor.tag == "PdVelhoSelf" && !imuneEfeitos)//Colisor do velho de segundo nivel
        {
            //saiu da area do velho
            barredbyOldMan = false;
            AtributosCarro.desacel = AtributosCarro.desacelPadrao;
            AtributosCarro.maxSpeed = AtributosCarro.maxSpeedPadrao;
        }

        if (objColisor.tag == "PdGatoSelf" && !imuneEfeitos)//Colisor do velho de segundo nivel
        {
            //saiu da area do gato
            barredbyCat = false;
            AtributosCarro.desacel = AtributosCarro.desacelPadrao;
            AtributosCarro.maxSpeed = AtributosCarro.maxSpeedPadrao;
        }

        if (objColisor.tag == "PdVelhoAT")
        { 
            Destroy(objColisor.gameObject);
            refScriptObjGerente.DisplayEventsPlane(false,0);//Quando sai dos triggers de área do gato e do velho para de mostrar o plano deles
        }
        else if (objColisor.tag == "PdGatoAT")
        {
            Destroy(objColisor.gameObject);
            refScriptObjGerente.DisplayEventsPlane(false, 1);//Quando sai dos triggers de área do gato e do velho para de mostrar o plano deles
        }

	}

	void OnCollisionEnter(Collision colisao)
	{
		if(!imuneEfeitos)//Quando o carro bate
		{
            refScriptObjGerente.DecrementarTempoBatida();
		}

		int temp;
		// Game Analytics:
		string driverName = "";
		short fase = 0;
		//

		switch(colisao.collider.tag)
		{
		case "ObjLixo":
			temp = Random.Range (0, 2);

			scriptSom.TocaSom("lixo");

			// Game Analytics:
			switch(statusTmp.IDPersonagem){
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
			switch(statusTmp.IDFase){
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
			GA.API.Design.NewEvent("Level"+fase+":Driver:"+driverName+":Collision:Trash", gameObject.transform.position);

			break;
		default:
			temp = Random.Range (0,2);

			if(AtributosCarro.speed > AtributosCarro.maxSpeedPadrao) // Com power up de velocidade
			{
				scriptSom.TocaSom("paredeFraco");
			}
			else // Sem power up de velocidade (som mais fraco)
			{
				scriptSom.TocaSom("paredeForte");
			}

			// Game Analytics:
			switch(statusTmp.IDPersonagem){
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
			switch(statusTmp.IDFase){
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

            GA.API.Design.NewEvent("Level" + fase + "Collision:Wall:Driver:" + driverName, gameObject.transform.position);
			
			break;
		}
	}

	void OnCollisionStay(Collision colisao)
	{
		if(!imuneEfeitos)
			AtributosCarro.speed = AtributosCarro.batida;
	}

	//Voids que sao chamados ao 'colidir' com PowerUps
	private void AumentarFuel()
	{
        if (AtributosCarro.fuel == AtributosCarro.fuelLimite)//se estiver de tanque cheio
            return;
        
        float QuantoAumentar = 5;//Aumenta 3 de combustivel a cd power up
        float combAntigo = AtributosCarro.fuel;

        AtributosCarro.fuel += (int)QuantoAumentar;//Aumenta combustivel

        if (AtributosCarro.fuel > AtributosCarro.fuelLimite)
        {
            //ver quanto aumentou

            float fuelQAumentou = AtributosCarro.fuelLimite - combAntigo;
            AtributosCarro.fuel = (int)AtributosCarro.fuelLimite;

            refScriptObjGerente.AumentarCombustivel(fuelQAumentou);
            //Debug.Log("Aumentou em " + fuelQAumentou); 
        }
        else
        {
            refScriptObjGerente.AumentarCombustivel(QuantoAumentar);
            //Debug.Log("Aumentou sem l em " + QuantoAumentar); 
        }


        //float QuantoAumentar = 3;//Aumenta 3 de combustivel a cd power up
        //float combAntigo = AtributosCarro.fuel;
		
        //AtributosCarro.fuel += (int)QuantoAumentar;//Aumenta combustivel
		
        //if (AtributosCarro.fuel > AtributosCarro.fuelLimite)
        //{
        //    //ver quanto aumentou
			
        //    float fuelQAumentou = AtributosCarro.fuelLimite - combAntigo;
        //    AtributosCarro.fuel = (int)AtributosCarro.fuelLimite;
			
        //    refScriptObjGerente.AumentarCombustivel( fuelQAumentou );
			
        //}
        //else
        //{
        //    refScriptObjGerente.AumentarCombustivel( QuantoAumentar );
        //}
	}
	
	private void LoopDoSom() // Muda o pitch do motor pra seguir com a velocidade do carro
	{
		somMotorPuma.pitch = AtributosCarro.speed / AtributosCarro.maxSpeedPadrao/1.2f +1;
		somMotorFusca.pitch = AtributosCarro.speed / AtributosCarro.maxSpeedPadrao/1.2f +1;
		somMotorOpala.pitch = AtributosCarro.speed / AtributosCarro.maxSpeedPadrao/1.2f +1;
	}

    public void SetChegouDestino()
    {
        chegouDestino = true;
    }
	
	public int GetTipoCarro()
	{
		return AtributosCarro.tipoCarro;
	}

    public void PlayOldManSound()
    { 
        //toca o som do velho reclamando
        //int temp = Random.Range(0, 2);
        scriptSom.TocaSom("velho");
    }

    public void PlayCatSound()
    {
        //toca o som do velho reclamando
        //int temp = Random.Range(0, 2);
        scriptSom.TocaSom("gato");
    }

    public bool GetImuneEfeitos()
    {
        return imuneEfeitos;
    }

    public void DestravarCarroVelhoPd()
    {
        //Apos o power down do velho ter acabado, garante que o carro volte a seu funcionamento normal
        AtributosCarro.maxSpeed = AtributosCarro.maxSpeedPadrao;

        if (barredbyOldMan)
            barredbyOldMan = false;

        //Desliga o plano do velho
        refScriptObjGerente.DisplayEventsPlane(false,0);
    
    }

    public void DestravarCarroGatoPd()
    {
        //Apos o power down do velho ter acabado, garante que o carro volte a seu funcionamento normal
        AtributosCarro.maxSpeed = AtributosCarro.maxSpeedPadrao;

        if (barredbyCat)
            barredbyCat = false;

        //Desliga o plano do velho
        refScriptObjGerente.DisplayEventsPlane(false,1);

    }

}