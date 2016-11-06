using UnityEngine;
using System.Collections;

namespace Grout
{
    [System.Serializable]
    public class MapLayer
    {
        public MapLayer(Map map) {
            this.map = map;
            
            Tiles = new TileArray[map.SizeX];
            for(int j = 0; j < map.SizeX; j++) {
                Tiles[j] = new TileArray(map.SizeY);
            }
            
            Resize();
        }
        
        private Map map;
        public TileArray[] Tiles;
 
        [System.Serializable]
        public class TileArray
        {
            public TileArray(int size)
            {
                Tiles = new MapTile[size];
            }

            public MapTile[] Tiles;
            public MapTile this[int key]
            {
                get
                {
                    return Tiles[key];
                }

                set
                {
                    Tiles[key] = value;
                }
            }

            public void Resize(int newLength) {
                System.Array.Resize<MapTile>(ref Tiles, newLength);
            }
        }
        
        public void Resize() {
            System.Array.Resize<TileArray>(ref Tiles, map.SizeX);
            for(int i = 0; i < Tiles.Length; i++) {
                Tiles[i].Resize(map.SizeY);
            }
        }
    }
}
