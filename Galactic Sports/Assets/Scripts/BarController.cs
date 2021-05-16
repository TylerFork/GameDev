using UnityEngine;
using System.Collections;

public class BarController : MonoBehaviour {

	public float speed = 1500f;
	public float rotationSpeed = 15f;

	private float movement = 0f;
	private float rotation = 0f;

	Rigidbody2D barRigidBody;


    private void Start()
    {
    
        barRigidBody = gameObject.transform.Find("Bar").gameObject.GetComponent<Rigidbody2D>();
    }

    void Update ()
	{
		movement = -Input.GetAxisRaw("Vertical") * speed;
		rotation = Input.GetAxisRaw("Horizontal");
	}

	void FixedUpdate ()
	{


        barRigidBody.AddTorque(-rotation * rotationSpeed * Time.fixedDeltaTime);
    }

}
