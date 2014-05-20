using UnityEngine;
using System.Collections;

public class Btn_Vic_N : MonoBehaviour {

    Camera camera1 = null;

	// Use this for initialization
	void Start () 
    {
        if (camera1 == null)
            camera1 = GameObject.Find("Main Camera").GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () 
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
                            case "BtnNext":
                                {
                                    Application.LoadLevel("Tela_Fases");
                                }
                                break;
                        }
                        //hit.transform.SendMessage("Clicked");
                    }
                }
            }
        }
        
        //if (OT.Clicked(this.gameObject, 0) || OT.Touched(this.gameObject))
        //{
        //    Application.LoadLevel("Tela_Fases");
        //}
	}

    //public void OnMouseDown()
    //{
    //    Application.LoadLevel("Tela_Fases");
    //}

    //public void Clicked()
    //{
    //    Application.LoadLevel("Tela_Fases");
    //}
}
