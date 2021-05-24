using UnityEngine;
using System.Collections;

public class DAX_Rotator : MonoBehaviour 
{
	public float AngularSpeed = 360.0f;

	// Use this for initialization
	void Awake () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		this.gameObject.transform.Rotate( 0.0f, 0.0f, this.AngularSpeed * Time.deltaTime, Space.Self );  
	}
}
