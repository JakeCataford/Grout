using UnityEngine;
using System.Collections.Generic;

namespace Grout
{   
    [CreateAssetMenu(fileName="New Grout Map", menuName="Grout/Map")]
    public class Map : ScriptableObject {
        public delegate void OnTileUpdateHandler(MapTile newTile, int layer, int x, int y);
        public event OnTileUpdateHandler OnTileUpdate;

        public delegate void OnMapEventHandler();
        public event OnMapEventHandler OnMapResize;

        public enum Planes {
            XZ,
            XY
        }
        
        public Planes Plane = Planes.XZ;
        public int TileScale = 1;
        
        public List<MapLayer> Layers = new List<MapLayer>();
        public MapLayer EventLayer;

        private int _sizeX = 10;
        public int SizeX { get { return _sizeX; } }
        private int _sizeY = 10;
        public int SizeY { get { return _sizeY; } }

        public void Resize(int sizeX, int sizeY) {
            _sizeX = SizeX;
            _sizeY = SizeY;
            foreach(MapLayer layer in Layers) {
                layer.Resize();
            }

            EventLayer.Resize();
            if (OnMapResize != null) OnMapResize();
        }
          
        public void AddLayer() {
            Layers.Add(new MapLayer(this));
            if (OnMapResize != null) OnMapResize();
        }
        
        public bool IsSpaceOccupied(int x, int y) {
            foreach(MapLayer layer in Layers) {
                if (layer.Tiles[x][y] != null)  return true;
            }

            return false;
        }
        
        public bool IsSpaceWalkable(int x, int y) {
             foreach(MapLayer layer in Layers) {
                if (layer.Tiles[x][y] != null && layer.Tiles[x][y].Tile != null && layer.Tiles[x][y].Tile.Walkable)  return true;
            }

            return false;
        }

        public void UpdateTile(Tile newTile, int layer, int x, int y, int rotation = 0) {  
            Layers[layer].Tiles[x][y] = new MapTile(newTile, rotation);

            if (OnTileUpdate != null) OnTileUpdate(Layers[layer].Tiles[x][y], layer, x, y);
        }
        
        public void RotateTile(int layer, int x, int y) {
            if (Layers[layer].Tiles[x][y] == null) return;
            Layers[layer].Tiles[x][y].Rotation = (Layers[layer].Tiles[x][y].Rotation + 1) % 4;
            if (OnTileUpdate != null) OnTileUpdate(Layers[layer].Tiles[x][y], layer, x, y);
        }
    }
}
