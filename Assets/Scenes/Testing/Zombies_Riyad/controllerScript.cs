using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class controllerScript : MonoBehaviour
{
    private Animator anim;
    Vector2 velocity = new Vector2(0.0f, 0.0f);
    float acceleration = 0.5f;
    float deceleration = 0.5f;
    float maxVelocity = 2f;
    [SerializeField] private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        bool forward = Input.GetKey(KeyCode.W); 
        bool left = Input.GetKey(KeyCode.A); ;
        bool right = Input.GetKey(KeyCode.D);
        

        //accelerate 
        if (forward && velocity.y < maxVelocity)
        {
            velocity.y += Time.deltaTime * acceleration;
        }
        if (left && velocity.x > -maxVelocity)
        {
            velocity.x -= Time.deltaTime * acceleration;
        }
        if (right && velocity.x < maxVelocity)
        {
            velocity.x += Time.deltaTime * acceleration;
        }

        //decelerate 

        if(!forward && velocity.y > 0f)
        {
            velocity.y -= Time.deltaTime * deceleration;
        }

        if (!left && velocity.x < 0f)
        { 
            velocity.x += Time.deltaTime * deceleration;
        }

        if (!right && velocity.x > 0f)
        {
            velocity.x -= Time.deltaTime * deceleration;
        }

        //reset

        if (!forward && velocity.y < 0f)
        {
            velocity.y = 0;
        }

        if (!left && !right && velocity.x != 0f && (velocity.x > 0.05f && velocity.x < 0.05f))
        {
            velocity.x = 0;
        }


        anim.SetFloat("Velocity X", velocity.x);
        anim.SetFloat("Velocity Z", velocity.y);

    }


    public void FixedUpdate()
    {
        //Vector3 pos = new Vector3(velocity.x,0f,velocity.y);
       // rb.velocity = pos;
    }


}