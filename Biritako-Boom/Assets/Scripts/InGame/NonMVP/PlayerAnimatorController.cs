using UnityEngine;

/// <summary>
/// �L�����N�^�[�ɒ��A�^�b�`�z��B(PlayerAttach�ɋ@�\�ڍs������ɁB
/// </summary>
public class PlayerAnimatorController : MonoBehaviour
{
    private Animator Animator;
    private Rigidbody2D _rb;

    //�A�j���[�^�[�̃p�����[�^�[���n�b�V����
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
        //velocity���擾���Ĉړ����Ă��邩�ǂ���
        var move = _rb.linearVelocity;

        //�c���̓����𑗂�B
        Animator.SetFloat(Vertical,move.y);
        Animator.SetFloat(Horizontal, move.x);
        //�c��������0�łȂ���΁B
        Animator.SetBool(Vertical_OnMove, move.y != 0);
        Animator.SetBool(Horizontal_OnMove, move.x != 0);
    }
}
