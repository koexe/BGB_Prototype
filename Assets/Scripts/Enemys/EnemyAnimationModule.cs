using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationModule : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer spriteRenderer;

    public void Initialization(RuntimeAnimatorController _anim)
    {
        this.animator.runtimeAnimatorController = _anim;
    }
    public void Flip(bool _isFlip)
    {
        this.spriteRenderer.flipX = _isFlip;
    }

    public void PlayIdleAnimation()
    {
        this.animator.Play("Idle");
    }
    public void PlayAttackAnimation()
    {
        this.animator.Play("Attack");
    }
    public void PlayWalkAnimation()
    {
        this.animator.Play("Walk");
    }
    public void PlayDeadAnimation()
    {
        this.animator.Play("Dead");
    }
}
