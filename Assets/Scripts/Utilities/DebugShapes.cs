using UnityEngine;

public static class DebugShapes
{

    public static void DrawSquare(Vector2 position, float length, Color color, float time)
    {
        
        length /= 2; 

        Debug.DrawLine(position + new Vector2(length, length), position + new Vector2(length, -length), color, time);
        Debug.DrawLine(position + new Vector2(length, length), position + new Vector2(-length, length), color, time);
        Debug.DrawLine(position + new Vector2(-length, -length), position + new Vector2(length, -length), color, time);
        Debug.DrawLine(position + new Vector2(-length, -length), position + new Vector2(-length, length), color, time);

    }
    
    public static void DrawPlus(Vector2 position, float length, Color color, float time)
    {

        length /= 2;

        Debug.DrawLine(position + new Vector2(0, length), position + new Vector2(0, -length), color, time);
        Debug.DrawLine(position + new Vector2(length, 0), position + new Vector2(-length, 0), color, time);

    }

    public static void DrawCircle(Vector3 position, float radius, Color color, float time)
    {

        DrawCircle(position, radius, color, time, 12);

    }
    
    public static void DrawCircle(Vector3 position, float radius, Color color, float time, float steps)
    {

        float smoothness = (Mathf.PI * 2) / steps;

        for (float i = 0; i < Mathf.PI * 2; i += smoothness)
        {

            Vector3 pos1 = position + new Vector3(Mathf.Cos(i) * radius, 0, Mathf.Sin(i) * radius);

            Vector3 pos2 = position + new Vector3(Mathf.Cos(i + smoothness) * radius, 0, Mathf.Sin(i + smoothness) * radius);

            Debug.DrawLine(pos1, pos2, color, time);

        }

    }
    
    public static void DrawSphere(Vector3 position, float radius, Color color, float time)
    {

        float smoothness = (Mathf.PI * 2) / 12f;

        for (float i = 0; i < Mathf.PI * 2; i += smoothness)
        {

            Debug.DrawLine(position + new Vector3(Mathf.Cos(i) * radius, Mathf.Sin(i) * radius, 0), position + new Vector3(Mathf.Cos(i + smoothness) * radius, Mathf.Sin(i + smoothness) * radius, 0), color, time);

        }
        
        for (float i = 0; i < Mathf.PI * 2; i += smoothness)
        {

            Debug.DrawLine(position + new Vector3(0, Mathf.Cos(i) * radius, Mathf.Sin(i) * radius), position + new Vector3(0, Mathf.Cos(i + smoothness) * radius, Mathf.Sin(i + smoothness) * radius), color, time);

        }
        
        for (float i = 0; i < Mathf.PI * 2; i += smoothness)
        {

            Debug.DrawLine(position + new Vector3(Mathf.Cos(i) * radius, 0, Mathf.Sin(i) * radius), position + new Vector3(Mathf.Cos(i + smoothness) * radius, 0, Mathf.Sin(i + smoothness) * radius), color, time);

        }

    }
    
}
