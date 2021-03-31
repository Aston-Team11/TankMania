using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Photon.Pun;

namespace Tests
{
    /// <summary>
    /// @author Riyad K Rahman, Balraj Bains <br></br>
    /// preforms a series of unit tests on the playerManager class
    /// </summary>
    public class TestSuitePlayer
    {
        GameObject player;                              //the new player instantiated 
        PlayerManager playerManagerstats;              //the player manager component of the new player 
        PlayerPowerupManager playerPowerups;
        GameObject zombie;

        /// <summary>
        /// @author Riyad K Rahman <br></br>
        /// this test checks if a player is instantiated and checks if it is loaded in the scene
        /// This is also the very first test and therefore will handle connecting to photon and loading the level finding the player gameobject
        /// </summary>
        [UnityTest]
        public IEnumerator A0_InstantiatePlayer_Enumerator()
        {
            GameObject launcher = new GameObject();
            launcher.name = "Launcher";
            launcher.AddComponent<Launcher>();
            launcher.GetComponent<Launcher>().SetTesting(true); //ensures loading straight to TankGame scene
            PhotonNetwork.ConnectUsingSettings();

            //waits 5 seconds before creating a room 
            yield return new WaitForSeconds(5f);
            PhotonNetwork.CreateRoom("test");

            //wait 5 sendons before spawning a player in 
            yield return new WaitForSeconds(8f);
            Vector3 pos = new Vector3(-8.44f, 0.5f, 3.4f);
            Quaternion rot = new Quaternion(0f, 0f, 0f, 0f);
            this.player = GameObject.FindGameObjectWithTag("Player");
            this.player.transform.position = pos;
            this.player.transform.rotation = rot;
            this.playerManagerstats = this.player.GetComponent<PlayerManager>();   //assigns the player manager component of the new player 
            this.playerPowerups = this.player.GetComponent<PlayerPowerupManager>();
            Assert.IsNotNull(this.player);
        }

        /// <summary>
        /// @author Riyad K Rahman <br></br>
        /// tests if the player health changed
        /// </summary>
        [UnityTest]
        public IEnumerator A1_PlayerDamage_Enumerator()
        {
            // Use yield to skip a frame.
            yield return null;
            this.playerManagerstats.DamagePlayer(30);
            Assert.AreEqual(70, this.playerManagerstats.GetHealth());
        }

        /// <summary>
        /// @author Riyad K Rahman <br></br>
        /// tests if the player has PVE canvas can be loaded 
        /// </summary>
        [UnityTest]
        public IEnumerator A2_PlayerGamemodeCheck_PVE_Enumerator()
        {
            yield return null;
            Assert.AreEqual(true, this.playerManagerstats.GetPVECanvas().activeSelf);

        }
        /// <summary>
        /// @author Riyad K Rahman <br></br>
        /// tests if the player has FFA canvas can be loaded 
        /// </summary>
        [UnityTest]
        public IEnumerator A3_PlayerGamemodeCheck_FFA_Enumerator()
        {
            yield return null;
            this.playerManagerstats.GameModeSetup(1);
            Assert.AreEqual(true, this.playerManagerstats.GetFFACanvas().activeSelf);
            this.playerManagerstats.GetFFACanvas().SetActive(false);
        }

        /// <summary>
        /// @author Riyad K Rahman <br></br>
        /// tests if the player has rotated left
        /// </summary>
        [UnityTest]
        public IEnumerator A4_PlayerRotateLeft_Enumerator()
        {
         
            yield return null;

            float initialrot = this.player.transform.rotation.y;        //the starting rotation

            //simulate player input 
            this.player.GetComponent<PlayerMovement>().direction.x = -15;
            yield return new WaitForEndOfFrame();
            this.player.GetComponent<PlayerMovement>().direction.x = -15;


            yield return new WaitForSeconds(3f);

           Assert.IsTrue(this.player.transform.rotation.y < initialrot);
        
        }

        /// <summary>
        /// @author Riyad K Rahman <br></br>
        /// tests if the player has rotated Right
        /// </summary>
        [UnityTest]
        public IEnumerator A5_PlayerRotateRight_Enumerator()
        {

            yield return null;

            float initialrot = this.player.transform.rotation.y;        //the starting rotation

            //simulate player input 
            this.player.GetComponent<PlayerMovement>().direction.x = 15;
            yield return new WaitForEndOfFrame();
            this.player.GetComponent<PlayerMovement>().direction.x = 15;


            yield return new WaitForSeconds(3f);

            Assert.IsTrue(this.player.transform.rotation.y > initialrot);
        }

        /// <summary>
        /// @author Riyad K Rahman <br></br>
        /// tests if the player has moved forward
        /// </summary>
        [UnityTest]
        public IEnumerator A6_PlayerMoveForward_Enumerator()
        {

            yield return null;

            float initialrot = this.player.transform.position.z;        //the starting rotation

            //simulate player input 
            this.player.GetComponent<PlayerMovement>().direction.y = 1;
            yield return new WaitForEndOfFrame();
            this.player.GetComponent<PlayerMovement>().direction.y = 1;


            yield return new WaitForSeconds(3f);

            Assert.IsTrue(this.player.transform.position.z > initialrot);
        }

        /// <summary>
        /// @author Riyad K Rahman <br></br>
        /// tests if the player has moved Backwards
        /// </summary>
        [UnityTest]
        public IEnumerator A7_PlayerMoveBack_Enumerator()
        {

            yield return null;

            float initialrot = this.player.transform.position.z;        //the starting rotation

            //simulate player input 
            this.player.GetComponent<PlayerMovement>().direction.y = -1;
            yield return new WaitForEndOfFrame();
            this.player.GetComponent<PlayerMovement>().direction.y = -1;


            yield return new WaitForSeconds(3f);

            Assert.IsTrue(this.player.transform.position.z < initialrot);
        }

        /// <summary>
        /// @author Balraj Bains <br></br>
        /// tests if kill counter works correctly 
        /// </summary>
        [UnityTest]
        public IEnumerator A8_KillsCounter()
        {
            yield return null;
            this.playerManagerstats.AddKill(1, 1);
            yield return new WaitForSeconds(1);
            Assert.AreEqual(1, this.playerManagerstats.GetKill());
        }

        /// <summary>
        /// @author Balraj Bains <br></br>
        /// tests if kill counter works correctly 
        /// </summary>
        [UnityTest]
        public IEnumerator A9_KillsSync()
        {
            yield return null;
            Assert.IsTrue(this.playerManagerstats.GetSharedStats().GetPlayerStats(this.playerManagerstats.GetOrder() - 1) == 1);
        }

        /// <summary>
        /// @author Balraj Bains <br></br>
        /// tests if player minigun powerup functions correctly 
        /// </summary>
        [UnityTest]
        public IEnumerator B0_AdditionalLife()
        {
            yield return null;
            int playerlives = playerManagerstats.GetPlayerLives();
            this.playerManagerstats.AddLife();
            yield return new WaitForEndOfFrame();
            Assert.AreEqual(this.playerManagerstats.GetPlayerLives(), playerlives + 1);
        }

        /// <summary>
        /// @author Riyad K Rahman <br></br>
        /// tests if player minigun powerup functions correctly 
        /// </summary>
        [UnityTest]
        public IEnumerator B1_LifeSync()
        {
            yield return null;
            int playerlives = playerManagerstats.GetPlayerLives();
            Assert.IsTrue(this.playerManagerstats.GetSharedStats().GetPlayerStats(this.playerManagerstats.GetOrder() - 1) == playerlives);

        }


        /// <summary>
        /// @author Balraj Bains <br></br>
        /// tests if player shooting functions correctly 
        /// </summary>
        [UnityTest]
        public IEnumerator B2_PlayerShooting()
        {
            yield return null;
            this.player.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<PhotonView>().RPC("Shoot", RpcTarget.All, false);
            yield return new WaitForEndOfFrame();
            Assert.IsNotNull(GameObject.FindGameObjectWithTag("Bullet"));
            GameObject.Destroy(GameObject.FindGameObjectWithTag("Bullet"));
        }

       
        /// <summary>
        /// @author Balraj Bains <br></br>
        /// tests if player health powerup functions correctly 
        /// </summary>
        [UnityTest]
        public IEnumerator B3_HealthUp()
        {
            yield return null;
            this.playerManagerstats.AddHealth(50);
            yield return new WaitForEndOfFrame();
            Assert.AreEqual(100, this.playerManagerstats.GetHealth()); 
        }

        /// <summary>
        /// @author Balraj Bains <br></br>
        /// tests if player shotgun powerup functions correctly 
        /// </summary>
        [UnityTest]
        public IEnumerator B4_Shotgun()
        {
            yield return null;
            this.player.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<PhotonView>().RPC("Shotgun", RpcTarget.All);
            yield return new WaitForEndOfFrame();
            Assert.AreEqual(GameObject.FindGameObjectsWithTag("Bullet").Length,3);
        }

        /// <summary>
        /// @author Balraj Bains <br></br>
        /// tests if player minigun powerup functions correctly 
        /// </summary>
        [UnityTest]
        public IEnumerator B5_Minigun()
        {
            yield return new WaitForSeconds(2f);
            for (int i = 0; i < 5; i++) { this.player.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<PhotonView>().RPC("Shoot", RpcTarget.All, true); }
            
            Assert.IsTrue(GameObject.FindGameObjectsWithTag("Bullet").Length > 3);
        }

        /// <summary>
        /// @author Balraj Bains <br></br>
        /// tests if player slow time powerup functions correctly 
        /// </summary>
        [UnityTest]
        public IEnumerator B6_Shield()
        {
            yield return null;
            this.playerPowerups.PowerupAttained("SHIELD");
            this.playerPowerups.setSpacePressed(true);
            yield return new WaitForSecondsRealtime(1f);
            Assert.IsTrue(this.player.transform.GetChild(1).gameObject.activeSelf);
        }


        /// <summary>
        /// @author Riyad K Rahman <br></br>
        /// tests if player slow time powerup functions correctly 
        /// </summary>
       [UnityTest]
        public IEnumerator B7_Slowmo()
        {
            yield return null;
            this.playerPowerups.setTimeObject(GameObject.Find("TimeManager"));
            this.playerPowerups.PowerupAttained("SLOMO");
            this.playerPowerups.setSpacePressed(true);
            yield return new WaitForSecondsRealtime(1f);
            Assert.IsTrue(Time.timeScale < 1);
        }

        /// <summary>
        /// @author Riyad K Rahman <br></br>
        /// tests if player poison damage functions correctly 
        /// </summary>
        [UnityTest]
        public IEnumerator B8_PoisonDamage()
        {
            yield return null;

            float healthBeforePoison = playerManagerstats.GetHealth();
            GameObject radiation = new GameObject();

            radiation.AddComponent<Radiation>();
            radiation.AddComponent<SphereCollider>();
            radiation.GetComponent<SphereCollider>().isTrigger = true;
            radiation.GetComponent<SphereCollider>().radius = 2.32f;

            GameObject.Instantiate(radiation, player.transform.position, player.transform.rotation);
            yield return new WaitForSecondsRealtime(1f);
            Assert.IsTrue(playerManagerstats.GetHealth() < healthBeforePoison);
        }

        /// <summary>
        /// @author Balraj Bains <br></br>
        /// tests if all zombie spawnpoints are loaded in 
        /// </summary>
        [UnityTest]
        public IEnumerator C1_ZombieSpawnersInit()
        {
            yield return null;
            GameObject.FindGameObjectWithTag("EnemySpawn").GetComponent<Spawner>();
            Assert.AreEqual(GameObject.FindGameObjectWithTag("EnemySpawn").GetComponent<Spawner>().spawners.Length, 5);
        }

        /// <summary>
        /// @author Balraj Bains <br></br>
        /// tests if the zombie count works correctly 
        /// </summary>
        [UnityTest]
        public IEnumerator C2_ZombiesInit()
        {
            yield return null;
            Assert.AreEqual(GameObject.FindGameObjectWithTag("EnemySpawn").GetComponent<Spawner>().getRemaining(), 5);
        }

        /// <summary>
        /// @author Riyad K Rahman <br></br>
        /// tests if zombie targetting functions correctly 
        /// </summary>
        [UnityTest]
        public IEnumerator C3_ZombieTargetting()
        {
            yield return new WaitForSecondsRealtime(6f);
            zombie = GameObject.FindGameObjectWithTag("Enemy");
            Assert.IsNotNull(zombie.GetComponent<AICharacterControl>().GetTarget());
        }

        /// <summary>
        /// @author Riyad K Rahman <br></br>
        /// tests if zombie movement functions correctly 
        /// </summary>
        [UnityTest]
        public IEnumerator C4_ZombieMovement()
        {
            yield return null;
            Vector3 lastpos = zombie.transform.position;
            yield return new WaitForSecondsRealtime(2f);
            Assert.AreNotEqual(lastpos,zombie.transform.position);
        }

        /// <summary>
        /// @author Riyad K Rahman <br></br>
        /// tests if zombie targetting functions correctly 
        /// </summary>
        [UnityTest]
        public IEnumerator C4_ZombieRetarget()
        {
            yield return null;
            GameObject player2 = new GameObject();
            player2.tag = "Player";
            Vector3 pos = new Vector3(-15f, 0.5f, 1.5f);
            Quaternion rot = new Quaternion(0f, 0f, 0f, 0f);

            GameObject player2Instance = GameObject.Instantiate(player2, pos, rot);

            yield return new WaitForSecondsRealtime(4f);
            Assert.IsTrue(zombie.GetComponent<AICharacterControl>().GetTarget() == player2Instance.transform);
        }


        /// <summary>
        /// @author Riyad K Rahman <br></br>
        /// tests if player Respawn functions correctly 
        /// </summary>
        [UnityTest]
        public IEnumerator D1_PlayerRespawn()
        {
            yield return null;
            player.SetActive(false);
            yield return new WaitForSecondsRealtime(3f);
            Assert.IsTrue(player.transform.GetChild(4).gameObject.activeSelf == true);
        }
    }
}
