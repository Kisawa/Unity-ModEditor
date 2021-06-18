using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ModEditor
{
    public class TexPassMerge : TextureUtilBase
    {
        public override string Name => "Texture Pass Merge";

        public override string Tip => "Extract color channels and merge them into a single image";
    }
}