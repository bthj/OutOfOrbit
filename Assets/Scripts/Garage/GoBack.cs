using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GoBack : TouchBehaviour {

    public Stack<GameObject> previousPositions = new Stack<GameObject>();
    public override void Awake()
    {
        base.Awake();
    }

    void Update()
    {
//        collider.enabled = renderer.enabled = Camera.main != null && previousPositions.Count > 0 && Camera.main.GetComponent<iTween>() == null;   
    }

	public override void OnTouchEnd(TouchController tc, int touchIndex, Vector2 position)
    {
        Ray r = Camera.main.ScreenPointToRay(position);
        RaycastHit hit;
        if (Physics.Raycast(r, out hit, tc.mask))
        {
            if (hit.collider == gameObject.GetComponent<Collider>())
            {
                Transform pos = previousPositions.Pop().transform;
                iTween.MoveTo(Camera.main.gameObject, pos.position, 2f);
                iTween.RotateTo(Camera.main.gameObject, pos.rotation.eulerAngles, 2f);

            }
        }
    }
}
