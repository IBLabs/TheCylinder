using TMPro;

using UnityEngine;
using UnityEngine.EventSystems;

namespace DefaultNamespace
{
    public class ClientUIController : MonoBehaviour
    {
        public NetworkManagerSetup networkManagerSetup;
        public TMP_InputField addressInputField;

        private void Start()
        {
            if (FindObjectsByType<ClientUIController>(FindObjectsSortMode.None).Length > 1)
            {
                Destroy(gameObject);
            }
            else
            {
                DontDestroyOnLoad(gameObject);
            }
        }

        public void StartClientWithConfiguredAddress()
        {
            string address = addressInputField.text;
            networkManagerSetup.StartClient(address);
        }
    }
}