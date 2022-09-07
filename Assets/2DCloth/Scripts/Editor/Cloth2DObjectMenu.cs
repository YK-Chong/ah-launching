using UnityEditor;
using UnityEngine;

namespace Cloth2D.Editor
{
    public class Cloth2DObjectMenu : UnityEditor.Editor
    {
        [MenuItem("GameObject/2D Object/2D Cloth", false, 32)]
        static void AddClothObject()
        {
            var clothObject = new GameObject("2D Cloth");
            clothObject.AddComponent<Cloth2D>();
            clothObject.AddComponent<Cloth2DSprite>();
        }
    }
}