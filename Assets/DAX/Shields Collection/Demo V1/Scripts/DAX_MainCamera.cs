using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DAX_MainCamera : MonoBehaviour 
{
	public GameObject[] Items;
	public string[] Descr;
	public Text OutText;
	public Text OutDescr;
	public int curIndex = 0;

	GameObject curPrefab = null;

	//Vector3 c_OrbitVector;
	//public float CameraRotationSpeed = 5.0f; 
	//float cRotationMoment = 0.0f;

	// Use this for initialization
	void Start () 
	{
		showPrefab( this.curIndex );

		//this.c_OrbitVector = this.transform.position;

		//this.transform.LookAt( new Vector3( 0f, 0f, 0f));
	}

	void Update()
	{
		//this.cRotationMoment += this.CameraRotationSpeed * Time.deltaTime;
		//if (this.cRotationMoment>360.0f){this.cRotationMoment-=360.0f;};
		//this.transform.position = Quaternion.AngleAxis( cRotationMoment , Vector3.up) * this.c_OrbitVector;
		//this.transform.LookAt( new Vector3( 0f, 0f, 0f));

	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		this.OutText.text = string.Format( "{0}/{1}", this.curIndex+1, this.Items.Length );
	}

	public void Next()
	{
		this.curIndex += 1;
		if (this.curIndex >= this.Items.Length )
		{
			this.curIndex = 0;
		}
		showPrefab( this.curIndex );
	}

	public void Prev()
	{
		this.curIndex -= 1;
		if (this.curIndex <0) { this.curIndex = this.Items.Length-1;};
		showPrefab( this.curIndex );
	}

	public void  showPrefab( int index )
	{
		if (this.curPrefab!=null)
		{
			GameObject.Destroy( this.curPrefab );
		}
		this.curPrefab = Instantiate( this.Items[ this.curIndex ] ) as GameObject;
		this.curPrefab.transform.position.Set( 0f, 0f, 0f );
		this.OutDescr.text = this.Descr [this.curIndex];
	}


}
