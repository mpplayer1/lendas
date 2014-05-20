using UnityEngine;
using System.Collections;

public class MelhoresTempos : MonoBehaviour 
{
    //Skin do relogio digital para a gui
    private GUISkin SkinGUIDigital;

	bool ShowTimer = false;
	private System.TimeSpan displayPlayer0, displayPlayer1, displayPlayer2;
	Status refScriptStatus = null;
	bool[] mostrarmelhorTempo = new bool[3] {false,false,false};

    bool primeiravez = true;

    private float optimalHeight;
    private float optimalWidth;
    private Vector2 screen;
    private Vector2 scale;
	Rect rectTempos, rectTempos2, rectTempos3;

	// Use this for initialization
	void Start () 
	{
        //Carrega a Skin da gui
        SkinGUIDigital = Resources.Load("Font/DS-DIGIISkin") as GUISkin;

        optimalWidth = 600f;
        optimalHeight = 1024f;

        screen = new Vector2(Screen.width, Screen.height);
        scale = new Vector2(screen.x / optimalWidth, screen.y / optimalHeight);
                
        
        primeiravez = true;

        if (refScriptStatus == null)
        {
            refScriptStatus = GameObject.Find("Status").GetComponent<Status>();
        }
		mostrarmelhorTempo[0] = mostrarmelhorTempo[1] = mostrarmelhorTempo[2] = false;


		ShowTimer = true;
		// Posicionando tempos na tela:
		Camera cameraT = GameObject.Find("Main Camera").GetComponent<Camera>();
	
        Vector3 vec3RetT = cameraT.ViewportToScreenPoint(new Vector3(0.14f, 0.29f, 1f));//28
	    rectTempos = new Rect(vec3RetT.x, vec3RetT.y, 150f * scale.x, 100f * scale.y);
		
        vec3RetT = cameraT.ViewportToScreenPoint(new Vector3(0.14f, 0.325f, 1f));
		rectTempos2 = new Rect(vec3RetT.x, vec3RetT.y, 150f * scale.x, 100f * scale.y);
		
        vec3RetT = cameraT.ViewportToScreenPoint(new Vector3(0.14f, 0.36f, 1f));
		rectTempos3 = new Rect(vec3RetT.x, vec3RetT.y, 150f * scale.x, 100f * scale.y);
	}

	public void TrocouFaseSelecionada()
	{
        if (refScriptStatus == null)
        {
            refScriptStatus = GameObject.Find("Status").GetComponent<Status>();
        }
            

        
        mostrarmelhorTempo[0] = mostrarmelhorTempo[1] = mostrarmelhorTempo[2] = false;

		if (refScriptStatus.IDFase >= 1 && refScriptStatus.IDFase < 7)
		{
			float p0,p1,p2;
			p0 = refScriptStatus.GetMelhorTempo(0);
			p1 = refScriptStatus.GetMelhorTempo(1);
			p2 = refScriptStatus.GetMelhorTempo(2);

			if (p0 > 0 && p0 != refScriptStatus.tempoDuracaoFaseAtual)//Caso o tempo salvo em arquivo for maior que zero e menor que o tempo inicial que cada fase tem
			{
				mostrarmelhorTempo[0] = true;
				displayPlayer0 = System.TimeSpan.FromSeconds( p0 );
			}
			else 
            {
				mostrarmelhorTempo[0] = false;
			}

			if (p1 > 0
                && p1 != refScriptStatus.tempoDuracaoFaseAtual)
			{
				mostrarmelhorTempo[1] = true;
				displayPlayer1 = System.TimeSpan.FromSeconds( p1 );
			}
            else
            {
                mostrarmelhorTempo[1] = false;
            }

            if (p2 > 0
                && p2 != refScriptStatus.tempoDuracaoFaseAtual)
			{
				mostrarmelhorTempo[2] = true;
				displayPlayer2 = System.TimeSpan.FromSeconds( p2 );
			}
            else
            {
                mostrarmelhorTempo[2] = false;
            }




			
		}
	}

	void OnGUI()
	{
		if (ShowTimer)
		{
            GUI.skin = SkinGUIDigital;

			GUI.contentColor = new Color(0.23f,0.15f,0.10f);//59.40.26
			GUI.skin.label.fontSize = 22;
			
			
			string niceTime;

			if (mostrarmelhorTempo[0])
			{
				niceTime = string.Format("{0:D2}:{1:D2}",
				                                displayPlayer0.Minutes,
				                                displayPlayer0.Seconds);
			}
			else
				niceTime = " --.--";
			
			GUI.Label(rectTempos, niceTime);

			//Player 2
			if (mostrarmelhorTempo[1])
			{
				niceTime = string.Format("{0:D2}:{1:D2}",
				                         displayPlayer1.Minutes,
				                         displayPlayer1.Seconds);
			}
			else
				niceTime = " --.--";

			GUI.Label(rectTempos2, niceTime);//GUI.Label(new Rect(rectTempos.x, rectTempos.y + 1f, rectTempos.width, rectTempos.height), niceTime);


			//Player 3
			if (mostrarmelhorTempo[2])
			{
				niceTime = string.Format("{0:D2}:{1:D2}",
				                         displayPlayer2.Minutes,
				                         displayPlayer2.Seconds);
			}
			else
				niceTime = " --.--";

			GUI.Label(rectTempos3, niceTime);


		}
		
		
	}


	void Update () 
    {
        if (primeiravez)
        {
            if (refScriptStatus.IDFase != 0)
            {
                refScriptStatus.SetTempoJogobasedOnFase();
                TrocouFaseSelecionada();
            }
            primeiravez = false;
        }
	

	}
}
