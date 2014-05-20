using UnityEngine;
using System.Collections;

public class Btn2 : MonoBehaviour {


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
        /*
		if(OT.Clicked(this.gameObject, 0) || OT.Touched(this.gameObject))
        { // Verifica se colidiu com mouse ou touch;
			Application.LoadLevel("Tela_Creditos");
		}
         * */

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
                                case "ButtonCredits":
                                    Application.LoadLevel("Tela_Creditos");
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
                Application.LoadLevel("Tela_Creditos");
            }
        }


	}
}
