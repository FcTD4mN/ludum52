using UnityEngine;
using UnityEngine.UI;

class cImage :
    cView
{
    internal Image mImage;

    public cImage(GameObject parentView, string name) : this(name)
    {
        mGameObject.transform.SetParent(parentView.transform);
    }
    public cImage(string name) : base(name)
    {
        mImage = mGameObject.AddComponent<Image>();
    }


    public void SetImageFromUnityResources( string name )
    {
        mImage.sprite = UnityEditor.AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/" + name + ".psd");
        mImage.type = Image.Type.Sliced;
        mImage.fillCenter = true;
    }
    public void SetImageFromUserResources( string name )
    {
        // TODO
    }
}