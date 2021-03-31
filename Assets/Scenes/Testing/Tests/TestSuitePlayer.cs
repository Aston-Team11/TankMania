using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Photon.Pun;

namespace Tests
{
    /// <summary>
    /// @author Riyad K Rahman <br></br>
    /// preforms a series of unit tests on the playerManager class
    /// </summary>
    public class TestSuitePlayer
    {
        GameObject player;                              //the new player instantiated 
        PlayerManager playerManagerstats;              //the player manager component of the new player 

        /// <summary>
        /// @author Riyad K Rahman <br></br>
        /// connects to the server before running any test 
        /// </summary>
        [SetUp]
        public void Setup()
        {
            GameObject launcher = new GameObject();
            launcher.name = "Launcher";
            launcher.AddComponent<Launcher>();
            launcher.GetComponent<Launcher>().SetTesting(true); //ensures loading straight to TankGame scene
            PhotonNetwork.ConnectUsingSettings();
        }

        /// <summary>
        /// @author Riyad K Rahman <br></br>
        /// this test instantiates a player and checks if it is loaded in the scene
        /// This is also the very first test and therefore will handle loading the level and spawning a player in the scene
        /// </summary>
        [UnityTest]
        public IEnumerator A1_InstantiatePlayer_Enumerator()
        {

            //waits 5 seconds before creating a room 
            yield return new WaitForSeconds(5f);
            PhotonNetwork.CreateRoom("test");

            //wait 5 sendons before spawning a player in 
            yield return new WaitForSeconds(5f);
            Vector3 pos = new Vector3(-8.44f, 0.5f, 3.4f);
            Quaternion rot = new Quaternion(0f, 0f, 0f, 0f);
            this.player = PhotonNetwork.Instantiate("Player_2", pos, rot);  //assigns the player to a new player gameobject 
            this.playerManagerstats = this.player.GetComponent<PlayerManager>();   //assigns the player manager component of the new player 
            Assert.IsNotNull(this.player);
        }

        [UnityTest]
        public IEnumerator A2_PlayerDamage_Enumerator()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
            this.playerManagerstats.DamagePlayer(10);
            Assert.AreEqual(90, this.playerManagerstats.GetHealth());
        }

        [UnityTest]
        public IEnumerator A3_PlayerGamemodeCheck_FFA_Enumerator()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
            this.playerManagerstats.GameModeSetup(1);
            Assert.AreEqual(true, this.playerManagerstats.GetFFACanvas().activeSelf);

        }

        /// <summary>
        /// tests if the player has changed rotations
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator A4_PlayerMove_Enumerator()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;

            float initialrot = this.player.transform.rotation.y;        //the starting rotation

            //simulate player input 
            this.player.GetComponent<PlayerMovement>().direction.x = -15;
            yield return new WaitForEndOfFrame();
            this.player.GetComponent<PlayerMovement>().direction.x = -15;


            yield return new WaitForSeconds(3f);

            if (this.player.transform.rotation.y < initialrot)
            {
                Assert.IsTrue(true);
            }
            else
            {
                Assert.IsTrue(false);
            }
        }

        [UnityTest]
        public IEnumerator A5_PlayerShooting()
        {
            yield return null;
            this.player.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<PhotonView>().RPC("Shoot", RpcTarget.All, false);
            yield return new WaitForEndOfFrame();
            Assert.IsNotNull(GameObject.FindGameObjectWithTag("Bullet"));
            GameObject.Destroy(GameObject.FindGameObjectWithTag("Bullet"));
        }

        [UnityTest]
        public IEnumerator A6_AdditionalLife()
        {
            yield return null;
            this.playerManagerstats.AddLife();
            yield return new WaitForEndOfFrame();
            Assert.AreEqual(this.playerManagerstats.GetPlayerLives(),1001);
        }

        [UnityTest]
        public IEnumerator A7_HealthUp()
        {
            yield return null;
            this.playerManagerstats.AddHealth(50);
            yield return new WaitForEndOfFrame();
            Assert.AreEqual(90, this.playerManagerstats.GetHealth()); 
        }
        [UnityTest]
        public IEnumerator A8_Shotgun()
        {
            yield return null;
            this.player.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<PhotonView>().RPC("Shotgun", RpcTarget.All);
            yield return new WaitForEndOfFrame();
            Assert.AreEqual(GameObject.FindGameObjectsWithTag("Bullet").Length,3);
        }
        [UnityTest]
        public IEnumerator A9_Minigun()
        {
            yield return new WaitForSeconds(2f);
            for (int i = 0; i < 5; i++) { this.player.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<PhotonView>().RPC("Shoot", RpcTarget.All, true); }
            
            Assert.IsTrue(GameObject.FindGameObjectsWithTag("Bullet").Length > 3);
        }

        [UnityTest]
        public IEnumerator B1_Shield()
        {
            yield return null;
            this.player.GetComponent<PhotonView>().RPC("spawnShield", RpcTarget.All);
            yield return new WaitForEndOfFrame();
            Assert.IsTrue(this.player.transform.GetChild(1).gameObject.activeSelf);
        }

     /*   [UnityTest]
        public IEnumerator B2_Slowmo()
        {
            // RIYAD
            yield return null;
            this.player.GetComponent<PlayerPowerupManager>().PowerupAttained("SLOMO");
            this.player.GetComponent<PlayerPowerupManager>().setSpacePressed(true);
            yield return new WaitForEndOfFrame();
            Assert.IsTrue(Time.timeScale != 1);
        }*/




        [UnityTest]
        public IEnumerator B3_ZombieSpawnersInit()
        {
            yield return null;
            GameObject.FindGameObjectWithTag("EnemySpawn").GetComponent<Spawner>();
            Assert.AreEqual(GameObject.FindGameObjectWithTag("EnemySpawn").GetComponent<Spawner>().spawners.Length, 5);
        }

        [UnityTest]
        public IEnumerator B4_ZombiesInit()
        {
            yield return null;
            Assert.AreEqual(GameObject.FindGameObjectWithTag("EnemySpawn").GetComponent<Spawner>().getRemaining(), 5);
        }
        [UnityTest]
        public IEnumerator B5_KillsCounter()
        {
            yield return null;
            this.playerManagerstats.AddKill(1,1);
            yield return new WaitForSeconds(1);
            Assert.AreEqual(1, this.playerManagerstats.GetKill());
        }






        // [Category ("dc")]
        // [UnityTest]
        // public IEnumerator Disconnect()
        // {
        //     PhotonNetwork.Disconnect();
        //     yield return new WaitForSeconds(5f);
        //
        //     Assert.IsTrue(PhotonNetwork.IsConnected);
        //
        // }
    }
}
