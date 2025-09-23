using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnManager : MonoBehaviour
{
    public Transform[] spawnPoints;
    private int nextSpawnIndex = 0;

    // PlayerInputManager (on Game) will call this via SendMessage
    void OnPlayerJoined(PlayerInput player)
    {
        SpawnAtPoint(player);
    }

    private void SpawnAtPoint(PlayerInput player)
    {
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            Transform sp = spawnPoints[nextSpawnIndex % spawnPoints.Length];

            // get CharacterController height to offset the feet
            var cc = player.GetComponent<CharacterController>();
            float yOffset = (cc != null) ? cc.height / 2f : 1f;

            // apply spawn pos + offset
            Vector3 spawnPos = sp.position + Vector3.up * yOffset;
            player.transform.position = spawnPos;
            player.transform.rotation = sp.rotation;

            Debug.Log($"Spawned player {player.playerIndex} at {spawnPos} (offset {yOffset})");

            nextSpawnIndex++;
        }
        else
        {
            Vector3 fallback = new Vector3(player.playerIndex * 3f, 1f, 0f);
            player.transform.position = fallback;
            Debug.LogWarning($"No spawn points assigned. Using fallback {fallback}");
        }
    }
}
