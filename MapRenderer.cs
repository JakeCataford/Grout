using UnityEngine;
using System.Collections;

namespace Grout
{
    public class MapRenderer
    {
        private Map map;
        private GameObject Container;

        private GameObject[][][] rendered;
        private bool hasBeenRendered = false;

        public MapRenderer(Map map) {
            this.map = map;

            rendered = new GameObject[map.Layers.Count][][];
            for (int i = 0; i < map.Layers.Count; i++) {
                rendered[i] = new GameObject[map.SizeX][];
                for(int j = 0; j < map.SizeX; j++) {
                    rendered[i][j] = new GameObject[map.SizeY];
                }
            }

            map.OnTileUpdate += Update;
            map.OnMapResize += OnMapResize;
        }

        public void OnMapResize() {
            rendered = new GameObject[map.Layers.Count][][];
            for (int i = 0; i < map.Layers.Count; i++) {
                rendered[i] = new GameObject[map.SizeX][];
                for(int j = 0; j < map.SizeX; j++) {
                    rendered[i][j] = new GameObject[map.SizeY];
                }
            }
            
            if (hasBeenRendered) {
                Cleanup();
                Render();
            }
        }
        
        public void Render() {
            Container = CreateContainer();
            for(int l = 0; l < map.Layers.Count; l++) {
                for(int i = 0; i < map.SizeX; i++) {
                    for(int j = 0; j < map.SizeY; j++) {
                        MapTile tile = map.Layers[l].Tiles[i][j];
                        if (tile == null || tile.Tile == null) continue;
                        GameObject go = GameObject.Instantiate(tile.Tile.Renderable);
                        go.transform.SetParent(Container.transform);
                        go.transform.localRotation = Quaternion.Euler(0, 90 * tile.Rotation, 0);
                        go.transform.localPosition = new Vector3(i * map.TileScale, 0, j * map.TileScale);
                        rendered[l][i][j] = go;
                    }
                }
            }

            hasBeenRendered = true;
        }

        public void Update(MapTile tile, int layer, int x, int y) {
            if (rendered[layer][x][y] != null) {
                GameObject.DestroyImmediate(rendered[layer][x][y]);
            }
            
            GameObject go = GameObject.Instantiate(tile.Tile.Renderable);
            go.transform.SetParent(Container.transform);
            go.transform.localRotation = Quaternion.Euler(0, 90 * tile.Rotation, 0);
            go.transform.localPosition = new Vector3(x * map.TileScale, 0, y * map.TileScale);
            rendered[layer][x][y] = go;
        }

        public void Cleanup() {
            GameObject.DestroyImmediate(Container);
            map.OnTileUpdate -= Update;
        }

        public virtual GameObject CreateContainer()
        {
            return new GameObject(map.name);
        }
    }
}
