using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceCream : MonoBehaviour
{
    public GameObject Puddle;
    
    [Range(1, 100)]
    public float MeltSpeed = 20f;
    public SkinnedMeshRenderer SkinnedMeshRenderer;

    private bool canMelt = true;

    private void Start()
    {
        Puddle.SetActive(false);
    }

    public void Melt()
    {
        if (!canMelt)
            return;

        StartCoroutine(MeltCoroutine());
    }

    private IEnumerator MeltCoroutine()
    {
        canMelt = false;
        var meltingValue = SkinnedMeshRenderer.GetBlendShapeWeight(0);

        if(meltingValue == 100)
        {
            if (gameObject.activeSelf)
            {
                gameObject.SetActive(false);
            }
            Puddle.SetActive(true);
            yield break;
        }

        meltingValue = Mathf.Min(meltingValue + MeltSpeed, 100);
        SkinnedMeshRenderer.SetBlendShapeWeight(0, meltingValue);
        yield return null;
        canMelt = true;
    }
}