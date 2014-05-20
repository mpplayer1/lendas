using UnityEngine;
using System.Collections;

public class Btn5 : MonoBehaviour {
	//public GameObject prefab;
	// Temporários:
	public Texture2D imgLoading;
	bool desenhaLoading = false;
	// Fim dos temporários;
    Camera camera1 = null;
    Status refScriptStatus = null;



	// Use this for initialization
	void Start () 
    {
        if (refScriptStatus == null)
            refScriptStatus = GameObject.Find("Status").GetComponent<Status>();

        if (camera1 == null)
            camera1 = GameObject.Find("Main Camera").GetComponent<Camera>();
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
                                        GameObject.Destroy(GameObject.Find("MusicaMenu")); // Para a musica do Menu
                                        GameObject obj = GameObject.Find("Fundo");
                                        ControladorBackground s = obj.GetComponent<ControladorBackground>();

                                        try
                                        {
                                            GameObject obj2 = GameObject.Find("Status");
                                            Status s2 = obj2.GetComponent<Status>();
                                            s2.IDPersonagem = s.ID;

                                            this.desenhaLoading = true;

											switch(s2.IDFase)//Carrega fase
											{
											case 1:
												Application.LoadLevel("Fase1");
												break;
											case 2:
												Application.LoadLevel("Fase2");
												break;
											case 3:
												Application.LoadLevel("Fase3");
												break;
											case 4:
												Application.LoadLevel("Fase4");
												break;
											case 5:
												Application.LoadLevel("Fase5");
												break;
											case 6:
												Application.LoadLevel("Fase6");
												break;
											}

                                        }
                                        catch
                                        {
                                            Debug.LogError("Status não criado!; Btn5.cs");
                                        }

                                        

                                        //Application.LoadLevel("Jogo"); // CARREGAR A CENA DO JOGO!
                                    }
                                    break;
                            }

                        }
                    }
                }
            }
        }
        else//PC
        {
            if (OT.Clicked(this.gameObject, 0) )
            { // Verifica se colidiu com mouse ou touch;
                //Application.LoadLevel("Jogo"); // CARREGAR A CENA DO JOGO!

                GameObject.Destroy(GameObject.Find("MusicaMenu")); // Para a musica do Menu

                GameObject obj = GameObject.Find("Fundo");
                ControladorBackground s = obj.GetComponent<ControladorBackground>();
                try
                {
                    GameObject obj2 = GameObject.Find("Status");
                    Status s2 = obj2.GetComponent<Status>();
                    s2.IDPersonagem = s.ID;

                    this.desenhaLoading = true;

                    switch(s2.IDFase)//Carrega fase
                    {
					case 1:
                        Application.LoadLevel("Fase1");
						break;
					case 2:
						Application.LoadLevel("Fase2");
						break;
					case 3:
						Application.LoadLevel("Fase3");
						break;
					case 4:
						Application.LoadLevel("Fase4");
						break;
					case 5:
						Application.LoadLevel("Fase5");
						break;
					case 6:
						Application.LoadLevel("Fase6");
						break;
                    }

                }
                catch
                {
                    Debug.LogError("Status não criado!; Btn5.cs");
                }

                //Application.LoadLevel("Jogo"); // CARREGAR A CENA DO JOGO!
               
            }
        }

       







	}

	void OnGUI(){
		if(desenhaLoading)
			GUI.DrawTexture(new Rect(0,0, Screen.width, Screen.height), imgLoading);
	}
}
