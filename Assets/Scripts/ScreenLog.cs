using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScreenLog : Singleton<ScreenLog>
{
    public TextMeshProUGUI logText;
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        logText.text = "Standing By..";
    }

    
    public static void Log(string message)
    {
        if(message[0] != '\n')
        {
            message = '\n' + message;
        }

        Instance.logText.text += message;

        Debug.Log(message);
    }
}
