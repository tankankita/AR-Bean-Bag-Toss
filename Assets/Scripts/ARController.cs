using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;
[System.Serializable]
public class BooleanEvent : UnityEvent<bool> {};

public class ARController : MonoBehaviour
{
    [SerializeField] private bool allowARMode = true;
    [SerializeField] private ARSession aRSession;
    [SerializeField] private ARSessionOrigin arOrigin;
    [SerializeField] private ARPlaneManager planeManager;
    [SerializeField] private GameObject scaneRoomUI;
    [SerializeField] private Camera defaultCamera;

    public static BooleanEvent OnARRunning = new BooleanEvent();
    private bool IsARAvailable => (allowARMode &&
        ARSession.state != ARSessionState.Unsupported &&
        ARSession.state != ARSessionState.NeedsInstall);

    private bool IsARRunning => (allowARMode && ARSession.state >= ARSessionState.Ready);
    // Start is called before the first frame update
    void Start()
    {
#if UNITY_EDITOR
        allowARMode = false;
#endif

        if (allowARMode)
        {
            ScreenLog.Log("AR mode = true");

           EnableAR(true);
            StartCoroutine(_WaitForARReady());
        }
        else
        {
            EnableAR(false);
            OnARRunning.Invoke(false);
        }

    }

    private IEnumerator _WaitForARReady()
    {
        bool checking = true;
        while(checking)
        {
            ScreenLog.Log("IsARAvailable " + IsARAvailable);
            if (!IsARAvailable)
            {
                ScreenLog.Log("AR is not supported on this device");
                OnARRunning.Invoke(false);
                yield break;
            }
           

            if (IsARRunning)
            {
                checking = false;
            }
            yield return null;
        }

        ScreenLog.Log("AR supported and running");

        _PromptToScan(true);

        while(planeManager.trackables.count == 0)
        {
            yield return null;
        }

        _PromptToScan(false);
        ScreenLog.Log("Tracking plane");
        OnARRunning.Invoke(true);

    }

    private void _PromptToScan(bool v)
    {
        scaneRoomUI.SetActive(v);

    }

    private void EnableAR(bool v)
    {
        aRSession.gameObject.SetActive(enabled);
        arOrigin.gameObject.SetActive(enabled);

        defaultCamera?.gameObject.SetActive(!enabled);
    }
}