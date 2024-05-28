using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Bootstrapper : MonoBehaviour
{
    public UnityEvent onBootstrapped;
    
    // Start is called before the first frame update
    void Start()
    {
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("ChoiceNetworkScene", UnityEngine.SceneManagement.LoadSceneMode.Additive).completed += OnSceneLoaded;
    }

    private void OnSceneLoaded(AsyncOperation asyncOp)
    {
        onBootstrapped.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
