using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombTrajectory : MonoBehaviour
{
    [SerializeField] int numberOfDots;
    [SerializeField] GameObject dotsParent;
    [SerializeField] GameObject dotPrefab;
    [SerializeField] float dotSpacing;
    [SerializeField][Range(0.01f, 0.2f)] float dotMinScale;
    [SerializeField][Range(0.2f, 1f)] float dotMaxScale;

    Transform[] dotsList;
    Vector2 pos;
    float timeStamp;

    void OnEnable()
    {
        Hide();
        PrepareDots();
    }

    void PrepareDots()
    {
        dotsList = new Transform[numberOfDots];
        dotPrefab.transform.localScale = Vector3.one * dotMaxScale;

        float scale = dotMaxScale;
        float scaleFactor = scale / numberOfDots;

        for (int i = 0; i < numberOfDots; i++)
        {
            dotsList[i] = Instantiate(dotPrefab, null).transform;
            dotsList[i].parent = dotsParent.transform;
            dotsList[i].localScale = Vector3.one * scale;
            if (scale > dotMinScale)
                scale -= scaleFactor;

        }
    }

    public void UpdateDots(Vector3 startPos, Vector2 forceApplied)
    {
        timeStamp = dotSpacing;
        for (int i = 0; i < numberOfDots; i++)
        {
            pos.x = (startPos.x + forceApplied.x * timeStamp);
            pos.y = (startPos.y + forceApplied.y * timeStamp) - (Physics2D.gravity.magnitude * timeStamp * timeStamp) / 2f;

            dotsList[i].position = pos;
            timeStamp += dotSpacing;
        }
    }

    public void Show()
    {
        dotsParent.SetActive(true);
    }

    public void Hide()
    {
        dotsParent.SetActive(false);
    }
}
