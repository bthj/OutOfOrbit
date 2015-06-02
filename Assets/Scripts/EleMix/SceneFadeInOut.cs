using UnityEngine;
using System.Collections;

public class SceneFadeInOut : MonoBehaviour {

	public AudioSource backgroundAudio;
	public float musicFadeTime = 2f;

	public float fadeSpeed = 1.5f;
    public int levelToLoad;

	private bool sceneStarting = false, sceneEnding = false;
	private float initialMusicVolume;
	
	void Awake() {
        transform.position = new Vector3(0, 0, 0);
		GetComponent<GUITexture>().pixelInset = new Rect( 0f, 0f, Screen.width, Screen.height );
        GetComponent<GUITexture>().color = Color.clear;
        GetComponent<GUITexture>().enabled = false;
	}

	void Start() {

		initialMusicVolume = backgroundAudio.volume;
	}
	
	void Update() {
		
		if( sceneStarting ) {

            Starting();
		}
        if (sceneEnding)
        {
            Ending();
        }
	}
	
	void FadeToClear() {
		
		GetComponent<GUITexture>().color = Color.Lerp( GetComponent<GUITexture>().color, Color.clear, fadeSpeed * Time.deltaTime );
	}
	
	void FadeToBlack() {
		GetComponent<GUITexture>().color = Color.Lerp( GetComponent<GUITexture>().color, Color.black, fadeSpeed * Time.deltaTime );
	}
	
	public void StartScene() {
        sceneStarting = true;
        GetComponent<GUITexture>().enabled = true;
        GetComponent<GUITexture>().color = Color.black;

		StartCoroutine( FadeMusic(true) );
	}
	
	public void EndScene() {
        sceneEnding = true;
		GetComponent<GUITexture>().enabled = true;
        GetComponent<GUITexture>().color = Color.clear;

		StartCoroutine( FadeMusic(false) );
	}

    private void Ending()
    {
        FadeToBlack();
        if (GetComponent<GUITexture>().color.a >= 0.95f)
        {
            Application.LoadLevel(levelToLoad);  // Garage Scene should be at index 0 in build settings
        }
    }

    private void Starting()
    {
        FadeToClear();
        if (GetComponent<GUITexture>().color.a <= 0.05f)
        {
            GetComponent<GUITexture>().enabled = false;
            sceneStarting = false;
        }
    }

	

	private IEnumerator FadeMusic( bool fadeIn ) {
		
		float fadeTimer = 0f;
		while( fadeTimer < musicFadeTime ) {

			if( fadeIn ) {

				backgroundAudio.volume = Mathf.Lerp( 0f, initialMusicVolume, fadeTimer / musicFadeTime );
			} else {

				backgroundAudio.volume = Mathf.Lerp( initialMusicVolume, 0f, fadeTimer / musicFadeTime );
			}

			fadeTimer += Time.deltaTime;
			
			yield return null;
		}
		backgroundAudio.volume = 0;
	}
}
