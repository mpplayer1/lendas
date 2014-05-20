using UnityEngine;
using System.Collections;

public class CheckTouchFase : MonoBehaviour {

    Camera camera1 = null;
    Status refScriptStatus = null; // referencia ao script status
	
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
                            switch (hit.collider.tag)
                            {
                                case "Btn":
                                    {
                                        Btn_Fase scriptBtnFase = hit.collider.gameObject.GetComponent<Btn_Fase>();

                                        // Se nao estiver bloqueado:
                                        if (!scriptBtnFase.bloqueado)
                                        {
                                            //refScriptStatus.SetOldFase(refScriptStatus.IDFase);
                                            //SelecionarBotao();

                                            if (refScriptStatus == null)
                                            {
                                                refScriptStatus = GameObject.Find("Status").GetComponent<Status>();
                                            }

                                            refScriptStatus.IDFase = scriptBtnFase.ID;
                                            refScriptStatus.SelecionarBotaoFaseDentroStatus();
                                        }

                                        break;
                                    }
                            }

                        }
                    }
                }
            }
        }//fim de android ou ios
        

	}
}