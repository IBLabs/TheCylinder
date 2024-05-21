using System;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class UIConsoleLogger : MonoBehaviour
{
    public TextMeshProUGUI logText;
    private Queue<string> logQueue = new Queue<string>();
    private string currentLog;
    
    private void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
        
        Debug.Log("[TEST]: testing, 1, 2...");
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        string newLog = string.Format("{0}: {1}\n", type, logString);
        logQueue.Enqueue(newLog);
        
        if (logQueue.Count > 15)
        {
            logQueue.Dequeue();
        }

        currentLog = string.Join("", logQueue.ToArray());
        logText.text = currentLog;
    }
}
