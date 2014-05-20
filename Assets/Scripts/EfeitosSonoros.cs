using UnityEngine;
using System.Collections;

public class EfeitosSonoros : MonoBehaviour
{
	public AudioClip somAcelera;//Power Up Velocidade
	//public AudioSource somDerrapada;//Fazer curva
	
	public AudioClip somLixeira1;//Bate na lixeira
	public AudioClip somLixeira2;
	
	public AudioClip somParede1;//Bate em paredes
	public AudioClip somParede2;
	public AudioClip somParede3;
	public AudioClip somParede4;
	
	public AudioClip somAgua;//Passa nos liquidos
	public AudioClip somOleo;
	
	public AudioClip somPolicial;//Animais
	public AudioClip somVelho1;
	public AudioClip somVelho2;
	public AudioClip somGato;

	public AudioClip somFuelUp;
	public AudioClip somFuelOver;

	public AudioClip somZumbi;
	public AudioClip somAtaque;

	public AudioClip somGritoMulher;
	public AudioClip somGritoGuri;
	public AudioClip somGritoHomem;

	private bool ativado;
	private bool tocando;

	// Use this for initialization
	void Start ()
	{
		ativado = false;
		tocando = false;
	}

	public void TocaSom(string som)
	{
		int temp = Random.Range(0,2);
		switch(som)
		{
		case "acelera":
			audio.PlayOneShot(somAcelera, 1);
			break;
		case "velho":
			if(temp==0)
				audio.PlayOneShot(somVelho1, 5);
			else if(temp==1)
				audio.PlayOneShot(somVelho2, 5);
			break;
		case "lixo":
			if(temp==0)
				audio.PlayOneShot(somLixeira1, 5);
			else if(temp==1)
				audio.PlayOneShot(somLixeira2, 5);
			break;
		case "paredeFraco":
			if(temp == 0)
				audio.PlayOneShot(somParede3, 5);
			else if(temp == 1)
				audio.PlayOneShot(somParede4, 5);
			break;
		case "paredeForte":
			if(temp == 0)
				audio.PlayOneShot(somParede1, 5);
			else if(temp == 1)
				audio.PlayOneShot(somParede2, 5);
			break;
		case "gato":
			audio.PlayOneShot(somGato, 5);
			break;
		case "policia":
			audio.PlayOneShot(somPolicial, 5);
			break;
		case "agua":
			audio.PlayOneShot(somAgua, 5);
			break;
		case "oleo":
			audio.PlayOneShot(somOleo, 5);
			break;
		case "fuelUp":
			audio.PlayOneShot(somFuelUp, 5);
			break;
		case "fuelOver":
			audio.PlayOneShot(somFuelOver, 5);
			break;
		case "ataque":
			audio.PlayOneShot(somAtaque, 5);
			break;
		case "zumbi":
			audio.PlayOneShot(somZumbi, 5);
			break;
		case "guri":
			audio.PlayOneShot(somGritoGuri, 5);
			break;
		case "mulher":
			audio.PlayOneShot(somGritoMulher, 5);
			break;
		case "homem":
			audio.PlayOneShot(somGritoHomem, 5);
			break;
		}
	}

	public void SomCurva(bool ativo)
	{
	/*	if(ativo && !tocando)
		{
			somDerrapada.audio.volume = 1;
			somDerrapada.audio.Play();
			tocando = true;
		}
		else if(!ativo && tocando)
		{
			tocando = false;
			ativado=true;
		}
		else if(ativado)
		{
			somDerrapada.audio.volume -= 0.1f;
			if(somDerrapada.audio.volume <= 0)
			{
				ativado = false;
			}
		}
		else
		{
			somDerrapada.audio.Stop();
		}*/
	}

	// Update is called once per frame
	void Update ()
	{
	
	}
}
