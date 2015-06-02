using UnityEngine;
using System;

class ResetButton : TouchBehaviour
{
    public override void OnTouchEnd(TouchController tc, int touchIndex, Vector2 position)
    {
        Ray r = Camera.main.ScreenPointToRay(position);
        RaycastHit hit;
        if (Physics.Raycast(r, out hit, tc.mask))
        {
            if (hit.collider == gameObject.GetComponent<Collider>())
            {
                GameStatus.Reset();
            }
        }
    }
}
