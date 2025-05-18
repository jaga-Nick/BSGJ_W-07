using UnityEngine;

/// <summary>
/// キャラクターに直アタッチ想定。(PlayerAttachに機能移行も視野に。
/// </summary>
public class PlayerAnimatorController : MonoBehaviour
{
    private Animator Animator;
    private Rigidbody2D _rb;

    //アニメーターのパラメーターをハッシュ化
    private static readonly int Vertical = Animator.StringToHash("Vertical");
    private static readonly int Horizontal = Animator.StringToHash("Horizontal");
    private static readonly int Horizontal_OnMove = Animator.StringToHash("Horizontal_OnMove");
    private static readonly int Vertical_OnMove = Animator.StringToHash("Vertical_OnMove");
    

    private void Awake()
    {
        Animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
    void Update()
    {
        //velocityを取得して移動しているかどうか
        var move = _rb.linearVelocity;

        //縦横の動きを送る。
        Animator.SetFloat(Vertical,move.y);
        Animator.SetFloat(Horizontal, move.x);
        //縦横方向に0でなければ。
        Animator.SetBool(Vertical_OnMove, move.y != 0);
        Animator.SetBool(Horizontal_OnMove, move.x != 0);
    }
}
