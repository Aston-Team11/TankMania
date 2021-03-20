using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameSession : MonoBehaviourPunCallbacks
{
    [SerializeField] private bool ingame = false;

      public bool getGameSession()
      {
          return ingame;
      }
  
      public void setGameSession()
      {
          photonView.RPC("Ingame", RpcTarget.AllBuffered);
      }
  
      /// <summary>
      /// informs other players that the game session has already started 
      /// </summary>
      [PunRPC]
      public void Ingame()
      {
          ingame = true;
      }




}
