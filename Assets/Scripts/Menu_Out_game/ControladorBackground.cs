using UnityEngine;
using System.Collections;

public class ControladorBackground : MonoBehaviour {
	// Atributos:
	int id = 0;// ID atual da imagem do vetor sizeImages;
	//MeshRenderer render;//OTSprite sprite;
	public Material[] listaMateriais;

	public int ID{
		set{this.id = value;}
		get{return this.id;}
	}
	// Metodos:
	// Trocar entre as imagens
	public void nextImg(){
		id++;
		if(id < listaMateriais.Length){//render.materials.Length){
			renderer.material = listaMateriais[id];//renderer.material = render.materials[id];//render.material = render.materials[id];
		} else{
			id = 0;
			renderer.material = listaMateriais[id];//render.material = render.materials[id];
		}
//		if(id < sprite.sizeImages.Length){
//			sprite.image = sprite.sizeImages[id].texture;
//		} else{
//			id = 0;
//			sprite.image = sprite.sizeImages[id].texture;
//		}
	}
	public void previousImg(){
		id--;
		if(id >= 0){
			renderer.material = listaMateriais[id];
			Debug.Log ("Trocou Esq");
		} else{
			id = listaMateriais.Length - 1;//render.materials.Length - 1;
			renderer.material = listaMateriais[id];
		}
//		if(id >= 0){
//			sprite.image = sprite.sizeImages[id].texture;
//		} else{
//			id = sprite.sizeImages.Length - 1;
//			sprite.image = sprite.sizeImages[id].texture;
//		}
	}
	
	// Use this for initialization
	void Start () {
		//renderer.material = listaMateriais[id];//render = this.gameObject.GetComponent<MeshRenderer>();//sprite = GetComponent<OTSprite>();

		//this.id = 0;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
