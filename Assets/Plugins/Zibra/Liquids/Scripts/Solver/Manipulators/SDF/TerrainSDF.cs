#if ZIBRA_LIQUID_PRO_VERSION
using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.Rendering;
using com.zibra.liquid.Manipulators;
using UnityEngine.Serialization;

namespace com.zibra.liquid.SDFObjects
{
    /// <summary>
    ///     Class containing terrain SDF.
    /// </summary>
    /// <remarks>
    ///     Terrain SDF is an SDF approximation based on the terrain's heightmap.
    ///     Since this is an approximation it might be worse in quality in some cases compared to other SDF types.
    /// </remarks>
    [AddComponentMenu("Zibra/Zibra Terrain SDF")]
    [DisallowMultipleComponent]
    public class TerrainSDF : SDFObject
    {
#region Public Interface

        /// <summary>
        ///     Returns size of bounding box for current shape.
        /// </summary>
        public Vector3 GetBBoxSize()
        {
            Terrain terrain = GetComponent<Terrain>();
            if (terrain != null)
            {
                return terrain.terrainData.bounds.size;
            }
            else
            {
                return Vector3.zero;
            }
        }

        public override ulong GetVRAMFootprint(DataStructures.ZibraLiquidSolverParameters param)
        { 
            Terrain terrain = GetComponent<Terrain>();
            if (terrain != null)
            {
                // 2 is the number of bytes per texel in the heightmap texture (R16Float)
                return (ulong) (param.HeightmapResolution * param.HeightmapResolution) * 2;
            }
            else
            {
                return 0;
            }
        }

#endregion
    }
}
#endif