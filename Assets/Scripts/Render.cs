using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Obi;
using UnityEngine;

public class Render : MonoBehaviour
{
    [SerializeField] private ObiFluidRenderer _mainRenderer;

    [SerializeField] private ObiFluidRenderer _connectableRenderer;

    public void UpdateRender(ObiParticleRenderer renderer)
    {
        var connectableParticleRendList = _connectableRenderer.particleRenderers.ToList();
        connectableParticleRendList.Remove(renderer);
        _connectableRenderer.particleRenderers = connectableParticleRendList.ToArray();

        var mainParticleRendList = _mainRenderer.particleRenderers.ToList();
        mainParticleRendList.Add(renderer);
        _mainRenderer.particleRenderers = mainParticleRendList.ToArray();
    }
}
