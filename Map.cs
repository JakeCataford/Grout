using UnityEngine;
using System.Collections;

namespace Grout
{   
    [CreateAssetMenu(fileName="New Grout Map", menuName="Grout/Map")]
    public class Map : ScriptableObject {
        public enum Planes {
            XZ,
            XY
        }
        
        public Planes Plane = Planes.XZ;
        
        [System.Serializable]
        public class TileArray {
            public TileArray(int size) {
                Tiles = new Tile[size];
                for(int i = 0; i < size; i++) {
                    Tiles[i] = new Tile();   
                } 
            }
            
            public Tile[] Tiles;
            public Tile this[int key] {
                get {
                    return Tiles[key];
                }
                
                set {
                    Tiles[key] = value;
                }
            }
        }
        
        private int tileScale = 1;
        private TileArray[] tiles;
        
        private int _sizeX;
        public int SizeX { get { return _sizeX; } }
        private int _sizeY;
        public int SizeY { get { return _sizeY; } }

        
        public void CreateInstance() {
            
        }
        
        public void Resize(int sizeX, int sizeY) {
            CleanPreview();
            _sizeX = SizeX;
            _sizeY = SizeY;
            tiles = new TileArray[SizeX];
            for(int i = 0; i < tiles.Length; i++) {
                tiles[i] = new TileArray(SizeY);
            }
        }
        
        public void RenderPreview() {
            for(int i = 0; i < _sizeX; i ++) {
                for(int j = 0; j < _sizeY; j ++) {
                    foreach(ITileProperty property in tiles[i][j].Properties) {
                        property.OnTilePreview(tileWorldPosition(i, j));
                    }
                }
            }
        }
        
        private Vector3 tileWorldPosition(int x, int y) {
            switch (Plane) {
                case Planes.XY:
                    return new Vector3(x * tileScale, y * tileScale, 0);
                case Planes.XZ:
                    return new Vector3(x * tileScale, 0, y * tileScale);
                default:
                   return Vector3.zero;
            }
        }
        
        private void CleanPreview() {
            foreach(TileArray tileArray in tiles) {
                foreach(Tile tile in tileArray.Tiles) {
                    foreach(ITileProperty property in tile.Properties) {
                        property.OnTilePreviewHide();
                    }
                }
            }
        }
    }
}
