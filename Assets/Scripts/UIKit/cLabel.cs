using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

class cLabel :
    cView
{
    public TextMeshProUGUI mText;

    public cLabel(GameObject parentView, string name) : this(name)
    {
        mGameObject.transform.SetParent(parentView.transform);
    }
    public cLabel(string name) : base(name)
    {
        mText = mGameObject.AddComponent<TextMeshProUGUI>();
        mText.fontSize = 18;
        mText.alignment = TextAlignmentOptions.Center;
    }
}