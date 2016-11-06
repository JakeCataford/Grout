using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class GroutMapUI {
    private GroutMapAssets assets;
    
    public GroutMapUI(GroutMapAssets assets) {
        this.assets = assets;
    }
    
    public void Divider() {
        GUIStyle dividerStyle = new GUIStyle();
        dividerStyle.stretchWidth = true;
        dividerStyle.normal.background = assets.Divider;
        dividerStyle.margin = new RectOffset(6, 6, 6, 6);

        GUILayout.BeginVertical();
        GUILayout.Box(assets.Divider, dividerStyle, GUILayout.ExpandWidth(true));
        GUILayout.EndVertical();
    }
    
    public Grout.Tile TilePalette(Grout.Tile selected, Grout.Tile[] tiles, float width) {
        if (tiles.Length == 0) return null;
        int selectedIndex = new List<Grout.Tile>(tiles).IndexOf(selected);
        
        if (selectedIndex == -1) selectedIndex = 0;
        
        Texture2D[] Images = tiles.Select((x) =>
            AssetPreview.GetAssetPreview((Object) x.Renderable ?? x)
        ).ToArray();

        selectedIndex = GUILayout.SelectionGrid(selectedIndex, Images, 4, GUILayout.Width(width));
        return selectedIndex < tiles.Length ? tiles[selectedIndex] : null;
    }
}
