using System;
using UnityEngine;

[Serializable]
public class Layer
{

    [SerializeField] private int value;

    public override int GetHashCode()
    {

        return base.GetHashCode();

    }

    public override bool Equals(object obj)
    {

        bool isEqual = false;

        if (obj is Layer)
        {

            isEqual = EqualsLayer((Layer)obj);

        }
        else if (obj is int)
        {

            isEqual = EqualsInt((int)obj);

        }

        return isEqual;

    }

    public bool EqualsLayer(Layer layer)
    {

        return value == layer.value;

    }

    public bool EqualsInt(int layer)
    {

        return value == layer;

    }
    
    public static bool operator ==(Layer layer1, Layer layer2)
    {

        return layer1.EqualsLayer(layer2);

    }
    
    public static bool operator ==(int layer1, Layer layer2)
    {

        return layer2.EqualsInt(layer1);

    }
    
    public static bool operator ==(Layer layer1, int layer2)
    {

        return layer1.EqualsInt(layer2);

    }

    public static bool operator !=(Layer layer1, Layer layer2)
    {

        return !layer1.EqualsLayer(layer2);

    }

    public static bool operator !=(int layer1, Layer layer2)
    {

        return !layer2.EqualsInt(layer1);

    }

    public static bool operator !=(Layer layer1, int layer2)
    {

        return !layer1.EqualsInt(layer2);

    }

    public override string ToString()
    {

        return value.ToString();

    }

}
