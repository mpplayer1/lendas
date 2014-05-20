using UnityEngine;
using System.Collections;

public class Velho : MonoBehaviour {

 
    bool continuarWalk;
    bool stoppedToComplain;
    bool iniciouCaminhada;//para nao iniciar duas coroutines ou mais
    bool complainedOnce;//se ja reclamou 1 vez
    bool ativouDelayParada;

    bool scriptPertencePai;
    GameObject objfilhoQestaPai;//guard ao obj filho quando o script esta no pai
    GameObject objPaiVelhoT;
  

	
	void Start () 
    {
        stoppedToComplain = false;
        iniciouCaminhada  = false;
        complainedOnce = false;
        ativouDelayParada = false;

        if (gameObject.name == "Velho")//Se esse script esta no objeto pai
        {
            scriptPertencePai = true;

            foreach (Transform child in gameObject.transform)
            {
                objfilhoQestaPai = child.gameObject;
            }
        }
        else//se esta no filho
        {
            objPaiVelhoT = gameObject.transform.parent.gameObject;//pega o objeto pai e guarda
            scriptPertencePai = false;
        }

	}

    public void StopAndComplain()
    {
        stoppedToComplain = true;
    }

    //public void DesligarTriggerVelho()
    //{
    //    if (!scriptPertencePai)
    //    {
    //        gameObject.collider.enabled = false;
    //    }
    //}


    IEnumerator Walk( int tipo)//Só deve rodar no objeto filho
    {
        float quantoMover = 0.05f;
        float tempoParado = 1.0f;//tempo que o velho fica parado reclamando
        float tempoEachWait = 0.1f;
        float tempoDelayAteParar = 1.7f;

        continuarWalk = true;
        Transform transfdoPaiVelhoT = gameObject.transform.parent.gameObject.transform;
     
        while (continuarWalk)
        {
            if (!stoppedToComplain)//se nao deve ficar parado e reclamar
            {
                if (tipo == 1)//diferentes tipos de velho (para diferentes ruas)
                {
                    //Anda em Z posi
                    transfdoPaiVelhoT.transform.position = new Vector3(transfdoPaiVelhoT.transform.position.x, transfdoPaiVelhoT.transform.position.y, (transfdoPaiVelhoT.transform.position.z + quantoMover));
                }
                else if (tipo == 2)//diferentes tipos de velho (para diferentes ruas)
                {
                    //Anda em X neg
                    transfdoPaiVelhoT.transform.position = new Vector3(transfdoPaiVelhoT.transform.position.x - quantoMover, transfdoPaiVelhoT.transform.position.y, (transfdoPaiVelhoT.transform.position.z));
                }
                else if (tipo == 3)//diferentes tipos de velho (para diferentes ruas)
                {
                    //Anda em X posi
                    transfdoPaiVelhoT.transform.position = new Vector3(transfdoPaiVelhoT.transform.position.x + quantoMover, transfdoPaiVelhoT.transform.position.y, (transfdoPaiVelhoT.transform.position.z));
                }
                else if (tipo == 4)//diferentes tipos de velho (para diferentes ruas)
                {
                    //Anda em Z neg
                    transfdoPaiVelhoT.transform.position = new Vector3(transfdoPaiVelhoT.transform.position.x, transfdoPaiVelhoT.transform.position.y, (transfdoPaiVelhoT.transform.position.z - quantoMover));
                }

                yield return new WaitForSeconds(tempoEachWait);

                if (ativouDelayParada)//se o delay de parada esta ativo, o velho anda mais um pouquinho e dai para - resolve problema dlee nao subir na calçada
                {
                    tempoDelayAteParar -= tempoEachWait;//msm tempo do wait for seconds

                    if (tempoDelayAteParar <= 0)//quando acabar o tempo ele para de andar definitivamente
                    {
                        continuarWalk = false;
                    }
                }

           
            }
            else//se o velho deve reclamar
            {
                if (!complainedOnce)
                {
                    complainedOnce = true;
                    //velho para de andar, toca animacao
                    GameObject tempPai = gameObject.transform.parent.gameObject;
                    Texture2D tempT = tempPai.renderer.material.mainTexture as Texture2D;

                    //toca o som do velho
                    GameObject.FindGameObjectWithTag("Player").GetComponent<ControleCarro>().PlayOldManSound();

                    //Animacao
                    tempPai.renderer.material.mainTexture = Resources.Load("Elementos/Velho2") as Texture2D;
                   
                    yield return new WaitForSeconds(tempoParado);//espera certo tempo parado
                   
                    tempPai.renderer.material.mainTexture = tempT;//volta a textura original


                    stoppedToComplain = false;//Volta a andar

                    yield return new WaitForSeconds(tempoEachWait);
                }
                else
                {
                    stoppedToComplain = false;
                }

                
            }

            
        }

        yield return new WaitForSeconds(10.0f);//Aguarda 10 sec e destroi o objeto

        Destroy(objPaiVelhoT.transform.parent.gameObject);

    
    }

    public void IniciarCaminhada()
    {
        if (!iniciouCaminhada)//se nao comecou a caminhar
        {
            if (this.gameObject.tag == "PdVelhoC")//PdVelhoC - velho nas ruas deitadas - anda poapra cima em Z
            {
                StartCoroutine(Walk(1));
            }
            else if (this.gameObject.tag == "PdVelhoE")
            {
                StartCoroutine(Walk(2));
            }
            else if (this.gameObject.tag == "PdVelhoD")
            {
                StartCoroutine(Walk(3));
            }
            else if (this.gameObject.tag == "PdVelhoB")
            {
                StartCoroutine(Walk(4));
            }

            iniciouCaminhada = true;
        }
    }

    public void PararWalk()
    {
        ativouDelayParada = true;
        //desliga o trigger
        gameObject.collider.enabled = false;

        //Quando o nodo filho do velho se desliga, garante que o carro volte a ter seu movimento
        GameObject.FindGameObjectWithTag("Player").GetComponent<ControleCarro>().DestravarCarroVelhoPd();

    }

    void OnTriggerEnter(Collider objColisor)
    {
        if (scriptPertencePai)
        {
                if (objColisor.gameObject.tag == "PdVelhoF" //) //DestinoFinal
                     && gameObject.tag == "PdVelhoSelf")
                {
                    gameObject.collider.enabled = false;
                    objfilhoQestaPai.GetComponent<Velho>().PararWalk();//para animacao que esta no filho
                    
                }
           
        }
    }
    
}
