using UnityEngine;
using System.Collections;

public class DAX_Rotator_XYZ : MonoBehaviour 
{
	public float XAngularSpeed = 360.0f;
	public bool XRotation = false;
	public float YAngularSpeed = 360.0f;
	public bool YRotation = false;
	public float ZAngularSpeed = 360.0f;
	public bool ZRotation = true;

	// Use this for initialization
	void Awake () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		float XR = 0.0f;
		float YR = 0.0f;
		float ZR = 0.0f;

		if (XRotation)
		{
			XR = this.XAngularSpeed * Time.deltaTime;
		}

		if (YRotation)
		{
			YR = this.YAngularSpeed * Time.deltaTime;
		}

		if (ZRotation)
		{
			ZR = this.ZAngularSpeed * Time.deltaTime;
		}

		this.gameObject.transform.Rotate( XR, YR, ZR, Space.Self );  
	}
}
