using UnityEngine;
using System.Collections;

public class SirenRotate : MonoBehaviour {


/*
*	FUNCTION: Rotate the siren on the police car.
*
*/

private Transform tBackgroundRotation;
private float fBackgroundRotateValue = 0.0f;

void Start (){
	tBackgroundRotation = this.transform;
}

void FixedUpdate (){
	fBackgroundRotateValue = Mathf.Lerp(fBackgroundRotateValue, 8.0f, Time.deltaTime);
	tBackgroundRotation.transform.Rotate(0,fBackgroundRotateValue,0);
}
}