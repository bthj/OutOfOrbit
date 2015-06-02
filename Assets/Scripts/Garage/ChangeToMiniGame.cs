using UnityEngine;
using System.Collections;

public class ChangeToMiniGame : MonoBehaviour {
    private SceneFadeInOut fader;

    void Awake()
    {
        fader = GameObject.Find("screenFader").GetComponent<SceneFadeInOut>();
    }
    void X()
    {
        //Application.LoadLevel(1);
        fader.EndScene();
    }
}
