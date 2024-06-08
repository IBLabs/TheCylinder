using System;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class UIConsoleLogger : MonoBehaviour
{
    public TextMeshProUGUI logText;

    public InputActionProperty desktopToggleConsoleAction;
    public InputActionProperty xrToggleConsoleAction;


    private Queue<string> logQueue = new Queue<string>();
    private string currentLog;

    private void OnEnable()
    {
        Application.logMessageReceived += HandleLog;

        Debug.Log("UIConsoleLogger enabled");

        RegisterActions();
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;

        UnregisterActions();
    }

    private void RegisterActions()
    {
        desktopToggleConsoleAction.action.performed += OnToggleConsoleActionPerformed;
        xrToggleConsoleAction.action.performed += OnToggleConsoleActionPerformed;
    }

    private void UnregisterActions()
    {
        desktopToggleConsoleAction.action.performed -= OnToggleConsoleActionPerformed;
        xrToggleConsoleAction.action.performed -= OnToggleConsoleActionPerformed;
    }

    private void OnToggleConsoleActionPerformed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            logText.enabled = !logText.enabled;
        }
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
