using UnityEngine;

namespace Grout
{
    [CreateAssetMenu(fileName = "New Grout Tile", menuName = "Grout/Tile")]
    public class Tile : ScriptableObject
    {
        public string Category = "default";
        public Event[] Events;
        public bool PlaceOnEventLayer = false;
        public bool Walkable = true;
        public GameObject Renderable;

        public virtual GameObject GetRenderable()
        {
            return Renderable;
        }
        
        public virtual GameObject GetPreview()
        {
            return Renderable;
        }
    }
}

