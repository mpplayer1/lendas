using UnityEngine;
using System.Collections;

public class FadeLogo : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        StartCoroutine( RealizarFade() );
	}

    IEnumerator RealizarFade()
    {
        bool keepLoop = true;
        float durTotal = 1.5f;

        while (keepLoop)
        {
            //Começa o fade in - até mostrar o logo
            if (durTotal > 0)
            {
                if (this.gameObject.renderer.material.color.a <= 0.99f)
                {
                    this.gameObject.renderer.material.color = new Color(1f, 1f, 1f, this.gameObject.renderer.material.color.a + 0.068f);//Aumenta o alpha, deve ir até 255
                }

                yield return new WaitForSeconds(0.15f);//tempo entre cada iteração de 
                durTotal -= 0.1f;
            }
            else
            {
                keepLoop = false;

                yield return new WaitForSeconds(1.4f);//Tempo que o logo fica visivel até começar o fade novamente


                //Começa o fade out
                bool segundoLoop = true;
                float durSegundoLoop = 1.5f;

                while (segundoLoop)
                {
                    if (durSegundoLoop > 0)
                    {
                        if (this.gameObject.renderer.material.color.a >= .0f)
                        {
                            this.gameObject.renderer.material.color = new Color(1f, 1f, 1f, this.gameObject.renderer.material.color.a - 0.068f);//Aumenta o alpha, deve ir até 255
                        }

                        yield return new WaitForSeconds(0.15f);
                        durSegundoLoop -= 0.1f;
                    }
                    else
                    {
                        segundoLoop = false;
                        yield return new WaitForSeconds(0.2f);//espera um pouco até trocar de cena

                    }

                }
            }

        }

               

        Application.LoadLevel("Tela_Principal");
    }

	// Update is called once per frame
	void Update () 
    {
	    
	}
}