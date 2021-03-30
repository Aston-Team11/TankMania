using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// @author Riyad K Rahman <br></br>
/// handles the dissolving animation for the shield
/// </summary>
public class Shield : MonoBehaviourPunCallbacks
{
    public GameObject player;                           //the owner of this shield 
    Renderer rend;                                      //the renderer component of the shield 
    public Material[] mats;                             // the materials assoicated to the shield

    float bright;                                           //the brightness of the shield
    bool change, triggerDissolve, triggerDestroy= false;    //the triggers to change the state of the animation
    Rigidbody rb;                                           // the rigidbody component of this shield

    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// ignores collisions between the shields owner and itself
    /// </summary>
    private void Start()
    {
        Physics.IgnoreCollision(this.GetComponent<Collider>(), player.GetComponent<Collider>());
        //!!!! if collisions with shield and player persist then delete above line
    }

    private void Update()
    {
        StartCoroutine(Dissolve());
    }

    private void FixedUpdate()
    {
        transform.position = player.transform.position;
    }

    /// <summary>
    ///  @author Riyad K Rahman <br></br>
    ///  manages the behaviour of what to do when colliding with different gameobjects 
    /// </summary>
    /// <param name="collision">the collider component of a gameobject </param>
    private void OnCollisionEnter(Collision collision)
    {
        //only let other players in the shield
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "PlayerAddOns")
        {
            Physics.IgnoreCollision(collision.collider, gameObject.GetComponent<Collider>());

        }
    }

    IEnumerator Dissolve()
    {
        yield return new WaitForSeconds(15f);
        FXAnimation();
    }

    /// <summary>
    ///  @author Riyad K Rahman <br></br>
    ///  makes the shield more birghter until a max brightness is reached,
    ///  then calls <see cref="slowlyDestroy"/> to dissolve and destroy the shield
    /// </summary>
    public void FXAnimation()
    {
        //changes fernal and offset, to look like shield is charging up and becoming more solid 
        if (bright > -2.00f && change == false)
        {
            bright -= 0.8f * Time.fixedDeltaTime;
            mats[0].SetFloat("Vector1_EBFAC9DB", bright);
            mats[0].SetFloat("Vector1_CD965979", bright + 2.05f);
            //!!!! change 2.05 value if you want alter brightness to change before material changes 
        }

        else
        {
            change = true;
           // StopAllCoroutines();
            rend.material = mats[1];
           // tell everyone to destroy this players shield
           photonView.RPC("slowlyDestroy", RpcTarget.OthersBuffered);
            slowlyDestroy();
          
        }
    }

    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// Resets shield's material values 
    /// </summary>
    /// <param name="val">default value of the brightness that should be set </param>
    public void resetFernal(float val)
    {
        //set brightness values
        mats[0].SetFloat("Vector1_EBFAC9DB", val);
        mats[0].SetFloat("Vector1_CD965979", 2.37f);
        mats[1].SetFloat("Vector1_7AFF87E4", -0.2f);
        mats[1].SetFloat("Vector1_2E8A09FF", 1);
        bright = val;

        rb = GetComponent<Rigidbody>();
        rend = GetComponent<Renderer>();
        rb.constraints = RigidbodyConstraints.FreezeAll;

        rend.material = mats[0];

        //RE-enable rigidbody and animation variables
        change = false;
        triggerDissolve= false;
        triggerDestroy = false;

       
    }

    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// a server function to ensure everyone runs the dissolving animation of this shield
    /// </summary>
    [PunRPC]
    private void slowlyDestroy()
    {
         // changing brightness
         var fernal = mats[1].GetFloat("Vector1_7AFF87E4");
        if (fernal < 2.37f && triggerDissolve == false)
        {
            fernal += 0.4f * Time.fixedDeltaTime;
            mats[1].SetFloat("Vector1_7AFF87E4", fernal);
        }

        else
        {
            triggerDissolve = true;
        }

        //changing dissolve values so the shield dissovles away before being disabled
        var dissolve = mats[1].GetFloat("Vector1_2E8A09FF");
        if (dissolve > -0.9f && triggerDestroy == false)
        {
            dissolve -= 0.6f * Time.fixedDeltaTime;
            mats[1].SetFloat("Vector1_2E8A09FF", dissolve);
        }

        else
        {
            triggerDestroy = true;
            Debug.Log("Destroy SHIELD");
            gameObject.SetActive(false);

        }
     
    }

    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// a server function to inform to everyone the shield is disabled
    /// </summary>
    [PunRPC]
    public void sendtoServer()
    {
        gameObject.SetActive(false);
        photonView.RPC("sendtoServer", RpcTarget.OthersBuffered);
    }

    public int getID()
    {
        return player.GetComponent<PhotonView>().ViewID;
    }


}

