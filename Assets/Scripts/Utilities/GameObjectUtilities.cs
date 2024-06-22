using System.Linq;

using UnityEngine;

public static class GameObjectUtilities
{
    public static void FindComponentWithTagInChildren<T>(this GameObject gameObject, string tag, out T component) where T : Component
    {
        component = null;
        foreach (Transform child in gameObject.transform)
        {
            if (child.CompareTag(tag))
            {
                component = child.GetComponent<T>();
                return;
            }
            FindComponentWithTagInChildren(child.gameObject, tag, out component);
        }
    }

    public static bool CheckAllPlayersDead()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        var allPlayersDead = players.Length == 0 || players.All(player => player == null);

        Debug.Log("players death state: " + allPlayersDead + " " + players.Length);

        if (allPlayersDead)
        {
            Debug.Log("[TEST]: all players are dead");
            return true;
        }

        return false;
    }
}
