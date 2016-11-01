using UnityEngine;
using System;

namespace Grout {
    public class SpawnObjectNodeProperty : ScriptableObject, ITileProperty {
        public GameObject Spawnable;
        
        public void OnTilePlacedInWorld(Vector3 TilePosition) {
            throw new NotImplementedException();
        }

        public void OnTilePreview(Vector3 previewPosition) {
            throw new NotImplementedException();
        }

        public void OnTilePreviewHide() {
            throw new NotImplementedException();
        }

        public void OnTileRemovedFromWorld() {
            throw new NotImplementedException();
        }
    }
}
