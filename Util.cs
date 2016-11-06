using UnityEngine;
using System.Collections;

namespace Grout
{
    public class Util {
        public Map[] AllMaps {
            get {
                return Resources.FindObjectsOfTypeAll<Grout.Map>();
            }
        }       
    }
}