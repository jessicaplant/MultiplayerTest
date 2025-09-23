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

            var cc = player.GetComponent<CharacterController>();
            bool wasEnabled = false;
            if (cc != null)
            {
                wasEnabled = cc.enabled;
                cc.enabled = false; // prevent the capsule from resolving collisions mid-teleport
            }

            Vector3 spawnPos = sp.position;
            player.transform.SetPositionAndRotation(spawnPos, sp.rotation);

            if (cc != null)
            {
                Physics.SyncTransforms();
                cc.enabled = wasEnabled;
            }

            Debug.Log($"Spawned player {player.playerIndex} at {spawnPos})");

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
