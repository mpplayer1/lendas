using UnityEngine;
using System.Collections;

public class AnimLoading : MonoBehaviour {
	OTAnimatingSprite anim;

	// Use this for initialization
	void Start () {
		anim = this.gameObject.GetComponent<OTAnimatingSprite>();
		anim.Play();

		//DontDestroyOnLoad(this.gameObject);
	}
	
	// Update is called once per frame
	void Update () {
//		if(!Application.isLoadingLevel){
//			Destroy(this.gameObject);
//		}
	}
}
