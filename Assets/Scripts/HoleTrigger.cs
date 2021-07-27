using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleTrigger : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        ScreenLog.Log("made by the bag");
        if (other.CompareTag("Beanbag"))
        {
            ScoreController.Instance.UpdateScore(3);
        }
    }
}
