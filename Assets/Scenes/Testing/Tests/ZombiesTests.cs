using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Photon.Pun;


namespace Tests
{
    public class ZombiesTests : MonoBehaviour
    {
        private Spawner spawner;

        [SetUp]
        public void Setup()
        {
            GameObject launcher = new GameObject();
            launcher.name = "Launcher";
            launcher.AddComponent<Launcher>();
            launcher.GetComponent<Launcher>().SetTesting(true); //ensures loading straight to TankGame scene
            PhotonNetwork.ConnectUsingSettings();

        }

        // Requires a timer since you need to connect, cant make setup an IEneumrator and cant launch CoRoutines either so this is required

        [UnityTest]
        public IEnumerator InstantiatePlayer_Enumerator()
        {
            yield return new WaitForSeconds(5f);
            PhotonNetwork.CreateRoom("test");

            //wait 5 sendons before spawning a player in 
            yield return new WaitForSeconds(5f);
            Vector3 pos = new Vector3(-8.44f, 0.5f, 3.4f);
            Quaternion rot = new Quaternion(0f, 0f, 0f, 0f);
            GameObject player = PhotonNetwork.Instantiate("Player_1", pos, rot);  //assigns the player to a new player gameobject    //assigns the player manager component of the new player 
            Assert.IsNotNull(player);

        }

        [UnityTest]
        public IEnumerator ZombieSpawnersInit()
        {
           yield return null;
            this.spawner = GameObject.FindGameObjectWithTag("EnemySpawn").GetComponent<Spawner>();
            Assert.AreEqual(this.spawner.spawners.Length, 5);
        }

        [UnityTest]
        public IEnumerator ZombiesInit()
        {
           yield return null;
           Assert.AreEqual(spawner.getRemaining(), 5);
        }

        
    }
}