using System.Collections;
using System;
using UnityEngine;

public class Utilities
{
    public static float PI = 3.14f;

    //========================================
    // Timer Tools
    //========================================


    public static IEnumerator ExecuteAfter(float time, Action onFinish)
    {
        yield return new WaitForSeconds(time);
        onFinish();
    }


    public static Rect Intersection(Rect rectA, Rect rectB)
    {
        float xMin = Math.Max(rectA.xMin, rectB.xMin);
        float xMax = Math.Min(rectA.xMax, rectB.xMax);
        float yMin = Math.Max(rectA.yMin, rectB.yMin);
        float yMax = Math.Min(rectA.yMax, rectB.yMax);


        if (xMin > xMax || yMin > yMax)
            return new Rect(0, 0, 0, 0);

        return new Rect(xMin, yMin, xMax - xMin, yMax - yMin);
    }


    public static Rect Union(Rect rectA, Rect rectB)
    {
        float xMin = Math.Min(rectA.xMin, rectB.xMin);
        float xMax = Math.Max(rectA.xMax, rectB.xMax);
        float yMin = Math.Min(rectA.yMin, rectB.yMin);
        float yMax = Math.Max(rectA.yMax, rectB.yMax);

        return new Rect(xMin, yMin, xMax - xMin, yMax - yMin);
    }


    // Returns BoundingBox base on transform in world space
    public static Rect GetBBoxFromTransform(GameObject obj)
    {
        Vector2 center = obj.transform.position;
        Vector2 size = obj.transform.localScale;

        Vector2 topLeft = center - size / 2;

        return new Rect(topLeft, size);
    }


    public static Rect ScreenToWorldRect(Rect rect)
    {
        Vector2 topLeft = new Vector2(rect.xMin, rect.yMin);
        Vector2 bottomRight = new Vector2(rect.xMax, rect.yMax);

        Vector2 topLeftMapped = Camera.main.ScreenToWorldPoint(topLeft);
        Vector2 bottomRightMapped = Camera.main.ScreenToWorldPoint(bottomRight);

        return new Rect(topLeftMapped, bottomRightMapped - topLeftMapped);
    }


    public static Rect WorldToScreenRect(Rect rect)
    {
        Vector2 topLeft = new Vector2(rect.xMin, rect.yMin);
        Vector2 bottomRight = new Vector2(rect.xMax, rect.yMax);

        Vector2 topLeftMapped = Camera.main.WorldToScreenPoint(topLeft);
        Vector2 bottomRightMapped = Camera.main.WorldToScreenPoint(bottomRight);

        return new Rect(topLeftMapped, bottomRightMapped - topLeftMapped);
    }

    public static void SetObjectToRect(GameObject gObj, Rect rect)
    {
        Vector2 bottomLeft = new Vector2(rect.x, rect.y);
        gObj.transform.localScale = new Vector2(rect.width, rect.height);
        gObj.transform.position = bottomLeft + new Vector2(rect.width / 2, rect.height / 2);
    }


    public static bool IsObjectOnScreen(GameObject theObject)
    {
        Rect objectBBoxScreen = WorldToScreenRect(GetBBoxFromTransform(theObject));
        Rect intersection = Intersection(Camera.main.pixelRect, objectBBoxScreen);

        return intersection.width > 0 && intersection.height > 0;
    }



    public static string FormatSecondsToMinuteAndSeconds(float seconds)
    {
        float minutes = (int)(seconds / 60f);
        float remainingSeconds = (int)seconds % 60;

        string minuteZero = minutes < 10 ? "0" : "";
        string secondZero = remainingSeconds < 10 ? "0" : "";

        return minuteZero + minutes + ":" + secondZero + remainingSeconds;
    }

    public static float GetScreenToWorldHeight()
    {
        Vector2 topRightCorner = new Vector2(1, 1);
        Vector2 edgeVector = Camera.main.ViewportToWorldPoint(topRightCorner);
        var height = edgeVector.y * 2;
        return height;
    }

    public static float GetScreenToWorldWidth()
    {
        Vector2 topRightCorner = new Vector2(1, 1);
        Vector2 edgeVector = Camera.main.ViewportToWorldPoint(topRightCorner);
        var width = edgeVector.x * 2;
        return width;

    }
}
