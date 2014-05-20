using UnityEngine;
using System.Collections;

public class SeguirLinkScript : MonoBehaviour {
	public string link;

    Camera camera1 = null;

    bool isAndroid = false;
    bool isiOS = false;

    void Start()
    {
        if (camera1 == null)
            camera1 = GameObject.Find("Main Camera").GetComponent<Camera>();

        if (Application.platform == RuntimePlatform.Android)
        {
            isAndroid = true;
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            isiOS = true;
        }
    }

	// Update is called once per frame
	void Update () 
    {

        if (isAndroid
            || isiOS)
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
                                case "Button_Ampli":
                                    {
                                        Application.OpenURL(@link);
                                    }
                                    break;

                                case "Button_Atomic":
                                    {
                                        Application.OpenURL(@link);
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
            if(OT.Clicked(this.gameObject) )
            Application.OpenURL(@link);
        }



		
	}
}
