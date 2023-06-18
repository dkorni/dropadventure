using Assets.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceCream : MonoBehaviour, ISmashable
{
    public GameObject Puddle;
    
    [Range(1, 100)]
    public float MeltSpeed = 20f;
    public AudioSource MeltingAudioSource;
    public AudioClip PuddleAppearanceAudioClip;
    public SkinnedMeshRenderer SkinnedMeshRenderer;
    public GameObject Confeti;

    private Animator animator;
    private AudioSource puddleAppearanceAudioSource;
    private Collider collider;

    private bool canMelt = true;

    private void Start()
    {
        Puddle.SetActive(false);
        puddleAppearanceAudioSource = Puddle.AddComponent<AudioSource>();
        puddleAppearanceAudioSource.volume = 0.4f;
        animator = GetComponent<Animator>();
        collider = GetComponent<Collider>();
    }

    public void Melt()
    {
        if (!canMelt)
            return;

        StartCoroutine(MeltCoroutine());
    }

    private IEnumerator MeltCoroutine()
    {
        MeltingAudioSource.Play();
        canMelt = false;
        var meltingValue = SkinnedMeshRenderer.GetBlendShapeWeight(0);

        if(meltingValue == 100)
        {
            Confeti.transform.parent = null;
            Confeti.SetActive(true);
            if (gameObject.activeSelf)
            {
                gameObject.SetActive(false);
            }
            Puddle.SetActive(true);
            puddleAppearanceAudioSource.PlayOneShot(PuddleAppearanceAudioClip);
            yield break;
        }

        meltingValue = Mathf.Min(meltingValue + MeltSpeed, 100);
        SkinnedMeshRenderer.SetBlendShapeWeight(0, meltingValue);
        yield return null;
        canMelt = true;
        MeltingAudioSource.Pause();
    }

    public void Smash(Collision collision)
    {
        animator.SetTrigger("Popup");
        transform.up = collision.transform.up;
        transform.SetParent(collision.transform);
        collider.enabled = true;
        transform.position = transform.position + new Vector3(0, 0.2f, 0);
        Puddle.transform.position = new Vector3(transform.position.x, Puddle.transform.position.y, transform.position.z);
    }

    public void Prepare()
    {
        // nothing
    }
}