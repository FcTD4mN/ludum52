using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldCollectable : Collectable
{
    public int maxValue = 1000;
    public int minValue = 10;
    public int desiredValue = 0;

    void OnEnable()
    {
        Initialize();
        label = "Gold";
        value = Random.Range(minValue, maxValue);

        if (desiredValue != 0)
            SetDesiredValue(desiredValue);
    }

    public override void Collect()
    {
        base.Collect();

        GameManager.mResourceManager.AddResource(cResourceDescriptor.eResourceNames.Gold.ToString(), value, true);
        GameManager.mUIManager.FloatingMessage(((int)value).ToString(), Color.yellow, transform.position);
    }

    public void SetDesiredValue(int desired)
    {
        value = desired;
        float scaleRatio = (value - minValue) / (maxValue - minValue);
        Vector3 newScale = new Vector3(
            (transform.localScale.x / 2) + (transform.localScale.x * scaleRatio),
            (transform.localScale.y / 2) + (transform.localScale.y * scaleRatio),
            (transform.localScale.z / 2) + (transform.localScale.y * scaleRatio)
        );

        transform.localScale = newScale;
    }
}
