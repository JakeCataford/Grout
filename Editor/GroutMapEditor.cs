using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class GroutMapEditor : EditorWindow {
    private Grout.Map _currentMap;
    public Grout.Map CurrentMap {
        get {
            return _currentMap;
        }
        set {
            if (previewer != null) {
                previewer.Cleanup();
            }
            
            _currentMap = value;
            previewer = new Grout.MapPreviewer(_currentMap);
            previewer.Render();
        }
    }
    
    private List<Grout.Map> _availableMaps;
    public List<Grout.Map> AvailableMaps {
        get {
            if (_availableMaps == null) {
                RefreshMaps();
            }
            
            return _availableMaps;
        }
    }

    private int selectedMapLayer = 0;
    private Grout.MapPreviewer previewer;
    private Vector2 scrollPos;

    private void RefreshMaps() {
        _availableMaps = new List<Grout.Map>(
            (Resources.FindObjectsOfTypeAll(typeof(Grout.Map)) as Grout.Map[])
        );
    }
    
    [MenuItem ("Grout/Map Editor")]
    private static void Init () {
        GroutMapEditor window = (GroutMapEditor) EditorWindow.GetWindow(typeof (GroutMapEditor));
        window.Show();
    }

    private Grout.Tile SelectedTile;
    private GroutMapPainter _painter;
    private GroutMapPainter painter {
        get {
            if (_painter == null)
            {
                _painter = new GroutMapPainter();
            }

            return _painter;
        }
    }

    private void OnGUI() {
        GUI.skin = groutSkin;

        if (AvailableMaps.Count == 0) {
            GUILayout.Label("Create a map in a resources folder to begin");
            return;
        }
        
        if (EditorApplication.isPlayingOrWillChangePlaymode) {
            GUILayout.Label("Exit play mode to edit maps");
            return;
        }
        
        EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
        GUILayout.Label("Grout Map Editor");
        EditorGUILayout.EndVertical();
       
        EditorGUILayout.BeginHorizontal();
        int selected = AvailableMaps.IndexOf(CurrentMap);
        string[] options = AvailableMaps.Select((x) => x.name).ToArray();
        int newSelected = EditorGUILayout.Popup("", selected, options, customSkin("Popup"), GUILayout.Height(25));
        if (newSelected != selected) {
            CurrentMap = AvailableMaps[newSelected];
            selectedMapLayer = 0;
        }
        
        if (GUILayout.Button("Refresh")) {
            RefreshMaps();
        }
        EditorGUILayout.EndHorizontal();
        
        if (CurrentMap == null) return;

        EditorGUILayout.BeginHorizontal();
        CurrentMap.Plane = (Grout.Map.Planes) EditorGUILayout.EnumPopup("Map Plane", CurrentMap.Plane);
        CurrentMap.TileScale = EditorGUILayout.IntField("Tile Scale", CurrentMap.TileScale);
        EditorGUILayout.EndHorizontal();

        UI.Divider();

        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Layers");
        if (GUILayout.Button("+", customSkin("Subtle Button"), GUILayout.Width(30))) {
            CurrentMap.AddLayer();
            selectedMapLayer = CurrentMap.Layers.Count() - 1;
        }
        
        if (GUILayout.Button("-", customSkin("Subtle Button"), GUILayout.Width(30))) {
            CurrentMap.Layers.Remove(CurrentMap.Layers.Last());
            selectedMapLayer = CurrentMap.Layers.Count() - 1;
        }
        
        EditorGUILayout.EndHorizontal();
        List<string> layerChoices = AllNumbersBelow(CurrentMap.Layers.Count).Select((x) => x.ToString()).ToList();
        selectedMapLayer = GUILayout.Toolbar(selectedMapLayer, layerChoices.ToArray());
        
        EditorGUILayout.EndVertical();

        List<string> availableCategories = TileCategories.Keys.ToList();
        int selectedCategoryIndex = availableCategories.IndexOf(selectedCategory);
        EditorGUILayout.BeginVertical();
        selectedCategoryIndex = GUILayout.Toolbar(selectedCategoryIndex, availableCategories.ToArray());
        selectedCategory = availableCategories[selectedCategoryIndex];
        EditorGUILayout.EndVertical();

        float width = position.width - 30;
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.ExpandHeight(true));
        SelectedTile = UI.TilePalette(SelectedTile, TileCategories[selectedCategory], width);
        EditorGUILayout.EndScrollView();
        painter.SelectedLayer = selectedMapLayer;
        painter.SelectedMap = CurrentMap;
        painter.SelectedTile = SelectedTile;
    }

    void OnEnable() {
        RefreshMaps();
        RefreshCategoryMap();
        CurrentMap = AvailableMaps.First();
    }
    
    private void OnFocus() {
        RefreshMaps();
        RefreshCategoryMap();
        SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
        SceneView.onSceneGUIDelegate += this.OnSceneGUI;
        EditorApplication.playmodeStateChanged += OnPlayModeStateChanged;
    }
    
    void OnPlayModeStateChanged() {
        previewer.Cleanup();
    }
    
    void OnDisable() {
        if (previewer != null) previewer.Cleanup();
    }
    
    private void OnDestroy() {
        if (previewer != null)  previewer.Cleanup();
        SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
    }
 
    void OnSceneGUI(SceneView sceneView) {
        RenderPreview();
        painter.OnSceneGUI(sceneView);
        Handles.BeginGUI();
        Handles.EndGUI();    
    }

    private string selectedCategory = "All Tiles";
    private Dictionary<string, Grout.Tile[]> TileCategories = new Dictionary<string, Grout.Tile[]>();
    void RefreshCategoryMap() {
        TileCategories.Clear();
        List<Grout.Tile> allTiles = Resources.FindObjectsOfTypeAll<Grout.Tile>().ToList();
        TileCategories["All Tiles"] = allTiles.ToArray();
        TileCategories["Events"] = allTiles.Where((x) => x.PlaceOnEventLayer).ToArray();

        List<string> allCategories = allTiles.Select((x) => x.Category).Distinct().ToList();
        allCategories.Remove("default");
        
        foreach(string category in allCategories) {
            TileCategories[category] = allTiles.Where((x) => x.Category == category).ToArray();
        }
    }

    private GUISkin _groutSkin;
    private GUISkin groutSkin {
        get {
            if (_groutSkin == null) {
               string[] guids = AssetDatabase.FindAssets("Grout Editor Skin");
               string path = AssetDatabase.GUIDToAssetPath(guids.First());
                _groutSkin = AssetDatabase.LoadAssetAtPath<GUISkin>(path);
            }

            return _groutSkin;
        }
    }
    
    private void applyAdditionalGUISkinOptions(ref GUISkin skin) {
    }
    
    private GUIStyle customSkin(string name) {
        return GUI.skin.customStyles.First((x) => x.name == name);
    }
    
    private int[] AllNumbersBelow(int number) {
        List<int> numbers = new List<int>();
        for(int i = 0; i < number; i++) {
            numbers.Add(i);
        }

        return numbers.ToArray();
    }

    private GroutMapAssets _assets;
    private GroutMapAssets Assets {
        get {
            if (_assets == null) {
                string guid = AssetDatabase.FindAssets("t:GroutMapAssets").First();
                string path = AssetDatabase.GUIDToAssetPath(guid);
                _assets = AssetDatabase.LoadAssetAtPath<GroutMapAssets>(path);
            }

            return _assets;
        }
    }

    private GroutMapUI _ui;
    private GroutMapUI UI {
        get {
            if (_ui == null) {
                _ui = new GroutMapUI(Assets);
            }

            return _ui;
        }
    }
    
    private void RenderPreview() {
        if (CurrentMap == null) return;

        for (int i = 0; i < CurrentMap.SizeX; i++)
        {
            for (int j = 0; j < CurrentMap.SizeY; j++)
            {
                Vector3 tileCenter = new Vector3(i * CurrentMap.TileScale, 0, j * CurrentMap.TileScale);
                float padding = ((float)CurrentMap.TileScale / 2f) - 0.05f;
                Vector3[] verts = new Vector3[] {
                    tileCenter + new Vector3(-padding, -0.1f, -padding),
                    tileCenter + new Vector3(-padding, -0.1f, padding),
                    tileCenter + new Vector3(padding, -0.1f, padding),
                    tileCenter + new Vector3(padding, -0.1f, -padding)
                };

                Color internalColor = CurrentMap.IsSpaceOccupied(i, j) ? new Color(0, 0.5f, 0.8f, 0.001f) : new Color(0, 0.5f, 0.8f, 0.1f);
                Handles.DrawSolidRectangleWithOutline(verts, internalColor, new Color(0, 0.3f, 0.5f, 0.1f));
            }
        }
    }
}
