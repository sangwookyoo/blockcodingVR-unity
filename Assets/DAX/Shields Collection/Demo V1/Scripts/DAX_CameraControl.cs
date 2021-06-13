using UnityEngine;
using System.Collections;

public class DAX_CameraControl : MonoBehaviour 
{

	public float sensitivityX = 8F;
	public float sensitivityY = 8F;
	
	float mHdg = 0F;
	float mPitch = 0F;
	
	void Start()
	{
		// owt?
	}
	
	void Update()
	{
		float ValH, ValV = 0.0f;

		ValH = Input.GetAxisRaw ("Horizontal");
		ValV = Input.GetAxisRaw( "Vertical" );

		transform.position += transform.right * (sensitivityX*ValH*0.33f); 
		transform.position += transform.forward *  (sensitivityY*ValV*0.33f); 

		if (!(Input.GetMouseButton(2) || Input.GetMouseButton(0) )) return;
		
		float deltaX = Input.GetAxis("Mouse X") * sensitivityX;
		float deltaY = Input.GetAxis("Mouse Y") * sensitivityY;
		
		if (Input.GetMouseButton(2) && Input.GetMouseButton(0))
		{
			Strafe(deltaX);
			ChangeHeight(deltaY);
		}
		else
		{
			if (Input.GetMouseButton(2))
			{
				MoveForwards(deltaY);
				ChangeHeading(deltaX);
			}
			else if (Input.GetMouseButton(0))
			{
				ChangeHeading(deltaX);
				ChangePitch(-deltaY);
			}
		}
	}
	
	void MoveForwards(float aVal)
	{
		Vector3 fwd = transform.forward;
		fwd.y = 0;
		fwd.Normalize();
		transform.position += aVal * fwd;
	}
	
	void Strafe(float aVal)
	{
		transform.position += aVal * transform.right;
	}
	
	void ChangeHeight(float aVal)
	{
		transform.position += aVal * Vector3.up;
	}
	
	void ChangeHeading(float aVal)
	{
		 mHdg += aVal;
		WrapAngle(ref mHdg);
		transform.localEulerAngles = new Vector3(mPitch, mHdg, 0);
	}
	
	void ChangePitch(float aVal)
	{
		mPitch += aVal;
		WrapAngle(ref mPitch);
		transform.localEulerAngles = new Vector3(mPitch, mHdg, 0);
	}
	
	public static void WrapAngle(ref float angle)
	{
		if (angle < -360F)
			angle += 360F;
		if (angle > 360F)
			angle -= 360F;
	}
	
	
} 
