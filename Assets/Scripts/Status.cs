//#define DEBUG_STATUS

using UnityEngine;
using System.Collections;
//using System.Collections.Generic;

public static class Extensions
{

    //Procura os filhos
    public static Transform Search(this Transform target, string name)
    {
        if (target.name == name) return target;

        for (int i = 0; i < target.childCount; ++i)
        {
            var result = Search(target.GetChild(i), name);

            if (result != null) return result;
        }

        return null;
    }
}


public class Status : MonoBehaviour 
{
       
    public bool isAndroid = false;
    public bool isiOS = false;

    //bool statusAntigo = true;
    //public Color corBotaoSelecionado = new Color(255, 0, 0, 0);// Cor do botao selecionado;
    //Color corBotaoNSelecionado = new Color(255, 255, 255, 255);
   
    // Auxiliares para marcar o botao selecionado:
    //private int OldFaseID = 0; // Gambiarra para marcar o botao selecionado;
	private Material spriteUltimoBtnSelecionado = null;//private OTSprite spriteUltimoBtnSelecionado = null; // Guarda a referencia do objeto sprite do ultimo botao selecionado;
    private bool selecionarBotaoFase = false;

	// Mostrar imagem de carregamento/loading:
	//protected bool _carregando = false; // Sinaliza que fase esta sendo carregada;
	protected short esperaCarregando = 0;
	public bool carregando(){
		if(this.esperaCarregando > 0) // Se estiver carregando;
			return true;
		return false;
	}
//	public carregando{
//		set{ this._carregando = value; }
//		get{ return this._carregando; }
//	}
//	public esperaCarregando{
//		set{ this._esperaCarregando = value; }
//		get{ return this._esperaCarregando; }
//	}
	
    //Lista de botoes
    //GameObject[] listaBtns = null;

    public GameObject refUltimoBotaoSel;

    public int _ultimoBotaoSelID = -1;
    public int ultimoBotaoSelID
    {
        get { return this._ultimoBotaoSelID; }
        set { this._ultimoBotaoSelID = value; }
    }

    private MelhoresTempos refMelhoresTemposS = null;
    //-----------------------------------------------------------------


    private float tempoVenceuJogo = 0;
	bool primeiraVez = true;
	float[,] melhorTempo = new float[6,3] 
	{   {-1,-1,-1},
		{-1,-1,-1},
		{-1,-1,-1},
		{-1,-1,-1},
		{-1,-1,-1},
		{-1,-1,-1} };

    
	// Atributos do Jogador, equivale ao status do save;
	private int faseID = 0; // Fase selecionada para jogar; se for igual a -2 significa que o objeto Status nao e o que deve persistir;
	public int IDFase{
		get{return this.faseID;}
		set{this.faseID = value;}
	}


//	public float tempoFase
//	{ // Melhor tempo (Tempo salvo). Esta variável deve receber o tempo de jogo;
//		//get{return this.melhorTempo;}
//		//set{this.melhorTempo = value;}
//		//set{this.melhorTempo = value;}
//	}
//
//	public void SetMelhorTempo(int qualPlayer, float tempoRecebido )
//	{
//		//so vai ter uma variavel agora, melhorTempo
//		this.melhorTempo[qualPlayer] = tempoRecebido;
//
//	}


	float _tempoAtual = -1;
	public float tempoAtual
	{
		get{return this._tempoAtual;}
		set{this._tempoAtual = value;}
	}

	private int personagemID = 0;
	public int IDPersonagem
	{
		get{return this.personagemID;}
		set{this.personagemID = value;}
	}

    private float[] _tempoDuracaoCadaFase = new float[6];

    //Contem o tempo da fase atual do gerente
    public float tempoDuracaoFaseAtual
    {   
        get { return this._tempoDuracaoCadaFase[this.faseID-1]; }
        set { this._tempoDuracaoCadaFase[this.faseID - 1] = value; }
    }

    bool ordemLiberarFase = false;


    //BOTAO BLOQUEADO É 0, DESBLOQUEADO É 1
    private void SalvarBotoesBloqueados()
    {
		this.esperaCarregando++;
        if (Application.loadedLevelName == "Tela_Fases")
        {
            //Pega todos os botoes
            GameObject[] listBotoes = GameObject.FindGameObjectsWithTag("Btn");

            for (int i = 1; i < (listBotoes.Length + 1); i++)
            {
                Btn_Fase btn = listBotoes[i - 1].gameObject.GetComponent<Btn_Fase>();// Tira a cor do botao atual e adiciona cor ao Tint do outro;

                if (btn.GetJaDefiniuBloqueio())//se o bloqueio para aquele botão ja foi definido
                {

                    if (btn.bloqueado)//se o botao esta bloqueado, salva como bloqueado
                    {
                        PlayerPrefs.SetInt("lvl" + (btn.ID) + "b", 0);//botoes começam de 1 a 6
                        //Debug.Log("Salvou Bloqueado: " + btn.ID);
                    }
                    else//salva como desbloqueado
                    {
                        PlayerPrefs.SetInt("lvl" + (btn.ID) + "b", 1);
                    }

                    if (btn.ID == 1)
                    {
                        //segurança - botao 1 é sempre liberado
                        PlayerPrefs.SetInt("lvl" + (btn.ID) + "b", 1);
                    }
                }
                else//nao foi definido se o botao foi bloqueado ou nao
                {
                    btn.DefinirBloqueio();//Se nao manda verificar e definir se bloqueia ou nao

                    if (btn.bloqueado)//se o botao esta bloqueado, salva como bloqueado
                    {
                        PlayerPrefs.SetInt("lvl" + (btn.ID) + "b", 0);//botoes começam de 1 a 6
                        //Debug.Log("Salvou Bloqueado: " + btn.ID);
                    }
                    else//salva como desbloqueado
                    {
                        PlayerPrefs.SetInt("lvl" + (btn.ID) + "b", 1);
                    }

                    if (btn.ID == 1)
                    {
                        //segurança - botao 1 é sempre liberado
                        PlayerPrefs.SetInt("lvl" + (btn.ID) + "b", 1);
                    }

                }
            }
        }
		this.esperaCarregando--;
    }

    private void SalvarTempos()
    {
		this.esperaCarregando++;
        for (int qualFase = 1; qualFase < 7; qualFase++)
        {
            PlayerPrefs.SetFloat("lvl" + 0 + "p" + qualFase + "t", this.melhorTempo[qualFase - 1, 0]); // Salva o tempo de jogo da fase selecionada;
            PlayerPrefs.SetFloat("lvl" + 1 + "p" + qualFase + "t", this.melhorTempo[qualFase - 1, 1]); // Salva o tempo de jogo da fase selecionada;
            PlayerPrefs.SetFloat("lvl" + 2 + "p" + qualFase + "t", this.melhorTempo[qualFase - 1, 2]); // Salva o tempo de jogo da fase selecionada;
        }

        PlayerPrefs.Save();
		this.esperaCarregando--;
    }

    private void LoadBotoesBloqueados()
    {
        if (Application.loadedLevelName == "Tela_Fases")
        {
			this.esperaCarregando++;
            //Pega todos os botoes
            GameObject[] listBotoes = GameObject.FindGameObjectsWithTag("Btn");

            for (int i = 1; i < (listBotoes.Length + 1); i++)
            {
                Btn_Fase btn = listBotoes[i - 1].gameObject.GetComponent<Btn_Fase>();// Tira a cor do botao atual e adiciona cor ao Tint do outro;

                int valorBloqueio = PlayerPrefs.GetInt("lvl" + (btn.ID) + "b");

                //BOTAO BLOQUEADO É 0, DESBLOQUEADO É 1
                if (valorBloqueio == 0)
                {
                    btn.bloqueado = true;
                    //Debug.Log("Btn blocked " + (btn.ID));
                }
                else 
                {
                    btn.bloqueado = false;
                }

                if (btn.ID == 1)
                {
                    btn.bloqueado = false;//Botao da fase 1 é sempre desbloqueado
                }
                
            }
			this.esperaCarregando--;
        }
    }
    
	public bool Save()
	{
		this.esperaCarregando++;
		try
		{
			// Abaixo segue tudo o que deve ser salvo, que são os atributos desta classe (Status);
			PlayerPrefs.SetInt("faseID", this.faseID); // Fase/nível selecionada;

			//if(this.melhorTempo > .0f && this._tempoAtual < this.melhorTempo) // Faz um teste e garante que o tempo salvo seja sempre melhor (Menor) que o melhor tempo já salvo;
			for (int qualFase=1; qualFase < 7; qualFase ++)
			{
				PlayerPrefs.SetFloat("lvl"+0+"p" +qualFase+"t", this.melhorTempo[qualFase-1,0]); // Salva o tempo de jogo da fase selecionada;
				PlayerPrefs.SetFloat("lvl"+1+"p" +qualFase+"t", this.melhorTempo[qualFase-1,1]); // Salva o tempo de jogo da fase selecionada;
				PlayerPrefs.SetFloat("lvl"+2+"p" +qualFase+"t", this.melhorTempo[qualFase-1,2]); // Salva o tempo de jogo da fase selecionada;
			}

            SalvarBotoesBloqueados();

		} 
		catch
		{
			this.esperaCarregando--;
			return false; // Não conseguiu carregar
		}

		PlayerPrefs.Save();
		this.esperaCarregando--;

		return true; // Conseguiu carregar





	}

	public bool Load()
	{
		this.esperaCarregando++;
		try{
            bool naoExistearquivo = false;
            
			this.IDFase = PlayerPrefs.GetInt("faseID");//o Valor que ele carregar vai ser o que deve ser marcado como ultima fase jogada

            if (this.faseID == 0//nao carregou nenhuma fase de arquivo(arquivo nao existia, retorna 0, caso contrario sempre vai salvar algum outro ID)
            && primeiraVez)
            {
                this.IDFase = 1;
                //this.OldFaseID = 0;

                naoExistearquivo = true;
            }
       

			//Carrega os melhores tempo para cada personage
			for (int qualFase=1; qualFase < 7; qualFase ++)
			{
				this.melhorTempo[qualFase -1,0] = PlayerPrefs.GetFloat("lvl"+0+"p"+qualFase+"t");
				this.melhorTempo[qualFase -1,1] = PlayerPrefs.GetFloat("lvl"+1+"p"+qualFase+"t");
				this.melhorTempo[qualFase -1,2] = PlayerPrefs.GetFloat("lvl"+2+"p"+qualFase+"t");
			}
			//this._tempoAtual = PlayerPrefs.GetFloat("lvl"+this.faseID+"t"); // Carrega o tempo de jogo da fase selecionada;


            //Se após carregar algum dos tempos veio 0, significa que nao existia um arquivo
            if (this.melhorTempo[0,0] == 0)
            {
                //reinicia os tempos - baseado no valor de cada fase
                for (int i = 0; i < 6; i++)
                {
                    float tempoNessaFase = GetTempoJogopelaFase(i + 1);//começa em 1, até a fase 6

                    melhorTempo[i, 0] = tempoNessaFase;//tempo inicial de cada fase
                    melhorTempo[i, 1] = tempoNessaFase;
                    melhorTempo[i, 2] = tempoNessaFase;

                    //Debug.Log("Salvou tempo certo, new, " + tempoNessaFase);
                }

                SalvarTempos();//Salva os novos tempos em arquivo
            }


            LoadBotoesBloqueados();

            if (naoExistearquivo)
            {
                //Se nao existe um arquivo, tu salva, criando um
                Save();

                naoExistearquivo = false;
                //Debug.Log("arquivo nao existia, criou um");
            }


		} 
        catch{
			this.esperaCarregando--;
			return false; // Não conseguiu carregar
		}
		this.esperaCarregando--;
		return true; // Conseguiu carregar
	}

	public void SetTempoJogobasedOnFase()
	{
        //Define o tempo de cada fase
        _tempoDuracaoCadaFase[0] = 180;//Primeira Fase - em segundos
        _tempoDuracaoCadaFase[1] = 180;
        _tempoDuracaoCadaFase[2] = 180;
        _tempoDuracaoCadaFase[3] = 210;//3 minutos e meio
        _tempoDuracaoCadaFase[4] = 240;//4 minutos
        _tempoDuracaoCadaFase[5] = 270;//4 minutos e meio

	}

    public float GetTempoJogopelaFase(int qualIDFase)
    {
        if (qualIDFase > 0 && qualIDFase < 7)
            return _tempoDuracaoCadaFase[qualIDFase - 1];
        else
            return 180;//default value
    }

    public void DisplayVitoria(float tempoJogo)
    {
        tempoVenceuJogo = tempoJogo;

        Application.LoadLevel("Venceu");
    }

    public float GettempoVenceujogo()
    {
        return tempoVenceuJogo;
    }

    /// <summary>
    /// Verifica o tempo que esta salvo na status como vitoria contra o tempo salvo na estrutura de dados
    /// </summary>
    public void VerificarDefinirTempo()
	{
		//Se o tempo de jogo for menor que o melhor tempo que ja existia, substitui
        if (tempoVenceuJogo < this.melhorTempo[this.faseID - 1, this.IDPersonagem])
		{
			this.melhorTempo[this.faseID-1,this.IDPersonagem] = tempoVenceuJogo;

            //Debug.Log("tempo salvo na status: " + tempoVenceuJogo);
            
            //Save();

            SalvarTempos();
		}
		
	}
	
	public float GetMelhorTempo(int qualPlayer)
	{
		
		return melhorTempo[this.IDFase -1,qualPlayer];
		
	}

    private void ResetPlayerPref()
    {
        //Roda uma vez e dps para- chama ela no fim da start
        //P reiniciar o status, deleteall uma vez, coom load e save comentado, dps comenta o delete, habilita o save, e na prox execucao tu comenta o save tb e habilit ao load

        PlayerPrefs.DeleteAll();
        Save();
        Load();
    }

	void Start () 
    {
        //PlayerPrefs.DeleteAll();

       

        // Codigo para evitar copia de objetos Status:
        DontDestroyOnLoad(this.gameObject);

        GameObject[] objs = GameObject.FindGameObjectsWithTag("Status");
        foreach (var element in objs)
        {
            if (element.gameObject != this.gameObject //se encontrou outro game object status, que nao é ele, se mata
                && this.IDFase == 0)
            {
                Destroy(this.gameObject);
                //Debug.Log("destruiu um status c id: " + this.IDFase);
            }
            
        }
        //-----------------------------------------------

        if (Application.platform == RuntimePlatform.Android)
        {
            isAndroid = true;
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            isiOS = true;
        }


        refUltimoBotaoSel = null;
        primeiraVez = true;
        ordemLiberarFase = false;

        //Define o tempo de jogo das fases
        SetTempoJogobasedOnFase();

        this.Load();

        //ResetPlayerPref();
        //PlayerPrefs.DeleteAll();

       
	}

    public void SetSelecionarBotaoFase()
    {
        selecionarBotaoFase = true;
    }

    //Selecao via status
    public void SelecionarBotaoFaseDentroStatus()
    {

        RemoverSelecaoBotaoFaseDentroStatus();
        
        GameObject[] listGO = GameObject.FindGameObjectsWithTag("Btn");
        for (int i = 0; i < listGO.Length; i++)
        {
            Btn_Fase btn = listGO[i].gameObject.GetComponent<Btn_Fase>();// Tira a cor do botao atual e adiciona cor ao Tint do outro;
			spriteUltimoBtnSelecionado = listGO[i].gameObject.renderer.material;//.GetComponent<OTSprite>();

            if (btn.ID == this.faseID)// Se for o botao correto;
            {
				GameObject obj = GameObject.Find("Images");
				ImagesScript iscript = obj.GetComponent<ImagesScript>();

				this.spriteUltimoBtnSelecionado.mainTexture = (Texture2D)iscript.imagem[btn.ID + 6]; // E somado 6 pois e a quantidade de posiçoes que devem ser puladas do array de imagens;//this.spriteUltimoBtnSelecionado.mainTexture = (Texture2D)iscript.imagem[7];// Selecionado;
                ultimoBotaoSelID = btn.ID;
                refUltimoBotaoSel = listGO[i].gameObject;
                btn.selecionado = true;

              
                break;
            }

        }


        //SetTempoJogobasedOnFase();

        //atualiza os melhores tempos
        if (refMelhoresTemposS == null)
        {
            refMelhoresTemposS = GameObject.Find("MelhoresTempos").GetComponent<MelhoresTempos>();
        }

        refMelhoresTemposS.TrocouFaseSelecionada();
    }

    public void RemoverSelecaoBotaoFaseDentroStatus()
    {

        if (refUltimoBotaoSel == null
            && _ultimoBotaoSelID == -1)//ou seja, realmente nao tem nenhum botao selecionado
        {
            return;
        }
        else
        {
            if (refUltimoBotaoSel != null)
            {
				Btn_Fase btn = refUltimoBotaoSel.GetComponent<Btn_Fase>();
				GameObject obj = GameObject.Find("Images");
				ImagesScript iscript = obj.GetComponent<ImagesScript>();
				
				this.spriteUltimoBtnSelecionado.mainTexture = (Texture2D)iscript.imagem[btn.ID]; // Botao sem seleçao;//spriteUltimoBtnSelecionado.color = corBotaoNSelecionado;//spriteUltimoBtnSelecionado.tintColor = corBotaoNSelecionado;
                ultimoBotaoSelID = -1;
                refUltimoBotaoSel = null;
                spriteUltimoBtnSelecionado = null;
                btn.selecionado = false;
            }
            else//utiliza do ID
            {
                if (Application.loadedLevelName == "Tela_Fases")
                {
                    GameObject[] listGO = GameObject.FindGameObjectsWithTag("Btn");
                    for (int i = 0; i < listGO.Length; i++)
                    {
                        Btn_Fase btn = listGO[i].gameObject.GetComponent<Btn_Fase>();// Tira a cor do botao atual e adiciona cor ao Tint do outro;

                        if (btn.ID == _ultimoBotaoSelID)// Se for o botao correto;
                        {
                            //if (btn.selecionado)
                            {
								GameObject obj = GameObject.Find("Images");
								ImagesScript iscript = obj.GetComponent<ImagesScript>();
								
								this.spriteUltimoBtnSelecionado.mainTexture = (Texture2D)iscript.imagem[btn.ID]; // Botao sem seleçao;//spriteUltimoBtnSelecionado.color = corBotaoNSelecionado;//spriteUltimoBtnSelecionado.tintColor = corBotaoNSelecionado;
                                ultimoBotaoSelID = -1;
                                refUltimoBotaoSel = null;
                                spriteUltimoBtnSelecionado = null;

                                break;
                            }

                        }
                    }
                }

            }
        }
        
        
    }
    //------------------------------------
	public GameObject prefabCavaletes1, prefabCavaletes2, prefabCavaletes3; // Prefabs

    void OnLevelWasLoaded()
    { // É chamado logo após a cena ter sido carregada;

        if (Application.loadedLevelName == "Tela_Fases")
        {
            //Se chegou nessa tela, seleciona algum botao

            if (selecionarBotaoFase)
            {
                selecionarBotaoFase = false;

                SelecionarBotaoFaseDentroStatus();
            }
            
            this.primeiraVez = false;


            if (ordemLiberarFase)//se tenho uma ordem para liberar a proxima fase
            {
                LiberarProxFase();//Testa se precisa e ja libera a prox fase

                ordemLiberarFase = false;
            }


		} 
        else if(Application.loadedLevelName == "Jogo")
        {
			switch(this.faseID){ // Logica para colocar cavaletes:
				case 3:
					// Nada;
					break;
				case 4:
					Instantiate(prefabCavaletes1);//Instantiate(Resources.Load<GameObject>(@"Assets/Prefabs/CavaletesHE"), new Vector3(310.1806f, 59.555f, 129.0308f), new Quaternion(0f, 0f, 0f, 0f)); // So carrega de dentro da pasta resources;
					break;
				case 5:
					Instantiate(prefabCavaletes2);
					break;
				case 6:
					Instantiate(prefabCavaletes3);
					break;
			}

		}
        else if (Application.loadedLevelName == "Fase1")
        {

        }

      

	}

	void Update () 
    {
#if DEBUG_STATUS
		if(Input.GetKey(KeyCode.Escape)){ // Como faria isso para o Android? Ajustar a lógica para funcionar somente em jogo;
			Save();
			Application.Quit(); // Sair;
		}
#endif
        
	}

    public void SetLiberarProxFase()
    {
        ordemLiberarFase = true;
    }

    private void LiberarProxFase()
    {

        if (this.IDFase >= 1 
            && this.IDFase <= 5)
        { 
            //Libera a proxima fase
            
            GameObject[] listBotoes = GameObject.FindGameObjectsWithTag("Btn");

            for (int i = 0; i < listBotoes.Length; i++)
            {
                Btn_Fase btn = listBotoes[i].gameObject.GetComponent<Btn_Fase>();// Tira a cor do botao atual e adiciona cor ao Tint do outro;
                
                if (btn.ID == (this.faseID + 1) )//Se for o botão pertence a proxima fase 
                { 
                    // Se for o botao correto;
                    try
                    {
                        int valorBloqueio = PlayerPrefs.GetInt("lvl" + (btn.ID) + "b");

                        if (valorBloqueio == 1)//se o botao nao esta bloqueado, significa que ja liberou ele numa partida anterior
                        {
                            //Debug.Log("ja liberou a fase");
                            SelecionarBotaoFaseDentroStatus();
                            return;
                        }

                        btn.bloqueado = false;
                        
                        //BOTAO BLOQUEADO É 0, DESBLOQUEADO É 1
                        PlayerPrefs.SetInt("lvl" + (this.faseID + 1) + "b", 1);//salva em arquivo essa nova fase como desbloqueada
                        
                        //Seleciona a nova fase
                        //this.OldFaseID = this.faseID;
                        this.faseID += 1;//aumenta a fase ID

                        PlayerPrefs.SetInt("faseID", this.faseID); // salva a nova fase liberada como a ultima

                        SalvarBotoesBloqueados();
                        PlayerPrefs.Save();

                        SelecionarBotaoFaseDentroStatus();
                    }
                    catch 
                    { 
                        Debug.LogError("StatusError - n conseguiu liberar fase"); 
                    }
                    
                 

                    break;
                }
            }
        }
    
    }

    //---------Old functions----------------

    //selecao por dentro do botao
    private void SelecionarBotaodaFase()
    {
        GameObject[] listGO = GameObject.FindGameObjectsWithTag("Btn");
        for (int i = 0; i < listGO.Length; i++)
        {
            Btn_Fase btn = listGO[i].gameObject.GetComponent<Btn_Fase>();// Tira a cor do botao atual e adiciona cor ao Tint do outro;

            if (btn.ID == this.faseID)// Se for o botao correto;
            {
                if (!btn.selecionado)
                    btn.SelecionarBotao();

                break;
            }

        }

    }

    //remove a seleção do ultimo botao
    public void RemoverSelecaoBotaoVelho()
    {

        if (refUltimoBotaoSel == null
            && _ultimoBotaoSelID == -1)//ou seja, realmente nao é nenhum botao selecionado
        {
            return;
        }
        else
        {
            if (refUltimoBotaoSel != null)
            {
                refUltimoBotaoSel.GetComponent<Btn_Fase>().DesselecionarBotao();
            }
            else//utiliza do ID
            {
                if (Application.loadedLevelName == "Tela_Fases")
                {
                    GameObject[] listGO = GameObject.FindGameObjectsWithTag("Btn");
                    for (int i = 0; i < listGO.Length; i++)
                    {
                        Btn_Fase btn = listGO[i].gameObject.GetComponent<Btn_Fase>();// Tira a cor do botao atual e adiciona cor ao Tint do outro;

                        if (btn.ID == _ultimoBotaoSelID)// Se for o botao correto;
                        {
                            //if (btn.selecionado)
                            {
                                btn.DesselecionarBotao();
                                break;
                            }

                        }
                    }
                }

            }
        }



    }
    //-----------------------
}
