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
}
