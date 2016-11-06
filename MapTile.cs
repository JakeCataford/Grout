namespace Grout
{
	[System.Serializable]
    public class MapTile {
        public int Rotation = 0;
        public Tile Tile;
            
        public MapTile(Tile tile, int rotation = 0) {
            this.Tile = tile;
            this.Rotation = rotation;
        }
    }
}
