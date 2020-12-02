using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ModEditor
{
    public class AssetModificationManagement : UnityEditor.AssetModificationProcessor
    {
        public static event System.Action<string[]> onWillSaveAssets;

        public static void OnWillSaveAssets(string[] path)
        {
            onWillSaveAssets?.Invoke(path);
        }
    }
}