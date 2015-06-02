        var Step : float; //A variable we increment
        var Offset : float; //How far to offset the object upwards
 
        function Start () {
        //Store where we were placed in the editor
        var InitialPosition = transform.position;
        //Create an offset based on our height
        //Offset = transform.position.y + transform.localScale.y;
        Offset = transform.position.y;
        }
 
        function FixedUpdate () {
 
        Step +=0.3;
          //Make sure Steps value never gets too out of hand 
          if(Step > 999999){Step = 1;}
 
          //Float up and down along the y axis, 
          //transform.position.y = Mathf.Sin(Step)+Offset;
          transform.position.y = Offset + Mathf.Sin(Step) * .001f;
          //Debug.Log("sine: " + transform.position.y);
        }