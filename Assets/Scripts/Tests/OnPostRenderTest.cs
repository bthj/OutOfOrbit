using UnityEngine;
using System.Collections;

public class OnPostRenderTest : MonoBehaviour {

	public Texture2D testTexture;
	
	/*
	 * test to render gui texture OnPostRender, when attached to the camera,
	 * instead of the expensive OnGUI
	 * ...so for not fruitful...
	 */
	void OnPostRender() {

		Debug.Log( "post rendering" );

		/* this matrix wrangling gotten from 
		 * http://answers.unity3d.com/questions/445490/is-using-onpostrender-to-draw-textures-pro-only.html
		 * ...but ain't changing anything
		 * 
		GL.PushMatrix();
		GL.LoadPixelMatrix();
		*/


		Graphics.DrawTexture( new Rect(5, 5, 10, 30), testTexture );

		// GL.PopMatrix();
	}
}
