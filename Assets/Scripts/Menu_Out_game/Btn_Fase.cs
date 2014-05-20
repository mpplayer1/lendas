#define v2

using UnityEngine;
using System.Collections;

#if v1
namespace Descritores_MD{
	public class Fase{
		protected int n; // Numero/ID da fase;
		protected bool locked; // Flag para marcar se esta bloqueada ou nao;
		
		// Construtor e destrutor:
		//DescritorFase(){}
		// DescritorFase(){} // Vai um "til" na frente do nome, mas por algum motivo nao esta aparecendo;
		
		// Getters and Setters:
		public int ID{
			set{ this.n = value; }
			get{ return this.n; }
		}
		public bool Bloqueado{
			set{this.locked = value;}
			get{return this.locked;}
		}
	}
}
#endif


public class Btn_Fase : MonoBehaviour 
{
#if v1
	public Descritores_MD.Fase fase;
#endif
	
#if v2
    

	public bool bloqueado = true;
    private bool jaDefiniuBloqueio = false;
	public int ID;
	public float tempo;
	Material sprite;//OTSprite sprite;
	Status refScriptStatus = null; // referencia ao script status
	MelhoresTempos refMelhoresTempos = null;
    public bool selecionado = false;

    Color corBotaoSelecionado = new Color(255, 0, 0, 0);
    Color corBotaoNSelecionado = new Color(255, 255, 255, 255);

    /*OTSprite*/Material refSeuSprite = null;

   // Camera camera1 = null;
   
#endif
	
	// Use this for initialization
	void Start () {
#if v1
		fase = new Descritores_MD.Fase();
#endif
#if v2
        //if (camera1 == null)
        //    camera1 = GameObject.Find("Main Camera").GetComponent<Camera>();

        if (refScriptStatus == null)
            refScriptStatus = GameObject.Find("Status").GetComponent<Status>();
        
        if (refSeuSprite == null)
			refSeuSprite = this.gameObject.renderer.material;//refSeuSprite = this.GetComponent<OTSprite>();
        
        if (refMelhoresTempos == null)
            refMelhoresTempos = GameObject.Find("MelhoresTempos").GetComponent<MelhoresTempos>();

		char c = this.gameObject.name[this.gameObject.name.Length-1]; // Pega a última posição;

        string str = "";
		str += c;
		this.ID = int.Parse(str); // Logica funciona de 0 a 9;

		// Carregar informações do nível:
		str = "lvl";
		str += this.ID;

        if (!jaDefiniuBloqueio)
        {
            int tmp = PlayerPrefs.GetInt((str + "b")); // Todos os levels tem sua chave salva como lvl+ID+info. Ex: lvl1b (Pega o bool para saber se está bloqueado ou não);
            if (tmp == 0)
            {
                if (this.ID != 1)
                {//botao 1 é sempre liberado
                    this.bloqueado = true;
                    jaDefiniuBloqueio = true;
                }
            }
            else if (tmp == 1)//se nao esta bloqueado
            {
                this.bloqueado = false;
                jaDefiniuBloqueio = true;
            }

            //LiberarTodasFasesDebug();
        }

        /*else
            Debug.LogError("Não foi lida uma das informações de arquivo!");
         */

		//this.tempo = PlayerPrefs.GetFloat((str+"t")); // Retorna o tempo;
		//if(this.tempo <= 0.0f){}// Tempo não exite;

		// Imagens:
		//this.sprite = this.gameObject.GetComponent<OTSprite>();

        this.sprite = this.gameObject.renderer.material;

        if (!selecionado)//Se  o botão nao estiver selecionado, dai tu define a imagem padrao baseado se o botao esta liberado ou nao
        {
            GameObject obj = GameObject.Find("Images");
            ImagesScript iscript = obj.GetComponent<ImagesScript>();

            if (this.bloqueado)
            {
                this.sprite.mainTexture = (Texture2D)iscript.imagem[0];
            }
            else//trocar dependente de qual sprite
            {

                //this.sprite.image = iscript.imagem[1];
                this.sprite.mainTexture = (Texture2D)iscript.imagem[this.ID];
            }

        }

        


#endif
	}

    //Somente para testar
    private void LiberarTodasFasesDebug()
    {
        this.bloqueado = false;
        jaDefiniuBloqueio = true;
    }

    public bool GetJaDefiniuBloqueio()
    {
        return jaDefiniuBloqueio;
    }

    public void DefinirBloqueio()
    {
        if (!jaDefiniuBloqueio)
        {
            char c = this.gameObject.name[this.gameObject.name.Length - 1]; // Pega a última posição;

            string str = "";
            str += c;
            this.ID = int.Parse(str); // Logica funciona de 0 a 9;

            // Carregar informações do nível:
            str = "lvl";
            str += this.ID;
            
            int tmp = PlayerPrefs.GetInt((str + "b")); // Todos os levels tem sua chave salva como lvl+ID+info. Ex: lvl1b (Pega o bool para saber se está bloqueado ou não);
            if (tmp == 0)
            {
                if (this.ID != 1)
                {//botao 1 é sempre liberado
                    this.bloqueado = true;
                    jaDefiniuBloqueio = true;
                }
            }
            else if (tmp == 1)//se nao esta bloqueado
            {
                this.bloqueado = false;
                jaDefiniuBloqueio = true;
            }
        }

        //LiberarTodasFasesDebug();

    }

    public void SetRefStatus(Status statuss)
    {
        refScriptStatus = statuss;
    }

    public void SelecionarBotao()
    {
        if (refScriptStatus == null)
        {
            refScriptStatus = GameObject.Find("Status").GetComponent<Status>();
        }
        
        //remove a referencia do antigo
        refScriptStatus.RemoverSelecaoBotaoVelho();
        
        this.selecionado = true;

        
        
        refScriptStatus.IDFase = this.ID;//Atribui o nivel a que o botao pertence ao Status
        
        //pinta o botao com a cor de seleção
        if (refSeuSprite == null)
        {
			refSeuSprite = renderer.material;//this.GetComponent<OTSprite>();
            refSeuSprite.color = corBotaoSelecionado;
        }
        else
        {
            refSeuSprite.color = corBotaoSelecionado;
        }

        //Variaveis de controle do status
        refScriptStatus.ultimoBotaoSelID = this.ID;
        refScriptStatus.refUltimoBotaoSel = this.gameObject;


        refScriptStatus.SetTempoJogobasedOnFase();

        //atualiza os melhores tempos

        if (refMelhoresTempos == null)
        {
            refMelhoresTempos = GameObject.Find("MelhoresTempos").GetComponent<MelhoresTempos>();
        }

        refMelhoresTempos.TrocouFaseSelecionada();
        
    }

    public void DesselecionarBotao()
    {
        selecionado = false;
		renderer.material.color = corBotaoNSelecionado;//refSeuSprite.color = corBotaoNSelecionado;

        //if (refScriptStatus == null)
        //{
        //    refScriptStatus = GameObject.Find("Status").GetComponent<Status>();
        //}

        //refScriptStatus.refUltimoBotaoSel = null;
        //refScriptStatus.ultimoBotaoSelID = -1;
    }

	// Update is called once per frame
	void Update () 
    {
        
        //if (Input.touchCount >= 1)
        //{
        //    foreach (Touch touch in Input.touches)
        //    {
        //        if (touch.phase == TouchPhase.Began)
        //        {
        //            Ray ray = camera1.ScreenPointToRay(touch.position);
        //            RaycastHit hit;

        //            if (Physics.Raycast(ray, out hit))
        //            {
        //                switch (hit.collider.tag)
        //                {
        //                    case "Btn":
        //                        {
        //                            // Se nao estiver bloqueado:
        //                            if (!this.bloqueado)
        //                            {
        //                                //refScriptStatus.SetOldFase(refScriptStatus.IDFase);
        //                                //SelecionarBotao();

        //                                if (refScriptStatus == null)
        //                                {
        //                                    refScriptStatus = GameObject.Find("Status").GetComponent<Status>();
        //                                }

        //                                refScriptStatus.IDFase = this.ID;
        //                                refScriptStatus.SelecionarBotaoFaseDentroStatus();

        //                                //refScriptStatus.SetTempoJogobasedOnFase();

        //                                ////atualiza os melhores tempos
        //                                //if (refMelhoresTempos == null)
        //                                //{
        //                                //    refMelhoresTempos = GameObject.Find("MelhoresTempos").GetComponent<MelhoresTempos>();
        //                                //}

        //                                //refMelhoresTempos.TrocouFaseSelecionada();

        //                            }
        //                            break;
        //                        }
        //                 }

        //            }
        //        }
        //    }
        //}

        if (!refScriptStatus.isAndroid
            && !refScriptStatus.isiOS)//se nao for nenhum dos 2
        {
            if (OT.Clicked(this.gameObject, 0))
            {
                // Se nao estiver bloqueado:
                if (!this.bloqueado)
                {
                    if (refScriptStatus == null)
                    {
                        refScriptStatus = GameObject.Find("Status").GetComponent<Status>();
                    }

                    refScriptStatus.IDFase = this.ID;
                    refScriptStatus.SelecionarBotaoFaseDentroStatus();
                }
            }
        
        }


//        if(OT.Clicked(this.gameObject, 0) || OT.Touched(this.gameObject))
//        { // Verifica se colidiu com mouse ou touch;
//            //Debug.Log("Clicou no botao para entrar em uma fase");
//            //Application.LoadLevel("Tela_Jogo");
//// Aqui iria o codigo que confere qual e a fase e entra no jogo no ponto inicial especifico.
//#if v2
						
//            // Se nao estiver bloqueado:
//            if(!this.bloqueado)
//            {
				
//                //refScriptStatus.SetOldFase(refScriptStatus.IDFase);

//                //SelecionarBotao();

//                if (refScriptStatus == null)
//                {
//                    refScriptStatus = GameObject.Find("Status").GetComponent<Status>();
//                }

//                refScriptStatus.IDFase = this.ID;
//                refScriptStatus.SelecionarBotaoFaseDentroStatus();

//                //refScriptStatus.SetTempoJogobasedOnFase();

//                ////atualiza os melhores tempos
//                //if (refMelhoresTempos == null)
//                //{
//                //    refMelhoresTempos = GameObject.Find("MelhoresTempos").GetComponent<MelhoresTempos>();
//                //}

//                //refMelhoresTempos.TrocouFaseSelecionada();

//            }
			
//#endif
//        }


	}

    public void PassarStatus(Status statusscript)
    {
        refScriptStatus = statusscript;
    
    }

}
