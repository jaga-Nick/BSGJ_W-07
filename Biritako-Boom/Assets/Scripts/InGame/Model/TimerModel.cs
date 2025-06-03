using UnityEngine;

/// <summary>
/// 時間制御
/// </summary>
public class TimerModel
{
    [Header("制限時間")]
    [SerializeField]
    private float maxLimitTime;
    public float MaxLimitTime { get=>maxLimitTime; private set=>maxLimitTime=value; }

    /// <summary>
    /// 現在時間読み取り専用
    /// </summary>
    public float CurrentLimitTime { get; private set; }
    /// <summary>
    /// 初期化
    /// </summary>
    public void RestorTime()
    {
        CurrentLimitTime = MaxLimitTime;
    }
    /// <summary>
    /// 時間減少
    /// </summary>
    public void DecrementTime()
    {
        CurrentLimitTime -= Time.deltaTime;
    }
    /// <summary>
    /// 時間加算
    /// </summary>
    /// <param name="plusTime"></param>
    public void PlusTime(float plusTime)
    {
        CurrentLimitTime += plusTime;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="minusTime"></param>
    public void MinusTime(float minusTime)
    {
        CurrentLimitTime -= minusTime;
    }
}
