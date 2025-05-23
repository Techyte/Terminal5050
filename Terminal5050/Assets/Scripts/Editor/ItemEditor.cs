using UnityEngine;
using UnityEditor;
using System.Reflection;
using Object = UnityEngine.Object;

[CustomEditor(typeof(ItemTemplate))]
public class ItemEditor : Editor
{
    private ItemTemplate item { get { return target as ItemTemplate; } }

    public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
    {
        if (item.sprite != null)
        {
            System.Type t = GetType("UnityEditor.SpriteUtility");
            if (t != null)
            {
                MethodInfo method = t.GetMethod("RenderStaticPreview", new[] { typeof(Sprite), typeof(Color), typeof(int), typeof(int) });
                if (method != null)
                {
                    object ret = method.Invoke("RenderStaticPreview", new object[] { item.sprite, Color.white, width, height });
                    if (ret is Texture2D)
                        return ret as Texture2D;
                }
            }
        }
        return base.RenderStaticPreview(assetPath, subAssets, width, height);
    }

    private static System.Type GetType(string typeName)
    {
        var type = System.Type.GetType(typeName);
        if (type != null)
            return type;

        var currentAssembly = Assembly.GetExecutingAssembly();
        var referencedAssemblies = currentAssembly.GetReferencedAssemblies();
        foreach (var assemblyName in referencedAssemblies)
        {
            var assembly = Assembly.Load(assemblyName);
            if (assembly != null)
            {
                type = assembly.GetType(typeName);
                if (type != null)
                    return type;
            }
        }
        return null;
    }
}
