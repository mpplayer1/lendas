using UnityEngine;
using System.Collections;

public class ControleCarroTutorial : MonoBehaviour {
	//Taxi
	public Vector3 orientacao;
	bool controleDoJogador = true, finalizouRotacaoCam = true; // Troca de ruas;
	private short fatorMC = 2; // Fator de incrementaçao na rotaçao de camera e carro respectivamente;
	bool faltaAjustar; // Auxiliar para ajustar câmera;
	//Vector3 orintacaoAlinhar; // Quando o jogo tiver o controle do carro este será o guia de direção para o carro e câmera;
	private float fatorLimite = 45f; // Por padrao era 45º;
	private float anguloLimiteD;
	private float anguloLimiteE;
	private short orientacaoAtual; // Orientação oposta da câmera, para o carro andar para cima sempre, sendo assim o valor e a rotaçao da camera + 180º;

	public float sensibilidade, speed;

	//Dispositivo
	private bool android = false;
	private int halfscreenWidth;
	
	public void AtualizarLimites(Transform transformDaCameraDaCena)
	{ // Atualiza os limítes de rotação em relação a câmera; OBS: a própria câmera chama esta função quando sua orientação for alterada (Ex: 0º (Cima), 90º (Direita), 180º (Baixo) e 270º (Esquerda));
		
		//Limitadores de angulo de rotação
		// Limitador direita:
		anguloLimiteD = transformDaCameraDaCena.eulerAngles.y + fatorLimite; // São os valores normalizados;
		if(transformDaCameraDaCena.eulerAngles.y - fatorLimite < 0) // Se negativo;
			anguloLimiteE = 360 + (transformDaCameraDaCena.eulerAngles.y - fatorLimite); // São os valores normalizados;
		else
			anguloLimiteE = transformDaCameraDaCena.eulerAngles.y - fatorLimite;
		
		// Pegar o ângulo de orientação da câmera e atualizar a variável orientacaoAtual;
		orientacaoAtual = (short)(transformDaCameraDaCena.eulerAngles.y + 180); // Sempre 180º da direçao da camera, pois ela esta apontando para baixo;
		if(orientacaoAtual >= 360)
			orientacaoAtual -= 360;
		
	}
	
	void Start() 
	{
		// Atualiza os limites da câmera:
		GameObject camT = GameObject.Find ("Main Camera");
		AtualizarLimites(camT.transform);
		//Debug.Log ("anguloLimiteD: "+anguloLimiteD+"; anguloLimiteE: "+anguloLimiteE);
		
		if (Application.platform == RuntimePlatform.Android)
		{
			android = true;
		}
		
		halfscreenWidth = (int)Screen.width/2;
		
	}
	// Ajusta o ângulo para que fique dentro dos limítes padrões:
	private static float AjustarAngulo(float ang){
		if(ang > 45 && ang < 135)
			ang = 90.0f;
		else if(ang > 135 && ang < 225)
			ang = 180.0f;
		else if(ang > 225 && ang < 315)
			ang = 270.0f;
		else
			ang = 0.0f;
		return ang;
	}
	private static int AjustarAngulo(int ang){
		if(ang > 45 && ang < 135)
			ang = 90;
		else if(ang > 135 && ang < 225)
			ang = 180;
		else if(ang > 225 && ang < 315)
			ang = 270;
		else
			ang = 0;
		return ang;
	}
	private static short AjustarAngulo(short ang){
		if(ang > 45 && ang < 135)
			ang = 90;
		else if(ang > 135 && ang < 225)
			ang = 180;
		else if(ang > 225 && ang < 315)
			ang = 270;
		else
			ang = 0;
		return ang;
	}
	
	//Funcao para rotacionar o carro e aplicar o movimento
	private void RotAndMove()
	{
		Vector2 positionTouch;
		bool clickEsq = false;
		bool clickDir = false;
		
		if (android)
		{
			positionTouch = Input.GetTouch(0).position;
			
			if (positionTouch.x >= halfscreenWidth)//Se o touch for na direita da tela
				clickDir = true;
			else 
				clickEsq = true;
		}
		else
		{
			//positionTouch = Input.mousePosition;
			clickDir = Input.GetMouseButton(1);
			clickEsq = Input.GetMouseButton(0);
		}
		
		//----Orientacao-------------------------------------------------
		orientacao = transform.eulerAngles;
		
		//		// Testes com RAYCAST:
		//		var forward = this.transform.TransformDirection(new Vector3(-1,0,1)) * 5; // Agora sim!
		//		Debug.DrawRay(this.transform.position, forward);//Debug.DrawLine(this.transform.position, new Vector3(this.transform.position.x, this.transform.position.y, (this.transform.position.z + 10)));
		//		RaycastHit rayT;
		//		if(Physics.Raycast(this.transform.position, new Vector3(-1,0,1), out rayT, 5)){ // a palavra chave "out" é semelhante a "ref";
		//			Debug.Log ("Raycast: "+rayT.transform.name); // Retorna o nome do objeto em que colidiu;
		//			Debug.Break();
		//		}
		
		//		RaycastHit ray;
		//		if(Physics.Raycast(this.transform.position, /*Vector3.forward*/new Vector3(0,0,1), out ray, 10)){ // Raycast que impede giro pois já está na calçada; // Se colidindo com prédios;
		//			Debug.Log ("Direita!!! Raycast");
		//			Debug.Break();
		//		}
		
		//if (positionTouch.x >= halfscreenWidth)//Toque na direita da tela
		if (clickEsq)
		{
			float orietemp = orientacao.z + ((1 * Time.deltaTime) * sensibilidade);
			
			// Limitador:
			if (orietemp > anguloLimiteD && orietemp < anguloLimiteD + 5)
			{
				orietemp = anguloLimiteD;
			}
			
			orientacao.z = orietemp;
			//orientacao.y += (1 * Time.deltaTime) * sensibilidade;
		}
		else if (clickDir)//Esquerda
		{
			float orietemp = orientacao.z + ((-1 * Time.deltaTime) * sensibilidade);
			
			// Limitador:
			if (orietemp < anguloLimiteE && orietemp > anguloLimiteE - 5)
			{
				orietemp = anguloLimiteE;
			}
			
			orientacao.z = orietemp;
			
			//orientacao.y += (-1 * Time.deltaTime) * sensibilidade;	
		}
		else // Alinhar com a rua;
		{
			//Debug.Log("orientacao.y="+orientacao.y+"; orientacaoAtual="+orientacaoAtual);
			short oTemp = (short)(orientacaoAtual + 180);
			if(oTemp >= 360){
				oTemp -= 360;
			}
			if((orientacao.z <= oTemp+5) && (orientacao.y >= oTemp-5)){ // Se a orientação estiver quase correta, igualar ela com a orientação Atual;
				orientacao.z = oTemp;
			}
			else
			{
				float orietemp = orientacao.z;
				
				if(orientacaoAtual == 0){ // Baixo;
					if(orietemp >= orientacaoAtual && orietemp <= 180){
						orietemp = orientacao.z + ((1 * Time.deltaTime) * sensibilidade);
						
					} else{ // Estando nos quadrantes opostos, entre 180 e 360;
						orietemp = orientacao.z + ((-1 * Time.deltaTime) * sensibilidade);
						
					}
				} else if(orientacaoAtual == 90){ // Esquerda;
					if(orietemp >= orientacaoAtual && orietemp <= 270){
						orietemp = orientacao.z + ((1 * Time.deltaTime) * sensibilidade);
					} else{
						orietemp = orientacao.z + ((-1 * Time.deltaTime) * sensibilidade);
					}
				} else if(orientacaoAtual == 180){ // Cima;
					if(orietemp >= orientacaoAtual && orietemp <= 360){
						orietemp = orientacao.z + ((1 * Time.deltaTime) * sensibilidade);
					} else{
						orietemp = orientacao.z + ((-1 * Time.deltaTime) * sensibilidade);
					}
				} else if(orientacaoAtual == 270){ // Direita;
					if(orietemp <= orientacaoAtual && orietemp >= 90){
						orietemp = orientacao.z + ((-1 * Time.deltaTime) * sensibilidade);
					} else{
						orietemp = orientacao.z + ((1 * Time.deltaTime) * sensibilidade);
					}
				}
				
				
				orientacao.z = orietemp;
			}
		}
		
		transform.eulerAngles = orientacao;//Aplica a rotacao
		//-----------------------------------------------------------
		
		
//		rigidbody.velocity = new Vector3 (0,0,0);// transform.forward * speed; //Aplica a velocidade
		
//		if(rigidbody.velocity.magnitude > 0.1f)
//		{
//			transform.forward = new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z);
//		}
	}

	void FixedUpdate ()
	{
		RotAndMove();
	}
}
