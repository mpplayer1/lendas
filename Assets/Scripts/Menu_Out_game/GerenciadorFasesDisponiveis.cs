#define versao1

using UnityEngine;
using System.Collections;


public class GerenciadorFasesDisponiveis : MonoBehaviour {
	protected int quantidade; // Numero de fases disponivies;
	public int Quant{
		get{return this.quantidade;}
		set{this.quantidade = value;}
	}
	
	public string path = @"Assets\ConfigFiles\niveis.txt";
	
	//public GameObject[] vo;//Array<GameObject> vo;
	
#if versao1
	void Start(){
		//this.vo[0] = (GameObject)Instantiate(Resources.Load(@"Assets\Prefabs\Button_Level"));//, new Vector3(0,0,0));
		//Instantiate(ButtonLevel);
	}
#elif versao2
	// Use this for initialization
	void Start () {
		// Lendo de arquivo:
        System.IO.StreamReader sr = System.IO.File.OpenText(path);
        string s = "";
		bool iniciarLeituraLevels = false;
        while ((s = sr.ReadLine()) != null)
        {
            if(s == "Levels[" && !iniciarLeituraLevels){//Debug.Log(s);
				iniciarLeituraLevels = true;
				Debug.Log("true");
			} else if(s == "]" && iniciarLeituraLevels){
				iniciarLeituraLevels = false;
				Debug.Log("false");
			} else if(iniciarLeituraLevels){
				//Debug.Log("Lido: s["+(s.Length-1)+"] = "+s[s.Length-1]+"; Size: "+s.Length);
				//Debug.Break();
				
				string sAux;
				int lastI = 0, i = 0;
				bool exit = false;
				do{//(s = sr.ReadLine()) != null){
					sAux = "";
					//while(s != "]"){
					i = s.IndexOf(" ", lastI);//sAux = s.Split(' ');//Debug.Log(s);
					
//					try{
//						Debug.Log(s[i]);
//					} catch{
//						Debug.Log("Erro!");
//					}
					
						//i = s.Length - 1;
						//Debug.Log(s.Length);
						//Debug.Log("i="+i);
						//break;
						//i = s.Length-1;
						//exit = true;
					if(i != -1){
						sAux = s.Substring(lastI, i);
						
						// Faz alguma coisa:
						//Debug.Log(sAux);//+"\n");
						//Debug.Log("lastI="+lastI+" i="+i+"\n");// Teste;
						
					} else{
						sAux = s;
						//Debug.Log(s.Length);
						break;
					}
					Debug.Log(sAux);
					
					//Debug.Log("lastI="+lastI+" i="+i);
					lastI = i+1; // Para pular o " " que e a posiçao atual de iniciaçao;
				} while(!exit);//i != -1);
			}
			/* else if(s == "..."){ // Preencher com as demais opçoes. Ex: "Inimigos[", etc;
				
			}*/
        }
		sr.Close();
	}
#endif
	
	// Update is called once per frame
	void Update () {
		
	}
}
