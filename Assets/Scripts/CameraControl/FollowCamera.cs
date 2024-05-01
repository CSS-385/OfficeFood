using UnityEngine;
using UnityEngine.Tilemaps;

// Borrowed from an old game of mine:
// https://github.com/WSPTA-Cat-Game/CatGame/blob/master/Assets/Scripts/CameraControl/FollowCamera.cs
// - kyler
namespace OfficeFood.CameraControl
{
    [RequireComponent(typeof(Camera))]
    public class FollowCamera : MonoBehaviour
    {
        public Tilemap tilemap;
        public Transform target;

        public bool lockX = false;
        public bool lockY = false;
        public Vector2 lockedPos = Vector2.zero;
        public float lerpSpeed = 0;

        private new Camera camera;
        private Bounds bounds;

        private void Start()
        {
            camera = GetComponent<Camera>();

            Tilemap.tilemapTileChanged += OnTilemapChanged;

            UpdateBounds();
        }

        private void Update()
        {
            // Create bounds that represent what the cam sees of the collider
            float camHeight = camera.orthographicSize * 2;
            float camWidth = camHeight * Screen.width / Screen.height;
            Bounds camBounds = new(target.position, new Vector3(camWidth, camHeight, 1));

            Vector3 adjustedPos = camBounds.center;
            // Lock pos
            if (lockX)
            {
                adjustedPos.x = lockedPos.x;
            }
            else
            {
                // If the size of the bounds exceeds the collider, set the pos
                // to the center of the collider's bounds
                if (camBounds.size.x > bounds.size.x)
                {
                    adjustedPos.x = bounds.center.x;
                }
                // If the bounds is outside the collider, then set pos to edge 
                // of the collider accounting for bounds size
                else if (camBounds.min.x < bounds.min.x)
                {
                    adjustedPos.x = bounds.min.x + camBounds.extents.x;
                }
                else if (camBounds.max.x > bounds.max.x)
                {
                    adjustedPos.x = bounds.max.x - camBounds.extents.x;
                }
            }


            // Repeat for y
            if (lockY)
            {
                adjustedPos.y = lockedPos.y - 0.25f;
            }
            else
            {
                if (camBounds.size.y > bounds.size.y)
                {
                    adjustedPos.y = bounds.center.y;
                }
                else if (camBounds.min.y < bounds.min.y)
                {
                    adjustedPos.y = bounds.min.y + camBounds.extents.y;
                }
                else if (camBounds.max.y > bounds.max.y)
                {
                    adjustedPos.y = bounds.max.y - camBounds.extents.y;
                }
            }


            adjustedPos.z = -10;
            camera.transform.position = Vector3.Lerp(
                camera.transform.position,
                adjustedPos,
                lerpSpeed * 60 * Time.deltaTime
            );
        }

        private void OnTilemapChanged(Tilemap tilemap, Tilemap.SyncTile[] _)
        {
            if (tilemap != this.tilemap)
            {
                return;
            }

            UpdateBounds();
        }

        // Also stolen from an old game of mine
        // https://github.com/WSPTA-Cat-Game/CatGame/blob/master/Assets/Scripts/Editor/LevelCreator/Common.cs#L46
        // - kyler
        // Get the bounds that contains all tiles with some sort of collision
        private void UpdateBounds()
        {
            BoundsInt cellBounds = tilemap.cellBounds;
            Vector3Int min = new(int.MaxValue, int.MaxValue);
            Vector3Int max = new(int.MinValue, int.MinValue);

            // Temp var to save memory
            Vector3Int currentTilePos = Vector3Int.zero;

            // Get get all sides of bounds
            for (int x = cellBounds.min.x; x <= cellBounds.max.x; x++)
            {
                currentTilePos.x = x;
                for (int y = cellBounds.min.y; y <= cellBounds.max.y; y++)
                {
                    currentTilePos.y = y;

                    if (tilemap.GetColliderType(currentTilePos) == Tile.ColliderType.None)
                    {
                        continue;
                    }

                    if (currentTilePos.x < min.x)
                    {
                        min.x = currentTilePos.x;
                    }

                    if (currentTilePos.y < min.y)
                    {
                        min.y = currentTilePos.y;
                    }

                    if (currentTilePos.x > max.x)
                    {
                        max.x = currentTilePos.x;
                    }

                    if (currentTilePos.y > max.y)
                    {
                        max.y = currentTilePos.y;
                    }
                }
            }

            // The pos represents the bottom left of the tile, so we have to 
            // add one to compensate for that
            bounds.SetMinMax(tilemap.CellToWorld(min), tilemap.CellToWorld(max + new Vector3Int(1, 1)));
        }
    }
}
