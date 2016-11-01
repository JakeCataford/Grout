using UnityEngine;

namespace  Grout {
   public interface ITileProperty {
        void OnTilePlacedInWorld(Vector3 TilePosition);
        void OnTileRemovedFromWorld();
        void OnTilePreview(Vector3 previewPosition);
        void OnTilePreviewHide();
    }
}

