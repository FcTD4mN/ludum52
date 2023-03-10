using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeRemoveBehaviour : StateMachineBehaviour
{
    public float fadeTime = 0.5f;
    public float delayTime = 0.2f;
    private float timeElapsed = 0f;
    private float delayElapsed = 0.0f;
    SpriteRenderer sr;
    GameObject objToRemove;
    Color startColor;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timeElapsed = 0f;
        sr = animator.GetComponent<SpriteRenderer>();
        startColor = sr.color;
        objToRemove = animator.gameObject;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (delayTime > delayElapsed)
            delayElapsed += Time.deltaTime;
        else
        {
            timeElapsed += Time.deltaTime;
            float newAlpha = startColor.a * (1 - timeElapsed / fadeTime);
            sr.color = new Color(startColor.r, startColor.g, startColor.b, newAlpha);
            if (timeElapsed > fadeTime)
                Destroy(objToRemove);
        }
    }
}
