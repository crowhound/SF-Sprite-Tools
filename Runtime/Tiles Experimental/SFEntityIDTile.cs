using System;
using AOT;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using Unity.Tilemaps.Experimental;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace SF.TileModule.Experimental
{
    /// <summary>
    ///     Generic visual tile for creating different tilesets like terrain, pipeline, random or animated tiles.
    /// </summary>
    [BurstCompile]
    [Serializable]
    [CreateAssetMenu(fileName = "SF Entity ID", menuName = "SF/Tiles/SF EntityID Tile")]
    public class SFEntityIDTile : EntityIdTileBase
    {
        #region Normal Tile Data
        [SerializeField] protected Sprite _sprite;

        public Sprite Sprite
        {
            get { return _sprite; }
            set
            {
                _sprite               = value;
                TileData.DefaultTileData.spriteEntityId = _sprite != null ? _sprite.GetEntityId() : EntityId.None;
            }
        }
        [SerializeField] protected Color _color;
        /// <summary>
        /// Color of the Tile.
        /// </summary>
        public Color Color
        {
            get { return _color; }
            set
            {
                _color       = value;
                TileData.DefaultTileData.color = _color;
            }
        }
        [SerializeField] protected Matrix4x4 _transform;
        /// <summary>
        /// Transform matrix of the Tile.
        /// </summary>
        public Matrix4x4 Transform
        {
            get { return _transform; }
            set
            {
                _transform       = value;
                TileData.DefaultTileData.transform = _transform;
            }
        }

        [SerializeField]
        private GameObject _instancedGameObject;
        /// <summary>
        /// GameObject to be instantiated at the position
        /// where the Tile is placed.
        /// </summary>
        public GameObject GameObject
        {
            get { return _instancedGameObject; }
            set
            {
                _instancedGameObject = value;
                TileData.DefaultTileData.gameObjectEntityId = _instancedGameObject != null ? _instancedGameObject.GetEntityId() : EntityId.None;
            }
        }

        [SerializeField] protected TileFlags _flags;
        /// <summary>
        /// TileFlags of the Tile.
        /// </summary>
        public TileFlags Flags
        {
            get { return _flags; }
            set
            {
                _flags       = value;
                TileData.DefaultTileData.flags = _flags;
            }
        }
        
        [SerializeField] protected Tile.ColliderType _colliderType;
        /// <summary>
        /// ColliderType of the Tile.
        /// </summary>
        public Tile.ColliderType ColliderType
        {
            get { return _colliderType; }
            set
            {
                _colliderType       = value;
                TileData.DefaultTileData.colliderType = _colliderType;
            }
        }
        
#endregion 
                
        /// <summary>
        /// The data passed via GetTile. Custom structs can be used to allow adding new logic to Runtime tile checks.
        /// </summary>
        public SFTileData TileData;

        public override Type structType
        {
            get => typeof(SFTileData);
        }
        
        public struct SFTileData : IDisposable
        {
            /// <summary>
            /// HexagonalRuleEntityIdTile's EntityId
            /// </summary>
            public EntityId TileId;
            
            public TileData DefaultTileData;
            public SFTileType SFTileType;

            public void Dispose()
            {
               
            }
        }
        
        /// <summary>
        /// Copies the Data struct used for this SFTileData to the outPtr buffer
        /// for use in Unity Jobs.
        /// </summary>
        /// <param name="outPtr">Data buffer to copy data struct from SFTileData to.</param>
        public override unsafe void CopyDataStruct(void* outPtr)
        {
            UnsafeUtility.CopyStructureToPtr(ref TileData, outPtr);
        }

        public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
        {
            return base.StartUp(position, tilemap, go);
        }

        public override void OnEnable()
        {
            base.OnEnable();
            OnValidate();
        }

        /// <summary>
        /// Used to make sure any custom tile data is set up and and ready for burst compiled jobs.
        /// </summary>
        private void OnValidate()
        {
            TileData = new SFTileData()
            {
                DefaultTileData = new TileData()
                {
                    spriteEntityId     = _sprite != null ? _sprite.GetEntityId() : EntityId.None,
                    color              = Color.white,
                    transform          = Matrix4x4.identity,
                    gameObjectEntityId = _instancedGameObject != null ? _instancedGameObject.GetEntityId() : EntityId.None,
                    flags              = _flags,
                    colliderType       = _colliderType,
                }
            };
        }

        protected override unsafe RefreshTileJobDelegate refreshTileJobDelegate => RefreshTileJob;

        protected override unsafe GetTileDataJobDelegate getTileDataJobDelegate => GetTileDataJob;

        protected override unsafe GetTileAnimationDataJobDelegate getTileAnimationDataJobDelegate =>
            GetTileAnimationDataJob;

        /// <summary>
        /// Refresh the tiles using a job delegate.
        /// </summary>
        /// <param name="count"></param>
        /// <param name="position"></param>
        /// <param name="data"></param>
        /// <param name="tilemapRefreshStruct"></param>
        [BurstCompile]
        [MonoPInvokeCallback(typeof(RefreshTileJobDelegate))]
        static unsafe void RefreshTileJob(int count, int3* position, void* data, ref TilemapRefreshStruct tilemapRefreshStruct)
        {
            for (var i = 0; i < count; ++i)
            {
                var pos = position + i;
                tilemapRefreshStruct.RefreshTile(*pos);
            }
        }

        public override void RefreshTile(Vector3Int position, ITilemap tilemap)
        {
            //Debug.Log($"Refreshing tile at position: {position} on TileMap: {tilemap}");
            base.RefreshTile(position, tilemap);
        }

        /// <summary>
        /// Retrieves the tile rendering data for the Tile.
        /// </summary>
        /// <param name="position">Position of the Tile on the Tilemap.</param>
        /// <param name="tilemap">The Tilemap the tile is present on.</param>
        /// <param name="tileData">Data to render the tile. This is filled with Tile, Tile.color and Tile.transform.</param>
        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            var tilemapData = new TilemapDataStruct(tilemap.GetComponent<Tilemap>());
            unsafe
            {
                UnsafeUtility.CopyPtrToStructure(UnsafeUtility.AddressOf(ref TileData.DefaultTileData)
                    , out tileData);
                var pos = position.ToInt3();
                GetTileDataJob(1, &pos
                    , UnsafeUtility.AddressOf(ref TileData)
                    , ref tilemapData
                    , (TileData*) UnsafeUtility.AddressOf(ref tileData));
            }
        }
        
        [BurstCompile]
        [MonoPInvokeCallback(typeof(GetTileDataJobDelegate))]
        static unsafe void GetTileDataJob(int count, int3* position, void* data, ref TilemapDataStruct tilemapDataStruct, TileData* outTileData)
        {
            // Convert the data located at the pointer void* data to a usable SFTileData struct.
            SFTileData dataStruct = UnsafeUtility.AsRef<SFTileData>(data);
            
            for (int i = 0; i < count; ++i)
            {
                ref TileData tileData = ref *(outTileData + i);
                UnsafeUtility.CopyPtrToStructure(UnsafeUtility.AddressOf(ref dataStruct.DefaultTileData), out tileData);
            }
        }

        public override bool GetTileAnimationData(Vector3Int position, ITilemap tilemap, ref TileAnimationData tileAnimationData)
        {
            return base.GetTileAnimationData(position, tilemap, ref tileAnimationData);
        }

        [BurstCompile]
        [MonoPInvokeCallback(typeof(GetTileAnimationDataJobDelegate))]
        static unsafe void GetTileAnimationDataJob(int count, int3* position, void* data, ref TilemapDataStruct tilemapDataStruct, TileAnimationEntityIdData* outTileAnimationEntityIdData)
        {
            
        }
    }

    public enum SFTileType
    {
        Static,
        Breakable,
        Meltable
    }
}
