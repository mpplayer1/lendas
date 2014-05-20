using UnityEngine;
using System.Collections;

public class Btn1 : MonoBehaviour {

    Camera camera1 = null;
    Status refScriptStatus = null;
   
	// Use this for initialization
	void Start () 
    {
		// Resumir sons:
		AudioListener.pause = false;

        if (camera1 == null)
            camera1 = GameObject.Find("Main Camera").GetComponent<Camera>();

        if (refScriptStatus == null)
            refScriptStatus = GameObject.Find("Status").GetComponent<Status>();
	}
	
	// Update is called once per frame
	void Update () 
    {
        //teste
        //var myCamera : Camera;
        //var hit : RaycastHit;
        //var ray : Ray;

        // if(Input.touchCount == 1) 
        // {
        //    RaycastHit hit;

        //    Touch touch = Input.touches[0];
        //    Ray raio = camera.ScreenPointToRay(Input.touches[0].position);
            
        //    if(touch.phase == TouchPhase.Ended && Physics.Raycast(raio.origin, raio.direction, hit)){
 
        //    switch(hit.collider.name)
        //    {
        //        case "object01":
        //        Debug.Log("Object01 tapped");
        //        //things
        //        break;
        //        case "object02":
        //        Debug.Log("Object02 tapped");
        //        //things
        //        break;
        //    }
        //}

        //if (Input.touchCount == 1)
        //{
        //    // touch on screen
        //    if (Input.GetTouch(0).phase == TouchPhase.Began)
        //    {
        //        Ray ray = camera.ScreenPointToRay(Input.GetTouch(0).position);
        //        RaycastHit hit = new RaycastHit();

        //        if (Physics.Raycast(ray, out hit))
        //        {
        //            switch (hit.collider.name)
        //            {
        //                case "ButtonPlay":
        //                    Application.LoadLevel("Tela_Tutorial");
        //                    break;
        //            }
        //        }

        //        //    moving = Physics.Raycast (ray, out hit);

        //        //    if(moving)
        //        //{
        //        //    go = hit.transform.gameObject;
        //        //    Debug.Log("Touch Detected on : "+go.name);
        //        //}

        //    }
        //}

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
                                        if (Application.loadedLevelName == "Tela_Fases")
                                        {
                                            //Status refScriptStatus = GameObject.FindGameObjectWithTag("Status").GetComponent<Status>();
                                            
                                            if (refScriptStatus == null)
                                                refScriptStatus = GameObject.FindGameObjectWithTag("Status").GetComponent<Status>();
                                            
                                            refScriptStatus.ultimoBotaoSelID = -1;
                                            refScriptStatus.refUltimoBotaoSel = null;
                                        }

                                        if (Application.loadedLevelName == "Tela_Principal")
                                        {
                                            Application.LoadLevel("Tela_Tutorial");
                                        }
                                    }
                                    break;

                                case "ButtonBack":
                                    {
                                        if (Application.loadedLevelName == "Tela_Fases")
                                        {
                                            //Status refScriptStatus = GameObject.FindGameObjectWithTag("Status").GetComponent<Status>();
                                            if (refScriptStatus == null)
                                                refScriptStatus = GameObject.FindGameObjectWithTag("Status").GetComponent<Status>();

                                            refScriptStatus.ultimoBotaoSelID = -1;
                                            refScriptStatus.refUltimoBotaoSel = null;
                                        }

                                        Application.LoadLevel("Tela_Tutorial");
                                    }
                                    break;
                            }
                            //hit.transform.SendMessage("Clicked");
                        }
                    }
                }

            }
        }
        else//pc?
        {
            if (OT.Clicked(this.gameObject, 0))
            {
                if (Application.loadedLevelName == "Tela_Fases")
                {
                    if (refScriptStatus == null)
                        refScriptStatus = GameObject.FindGameObjectWithTag("Status").GetComponent<Status>();

                    refScriptStatus.ultimoBotaoSelID = -1;
                    refScriptStatus.refUltimoBotaoSel = null;
                }

                Application.LoadLevel("Tela_Tutorial");
            }
        }
        //if (OT.Clicked(this.gameObject, 0) || OT.Touched(this.gameObject))
        //{ // Verifica se colidiu com mouse ou touch;

        //    if (Application.loadedLevelName == "Tela_Fases")
        //    {
        //        Status refScriptStatus = GameObject.FindGameObjectWithTag("Status").GetComponent<Status>();
        //        refScriptStatus.ultimoBotaoSelID = -1;
        //        refScriptStatus.refUltimoBotaoSel = null;
        //    }

        //    Application.LoadLevel("Tela_Tutorial");
        //}
		



//		if(OT.Over(this.gameObject)){// Hover (pairando sobre);
//			Debug.Log("Sobre o botao!");//...
//		} else{
//			Debug.Log("Fora do botao!");
//		}
	}

   
}
