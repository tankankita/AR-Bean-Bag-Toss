using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    public void OnTriggerEnter(Collider other)
    {
        ScreenLog.Log("Hit by the bag");
        if (other.CompareTag("Beanbag"))
        {
            ScoreController.Instance.UpdateScore(1);
        }
    }

    // Start is called before the first frame update
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Beanbag"))
        {
            ScoreController.Instance.UpdateScore(-1);
        }
    }
}
