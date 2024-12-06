
using System;
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class TimerStart : UdonSharpBehaviour
{
    [SerializeField] TextMeshProUGUI textMeshStart;
    [SerializeField] TextMeshProUGUI textMeshGoal;

    [UdonSynced] private long startTick = 0;
    [UdonSynced] private long goalTick = 0;

    public override void Interact()
    {
        // 既に開始されていたら、処理しない
        if (0 != startTick)
        {
            return;
        }

        // スタート時間を記録
        CountStart();

        return;
    }

    void FixedUpdate()
    {
        // 既に開始されていなければ、処理しない
        if (0 == startTick)
        {
            return;
        }

        // 経過時間を算出
        DateTime elapsedTime = DateTime.Now;

        //ゴールしていれば、ゴール時間で経過時間を算出
        if (0 != goalTick)
        {
            elapsedTime = new DateTime(this.goalTick);
        }

        // スタート時間から経過時間を表示
        var timeSpan = elapsedTime - new DateTime(this.startTick);
        var timerStr = timeSpan.ToString(@"h\:mm\:ss");

        textMeshStart.text = timerStr;
        textMeshGoal.text = timerStr;

        return;
    }

    public void CountStart()
    {
        // オーナ権限を取得
        if (false == Networking.LocalPlayer.IsOwner(this.gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
        }

        // スタート時間を設定
        startTick = DateTime.Now.Ticks;

        // 同期処理
        RequestSerialization();

        return;
    }

    public void CountStop()
    {
        // オーナ権限を取得
        if (false == Networking.LocalPlayer.IsOwner(this.gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
        }

        // ゴール時間を設定
        goalTick = DateTime.Now.Ticks;

        // 同期処理
        RequestSerialization();

        return;
    }
}
