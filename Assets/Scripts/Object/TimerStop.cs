
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class TimerStop : UdonSharpBehaviour
{
    [SerializeField] TimerStart timerStart;

    public override void Interact()
    {
        timerStart.SendCustomEvent("CountStop");
    }
}
