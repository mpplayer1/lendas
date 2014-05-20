using UnityEngine;
using System.Collections;

public class Btn7 : MonoBehaviour {



    Camera camera1 = null;
    Status refScriptStatus = null;

	// Use this for initialization
	void Start () 
    {

        if (camera1 == null)
            camera1 = GameObject.Find("Main Camera").GetComponent<Camera>();

        if (refScriptStatus == null)
            refScriptStatus = GameObject.Find("Status").GetComponent<Status>();
	}
	
	// Update is called once per frame
	void Update () 
    {
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
                                       
                                        if (refScriptStatus == null)
                                            refScriptStatus = GameObject.Find("Status").GetComponent<Status>();

                                        ////Debug.Log (s.IDFase); Debug.Break ();
                                        //GameObject obj2 = GameObject.Find(("ButtonLevel-" + scriptStatus.IDFase));

                                        //Btn_Fase s2 = obj2.GetComponent<Btn_Fase>();

                                        //			if(s2.tempo > .0f)
                                        //				s.tempoFase = s2.tempo;
                                        //			else
                                        //				s.tempoFase = -1f; // Indica que não exite este tempo, corrida nunca foi executada;

                                        //Quando aperta o botao de play
                                        if (refScriptStatus.IDFase >= 1
                                            && refScriptStatus.IDFase <= 6)
                                        {
                                            refScriptStatus.ultimoBotaoSelID = -1;
                                            refScriptStatus.refUltimoBotaoSel = null;
                                            refScriptStatus.IDPersonagem = 0;//define o personagem como o 0 - dai nao buga entre recomeços de jogo

                                            Application.LoadLevel("Tela_Escolha");
                                        }
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
            if (OT.Clicked(this.gameObject, 0) )
            { // Verifica se colidiu com mouse ou touch;

                if (refScriptStatus == null)
                    refScriptStatus = GameObject.Find("Status").GetComponent<Status>();

                ////Debug.Log (s.IDFase); Debug.Break ();
                //GameObject obj2 = GameObject.Find(("ButtonLevel-" + scriptStatus.IDFase));

                //Btn_Fase s2 = obj2.GetComponent<Btn_Fase>();

                //			if(s2.tempo > .0f)
                //				s.tempoFase = s2.tempo;
                //			else
                //				s.tempoFase = -1f; // Indica que não exite este tempo, corrida nunca foi executada;

                //Quando aperta o botao de play
                if (refScriptStatus.IDFase >= 1
                    && refScriptStatus.IDFase <= 6)
                {
                    refScriptStatus.ultimoBotaoSelID = -1;
                    refScriptStatus.refUltimoBotaoSel = null;
                    refScriptStatus.IDPersonagem = 0;//define o personagem como o 0 - dai nao buga entre recomeços de jogo
                    Application.LoadLevel("Tela_Escolha");
                }
            }

        
        }

        




	}
}
