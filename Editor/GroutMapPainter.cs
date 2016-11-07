using UnityEngine;
using UnityEditor;
using System.Collections;

public class GroutMapPainter
{
    public int SelectedLayer;
    public Grout.Tile SelectedTile;
    public Grout.Map SelectedMap;
    private int Rotation = 0;
    private Grout.Position selectedTileIndex = new Grout.Position(0, 0);

    private Vector2 mousePosition = Vector2.zero;
    private bool focused;
    private Grout.Position lastTileIndex;
    private bool mouseIsDown = false;

    public void OnSceneGUI(SceneView sceneView)
    {
        if (!SelectedTile || !SelectedMap) return;
        int id = GUIUtility.GetControlID("ALFALHPA BILL!".GetHashCode(), FocusType.Passive);
        Event e = Event.current;
        EventType type = e.GetTypeForControl(id);

        if (type == EventType.Layout)
        {
            HandleUtility.AddDefaultControl(id);
        }
        
        if (type == EventType.mouseMove)
        {
            mousePosition = e.mousePosition;
        }

        getSelectedTile();
        highlightTile();

        if (type == EventType.MouseDown)
        {
            SelectedMap.UpdateTile(e.functionKey ? null : SelectedTile, SelectedLayer, selectedTileIndex.x, selectedTileIndex.y, Rotation);
            mouseIsDown = true;
            
            if (focused)
            {
                GUIUtility.hotControl = id;
                e.Use();
            }
        }
        
        if (type == EventType.MouseUp)
        {
            mouseIsDown = false;
            GUIUtility.hotControl = id;
            if (GUIUtility.hotControl == id)
                GUIUtility.hotControl = 0;
                
            if (focused)
            {
                e.Use();
            }
        }

        if (mouseIsDown && type == EventType.MouseDrag)
        {
            mousePosition = e.mousePosition;
            getSelectedTile();
            highlightTile();
            
            if (lastTileIndex != selectedTileIndex)
            {
                SelectedMap.UpdateTile(e.functionKey ? null : SelectedTile, SelectedLayer, selectedTileIndex.x, selectedTileIndex.y, Rotation);
            }
            
            if (focused)
            {
                e.Use();
            }
        }
        
        if (type == EventType.KeyDown) {
            if (e.keyCode == KeyCode.Space) {
                Rotation = (Rotation + 1) % 4;
                SelectedMap.RotateTile(SelectedLayer, selectedTileIndex.x, selectedTileIndex.y);
                e.Use();
            }
        }

        Handles.BeginGUI();
        GUI.Label(new Rect(0, 0, 400, 100), "Click/Drag To Paint\nSpace to rotate tile\nX To Delete\nCurrent Tile Rotation: " + (Rotation * 90));
        Handles.EndGUI();
    }

    private void getSelectedTile()
    {
        Ray cameraRay = HandleUtility.GUIPointToWorldRay(mousePosition);
        Plane ground = new Plane(Vector3.up, 0);
        Handles.DrawLine(cameraRay.origin, cameraRay.origin + (cameraRay.direction.normalized * 10f));
        float enter;
        if (ground.Raycast(cameraRay, out enter))
        {
            Vector3 worldPosition = cameraRay.origin + (cameraRay.direction.normalized * enter);
            Grout.Position newPosition = positionFromWorldPoint(worldPosition);
            
            if (newPosition.x < 0 || newPosition.x >= SelectedMap.SizeX || newPosition.y < 0 || newPosition.y >= SelectedMap.SizeY) {
                focused = false;
                return;
            } else {
                focused = true;
            }

            lastTileIndex = selectedTileIndex;
            selectedTileIndex = newPosition;
        }
    }
    
    private void highlightTile() {
        Vector3 tileCenter = new Vector3(selectedTileIndex.x * SelectedMap.TileScale, 0, selectedTileIndex.y * SelectedMap.TileScale);
                float padding = ((float)SelectedMap.TileScale / 2f) - 0.05f;
                Vector3[] verts = new Vector3[] {
                    tileCenter + new Vector3(-padding, -0f, -padding),
                    tileCenter + new Vector3(-padding, -0f, padding),
                    tileCenter + new Vector3(padding, -0f, padding),
                    tileCenter + new Vector3(padding, -0f, -padding)
                };
        Handles.DrawSolidRectangleWithOutline(verts, new Color(0, 0.3f, 0.8f, 0.05f), Color.white);
    }

    private Grout.Position positionFromWorldPoint(Vector3 point)
    {
        point /= SelectedMap.TileScale;
        return new Grout.Position(Mathf.RoundToInt(point.x), Mathf.RoundToInt(point.z));
    }
}
