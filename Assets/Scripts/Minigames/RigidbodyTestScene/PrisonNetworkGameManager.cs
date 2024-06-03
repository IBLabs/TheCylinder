using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Unity.Netcode;

using UnityEngine;

public class PrisonNetworkGameManager : NetworkBehaviour
{
    private const int LIGHTS_ON_TO_WIN = 3;

    [SerializeField] private SceneLoader sceneLoader;

    private int lightsOnCount = 0;

    public void OnLightTurnedOn()
    {
        if (!IsServer) return;

        lightsOnCount++;

        if (lightsOnCount >= LIGHTS_ON_TO_WIN)
        {
            Debug.Log("Lights on count reached, desktop players wins!");
            StartCoroutine(ReloadSceneAfterDelay());
        }
    }

    public void OnPlayerKilled()
    {
        StartCoroutine(CheckAllPlayersDead());
    }

    private IEnumerator ReloadSceneAfterDelay()
    {
        yield return new WaitForSeconds(3);
        sceneLoader.ReloadScene();
    }

    private IEnumerator CheckAllPlayersDead()
    {
        yield return new WaitForEndOfFrame();

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        var allPlayersDead = players.Length == 0 || players.All(player => player == null);

        Debug.Log("players death state: " + allPlayersDead + " " + players.Length);

        if (allPlayersDead)
        {
            Debug.Log("VR player wins!");
            StartCoroutine(ReloadSceneAfterDelay());
        }
    }
}
