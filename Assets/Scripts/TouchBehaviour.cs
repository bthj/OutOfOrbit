using UnityEngine;
using System.Collections;

public class TouchBehaviour : MonoBehaviour {

    public virtual void Awake()
    {
        if (gameObject.layer != TouchController.TOUCH_LAYER && gameObject.layer != TouchController.DIODE_LAYER)
            Debug.LogWarning("GameObject "+gameObject.name+" can not receive touch input");
    }


    public virtual void OnTouchStart(TouchController tc, int touchIndex, Vector2 pos) { }

    public virtual void OnTouchDrag(TouchController tc, int touchIndex, Vector2 pos) { }

	public virtual void OnTouchEnd(TouchController tc, int touchIndex, Vector2 pos) { }
}
