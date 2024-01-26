using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCubeTest : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        // ��ȡAnimator���
        animator = GetComponent<Animator>();

        // ���Ŷ���
        PlayFloatAnimation();
    }

    void PlayFloatAnimation()
    {
        // ������Ϊ"FloatAnimationClip"�Ķ���
        animator.SetTrigger("Die");
    }

    void Update()
    {
        // ��ĳ�������´������������簴�¿ո��
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayFloatAnimation();
        }

        // ��ȡ��ǰ����״̬��Ϣ
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // ��ӡ��ǰ״̬��ȫ��
        Debug.Log("Current Animation State: " + stateInfo);

    }

}
