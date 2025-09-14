using UnityEngine;

public class Util
{
    public static GameObject FindChild(GameObject obj, string name = null, bool recursive = true)
    {
        Transform transform = FindChild<Transform>(obj, name, recursive);

        if (transform == null)
        {
            return null;
        }

        return transform.gameObject;
    }

    public static T FindChild<T>(GameObject obj, string name = null, bool recursive = true) where T : UnityEngine.Object
    {
        if (obj == null)
        {
            return null;
        }

        if (recursive)
        {
            foreach (T component in obj.GetComponentsInChildren<T>())
            {
                if (component.name == name || string.IsNullOrEmpty(name))
                {
                    return component;
                }
            }
        }
        else
        {
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                Transform transform = obj.transform.GetChild(i);

                if (transform.name == name || string.IsNullOrEmpty(name))
                {
                    T component = transform.GetComponent<T>();

                    if (component != null)
                    {
                        return component;
                    }
                }
            }
        }

        return null;
    }

    public static Color GetColorFromHex(string hex)
    {
        if (ColorUtility.TryParseHtmlString(hex, out Color color))
        {
            return color;
        }

        return Color.white;
    }
}
