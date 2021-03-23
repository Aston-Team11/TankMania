using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
    [RequireComponent(typeof(ThirdPersonCharacter))]
    public class AICharacterControl : MonoBehaviourPunCallbacks
    {

        [SerializeField] private Transform target;      // the transform of the target to aim for
        public GameObject[] playerList;                 // a list of players


        [SerializeField] private float minimumDist; //edit this field to mkae the zombie change target
        [SerializeField] private float dist;
        [SerializeField] private int poisonCloudSpawnRate;

        private GameObject spawner; //used to access the spawner script 
        public GameObject Blood_Sound_Effect;   // bloodspray sound effect

        public UnityEngine.AI.NavMeshAgent agent { get; private set; }             // the navmesh agent required for the path finding
        public ThirdPersonCharacter character { get; private set; } // the character we are controlling                  


        public GameObject bombSpray;  //this is the effect of the zombie exploding
        public GameObject radiation;  //this is the effect of the explosion remnants
        public GameObject bloodSpray; // bloodspray particle effect


        /// <summary>
        /// @author Riyad K Rahman <br></br>
        /// Only to be used once when system gameobject spawns the first zombies
        /// </summary>
        public void TargetPlayer1()
        {
            photonView.RPC("Retarget", RpcTarget.AllViaServer, "1002");
        }



        /// <summary>
        /// @author Riyad K Rahman <br></br>
        /// sets local target to be a player's transform position
        /// <see cref="Spawner.targetPlayer(int)"/> 
        /// </summary>
        /// <param name="Player"></param>
        public void getPlayers(Transform Player)
        {
            this.target = Player;
        }



        /// <summary>
        /// @author Riyad K Rahman <br></br>
        /// Calculate's which player is closest then
        /// sets local target to be the player's transform position
        /// </summary>
        /// <param name="Player"></param>
        private void DistanceCalculator()
        {
            playerList = GameObject.FindGameObjectsWithTag("Player");

            foreach (GameObject player in playerList)
            {
                // if player is alive then check if he is cloesest to the zombie
                if (player.activeSelf == true)
                {
                    dist = Vector3.Distance(player.transform.position, transform.position);
                    //if a different player comes close enough the target changes
                    if (dist < minimumDist)
                    {
                        minimumDist = dist;
                        //target = player.transform;
                        photonView.RPC("Retarget", RpcTarget.AllViaServer, player.name);
                    }

                }
            }
        }

        /// <summary>
        /// sets the target of a zombie
        /// </summary>
        /// <param name="name">name of the player</param>
        [PunRPC]
        public void Retarget(string name)
        {
            try
            {
                target = GameObject.Find(name).transform;
            }
            catch(NullReferenceException)
            {
                //stay stationary
                target = this.transform;
                minimumDist = 7;
            }
        }



        private void Start()
        {
            // get the components on the object we need ( should not be null due to require component so no need to check )
            agent = GetComponentInChildren<UnityEngine.AI.NavMeshAgent>();
            character = GetComponent<ThirdPersonCharacter>();

            agent.updateRotation = false;
            agent.updatePosition = true;

            spawner = GameObject.FindGameObjectWithTag("EnemySpawn");
            Blood_Sound_Effect = GameObject.Find("ZombieSpawn");

        }



        private void Update()
        {
            //if target is empty just stay put
            if (target == null)
            {
                target = this.transform;

            }

            //go to the nearest player
            DistanceCalculator();
        
          
            //sets the target
            agent.SetDestination(target.position);

            if (agent.remainingDistance > agent.stoppingDistance)
                character.Move(agent.desiredVelocity, false, false);

            else
                //stop moving (vector.zero) 
                character.Move(Vector3.zero, false, false);
            //!!!! add attacking animation here   
        }



        /// <summary>
        /// @author Riyad K Rahman <br></br>
        /// Defines how this gameobject reacts on collision with other objects in the scene
        /// </summary>
        /// <param name="collision"> This is used to detect shich object this zombie has collided with</param>
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "PlayerAddOns")
            {
                //!!!! add attacking animation here?
                collision.gameObject.GetComponentInParent<PlayerManager>().DamagePlayer(10);
                //Explode();
                photonView.RPC("Explode", RpcTarget.AllBuffered);
                //Debug.Log("attack");
            }

            else if (collision.gameObject.tag == "Bullet")
            {
                Debug.Log("enemy dead");
                // Death();
                photonView.RPC("Death", RpcTarget.AllBuffered);
            }

            else if (collision.gameObject.tag == "Shield")
            {
                Debug.Log("enemy Vapourised");
                //Death();
                photonView.RPC("Death", RpcTarget.AllBuffered);
            }
        }


        /// <summary>
        /// @author Riyad K Rahman <br></br>
        /// triggers particle effect and destroys this object/bloodspray object both locally and on the server
        /// </summary>
        [PunRPC]
        private void Death()
        {
            Quaternion rot = new Quaternion(transform.rotation.x, transform.rotation.y, 90f, transform.rotation.w);

            var blood = Instantiate(bloodSpray, transform.position, bloodSpray.transform.rotation);
            Destroy(blood, 2f);
         

            Blood_Sound_Effect.GetComponent<AudioSource>().Play();


            // spawn.enemiesKilled++;
            spawner.SendMessage("IncrementEnemies");

            //kill zombie after contact
            this.gameObject.SetActive(false);
        }

        /// <summary>
        /// @author Qabais Mohammed & Riyad K Rahman <br></br>
        /// triggers explosion effect when zombies collide with player.
        /// removed zombie object and local visual effects
        /// </summary>
        [PunRPC]
        private void Explode()
        {
            Quaternion rot = new Quaternion(transform.rotation.x, transform.rotation.y, 90f, transform.rotation.w);

            //call explosion effect
            var zExplosion = Instantiate(bombSpray, transform.position, bombSpray.transform.rotation);
            Destroy(zExplosion, 2f);

            //only master client hnadles whether radiation cloud should spawn or not 
            if (photonView.IsMine)
            {
               if (UnityEngine.Random.Range(0, poisonCloudSpawnRate) == 1)
                {
                    photonView.RPC("SpawnCloud", RpcTarget.All);
                }
 
            }
            
            
  

            // spawn.enemiesKilled++;
            spawner.SendMessage("IncrementEnemies");

            //kill zombie after contact
            this.gameObject.SetActive(false);

     
        }


        /// <summary>
        /// @auhtor Riyad K Rahman <br></br>
        /// spawns radiation cloud on all clients
        /// </summary>
        [PunRPC]
        private void SpawnCloud()
        {
            //call toxic effect 
             Instantiate(radiation, transform.position, radiation.transform.rotation);
        }

        /// <summary>
        /// @author Riyad K Rahman <br></br>
        /// when the game object is disabled it will destroy itself only triggered by the master client
        /// </summary>
        public override void OnDisable()
        {
            if (photonView.IsMine)
            {
                PhotonNetwork.Destroy(this.photonView);
            }
        }
    }
}
