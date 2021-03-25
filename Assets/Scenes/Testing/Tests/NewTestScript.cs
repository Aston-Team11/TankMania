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
    public class TestScriptPlayerManager
    {
        GameObject player;                      //the new player instantiated 
        PlayerManager playerstats;              //the player manager component of the new player 

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
        public IEnumerator InstantiatePlayer_Enumerator()
        {

            //waits 5 seconds before creating a room 
            yield return new WaitForSeconds(5f);
            PhotonNetwork.CreateRoom("test");

            //wait 5 sendons before spawning a player in 
            yield return new WaitForSeconds(5f);
            Vector3 pos = new Vector3(-8.44f, 0.5f, 3.4f);
            Quaternion rot = new Quaternion(0f, 0f, 0f, 0f);
            this.player = PhotonNetwork.Instantiate("Player_1", pos, rot);  //assigns the player to a new player gameobject 
            this.playerstats = this.player.GetComponent<PlayerManager>();   //assigns the player manager component of the new player 
            Assert.IsNotNull(this.player);
        }

        [UnityTest]
        public IEnumerator PlayerDamage_Enumerator()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
            this.playerstats.DamagePlayer(10);
            Assert.AreEqual(90, this.playerstats.GetHealth());
            
        }

        [UnityTest]
        public IEnumerator PlayerGamemodeCheck_FFA_Enumerator()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
            this.playerstats.GameModeSetup(1);
            Assert.AreEqual(true, this.playerstats.GetFFACanvas().activeSelf);
           
        }
    }
}
