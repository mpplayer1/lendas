using UnityEngine;
using System.Collections;

public class Gato : MonoBehaviour 
{

    bool continuarWalk;
    bool stoppedToComplain;
    bool iniciouCaminhada;//para nao iniciar duas coroutines ou mais
    bool complainedOnce;//se ja reclamou 1 vez
    bool ativouDelayParada;

    bool scriptPertencePai;
    GameObject objfilhoQestaPai;//guard ao obj filho quando o script esta no pai
    GameObject objPaiGatoT;



    void Start()
    {
        stoppedToComplain = false;
        iniciouCaminhada = false;
        complainedOnce = false;
        ativouDelayParada = false;

        if (gameObject.name == "Gato")//Se esse script esta no objeto pai
        {
            scriptPertencePai = true;

            foreach (Transform child in gameObject.transform)
            {
                objfilhoQestaPai = child.gameObject;
            }
        }
        else//se esta no filho
        {
            objPaiGatoT = gameObject.transform.parent.gameObject;//pega o objeto pai e guarda
            scriptPertencePai = false;
        }

    }

    public void StopAndComplain()
    {
        stoppedToComplain = true;
    }

    IEnumerator Walk(int tipo)//Só deve rodar no objeto filho
    {
        float quantoMover = 0.1f;
        float velocidadeAssustado = 0.25f;
        float tempoMoverAssustado = 0.8f;//tempo que o gato anda mais rapido
        bool assustado = false;

        float tempoEachWait = 0.1f;
        float tempoParado = 0.5f;//tempo que o Gato fica parado assustado

        float tempoDelayAteParar = 1.1f;
        
        continuarWalk = true;
        Transform transfdoPaiGatoT = gameObject.transform.parent.gameObject.transform;

        while (continuarWalk)
        {
            if (!stoppedToComplain)//se nao deve ficar parado e reclamar
            {
                if (assustado)
                {
                    quantoMover = velocidadeAssustado;
                }


                if (tipo == 1)//diferentes tipos de Gato (para diferentes ruas)
                {
                    //Anda em Z posi
                    transfdoPaiGatoT.transform.position = new Vector3(transfdoPaiGatoT.transform.position.x, transfdoPaiGatoT.transform.position.y, (transfdoPaiGatoT.transform.position.z + quantoMover));
                }
                else if (tipo == 2)//diferentes tipos de Gato (para diferentes ruas)
                {
                    //Anda em X neg
                    transfdoPaiGatoT.transform.position = new Vector3(transfdoPaiGatoT.transform.position.x - quantoMover, transfdoPaiGatoT.transform.position.y, (transfdoPaiGatoT.transform.position.z));
                }
                else if (tipo == 3)//diferentes tipos de Gato (para diferentes ruas)
                {
                    //Anda em X posi
                    transfdoPaiGatoT.transform.position = new Vector3(transfdoPaiGatoT.transform.position.x + quantoMover, transfdoPaiGatoT.transform.position.y, (transfdoPaiGatoT.transform.position.z));
                }
                else if (tipo == 4)//diferentes tipos de Gato (para diferentes ruas)
                {
                    //Anda em Z neg
                    transfdoPaiGatoT.transform.position = new Vector3(transfdoPaiGatoT.transform.position.x, transfdoPaiGatoT.transform.position.y, (transfdoPaiGatoT.transform.position.z - quantoMover));
                }

                yield return new WaitForSeconds(tempoEachWait);

                if (assustado)
                {
                    tempoMoverAssustado -= tempoEachWait;

                    if (tempoMoverAssustado <= 0)
                    {

                        assustado = false;
                        quantoMover = 0.1f;
                    }
                }


                if (ativouDelayParada)//se o delay de parada esta ativo, o Gato anda mais um pouquinho e dai para - resolve problema dlee nao subir na calçada
                {
                    tempoDelayAteParar -= tempoEachWait;//msm tempo do wait for seconds

                    if (tempoDelayAteParar <= 0)//quando acabar o tempo ele para de andar definitivamente
                    {
                        continuarWalk = false;
                    }
                }


            }
            else//se o Gato deve reclamar
            {
                if (!complainedOnce)
                {
                    complainedOnce = true;
                    //Gato para de andar, toca animacao
                    GameObject tempPai = gameObject.transform.parent.gameObject;
                    Texture2D tempT = tempPai.renderer.material.mainTexture as Texture2D;

                    //toca o som do Gato
                    GameObject.FindGameObjectWithTag("Player").GetComponent<ControleCarro>().PlayCatSound();

                    //Animacao
                    tempPai.renderer.material.mainTexture = Resources.Load("Elementos/Gato2") as Texture2D;
                    assustado = true;

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

        Destroy(objPaiGatoT.transform.parent.gameObject);


    }

    public void IniciarCaminhada()
    {
        if (!iniciouCaminhada)//se nao comecou a caminhar
        {
            string tagPu = this.gameObject.transform.parent.gameObject.transform.parent.tag;//Pega a tag do pai do pai desse objeto (estou no GatoT, subo 1 nivel, GatoSelf, subo 1 nivel, topo prefab

            if (tagPu == "PdGatoC")//PdGatoC - Gato nas ruas deitadas - anda poapra cima em Z
            {
                StartCoroutine(Walk(1));
            }
            else if (tagPu == "PdGatoE")
            {
                StartCoroutine(Walk(2));
            }
            else if (tagPu == "PdGatoD")
            {
                StartCoroutine(Walk(3));
            }
            else if (tagPu == "PdGatoB")
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

        //Quando o nodo filho do Gato se desliga, garante que o carro volte a ter seu movimento
        GameObject.FindGameObjectWithTag("Player").GetComponent<ControleCarro>().DestravarCarroGatoPd();

    }

    void OnTriggerEnter(Collider objColisor)
    {
        if (scriptPertencePai)
        {
            if (objColisor.gameObject.tag == "PdGatoF" //) //DestinoFinal
                 && gameObject.tag == "PdGatoSelf")
            {
                gameObject.collider.enabled = false;
                objfilhoQestaPai.GetComponent<Gato>().PararWalk();//para animacao que esta no filho

            }

        }
    }
}