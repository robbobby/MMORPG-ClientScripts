using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static GameManager singletonInstance;
    public static Dictionary<int, PlayerManager> players = new Dictionary<int, PlayerManager>();
    public GameObject localPlayerPrefab;
    public GameObject playerPrefab;
    private void Awake() {
        GenerateSingleton();
    }

    private void GenerateSingleton() {
        if (singletonInstance == null) {
            singletonInstance = this;
        }
        else if (singletonInstance != this) {
            Debug.Log("Instance already exists, destroying instance");
            Destroy(this);
        }
    }

    public void SpawnPlayer(int id, string userName, Vector3 position, Quaternion rotation) {
        GameObject player;
        if(id == Client.SingletonInstance.clientId)
            player = Instantiate(localPlayerPrefab, position, rotation);
        else {
            player = Instantiate(playerPrefab, position, rotation);
        }

        PlayerManager  playerManager = player.GetComponent<PlayerManager>();
        playerManager.Id = id; 
        playerManager.Username = userName;
        players.Add(id, player.GetComponent<PlayerManager>());
    }
}
