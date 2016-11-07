using UnityEngine;
using System.Collections;

namespace Grout
{
    public class Util {
        public static Map[] AllMaps {
            get {
                return Resources.FindObjectsOfTypeAll<Grout.Map>();
            }
        }
        
        public static Map RandomMap {
            get {
                return AllMaps[Mathf.FloorToInt(Random.value * AllMaps.Length)];
            }
        }
    }
}