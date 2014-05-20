using UnityEngine;
using System.Collections;

public class GatoAreaT : MonoBehaviour 
{

    ScriptObjGerenciador refScriptGerente;
    ControleCarro refScriptCarro;

    void Start()
    {
        refScriptGerente = GameObject.FindGameObjectWithTag("GameController").GetComponent<ScriptObjGerenciador>();
        
    }

    void OnTriggerEnter(Collider objColisor)//detecta colisão com o carro, e dai faz o nodo pai iniciar a caminhada
    {
        if (objColisor.tag == "Player")//Se colidiu com o carro do jogador
        {
            refScriptCarro = objColisor.gameObject.GetComponent<ControleCarro>();

            //desliga seu trigger
            //gameObject.collider.enabled = false;

            //move o nodo pai
            gameObject.transform.parent.gameObject.GetComponent<Gato>().IniciarCaminhada();

            if (!refScriptCarro.GetImuneEfeitos())
            {
                //chama display do velho no gerente
                refScriptGerente.ChangeImgEventsPlane(1);
            }

            
        }
    
    }

}