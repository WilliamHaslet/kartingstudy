using UnityEngine;

public static class UtilityFunctions
{

    public static float Remap(float value, float inMin, float inMax, float outMin, float outMax)
    {

        return outMin + (value - inMin) * (outMax - outMin) / (inMax - inMin);

    }

    public static bool LayerInMask(int layer, LayerMask mask)
    {

        int layerBitmask = 1 << layer;

        return (layerBitmask & mask.value) > 0;

    }

}
