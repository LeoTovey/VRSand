using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCubeTest : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        // 获取Animator组件
        animator = GetComponent<Animator>();

        // 播放动画
        PlayFloatAnimation();
    }

    void PlayFloatAnimation()
    {
        // 播放名为"FloatAnimationClip"的动画
        animator.SetTrigger("Die");
    }

    void Update()
    {
        // 在某个条件下触发动画，比如按下空格键
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayFloatAnimation();
        }

        // 获取当前动画状态信息
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // 打印当前状态的全名
        Debug.Log("Current Animation State: " + stateInfo);

    }

}
