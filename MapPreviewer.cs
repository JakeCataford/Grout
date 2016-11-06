using UnityEngine;
using System.Collections;

namespace Grout
{
    public class MapPreviewer : MapRenderer
    {
        public MapPreviewer(Map map) : base(map) { }
        
        public override GameObject CreateContainer() {
            GameObject go = new GameObject("Map Preview");
            go.hideFlags = HideFlags.HideAndDontSave;
            return go;
        }
    }
}
