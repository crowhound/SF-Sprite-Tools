using System;
using System.Collections.Generic;
using AOT;
using Unity.Burst;
using Unity.Collections;
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
    public class SFRuleEntityIDTile : EntityIdTile
    {

        /// <summary>
        /// Angle in which the <see cref="SFEntityIDTile"/> is rotated by for the matching value in degrees 
        /// </summary>
        public int RotationAngle => 90;

        /// <summary>
        /// Number of supported rotations for this <see cref="SFEntityIDTile"/>
        /// </summary>
        public int RotationCount => 360 / RotationAngle;

        private HashSet<Vector3Int> _neighborPositions = new ();
        public HashSet<Vector3Int> NeighborPositions
        {
            get
            {
                if (_neighborPositions.Count == 0)
                {
                    UpdateNeighborPositions(ref _data.NeighborPositions, ref _data.Bounds);
                    foreach (var neighborPosition in _data.NeighborPositions)
                    {
                        _neighborPositions.Add(neighborPosition.ToVector3Int());
                    }
                }
                
                return _neighborPositions;
            }
        }
        
        protected override unsafe RefreshTileJobDelegate refreshTileJobDelegate => RefreshTileJob;
        
        /// <summary>
        /// Returns the delegate function called by <see cref="Tilemap"/> using Unity's Jobs to get the Tile Data from <see cref="EntityIdTile"/>
        /// </summary>
        /// <param name="count"></param>
        /// <param name="position"></param>
        /// <param name="data"></param>
        /// <param name="tilemapDataStruct"></param>
        /// <param name="outTileData"></param>
        protected override unsafe GetTileDataJobDelegate getTileDataJobDelegate => GetTileDataJob;
        protected override unsafe GetTileAnimationDataJobDelegate getTileAnimationDataJobDelegate { get; }
        public override Type structType { get => typeof(SFEntityIDTileDataStruct); }

        private SFEntityIDTileDataStruct _data;

        [HideInInspector] public List<RuleEntityIdTile.TilingRule> TilingRules = new();
        
        public override void OnEnable()
        {
            base.OnEnable();
            OnValidate();
        }

        private void Dispose()
        {
            _data.Dispose();
        }

        private void OnValidate()
        {
            Dispose();

            /*
            // The reference of the entity id for this over all Tile.
            uint reference = 0;
            
            var  entityID  = cachedEntityId;
            if (entityID == EntityId.None)
                return;
            reference = UnsafeUtility.As<EntityId, uint>(ref entityID);
            
            // Put inside of the SFEntityIDTileDataStruct if we choose to have random sprite support per rule tile check.
            var random = new Unity.Mathematics.Random(reference);
            */
            
            _data = new SFEntityIDTileDataStruct()
            {
                TileID           = cachedEntityId,
                RotationAngle    = 90,
                RuleMatchInputs  = new NativeArray<RuleMatchInput>(TilingRules.Count, Allocator.Persistent),
                RuleMatchOutputs = new NativeArray<RuleMatchOutput>(TilingRules.Count, Allocator.Persistent),
                DefaultTileData = new TileData()
                {
                    sprite = sprite,
                    spriteEntityId     = sprite != null ? sprite.GetEntityId() : EntityId.None,
                    color              = color,
                    transform          = transform,
                    gameObject = gameObject,
                    gameObjectEntityId = gameObject != null ? gameObject.GetEntityId() : EntityId.None,
                    flags              = flags,
                    colliderType       = colliderType
                }
            };
            
            for (int i = 0; i < TilingRules.Count; i++)
            {
                var tilingRule = TilingRules[i];
                unsafe
                {
                    tilingRule.ToRuleMatchInput(ref UnsafeUtility.ArrayElementAsRef<RuleMatchInput>(_data.RuleMatchInputs.GetUnsafePtr(), i));
                    tilingRule.ToRuleMatchOutput(ref UnsafeUtility.ArrayElementAsRef<RuleMatchOutput>(_data.RuleMatchOutputs.GetUnsafePtr(), i));
                }
            }

            UpdateNeighborPositions(ref _data.NeighborPositions, ref _data.Bounds);
        }

        /// <summary>
        /// Copies the <see cref="SFEntityIDTileDataStruct"/> and applies it's structure
        /// to the outPtr.
        /// </summary>
        /// <param name="outPtr"></param>
        public override unsafe void CopyDataStruct(void* outPtr)
        {
            UnsafeUtility.CopyStructureToPtr(ref _data, outPtr);
        }
        
        /// <summary>
        ///     This method is called when the tile is refreshed.
        /// </summary>
        /// <param name="position">Position of the Tile on the Tilemap.</param>
        /// <param name="tilemap">The Tilemap the tile is present on.</param>
        public override void RefreshTile(Vector3Int position, ITilemap tilemap)
        {
            if(!_data.NeighborPositions.IsCreated)
            {
                Debug.Log("m_Data.neighborPositions native has set has been deallocated.");
                return;
            }
            
            var refreshPositions = new NativeArray<Vector3Int>(_data.NeighborPositions.Count + 1, Allocator.TempJob);
            var i                = 0;
            foreach (var neighborPosition in _data.NeighborPositions)
            {
                var curpos         = position.ToInt3();
                var neighborOffset = neighborPosition;
                GetOffsetPosition(ref curpos, ref neighborOffset);
                refreshPositions[i++] = curpos.ToVector3Int();
            }
            refreshPositions[i] = position;

            if(tilemap == null)
            {
                Debug.Log($"Tilemap was null when refreshing a tile.");
                return;
            }

            for(int posIndex = 0; i < refreshPositions.Length; i++)
            {
                var tile = tilemap.GetTile(refreshPositions[i]);
                if(tile is null)
                {
					
                    Debug.Log($"The tile at position: {refreshPositions[i]} was null for the refresh list, on the TileMap: {tilemap}");
                }
            }
			
            tilemap.RefreshTiles(refreshPositions);
            refreshPositions.Dispose();
        }

        protected void UpdateNeighborPositions(ref NativeHashSet<int3> refNeighborPositions, ref BoundsInt refBounds)
        {
            var positions = refNeighborPositions;
            if (positions.IsCreated)
                positions.Dispose();

            positions = new NativeHashSet<int3>(TilingRules.Count, Allocator.Persistent);

            // Go through each rule that has been set for the specific tile we are settings and check if the neighbor rules match as well.
            foreach (var rule in TilingRules)
            {
                foreach (var neighbor in rule.GetNeighbors())
                {
                    var position = neighbor.Key.ToInt3();
                    positions.Add(position);

                    if (rule.m_RuleTransform == RuleMatchSerializable.TransformMatch.Rotated)
                    {
                        // Go through each of the four possible rotations angles.
                        for (var angle = RotationAngle; angle < 360; angle += RotationAngle)
                        {
                            positions.Add(GetRotatedPosition(position, angle));
                        }
                    }
                    else if (rule.m_RuleTransform == RuleMatchSerializable.TransformMatch.MirrorXY)
                    {
                        positions.Add(GetMirroredPosition(position, true, true));
                        positions.Add(GetMirroredPosition(position, true, false));
                        positions.Add(GetMirroredPosition(position, false, true));
                    }
                    else if (rule.m_RuleTransform == RuleMatchSerializable.TransformMatch.MirrorX)
                    {
                        positions.Add(GetMirroredPosition(position, true, false));
                    }
                    else if (rule.m_RuleTransform == RuleMatchSerializable.TransformMatch.MirrorY)
                    {
                        positions.Add(GetMirroredPosition(position, false, true));
                    }
                    else if (rule.m_RuleTransform == RuleMatchSerializable.TransformMatch.RotatedMirror)
                    {
                        var mirPos = GetMirroredPosition(position, true, false);
                        
                        for (var angle = RotationAngle; angle < 360; angle += RotationAngle)
                        {
                            positions.Add(GetRotatedPosition(position, angle));
                            positions.Add(GetRotatedPosition(mirPos, angle));
                        }
                    }
                }
            }
            
            refBounds = new BoundsInt();

            foreach (var positon in positions)
            {
                if (positon.x < refBounds.xMin)
                    refBounds.xMin = positon.x;
                if (positon.x > refBounds.xMax)
                    refBounds.xMax = positon.x;
                if (positon.y < refBounds.yMin)
                    refBounds.yMin = positon.y;
                if (positon.y > refBounds.yMax)
                    refBounds.yMax = positon.y;
            }

            refNeighborPositions = positions;
            positions.Dispose();
        }

        private int3 GetMirroredPosition(int3 position, bool mirrorX, bool mirrorY)
        {
            var curPosition = position;
            GetMirroredPosition(ref curPosition, mirrorX, mirrorY);
            return curPosition;
        }

        [BurstCompile]
        private static void GetMirroredPosition(ref int3 position, bool mirrorX, bool mirrorY)
        {
            if (mirrorX)
                position.x *= -1;
            if (mirrorY)
                position.y *= -1;
        }

        private int3 GetRotatedPosition(int3 position, int rotation)
        {
            var curPosition = position;
            GetRotatedPosition(ref curPosition, rotation);
            return curPosition;
        }

        [BurstCompile]
        private static void GetRotatedPosition(ref int3 position, int rotation)
        {
            switch (rotation)
            {
                case 90:
                    position = new int3(position.y, -position.x, 0);
                    break;
                case 180:
                    position = new int3(-position.x, -position.y, 0);
                    break;
                case 270:
                    position = new int3(-position.y, position.x, 0);
                    break;
            }
        }

        [BurstCompile]  
        [MonoPInvokeCallback(typeof(RefreshTileJobDelegate))]
        static unsafe void RefreshTileJob(int count, int3* position, void* data, ref TilemapRefreshStruct tilemapRefreshStruct)
        {
            // Grab the struct data that was converted to the void* in the CopyDataStruct for SFEntityIDTile
            var dataStruct = UnsafeUtility.AsRef<SFEntityIDTileDataStruct>(data);
            var refreshPositons = new NativeArray<int3>(dataStruct.NeighborPositions.Count + 1, 
                Allocator.Temp,
                NativeArrayOptions.UninitializedMemory);

            for (int i = 0; i < count; ++i)
            {
                // Convert the pointer of the Tile position back into the proper int3 type.
                var tilePos = *(position + i);
                var j       = 0;
                foreach (var neighborPosition in dataStruct.NeighborPositions)
                {
                    // The offset position in memory.
                    var offsetPosition = tilePos;
                    // The actual tile neighbor position in the TileMap.
                    var neighborOffset = neighborPosition;
                    GetOffsetPosition(ref offsetPosition, ref neighborOffset);
                    refreshPositons[j++] = offsetPosition;
                }
                // Grab the tilePos needing refreshed in the memory with a pointer.
                refreshPositons[j] = tilePos;
                // Refresh the needed tiles.
                for (int k = 0; k < refreshPositons.Length; ++k)
                {
                    tilemapRefreshStruct.RefreshTile(refreshPositons[k]);
                }
            }

            refreshPositons.Dispose();
        }
        
        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            var tilemapData = new TilemapDataStruct(tilemap.GetComponent<Tilemap>());
            unsafe
            {
                UnsafeUtility.CopyPtrToStructure(UnsafeUtility.AddressOf(ref _data.DefaultTileData), out tileData);

                var tilePos = position.ToInt3();
                GetTileDataJob(1, 
                            &tilePos,
                            UnsafeUtility.AddressOf(ref _data),
                            ref tilemapData,
                            (TileData*) UnsafeUtility.AddressOf(ref tileData)
                        );
            }
        }
        

        /// <summary>
        /// Returns the intPrt that stores data from the <see cref="Tilemap"/> using Unity's Jobs.
        /// </summary>
        /// <param name="count"></param>
        /// <param name="position"></param>
        /// <param name="data"></param>
        /// <param name="tilemapDataStruct"></param>
        /// <param name="outTileData"></param>
        [BurstCompile]
        [MonoPInvokeCallback(typeof(GetTileDataJobDelegate))]
        static unsafe void GetTileDataJob(int count, int3* position, void* data,
            ref TilemapDataStruct tilemapDataStruct, TileData* outTileData)
        {
            var dataStruct = UnsafeUtility.AsRef<SFEntityIDTileDataStruct>(data);
            // count is the amount of tiles we got to get the TileData for.
            for (int i = 0; i < count; ++i)
            {
                ref int3     inPos    = ref *(position + i);
                ref TileData tileData = ref *(outTileData + 1);
                UnsafeUtility.CopyPtrToStructure(UnsafeUtility.AddressOf(ref dataStruct.DefaultTileData), out tileData);

                for (int j = 0; j < dataStruct.RuleMatchInputs.Length; j++)
                {
                    var tilePosition = inPos;
                    var transform    = AffineTransform.identity;
                    var ruleMatch    = dataStruct.RuleMatchInputs[j];

                    if (RuleMatches(ref ruleMatch, ref tilemapDataStruct, ref dataStruct, ref tilePosition, ref transform))
                    {
                        var ruleMatchOutput = dataStruct.RuleMatchOutputs[j];
                        switch (ruleMatchOutput.spriteOutput)
                        {
                            case RuleMatchSerializable.OutputSprite.Single:
                            case RuleMatchSerializable.OutputSprite.Animation:
                                tileData.spriteEntityId = ruleMatchOutput.sprites.Length > 0 ? ruleMatchOutput.sprites[0] : EntityId.None;
                                break;
                        }

                        var trans = (float4x4)transform;
                        tileData.transform = UnsafeUtility.As<float4x4, Matrix4x4>(ref trans);
                        tileData.gameObjectEntityId = ruleMatchOutput.gameObjects.Length > 0 ? ruleMatchOutput.gameObjects[0] : EntityId.None;
                        tileData.colliderType = ruleMatchOutput.colliderType;
                        break;
                    }
                }
            }
        }

        [BurstCompile]
        private static void GetOffsetPosition(ref int3 offsetPosition, ref int3 offset)
        {
            offsetPosition += offset;
        }

#region Rule Matches
        /// <summary>
        ///     Checks if there is a match given the neighbor matching rule and a Tile.
        /// </summary>
        /// <param name="neighborRule">Neighbor matching rule.</param>
        /// <param name="thisOne">Tile to match.</param>
        /// <param name="otherOne">Other Tile to match.</param>
        /// <returns>True if there is a match, False if not.</returns>
        [BurstCompile]
        public static bool RuleMatch(int neighborRule, EntityId thisOne, EntityId otherOne)
        {
            switch (neighborRule)
            {
                case RuleEntityIdTile.TilingRuleOutput.Neighbor.This: return thisOne == otherOne;
                case RuleEntityIdTile.TilingRuleOutput.Neighbor.NotThis: return thisOne != otherOne;
            }
            return true;
        }
        
          /// <summary>
        ///     Checks if there is a match given the neighbor matching rule and a Tile with a rotation angle.
        /// </summary>
        /// <param name="ruleMatch">Neighbor matching rule.</param>
        /// <param name="tilemapData">Tilemap to match.</param>
        /// <param name="tileData">Tile to match.</param>
        /// <param name="position">Position of the Tile on the Tilemap.</param>
        /// <param name="angle">Rotation angle for matching.</param>
        /// <param name="mirrorX">Mirror X Axis for matching.</param>
        /// <returns>True if there is a match, False if not.</returns>
        [BurstCompile]
        public static bool RuleMatches(ref RuleMatchInput ruleMatch
            , ref TilemapDataStruct tilemapData
            , ref SFEntityIDTileDataStruct tileData, ref int3 position, int angle, bool mirrorX = false)
        {
            var minCount = math.min(ruleMatch.neighbors.Length, ruleMatch.matchingRules.Length);
            for (var i = 0; i < minCount; i++)
            {
                var neighborPosition = position;
                var neighbor = ruleMatch.matchingRules[i];
                var neighborOffset = ruleMatch.neighbors[i];
                if (mirrorX)
                    GetMirroredPosition(ref neighborOffset, true, false);
                GetRotatedPosition(ref neighborOffset, angle);
                GetOffsetPosition(ref neighborPosition, ref neighborOffset);
                var other = tilemapData.GetTileId(neighborPosition);
                if (!RuleMatch(neighbor, tileData.TileID, other)) return false;
            }
            return true;
        }

        /// <summary>
        ///     Checks if there is a match given the neighbor matching rule and a Tile with mirrored axii.
        /// </summary>
        /// <param name="ruleMatch">Neighbor matching rule.</param>
        /// <param name="tilemapData">Tilemap to match.</param>
        /// <param name="tileData">Tile to match.</param>
        /// <param name="position">Position of the Tile on the Tilemap.</param>
        /// <param name="mirrorX">Mirror X Axis for matching.</param>
        /// <param name="mirrorY">Mirror Y Axis for matching.</param>
        /// <returns>True if there is a match, False if not.</returns>
        [BurstCompile]
        public static bool RuleMatches(ref RuleMatchInput ruleMatch
            , ref TilemapDataStruct tilemapData
            , ref SFEntityIDTileDataStruct tileData, ref int3 position, bool mirrorX, bool mirrorY)
        {
            var minCount = math.min(ruleMatch.neighbors.Length, ruleMatch.matchingRules.Length);
            for (var i = 0; i < minCount; i++)
            {
                var neighborPosition = position;
                var neighbor = ruleMatch.matchingRules[i];
                var neighborOffset = ruleMatch.neighbors[i];
                GetMirroredPosition(ref neighborOffset, mirrorX, mirrorY);
                GetOffsetPosition(ref neighborPosition, ref neighborOffset);
                var other = tilemapData.GetTileId(neighborPosition);
                if (!RuleMatch(neighbor, tileData.TileID, other)) return false;
            }
            return true;
        }
        
        /// <summary>
        ///     Does a Rule Match given a Tiling Rule and neighboring Tiles.
        /// </summary>
        /// <param name="ruleMatch">The Tiling Rule to match with.</param>
        /// <param name="tilemapData">The tilemap to match with.</param>
        /// <param name="tileData">The Tile's data as a struct.</param>
        /// <param name="position">Position of the Tile on the Tilemap.</param>
        /// <param name="transform">A transform matrix which will match the Rule.</param>
        /// <returns>True if there is a match, False if not.</returns>
        [BurstCompile]
        public static bool RuleMatches(ref RuleMatchInput ruleMatch
            , ref TilemapDataStruct tilemapData
            , ref SFEntityIDTileDataStruct tileData
            , ref int3 position
            , ref AffineTransform transform)
        {
            if (RuleMatches(ref ruleMatch, ref tilemapData, ref tileData, ref position, 0))
            {
                transform = math.AffineTransform(int3.zero, quaternion.identity, new float3(1f));
                return true;
            }

            // Check rule against rotations of 0, 90, 180, 270
            if (ruleMatch.transformMatch == RuleMatchSerializable.TransformMatch.Rotated)
            {
                for (var angle = tileData.RotationAngle; angle < 360; angle += tileData.RotationAngle)
                    if (RuleMatches(ref ruleMatch, ref tilemapData, ref tileData, ref position, angle))
                    {
                        transform = math.AffineTransform(int3.zero, quaternion.Euler(0f, 0f, math.radians(-angle)), new float3(1f));
                        return true;
                    }
            }
            // Check rule against x-axis, y-axis mirror
            else if (ruleMatch.transformMatch == RuleMatchSerializable.TransformMatch.MirrorXY)
            {
                if (RuleMatches(ref ruleMatch, ref tilemapData, ref tileData, ref position, true, true))
                {
                    transform = math.AffineTransform(int3.zero, quaternion.identity, new float3(-1f, -1f, 1f));
                    return true;
                }

                if (RuleMatches(ref ruleMatch, ref tilemapData, ref tileData, ref position, true, false))
                {
                    transform = math.AffineTransform(int3.zero, quaternion.identity, new float3(-1f, 1f, 1f));
                    return true;
                }

                if (RuleMatches(ref ruleMatch, ref tilemapData, ref tileData, ref position, false, true))
                {
                    transform = math.AffineTransform(int3.zero, quaternion.identity, new float3(-1f, -1f, 1f));
                    return true;
                }
            }
            // Check rule against x-axis mirror
            else if (ruleMatch.transformMatch == RuleMatchSerializable.TransformMatch.MirrorX)
            {
                if (RuleMatches(ref ruleMatch, ref tilemapData, ref tileData, ref position, true, false))
                {
                    transform = math.AffineTransform(int3.zero, quaternion.identity, new float3(-1f, 1f, 1f));
                    return true;
                }
            }
            // Check rule against y-axis mirror
            else if (ruleMatch.transformMatch == RuleMatchSerializable.TransformMatch.MirrorY)
            {
                if (RuleMatches(ref ruleMatch, ref tilemapData, ref tileData, ref position, false, true))
                {
                    transform = math.AffineTransform(int3.zero, quaternion.identity, new float3(1f, -1f, 1f));
                    return true;
                }
            }
            // Check rule against x-axis mirror with rotations of 0, 90, 180, 270
            else if (ruleMatch.transformMatch == RuleMatchSerializable.TransformMatch.RotatedMirror)
            {
                for (var angle = 0; angle < 360; angle += tileData.RotationAngle)
                {
                    if (angle != 0 && RuleMatches(ref ruleMatch, ref tilemapData, ref tileData, ref position, angle))
                    {
                        transform = math.AffineTransform(int3.zero, quaternion.Euler(0f, 0f, math.radians(-angle)), new float3(1f));
                        return true;
                    }

                    if (RuleMatches(ref ruleMatch, ref tilemapData, ref tileData, ref position, angle, true))
                    {
                        transform = math.AffineTransform(int3.zero, quaternion.Euler(0f, 0f, math.radians(-angle)), new float3(-1f, 1f, 1f));
                        return true;
                    }
                }
            }

            return false;
        }
        
        #endregion
        
        /// <summary>
        /// Tracks custom data used to allow the low level physics 2D API to generate custom Contacts/Trigger Events.
        /// </summary>
        public struct SFEntityIDTileDataStruct : IDisposable
        {
            /// <summary>
            /// The <see cref="SFEntityIDTile"/> EntityID 
            /// </summary>
            public EntityId TileID;

            [NativeDisableContainerSafetyRestriction]
            public NativeArray<RuleMatchInput> RuleMatchInputs;
            
            [NativeDisableContainerSafetyRestriction]
            public NativeArray<RuleMatchOutput> RuleMatchOutputs;
            
            public NativeHashSet<int3> NeighborPositions;

            public BoundsInt Bounds;

            /// <summary>
            /// The rotation of the Tile Sprite being placed on a TileMap.
            /// </summary>
            /// <returns></returns>
            public int RotationAngle;

            /// <summary>
            /// The Default Tile Data 
            /// </summary>
            public TileData DefaultTileData;

            public void Dispose()
            {
                if (NeighborPositions.IsCreated)
                    NeighborPositions.Dispose();
                if (RuleMatchInputs.IsCreated)
                    RuleMatchInputs.Dispose();
                if (RuleMatchOutputs.IsCreated)
                    RuleMatchOutputs.Dispose();
            }
        }
    }
}