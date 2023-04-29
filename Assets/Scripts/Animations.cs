using System.Collections;
using System;
using UnityEngine;
using TMPro;

public class cAnimation
{
    public static bool mFloatingTextsEnabled = true;
    public MonoBehaviour mParent;
    public cAnimation(MonoBehaviour parent)
    {
        mParent = parent;
    }


    public void ScaleByOverTime(float scale, float time, GameObject theObject, bool bounce, Action completion = null)
    {
        _ScaleByOverTime(scale, time, theObject, bounce, mParent, completion);
    }

    public void TranslateToOverTime(Vector3 dstPosition, float time, GameObject theObject, bool bounce, Action completion = null)
    {
        _TranslateToOverTime(dstPosition, time, theObject, bounce, mParent, completion);
    }

    public void Blink(float duration, float speed, GameObject theObject, Action completion = null)
    {
        _Blink(duration, speed, theObject, mParent, completion);
    }



    public void DisplayAnimatedText(Vector2 atLocationWorld, string text, Color color, float duration, float textSize, Transform parent)
    {
        if (!mFloatingTextsEnabled)
            return;

        Vector2 locationScreen = Camera.main.WorldToScreenPoint(atLocationWorld);
        GameObject floatingTextPrefab = Resources.Load<GameObject>("Prefabs/UI/FloatingText");
        GameObject newText = MonoBehaviour.Instantiate(floatingTextPrefab, locationScreen, Quaternion.Euler(0, 0, 0), parent);
        TextMeshProUGUI textMesh = newText.GetComponent<TextMeshProUGUI>();

        textMesh.color = color;
        textMesh.text = text;
        textMesh.fontSize = textSize;

        Vector2 offset = new Vector3(0, 200);
        TranslateToOverTime(locationScreen + offset, 1f, newText, false, () => { MonoBehaviour.Destroy(newText); });
    }










    // ========================================================
    // ========================================================
    // ========================================================
    // ========================================================
    // IMPLEMENTATIONS ======
    // ========================================================
    // ========================================================
    // ========================================================
    // ========================================================




    // Animates the rescaling of a given object
    struct sResizedObject
    {
        public Vector3 mDstScale;
        public Vector3 mDstPosition;
        public bool mActiveState;
        public GameObject mTheObject;
    }

    private sResizedObject mObjectBeingAnimated;
    private IEnumerator mResizeCorountine;
    private void CheckRunningAnimation(MonoBehaviour runner)
    {
        if (mObjectBeingAnimated.mTheObject != null)
        {
            mObjectBeingAnimated.mTheObject.transform.localScale = mObjectBeingAnimated.mDstScale;
            mObjectBeingAnimated.mTheObject.transform.position = mObjectBeingAnimated.mDstPosition;
            mObjectBeingAnimated.mTheObject.SetActive(mObjectBeingAnimated.mActiveState);
            runner.StopCoroutine(mResizeCorountine);
        }
    }


    private void _ScaleByOverTime(float scale, float time, GameObject theObject, bool bounce, MonoBehaviour animationHolder, Action completion = null)
    {
        CheckRunningAnimation(animationHolder);

        mObjectBeingAnimated.mTheObject = theObject;
        mObjectBeingAnimated.mDstScale = bounce ? theObject.transform.localScale : theObject.transform.localScale * scale;
        mObjectBeingAnimated.mDstPosition = theObject.transform.position;
        mObjectBeingAnimated.mActiveState = theObject.activeSelf;

        mResizeCorountine = ScaleByOverTimeRountine(scale, time, theObject, bounce, completion);
        animationHolder.StartCoroutine(mResizeCorountine);


    }
    private IEnumerator ScaleByOverTimeRountine(float scaleFactor, float time, GameObject theObject, bool bounce, Action completion = null)
    {
        float initTime = Time.fixedTime;
        float finalTime = initTime + time;

        float halfTime = initTime + time / 2.0f;
        float firstLoopTimeCheck = bounce ? halfTime : finalTime;

        Vector3 orgScale = theObject.transform.localScale;
        Vector3 dstScale = theObject.transform.localScale * scaleFactor;
        Vector3 safetyDSTScale = bounce ? theObject.transform.localScale : theObject.transform.localScale * scaleFactor;

        while (Time.fixedTime < firstLoopTimeCheck)
        {
            float deltaScale = (Time.fixedTime - initTime) / (firstLoopTimeCheck - initTime);
            theObject.transform.localScale = (dstScale * deltaScale) + (orgScale * (1 - deltaScale));
            yield return null;
        }

        if (bounce)
        {
            initTime = Time.fixedTime;
            finalTime = initTime + time / 2.0f;
            Vector3 tmp = dstScale;
            dstScale = orgScale;
            orgScale = tmp;
            while (Time.fixedTime < finalTime)
            {
                float deltaScale = (Time.fixedTime - initTime) / (finalTime - initTime);
                theObject.transform.localScale = (dstScale * deltaScale) + (orgScale * (1 - deltaScale));
                yield return null;
            }
        }

        theObject.transform.localScale = safetyDSTScale;
        mObjectBeingAnimated.mTheObject = null;

        if (completion != null)
            completion();
    }





    private void _TranslateToOverTime(Vector3 dstPosition, float time, GameObject theObject, bool bounce, MonoBehaviour animationHolder, Action complection = null)
    {
        CheckRunningAnimation(animationHolder);

        mObjectBeingAnimated.mTheObject = theObject;
        mObjectBeingAnimated.mDstScale = theObject.transform.localScale;
        mObjectBeingAnimated.mDstPosition = bounce ? theObject.transform.position : dstPosition;
        mObjectBeingAnimated.mActiveState = theObject.activeSelf;

        mResizeCorountine = TranslateToOverTimeRountine(dstPosition, time, theObject, bounce, complection);
        animationHolder.StartCoroutine(mResizeCorountine);
    }
    private IEnumerator TranslateToOverTimeRountine(Vector3 dstPosition, float time, GameObject theObject, bool bounce, Action complection = null)
    {
        float initTime = Time.fixedTime;
        float finalTime = initTime + time;

        float halfTime = initTime + time / 2.0f;
        float firstLoopTimeCheck = bounce ? halfTime : finalTime;

        Vector3 orgPos = theObject.transform.position;
        Vector3 dstPos = dstPosition;
        Vector3 safetyDSTPos = bounce ? theObject.transform.position : dstPosition;

        while (Time.fixedTime < firstLoopTimeCheck)
        {
            float deltaScale = (Time.fixedTime - initTime) / (firstLoopTimeCheck - initTime);
            theObject.transform.position = (dstPos * deltaScale) + (orgPos * (1 - deltaScale));
            yield return null;
        }

        if (bounce)
        {
            initTime = Time.fixedTime;
            finalTime = initTime + time / 2.0f;
            dstPos = orgPos;
            orgPos = dstPosition;
            while (Time.fixedTime < finalTime)
            {
                float deltaScale = (Time.fixedTime - initTime) / (finalTime - initTime);
                theObject.transform.position = (dstPos * deltaScale) + (orgPos * (1 - deltaScale));
                yield return null;
            }
        }

        theObject.transform.position = safetyDSTPos;
        mObjectBeingAnimated.mTheObject = null;

        if (complection != null)
            complection();
    }



    private void _Blink(float duration, float speed, GameObject theObject, MonoBehaviour animationHolder, Action complection = null)
    {
        CheckRunningAnimation(animationHolder);

        mObjectBeingAnimated.mTheObject = theObject;
        mObjectBeingAnimated.mDstScale = theObject.transform.localScale;
        mObjectBeingAnimated.mDstPosition = theObject.transform.position;
        mObjectBeingAnimated.mActiveState = theObject.activeSelf;

        mResizeCorountine = BlinkRountine(duration, speed, theObject, complection);
        animationHolder.StartCoroutine(mResizeCorountine);
    }
    private IEnumerator BlinkRountine(float duration, float speed, GameObject theObject, Action complection = null)
    {
        float initTime = Time.fixedTime;
        float finalTime = initTime + duration;

        float previousTimeStamp = initTime;
        float previousBlink = 0f;

        bool originalState = theObject.activeSelf;

        while (Time.fixedTime < finalTime)
        {
            float delta = Time.fixedTime - previousTimeStamp;
            previousTimeStamp = Time.fixedTime;
            previousBlink += delta;

            if (previousBlink > speed)
            {
                theObject.SetActive(!theObject.activeSelf);
                previousBlink = speed - previousBlink;
            }

            yield return null;
        }

        theObject.SetActive(originalState);
        mObjectBeingAnimated.mTheObject = null;

        if (complection != null)
            complection();
    }
}