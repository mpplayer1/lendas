using UnityEngine;
using System.Collections;

public class Btn6 : MonoBehaviour {
	
	ControladorBackground c;
	public bool esq;

    Camera camera1 = null;
    Status refScriptStatus = null;

	// Use this for initialization
	void Start () 
    {
		GameObject obj = GameObject.Find("Fundo");
		c = obj.GetComponent<ControladorBackground>();


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
                                case "ButtonLeft":
                                    {
                                        c.previousImg();
                                        break;
                                    }
                                case "ButtonRight":
                                    {
                                        if (!esq)
                                            c.nextImg();

                                        break;
                                    }
                            }

                        }
                    }
                }
            }
        }
        else//PC
        {
            if (OT.Clicked(this.gameObject, 0) )
            { // Verifica se colidiu com mouse 
                if (!esq)
                    c.nextImg();
                else
                    c.previousImg();
            }
        }
        
	}
}
