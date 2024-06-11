using UnityEngine;
public static class Utils
{
    public static Rect GetScreenRect(Vector3 screenPosition1, Vector3 screenPosition2)
    {
        screenPosition1.y = Screen.height - screenPosition1.y;
        screenPosition2.y = Screen.height - screenPosition2.y;
        return Rect.MinMaxRect(
            Mathf.Min(screenPosition1.x, screenPosition2.x),
            Mathf.Min(screenPosition1.y, screenPosition2.y),
            Mathf.Max(screenPosition1.x, screenPosition2.x),
            Mathf.Max(screenPosition1.y, screenPosition2.y)
        );
    }

    public static void DrawScreenRect(Rect rect, Color color)
    {
        GUI.color = color;
        GUI.DrawTexture(rect, Texture2D.whiteTexture);
    }

    public static void DrawScreenRectBorder(Rect rect, float thickness, Color color)
    {
        GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, rect.width, thickness), Texture2D.whiteTexture, ScaleMode.StretchToFill, true, 10.0f, color, 0, 0);
        GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, thickness, rect.height), Texture2D.whiteTexture, ScaleMode.StretchToFill, true, 10.0f, color, 0, 0);
        GUI.DrawTexture(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), Texture2D.whiteTexture, ScaleMode.StretchToFill, true, 10.0f, color, 0, 0);
        GUI.DrawTexture(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), Texture2D.whiteTexture, ScaleMode.StretchToFill, true, 10.0f, color, 0, 0);
    }
}