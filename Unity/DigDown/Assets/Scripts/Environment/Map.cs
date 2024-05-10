using System;
using System.Collections.Generic;
using System.Linq;
using GameResources;
using UnityEngine;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;
using EditorScripts;
using Unity.Collections;
using UnityEngine.Serialization;

namespace Environment
{
	/// <summary>
	/// Manages the map; exposes interactions with tiles.
	/// </summary>
	public class Map : MonoBehaviour
	{
		// SINGLETON
		public static Map Instance;
		
		// CONSTANTS
		/// <summary>
		/// The full-size (size measured between opposite edges) of tiles in the scene (width & height).
		/// </summary>
		public const float TileSize = 1f;
		/// <summary>
		/// The half-size (size measured from the center to the edge) of tiles in the scene (width & height).
		/// </summary>
		public const float TileHalfSize = TileSize / 2;
		/// <summary>
		/// The offset to apply to the position of any given tile.
		/// </summary>
		public static readonly Vector2 TileOffset = new(TileSize, TileSize);

		// FIELDS - TILE INFO
		/// <summary>
		/// A dictionary keying ore types to their respective overlay sprite IDs for display on tiles.
		/// </summary>
		public static readonly Dictionary<TileInstance.OverlayType, SpriteName> OreSpriteIDs = new()
		{
			{TileInstance.OverlayType.None, SpriteName.None},
			{TileInstance.OverlayType.Iron, SpriteName.IronOreOverlay},
			{TileInstance.OverlayType.Copper, SpriteName.CopperOreOverlay},
			{TileInstance.OverlayType.Silver, SpriteName.SilverOreOverlay},
			{TileInstance.OverlayType.Gold, SpriteName.GoldOreOverlay},
			{TileInstance.OverlayType.Diamond, SpriteName.DiamondOreOverlay},
			{TileInstance.OverlayType.Azurite, SpriteName.AzuriteOreOverlay},
			{TileInstance.OverlayType.CloudOne, SpriteName.CloudOneOverlay},
			{TileInstance.OverlayType.CloudTwo, SpriteName.CloudTwoOverlay},
			{TileInstance.OverlayType.CloudThree, SpriteName.CloudThreeOverlay},
		};
		/// <summary>
		/// A dictionary keying background types to their respective underlay sprite IDs for display under tiles.
		/// </summary>
		public static readonly Dictionary<TileInstance.UnderlayType, SpriteName> BackgroundSpriteIDs = new()
		{
			{TileInstance.UnderlayType.Sky, SpriteName.SkyBackground},
			{TileInstance.UnderlayType.Dirt, SpriteName.DirtBackground},
			{TileInstance.UnderlayType.Dirter, SpriteName.DirterBackground},
			{TileInstance.UnderlayType.Dirtest, SpriteName.DirtestBackground},
			{TileInstance.UnderlayType.Stone, SpriteName.StoneBackground},
			{TileInstance.UnderlayType.DeepStone, SpriteName.DeepStoneBackground},
			{TileInstance.UnderlayType.Lava, SpriteName.LavaBackground}
		};
		/// <summary>
		/// The different types tiles could be.
		/// </summary>
		public static readonly Dictionary<TileTypeName, TileType> TileTypes =
			new()
		{
			{TileTypeName.Air, new TileType(TileTypeName.Air, SpriteName.None, durabilityMax: 0, durabilityHardness: 1, isSolid: false)},
			{TileTypeName.GrassToDirt, new TileType(TileTypeName.GrassToDirt, SpriteName.GrassToDirt, durabilityMax: 1, durabilityHardness: 1, isSolid: true)},
			{TileTypeName.Dirt, new TileType(TileTypeName.Dirt, SpriteName.Dirt, durabilityMax: 2, durabilityHardness: 255/20, isSolid: true)},
			{TileTypeName.DirtToDirter, new TileType(TileTypeName.DirtToDirter, SpriteName.DirtToDirter, durabilityMax: 4, durabilityHardness: 255/8, isSolid: true)},
			{TileTypeName.Dirter, new TileType(TileTypeName.Dirter, SpriteName.Dirter, durabilityMax: 6, durabilityHardness: 255/4, isSolid: true)},
			{TileTypeName.DirterToDirtest, new TileType(TileTypeName.DirterToDirtest, SpriteName.DirterToDirtest, durabilityMax: 8, durabilityHardness: 255/8*3, isSolid: true)},
			{TileTypeName.Dirtest, new TileType(TileTypeName.Dirtest, SpriteName.Dirtest, durabilityMax: 12, durabilityHardness: 255/8*5, isSolid: true)},
			{TileTypeName.DirtestToStone, new TileType(TileTypeName.DirtestToStone, SpriteName.DirtestToStone, durabilityMax: 14, durabilityHardness: 255/8*6, isSolid: true)},
			{TileTypeName.Stone, new TileType(TileTypeName.Stone, SpriteName.Stone, durabilityMax: 20, durabilityHardness: 255, isSolid: true)},
			{TileTypeName.MoltenRock, new TileType(TileTypeName.MoltenRock, SpriteName.MoltenRock, durabilityMax: 10, durabilityHardness: 255, isSolid: true)},
		    {TileTypeName.Lava, new TileType(TileTypeName.Lava, SpriteName.Lava, durabilityMax: 2, durabilityHardness: 255, isSolid: true)},
			{TileTypeName.Void, new TileType(TileTypeName.Void, SpriteName.Void, durabilityMax: 255, durabilityHardness: 1, isSolid: true)},
			{TileTypeName.IronOre, new TileType(TileTypeName.IronOre, SpriteName.IronOreOverlay, durabilityMax: 20, durabilityHardness: 255, isSolid: true)},
			{TileTypeName.CopperOre, new TileType(TileTypeName.CopperOre, SpriteName.CopperOreOverlay, durabilityMax: 20, durabilityHardness: 255, isSolid: true)},
			{TileTypeName.SilverOre, new TileType(TileTypeName.SilverOre, SpriteName.SilverOreOverlay, durabilityMax: 30, durabilityHardness: 255, isSolid: true)},
			{TileTypeName.GoldOre, new TileType(TileTypeName.GoldOre, SpriteName.GoldOreOverlay, durabilityMax: 20, durabilityHardness: 255, isSolid: true)},
			{TileTypeName.DiamondOre, new TileType(TileTypeName.DiamondOre, SpriteName.DiamondOreOverlay, durabilityMax: 80, durabilityHardness: 255, isSolid: true)},
			{TileTypeName.AzuriteOre, new TileType(TileTypeName.AzuriteOre, SpriteName.AzuriteOreOverlay, durabilityMax: 40, durabilityHardness: 255, isSolid: true)},
			{TileTypeName.Cloud, new TileType(TileTypeName.Cloud, SpriteName.CloudOneOverlay, durabilityMax: 40, durabilityHardness: 255, isSolid: false)},
		};
		/// <summary>
		/// A dictionary of cached tile instance copies for cleaner/streamlined tile creation.
		/// </summary>
		public static readonly Dictionary<TileTypeName, TileInstance> TileInstanceCopies = TileTypes
		                                                                                  .Select(tileType => new TileInstance(tileType.Value))
		                                                                                  .ToDictionary(tileInstance => tileInstance.TypeID);
		
		// FIELDS - WORLD DATA
		/// <summary>
		/// The GameObject which parents all the map tiles.
		/// </summary>
		[Space(Formatting.CategoryTopPadding), Header("World Data")]
		private GameObject _mapHolder;
		/// <summary>
		/// The instances of tiles for the current map.
		/// </summary>
		public static TileInstance[,] InstanceTiles => Instance._instanceTiles;
		/// <summary>
		/// The backing field for instances of tiles for the current map.
		/// </summary>
		private TileInstance[,] _instanceTiles; // Backing field for property.
		/// <summary>
		/// The width of the map (in tiles).
		/// </summary>
		public static uint MapWidth
		{
			get => Instance._mapWidth;
			private set => Instance._mapWidth = value;
		}
		/// <summary>
		/// The backing field for the width of the map (in tiles).
		/// </summary>
		private uint _mapWidth;
		/// <summary>
		/// The height of the map (in tiles).
		/// </summary>
		public static uint MapHeight
		{
			get => Instance._mapHeight;
			private set => Instance._mapHeight = value;
		}
		/// <summary>
		/// The backing field for the height of the map (in tiles).
		/// </summary>
		private uint _mapHeight;

		// FIELDS - WORLD GENERATION SETTINGS
		/// <summary>
		/// The horizontal size caves should generate as.
		/// </summary>
		[Space(Formatting.CategoryTopPadding), Header("World Generation Settings")]
		public uint generationCaveLength = 15;
		/// <summary>
		/// The vertical size caves should generate as.
		/// </summary>
		public uint generationCaveHeight = 4;
		/// <summary>
		/// The number of caves to generate in the world.
		/// </summary>
		public uint generationCaveCount = 6;
		/// <summary>
		/// The width to generate the world at.
		/// </summary>
		public uint generationMapWidth = 30; 
		/// <summary>
		/// The height to generate the world at.
		/// </summary>
		public uint generationMapHeight = 120;
		/// <summary>
		/// The number of layers to generate the world with.
		/// </summary>
		/// <remarks>Set this to be no larger than the number of types established in the TileTypes, otherwise it will miss a key and crash.</remarks>
		public uint generationLayerCount = 6;
		/// <summary>
		/// The vertical size of each layer in the world.
		/// </summary>
		/// <remarks>If the total size of the layers (layerCount * layerSize) is greater than the map size, it will be truncated.
		/// If it is smaller, the last layer will be stretched.</remarks>
		public uint generationLayerSize = 20;


		// FIELDS - VIEWPORT
		/// <summary>
		/// The offset of the map from the world origin.
		/// </summary>
		private Vector2 _mapOffset;
		/// <summary>
		/// The bottom-left coordinate of the window in which tiles are rendered to TileObjects.
		/// </summary>
		private (uint x, uint y) _renderCorner;
		/// <summary>
		/// The width and height (in tiles) of the window in which tiles are rendered to TileObjects.
		/// Ideally equal to the current viewport size divided by TileSize plus a couple tile padding.
		/// </summary>
		private (ushort width, ushort height) _renderSize;
		/// <summary>
		/// The array of GameObjects used to represent tileInstance's within the scene (around the player).
		/// </summary>
		private TileObject[,] _renderTiles;


		// METHODS - VIEWPORT
		/// <summary>
		/// Adjusts the tile GameObjects in-scene to reflect the underlying map.
		/// Pools ones that are no longer visible, allocates now-visible ones.
		/// Note: The coordinates of the new corner position will be clamped to be within the true map dimensions.
		/// </summary>
		/// <param name="offset">Direction/amount to move the render window.</param>
		public void MoveRenderArea((long, long) offset)
		{
			// Unbox the offset X & Y.
			(long offsetX, long offsetY) = offset;
			// Cache the old lower-left coordinates of the render window.
			(long cornerX, long cornerY) = _renderCorner;
			// Cache the render dimensions
			(long renderWidth, long renderHeight) = _renderSize;

			// Clamp the offset to not over/underflow.
			uint newCoordinateX = (uint)Math.Clamp(offsetX + cornerX, 0, MapWidth - renderWidth);
			uint newCoordinateY = (uint)Math.Clamp(offsetY + cornerY, 0, MapHeight - renderHeight);

			// Set as normal.
			SetRenderArea((newCoordinateX, newCoordinateY));
		}
		/// <summary>
		/// Adjusts the tile GameObjects in-scene to reflect the underlying map.
		/// Note: The coordinates of the new corner position will be clamped to be within the true map dimensions.
		/// </summary>
		/// <param name="newBottomLeftCorner">The bottom-left coordinate to move the render window to.</param>
		public void SetRenderArea((uint, uint) newBottomLeftCorner)
		{
			// Note: Arrays are ordered [row, column], aka [y, x].
			// Note: Use of 'long' due to uints having huge range.

			// Cache the old lower-left coordinates of the render window.
			(long oldCornerX, long oldCornerY) = _renderCorner;
			// Compute the new lower-left coordinates of the render window.
			(long newCornerX, long newCornerY) = newBottomLeftCorner;
			// Compute the change in the (X,Y) coordinates.
			(long offsetX, long offsetY) = (newCornerX - oldCornerX, newCornerY - oldCornerY);
			// Cache the grid width and height of the render window.
			(long renderWidth, long renderHeight) = _renderSize;

			// Clamp the new coordinates to be within the map bounds.
			newCornerX = Math.Min(newCornerX, MapWidth - renderWidth);
			newCornerY = Math.Min(newCornerY, MapHeight - renderHeight);

			// Todo: Profile for-loop efficiency (swap inner/outer loops?).
			// Note: Row = Y = [rowIndex, 0]
			// Create an array with a write-index to store which rows of TileObjects need moving.
			HashSet<uint> markedRows = new();
			// Store the sign of the movement to allow movement either direction to be accounted for.
			long offsetYSign = Math.Sign(offsetY);
			// Compute which TileObject rows need to be moved, up to a maximum of all rows.
			switch (offsetYSign)
			{
				case -1:
					for (long rowIndex = 0; rowIndex < Math.Min(Math.Abs(offsetY), renderHeight); rowIndex++)
					{
						// Compute the index of the current row that safely wraps around the TileObject pool.
						markedRows.Add((uint)MathExtras.Operations.Modulo(value: oldCornerY - 1 - rowIndex,
																		   divisor: renderHeight));
					}
					break;
				case 1:
					for (long rowIndex = 0; rowIndex < Math.Min(Math.Abs(offsetY), renderHeight); rowIndex++)
					{
						// Compute the index of the current row that safely wraps around the TileObject pool.
						markedRows.Add((uint)MathExtras.Operations.Modulo(value: oldCornerY + rowIndex,
																		   divisor: renderHeight));
					}
					break;
			}


			// Note: Column = X = [0, rowIndex]
			// Create an array with a write-index to store which columns of TileObjects need moving.
			HashSet<uint> markedColumns = new();
			// Store the sign of the movement to allow movement either direction to be accounted for.
			long offsetXSign = Math.Sign(offsetX);
			// Compute which TileObject columns need to be moved, up to a maximum of all columns.
			// Each iteration: Modulo allows a clean way of wrapping around the list
			switch (offsetXSign)
			{
				case -1:
					for (long columnIndex = 0; columnIndex < Math.Min(Math.Abs(offsetX), renderWidth); columnIndex++)
					{
						markedColumns.Add((uint)MathExtras.Operations.Modulo(value: oldCornerX - 1 - columnIndex,
																			  divisor: renderWidth));
					}
					break;
				case 1:
					for (long columnIndex = 0; columnIndex < Math.Min(Math.Abs(offsetX), renderWidth); columnIndex++)
					{
						markedColumns.Add((uint)MathExtras.Operations.Modulo(value: oldCornerX + columnIndex,
																			  divisor: renderWidth));
					}
					break;
			}


			// Todo: Investigate other methods of iterating this (besides nested for loop).
			// Update each flagged TileObject by iterating over the entire 2D array and updating each one that's in either column/row.
			for (long renderRowIndex = 0; renderRowIndex < renderHeight; renderRowIndex++)
			{
				bool isWithinRow = markedRows.Contains((uint)renderRowIndex);

				for (long renderColumnIndex = 0; renderColumnIndex < renderWidth; renderColumnIndex++)
				{
					// Check if this tile type is within any of the
					bool isWithinColumn = markedColumns.Contains((uint)renderColumnIndex);

					// Do nothing if the tile does not need to be updated.
					if (!isWithinRow && !isWithinColumn)
					{
						continue;
					}

					// Compute the "chunk" the current render tile should sit within.
					// Chunk is found as the positively-clamped, rounded-up value of: (corner - index) / size.
					// E.g. Corner at 1, Index of render tile at 2, size at 3, means we have pos-ceil((1-1)/3) = pos-ceil(-2/3) = pos(-1) = 0
					long chunkCoordinateX = (long)Math.Max(Math.Ceiling((newCornerX - renderColumnIndex) / (double)renderWidth),
															0);
					long chunkCoordinateY = (long)Math.Max(Math.Ceiling((newCornerY - renderRowIndex) / (double)renderHeight),
															0);

					// Compute the new coordinate to place the tile at.
					uint newTileXCoordinate = (uint)(chunkCoordinateX * renderWidth + renderColumnIndex);
					uint newTileYCoordinate = (uint)(chunkCoordinateY * renderHeight + renderRowIndex);

					// Store the tile information that will be applied.
					TileInstance tileInstance = _instanceTiles[newTileYCoordinate,
															  newTileXCoordinate];
					TileType tileType = TileTypes[tileInstance.TypeID];
					Sprite tileSprite = ResourceManager.GetSprite(tileType.spriteID);
					Sprite tileOverlaySprite = ResourceManager.GetSprite(OreSpriteIDs[tileInstance.Overlay]);
					Sprite tileUnderlaySprite = ResourceManager.GetSprite(BackgroundSpriteIDs[tileInstance.Underlay]);
					bool isSolid = tileType.isSolid;

					// Compute the world-coordinates to place the tile at.
					Vector2 newTilePosition = GetWorldCoordinates((newTileXCoordinate, newTileYCoordinate));

					// Configure the tile object.
					TileObject tileObject = _renderTiles[renderRowIndex, renderColumnIndex];
					tileObject.Configure(newTilePosition, 
					                     sprite: tileSprite, 
					                     overlaySprite: tileOverlaySprite, 
					                     underlaySprite: tileUnderlaySprite, 
					                     isSolid);
				}
			}

			_renderCorner = ((uint)newCornerX, (uint)newCornerY);
		}
		/// <summary>
		/// Re-renders all render tiles.
		/// </summary>
		public void RerenderObjectTiles()
		{
			// Cache the grid width and height of the render window.
			(long renderWidth, long renderHeight) = _renderSize;

			// Update each tile.
			for (long rowIndex = 0; rowIndex < renderHeight; rowIndex++)
			{
				for (long columnIndex = 0; columnIndex < renderWidth; columnIndex++)
				{
					// Store the tile information that will be applied.
					TileInstance tileInstance = _instanceTiles[rowIndex, columnIndex];
					TileType tileType = TileTypes[tileInstance.TypeID];
					Sprite tileSprite = ResourceManager.GetSprite(tileType.spriteID);
					Sprite tileOverlaySprite = ResourceManager.GetSprite(OreSpriteIDs[tileInstance.Overlay]);
					Sprite tileUnderlaySprite = ResourceManager.GetSprite(BackgroundSpriteIDs[tileInstance.Underlay]);
					bool isSolid = tileType.isSolid;

					(uint xCoordinate, uint yCoordinate) = _renderCorner;

					// Compute the world-coordinates to place the tile at.
					Vector2 newTilePosition = GetWorldCoordinates((xCoordinate + (uint)columnIndex, yCoordinate + (uint)rowIndex));

					// Configure the tile object.
					TileObject tileObject = _renderTiles[rowIndex, columnIndex];
					tileObject.Configure(newTilePosition, 
					                     sprite: tileSprite, 
					                     overlaySprite: tileOverlaySprite,
					                     underlaySprite: tileUnderlaySprite,
					                     isSolid);
				}
			}
		}

		
		// METHODS - MAP INFO
		/// <summary>
		/// Gets the grid-coordinates of the tile at the given position.
		/// </summary>
		/// <param name="position">The world-space position to get the tile at.</param>
		/// <returns>The (X,Y) indices/grid-coordinates of the tile at the given position.</returns>
		public static (uint xCoordinate, uint yCoordinate) GetTileCoordinates(Vector2 position)
		{
			// Compute the coordinate by getting the relative position from the bottom-left corner of the map, then converting into tiles (instead of distance). 
			Vector2 offset = (position - Instance._mapOffset) / TileSize;
			// Round and clamp the coordinates to make them safe for use in indexing the map array.
			// Note: This means out-of-range coordinates will be clamped to the edge of the map. Add a warn?
			uint xCoordinate = (uint)Math.Clamp(offset.x, 0, MapWidth - 1);
			uint yCoordinate = (uint)Math.Clamp(offset.y, 0, MapHeight - 1);
			// Return the coordinates as a tuple.
			return (xCoordinate, yCoordinate);
		}
		/// <summary>
		/// Gets the world-space position of the tile at the given grid-coordinates.
		/// </summary>
		/// <param name="coordinates">The (X,Y) grid-coordinates of the tile.</param>
		/// <returns>The world-space position of hte tile at the given coordinates.</returns>
		public static Vector2 GetWorldCoordinates((uint, uint) coordinates)
		{
			// Unbox the x and y components.
			(uint xCoordinate, uint yCoordinate) = coordinates;
			return Instance._mapOffset + new Vector2(xCoordinate, yCoordinate) * TileSize;
		}
		/// <summary>
		/// Checks whether or not the tile at the specified coordinates is an Air tile.
		/// </summary>
		/// <param name="coordinates">The grid-coordinates (X,Y) of the tile to check.</param>
		/// <param name="mapSize">The dimensions (width, height) of the map being checked.</param>
		/// <param name="tileInstances">The map containing the tile to be checked.</param>
		/// <returns>True when the tile at the given coordinates is Air; False when it is not.</returns>
		public static bool IsAirTileAtCoordinates((uint, uint) coordinates, (uint, uint) mapSize, TileInstance[,] tileInstances)
		{
			// Unbox the x and y components.
			(uint xCoordinate, uint yCoordinate) = coordinates;
			(uint mapWidth, uint mapHeight) = mapSize;
			// Check if the coordinates are within the map bounds
			if (xCoordinate >= mapWidth || yCoordinate >= mapHeight)
			{
				Debug.LogError("Coordinates are out of map bounds!");
				return false;
			}

			// Get the tile instance at the specified coordinates
			TileInstance tileInstance = tileInstances[yCoordinate, xCoordinate];

			// Check if the tile type ID corresponds to an air tile
			return tileInstance.TypeID == 0;
		}

		
		// METHODS - SCENE INTERACTION
		/// <summary>
		/// Positions the map such that the middle of the bottom of it lies on the given point.
		/// </summary>
		/// <param name="position">The position (in world-space) to position the bottom-center of the map over.</param>
		public static void MoveMap(Vector2 position)
		{
			// Cache the map's transform.
			Transform mapTransform = Instance._mapHolder.transform;
			// Set the map's position.
			mapTransform.position = position + Vector2.left * TileSize;
			// Update the offset.
			Instance._mapOffset = mapTransform.position;
		}

		
		// METHODS - WORLD GEN
		/// <summary>
		/// Generates a map with the given dimensions.
		/// </summary>
		/// 
		public static void GenerateMap()
		{
			// Cache the map generation settings.
			uint mapWidth = Instance.generationMapWidth;
			uint mapHeight = Instance.generationMapHeight;
			(uint, uint) mapSize = (mapWidth, mapHeight);
			// Layers.
			uint layerSize = Instance.generationLayerSize;
			uint layerCount = Instance.generationLayerCount;
			// Caves.
			uint caveLength = Instance.generationCaveLength;
			uint caveHeight = Instance.generationCaveHeight;
			uint caveCount = Instance.generationCaveCount;
			
			// Update the stored map size.
			MapHeight = mapHeight;
			MapWidth = mapWidth;
			// Initialize the map instance array.
			Instance._instanceTiles = new TileInstance[MapHeight, MapWidth];
			
			// Cache other references.
			Dictionary<TileTypeName, TileInstance> tileInstanceCopies = TileInstanceCopies;
			TileInstance[,] instanceTiles = InstanceTiles;
			
			/* // Debug: This comment shows the basic process for creating and settings tiles in the world.
			 // 1. Get the type from the ID
			 TileType newTilesType = TileTypes[TileTypeName.Air];
			 // 2. Create the tile from the type
			 TileInstance newTileA = new TileInstance(newTilesType);
			 // 1+2. Create the tile from the ID
			 TileInstance newTileB = TileInstanceCopies[TileTypeName.Air];
			 // 3. Set the tile in the world
			 InstanceTiles[0, 0].Set(newTileB);
			*/

			// Each layer needs similar amount of rows excluding transition tiles (0,2,4,6)
			//Sample Output: 9 9 9 9, 8, 7 7 7 7, 6, 5 5 5 5, 4, 3 3 3 3, 2, 1 1 1 1, 0
			// Note: When adding new environment tiles add them in pairs to avoid changing this code a lot
			
			// Generate the terrain.
			_GenerateTerrain(mapSize: mapSize, 
			                 layerSize: layerSize, layerCount: layerCount,
			                 instanceTiles, tileInstanceCopies);

			// Generate the caves.
			Vector4 cavePadding = new Vector4(0, 1, 0f, 0.75f);
			_GenerateCaves(mapSize: mapSize,
			               instanceTiles, tileInstanceCopies,
			               caveCount: caveCount, caveLength: caveLength, caveHeight: caveHeight, 
			               cavePadding: cavePadding);
			
			// Generate ores.
			Vector4 commonOrePadding = new Vector4(0, 1, 0.5f, 0.85f);
			Vector4 uncommonOrePadding = new Vector4(0, 1, 0.15f, 0.65f);
			Vector4 rareOrePadding = new Vector4(0, 1, 0f, 0.25f);
			_GenerateOres(mapSize: mapSize,
			              mapInstanceTiles: instanceTiles,
			              commonOrePadding, uncommonOrePadding, rareOrePadding);

			// Generate decorations.
			Vector4 cloudPadding = new Vector4(0, 1, 0.95f, 1f);
			_GenerateDecorations(mapSize: (mapWidth, mapHeight),
			                     mapInstanceTiles: instanceTiles,
			                     cloudPadding);
			
			// Todo: Perform generation
			// Todo: Perform point-of-interest (caves, loot, etc.) generation
			// Todo: Perform world-gen validation
			// Todo: Perform navigation graph generation.
		}
		private static void _GenerateTerrain((uint, uint) mapSize, uint layerSize, uint layerCount,
		                                     TileInstance[,] mapInstanceTiles,
		                                     IReadOnlyDictionary<TileTypeName, TileInstance> tileInstanceCache)
		{
			// Unpack the map size.
			(uint mapWidth, uint mapHeight) = mapSize;
			
			// Generate the map terrain.
			for (uint row = 0; row < mapHeight; row++)
			{
				// Note: Must subtract from odd number or will fill map with transitional tiles instead
				uint distanceFromSurface = mapHeight - row - 1;
				// Compute the current layer.
				uint layerCurrent = distanceFromSurface / layerSize;
				// Clamp the layer to the number of layers.
				layerCurrent = Math.Min(layerCurrent, layerCount - 1);
				
				// Get the tile ID for the current layer.
				uint tileID = layerCurrent * 2;
				
				// Check if we're on the transition to the next layer.
				bool isLayerBoundary = (distanceFromSurface + 1) % layerSize == 0;
				// Change the tile to a layer-transition tile if we're on the edge of two layers.
				if (isLayerBoundary)
				{
					tileID += 1;
				}

				// Get the tile's type name.
				TileTypeName tileTypeName = (TileTypeName)tileID;
				// Configure the tile preset for the row (a little more optimized than doing it for each tile).
				TileInstance modifiedTile = tileInstanceCache[tileTypeName];
				modifiedTile.Underlay = (TileInstance.UnderlayType) layerCurrent;
				
				// Set the tile for each column in the row.
				for (int column = 0; column < mapWidth; column++)
				{
					// Set the instance for the current column/tile.
					mapInstanceTiles[row, column].Set(modifiedTile);
				}
			}
		}
		private static void _GenerateCaves((uint, uint) mapSize, 
		                                   TileInstance[,] mapInstanceTiles,
		                                   IReadOnlyDictionary<TileTypeName, TileInstance> tileInstanceCache,
		                                   uint caveCount, uint caveLength, uint caveHeight,
		                                   Vector4 cavePadding)
		{
			// Catch no caves generated.
			if (caveCount == 0) return;
			
			// Unpack the map size.
			(uint mapWidth, uint mapHeight) = mapSize;
			
			// Compute the cave padding.
			ClampPadding(ref cavePadding);
			Vector2 mapMaximums = new Vector2(mapWidth - 1, mapHeight - 1);
			(uint minCaveColumnPad, uint maxCaveColumnPad, uint minCaveRowPad, uint maxCaveRowPad) = ApplyPadding(cavePadding, 
			                                                                                                      mapMaximums);
			
			// Compute the maximum cave bounds.
			int minCaveColumn = (int)minCaveColumnPad;
			int maxCaveColumn = (int)maxCaveColumnPad - (int)caveLength;
			int minCaveRow = (int)minCaveColumnPad;
			int maxCaveRow = (int)maxCaveRowPad - (int)caveHeight;
			
			// Catch if the map/caves too large.
			if (maxCaveColumn < 0
			    || maxCaveRow < 0)
			{
				Debug.LogError($"Cave dimensions are too large for the map with the given size & padding.\nCave: {caveLength} x {caveHeight} tiles, <Left: {minCaveColumnPad}, Right: {maxCaveColumnPad}, Bottom: {minCaveRowPad}, Top: {maxCaveRowPad}> tiles.\nMap: {mapWidth} x {mapHeight} tiles.");
				return;
			}
			// Catch if the padding too small.
			if (caveLength > maxCaveColumnPad - minCaveColumnPad + 1
			    || caveHeight > maxCaveRowPad - maxCaveColumnPad + 1)
			{
				Debug.LogWarning($"Cave generation padding is too small; overrunning upper bounds.\nCave: {caveLength} x {caveHeight} tiles.\n<Left: {minCaveColumnPad}, Right: {maxCaveColumnPad}, Bottom: {minCaveRowPad}, Top: {maxCaveRowPad}> tiles.");
			}
			
			// Generate each cave.
			for (int caveIndex = 0; caveIndex < caveCount; caveIndex++)
			{
				// Compute the start column for the cave.
				uint startColumn = (uint) Random.Range(minCaveColumn, maxCaveColumn);
				// Compute the start row for the cave.
				uint startRow = (uint) Random.Range(minCaveRow, maxCaveRow);
				
				// Create the cave area.
				for (uint row = 0; row < caveHeight; row++)
				{
					for (uint column = 0; column < caveLength; column++)
					{
						// Store the tile's background type before changing it.
						TileInstance.UnderlayType underlayType = mapInstanceTiles[row + startRow, column + startColumn].Underlay;
						// Create an air tile.
						TileInstance airTile = tileInstanceCache[TileTypeName.Air];
						// Set the tile instance at the current coordinates to the new tile.
						mapInstanceTiles[row + startRow, column + startColumn].Set(airTile);
						// Set the tile's background type back to what it was.
						SetTileUnderlay(coordinates: (column + startColumn, row + startRow), 
						                underlayType, mapInstanceTiles);
					}
				}
			}
		}
		private static void _GenerateOres((uint, uint) mapSize,
		                                  TileInstance[,] mapInstanceTiles,
		                                  Vector4 commonOrePadding, Vector4 uncommonOrePadding, Vector4 rareOrePadding)
		{
			// Unpack the map size.
			(uint mapWidth, uint mapHeight) = mapSize;
			
			// Common ores.
			SetRandomTiles(id: TileTypeName.Air,
			               padding: commonOrePadding,
			               mapSize: (mapWidth, mapHeight),
			               instanceTiles: mapInstanceTiles,
			               150); // Set random air.
			SetRandomTiles(id: TileTypeName.IronOre,
			               padding: commonOrePadding,
			               mapSize: (mapWidth, mapHeight),
			               instanceTiles: mapInstanceTiles,
			               30); // Set random iron.
			SetRandomTiles(id: TileTypeName.CopperOre,
			               padding: commonOrePadding,
			               mapSize: (mapWidth, mapHeight),
			               instanceTiles: mapInstanceTiles,
			               20); // Set random copper.
			
			// Uncommon ores.
			SetRandomTiles(id: TileTypeName.SilverOre,
			               padding: uncommonOrePadding,
			               mapSize: (mapWidth, mapHeight),
			               instanceTiles: mapInstanceTiles,
			               16); // Set random silver.
			SetRandomTiles(id: TileTypeName.GoldOre,
			               padding: uncommonOrePadding,
			               mapSize: (mapWidth, mapHeight),
			               instanceTiles: mapInstanceTiles,
			               10); // Set random gold.
			
			// Rare ores.
			SetRandomTiles(id: TileTypeName.DiamondOre,
			               padding: rareOrePadding,
			               mapSize: (mapWidth, mapHeight),
			               instanceTiles: mapInstanceTiles,
			               5); // Set random diamond.
			SetRandomTiles(id: TileTypeName.AzuriteOre,
			                padding: rareOrePadding,
			                mapSize: (mapWidth, mapHeight),
			                instanceTiles: mapInstanceTiles,
			                5); // Set random azurite.
		}
		private static void _GenerateDecorations((uint, uint) mapSize,
		                                         TileInstance[,] mapInstanceTiles,
		                                         Vector4 cloudPadding)
		{
			SetRandomTiles(id: TileTypeName.Cloud,
			               padding: cloudPadding,
			               mapSize: mapSize,
			               instanceTiles: mapInstanceTiles,
			               25); // Set random clouds.
		}
		

		// METHODS - MAP CONFIGURATION
		/// <summary>
		/// Configures the map gen's width & height, and layer count & size (depth).
		/// </summary>
		/// <param name="newMapDimensions">The dimensions (width, height) of the generated map (in tiles).</param>
		/// <param name="layerCount">The number of layers to generate in the world.</param>
		/// <param name="layerSize">The size of each layer (in tiles) generated in the world.</param>
		/// <remarks>Do not pass a layerCount larger than the number of types established in TileTypes, otherwise unexpected behaviour will arise.</remarks>
		public static void ConfigureMap((uint, uint) newMapDimensions, uint layerSize, uint layerCount)
		{
			// Check if we need to create the MapHolder GameObject.
			if (Instance._mapHolder == null)
			{
				Instance._mapHolder = new GameObject("MapHolder");
			}
			// Unpack the dimensions.
			(uint width, uint height) = newMapDimensions;
			// Set the map gen dimensions.
			Instance.generationMapWidth = width;
			Instance.generationMapHeight = height;
			// Set the layer gen dimensions.
			Instance.generationLayerSize = layerSize;
			Instance.generationLayerCount = layerCount;
		}
		/// <summary>
		/// Configures the render tiles objects of the map.
		/// </summary>
		/// <param name="width">The width (in tiles) of the render view.</param>
		/// <param name="height">The height (in tiles) of the render view.</param>
		public static void ConfigureRenderTiles(ushort width, ushort height)
		{
			// Cache the old render size.
			(ushort oldWidth, ushort oldHeight) = Instance._renderSize;

			// Remake the array if it's a different size than before.
			if (width != oldWidth || height != oldHeight)
			{
				TileObject[,] oldRenderTiles = Instance._renderTiles;
				TileObject[,] newRenderTiles = new TileObject[height, width];

				for (int rowIndex = 0; rowIndex < height; rowIndex++)
				{
					for (int columnIndex = 0; columnIndex < width; columnIndex++)
					{
						bool isWithinOldWidth = columnIndex < oldWidth;
						bool isWithinOldHeight = rowIndex < oldHeight;

						switch (isWithinOldWidth)
						{
							// Try to copy data from the old array.
							case true when isWithinOldHeight: // Copy successful.
								newRenderTiles[rowIndex, columnIndex] = oldRenderTiles[rowIndex, columnIndex];
								break;
							case false when !isWithinOldHeight: // Copy unsuccessful- must create new!
								// Create the tile.
								GameObject newInstanceTile = new GameObject(name: $"Tile ({columnIndex}, {rowIndex})",
																 typeof(BoxCollider2D),
																 typeof(SpriteRenderer))
								{
									transform =
									{
										parent = Instance._mapHolder.transform,
										localScale = new Vector3(TileSize, TileSize, TileSize)
									}
								};
								
								// Create the tile sprite overlay.
								GameObject newInstanceTileOverlay = new GameObject(name: "Tile Sprite Overlay")
								{
									transform =
									{
										parent = newInstanceTile.transform,
										localScale = new Vector3(1, 1, 1),
										localPosition = new Vector3(0, 0, 0),
									}
								};
								SpriteRenderer spriteOverlayRenderer = newInstanceTileOverlay.AddComponent<SpriteRenderer>();
								spriteOverlayRenderer.sortingOrder = 1;
								
								// Create the tile sprite underlay.
								GameObject newInstanceTileUnderlay = new GameObject(name: "Tile Sprite Underlay")
								{
									transform =
									{
										parent = newInstanceTile.transform,
										localScale = new Vector3(1, 1, 1),
										localPosition = new Vector3(0, 0, 0),
									}
								};
								SpriteRenderer spriteUnderlayRenderer = newInstanceTileUnderlay.AddComponent<SpriteRenderer>();
								spriteUnderlayRenderer.sortingOrder = -1;
								
								// Add the tile component to the GameObject. Note: Hacky workaround to making the Awake() not brick itself.
								newInstanceTile.AddComponent<TileObject>();
								
								// Store the created tile.
								newRenderTiles[rowIndex, columnIndex] = newInstanceTile.GetComponent<TileObject>();
								break;
						}
					}
				}

				// Clean up old tiles that will be out-of-range of the new.
				if (height < oldHeight)
				{
					// Destroy each GameObject in each unused row.
					for (int rowIndex = height; rowIndex < oldHeight; rowIndex++)
					{
						for (int columnIndex = 0; columnIndex < oldWidth; columnIndex++)
						{
							Destroy(oldRenderTiles[rowIndex, columnIndex]);
						}
					}
				}
				if (width < oldWidth)
				{
					// Destroy each GameObject in each unused row.
					for (int rowIndex = 0; rowIndex < oldHeight; rowIndex++)
					{
						for (int columnIndex = width; columnIndex < oldWidth; columnIndex++)
						{
							Destroy(oldRenderTiles[rowIndex, columnIndex]);
						}
					}
				}

				// Assign the new render tile array.
				Instance._renderTiles = newRenderTiles;
			}

			// Update the render size.
			Instance._renderSize = (width, height);
		}
		
		
		// METHODS - MAP INTERACTION
		/// <summary>
		/// Changes a number of random tiles in the map into the provided type.
		/// </summary>
		/// <param name="id">The ID to set a random tile to.</param>
		/// <param name="padding">The padding (area excluded, in percent of map width and height) for the left (X), right (Y), bottom (Z), and top (W) sides respectively.</param>
		/// <param name="mapSize">The size (width, height) of the generated map (in tiles).</param>
		/// <param name="instanceTiles">The map to randomly set tiles within.</param>
		/// <param name="repetitions">The number of times to repeat the random change.</param>
		/// <param name="maxRepetitions">The maximum number of times to repeat the random change before giving up.</param>
		public static void SetRandomTiles(TileTypeName id,
		                                  Vector4 padding,
		                                  (uint, uint) mapSize,
		                                  TileInstance[,] instanceTiles,
		                                  int repetitions, int maxRepetitions = 100)
		{
			// Unpack the dimensions.
			(uint mapWidth, uint mapHeight) = mapSize;
			
			// Clamp the padding to be within the map bounds.
			ClampPadding(ref padding);
			
			// Compute the bounds from the padding.
			Vector2 mapMaximums = new Vector2(mapWidth - 1, mapHeight - 1);
			(uint minX, uint maxX, uint minY, uint maxY) = ApplyPadding(padding, mapMaximums);
			
			// Track the number of failed repetitions.
			uint failedRepetitions = 0;
			
			// Add random ore & air pockets to the map.
			for (int i = 0; i <= repetitions; i++)
			{
				// Get random coordinates from the Map
				ushort randomX = (ushort) Random.Range(minX, maxX);
				ushort randomY = (ushort) Random.Range(minY, maxY);
				
				// Check if the random tile is air.
				bool isAirTile = IsAirTileAtCoordinates((randomX, randomY), mapSize, instanceTiles);
				
				// Perform the tile change based on the ID.
				switch (id)
				{
					case TileTypeName.Air: // Air
						// Save the background at the tile's location.
						TileInstance.UnderlayType currentUnderlay = instanceTiles[randomY, randomX].Underlay;
						// Get & configure the tile instance that will replace the current tile.
						TileInstance airTile = TileInstanceCopies[id];
						// Set the tile instance at the current coordinates to the new tile.
						instanceTiles[randomY, randomX].Set(airTile);
						// Set the underlay to the previous underlay.
						SetTileUnderlay(coordinates: (randomX, randomY), currentUnderlay, instanceTiles);
						break;
					
					case TileTypeName.IronOre:
						if (isAirTile == false)
						{
							SetTileOverlay(coordinates: (randomX, randomY), TileInstance.OverlayType.Iron, instanceTiles);
						}
						else
						{
							// Attempt another random tile.
							i--;
							// Track the number of failed repetitions.
							failedRepetitions += 1;
							if (failedRepetitions > maxRepetitions)
							{
								Debug.LogError("Failed to set random tiles after 100 attempts.");
								return;
							}
						}
						break;
					
					case TileTypeName.CopperOre:
						if (isAirTile == false)
						{
							SetTileOverlay(coordinates: (randomX, randomY), TileInstance.OverlayType.Copper, instanceTiles);
						}
						else
						{
							// Attempt another random tile.
							i--;
							// Track the number of failed repetitions.
							failedRepetitions += 1;
							if (failedRepetitions > maxRepetitions)
							{
								Debug.LogError("Failed to set random tiles after 100 attempts.");
								return;
							}
						}
						break;
					
					case TileTypeName.SilverOre:
						if (isAirTile == false)
						{
							SetTileOverlay(coordinates: (randomX, randomY), TileInstance.OverlayType.Silver, instanceTiles);
						}
						else
						{
							// Attempt another random tile.
							i--;
							// Track the number of failed repetitions.
							failedRepetitions += 1;
							if (failedRepetitions > maxRepetitions)
							{
								Debug.LogError("Failed to set random tiles after 100 attempts.");
								return;
							}
						}
						break;
					
					case TileTypeName.GoldOre:
						if(isAirTile == false)
						{
							SetTileOverlay(coordinates: (randomX, randomY), TileInstance.OverlayType.Gold, instanceTiles);
						}
						else
						{
							// Attempt another random tile.
							i--;
							// Track the number of failed repetitions.
							failedRepetitions += 1;
							if (failedRepetitions > maxRepetitions)
							{
								Debug.LogError("Failed to set random tiles after 100 attempts.");
								return;
							}
						}
						break;
					
					case TileTypeName.DiamondOre:
						if (isAirTile == false)
						{
							SetTileOverlay(coordinates: (randomX, randomY), TileInstance.OverlayType.Diamond, instanceTiles);
						}
						else
						{
							// Attempt another random tile.
							i--;
							// Track the number of failed repetitions.
							failedRepetitions += 1;
							if (failedRepetitions > maxRepetitions)
							{
								Debug.LogError("Failed to set random tiles after 100 attempts.");
								return;
							}
						}
						break;
					
					case TileTypeName.AzuriteOre:
						if (isAirTile == false)
						{
							SetTileOverlay(coordinates: (randomX, randomY), TileInstance.OverlayType.Azurite, instanceTiles);
						}
						else
						{
							// Attempt another random tile.
							i--;
							// Track the number of failed repetitions.
							failedRepetitions += 1;
							if (failedRepetitions > maxRepetitions)
							{
								Debug.LogError("Failed to set random tiles after 100 attempts.");
								return;
							}
						}
						break;
					
					case TileTypeName.Cloud: // Debug: Temp cloud
						if (isAirTile)
						{
							TileInstance.OverlayType cloudType = Random.Range(0, 4) switch
							{
								0 => TileInstance.OverlayType.CloudOne,
								1 => TileInstance.OverlayType.CloudTwo,
								2 => TileInstance.OverlayType.CloudThree,
								_ => TileInstance.OverlayType.CloudThree
							};
							SetTileOverlay(coordinates: (randomX, randomY), cloudType, instanceTiles);
						}
						else
						{
							// Attempt another random tile.
							i--;
							// Track the number of failed repetitions.
							failedRepetitions += 1;
							if (failedRepetitions > maxRepetitions)
							{
								Debug.LogError("Failed to set random tiles after 100 attempts.");
								return;
							}
						}
						break;
				}
			}
		}
		/// <summary>
		/// Sets the overlay on the tile at the given coordinates.
		/// </summary>
		/// <param name="coordinates">The (X,Y) grid-coordinates of the tile to set the overlay of.</param>
		/// <param name="overlayType">The overlay type to set on the tile.</param>
		/// <param name="instanceTiles">The map holding the tile.</param>
		public static void SetTileOverlay((uint, uint) coordinates, TileInstance.OverlayType overlayType, TileInstance[,] instanceTiles)
		{
			// Unpack the coordinates.
			(uint x, uint y) = coordinates;
			// Get the tile.
			TileInstance tile = instanceTiles[y, x];
			// Update the tile's overlay type.
			tile.Overlay = overlayType;
			// Reassign the tile.
			instanceTiles[y, x] = tile;
		}
		/// <summary>
		/// Sets the underlay (background) on the tile at the given coordinates.
		/// </summary>
		/// <param name="coordinates">The (X,Y) grid-coordinates of the tile to set the overlay of.</param>
		/// <param name="underlayType">The underlay (background) type to set on the tile.</param>
		/// <param name="instanceTiles">The map holding the tile.</param>
		public static void SetTileUnderlay((uint, uint) coordinates, TileInstance.UnderlayType underlayType, TileInstance[,] instanceTiles)
		{
			// Unpack the coordinates.
			(uint x, uint y) = coordinates;
			// Get the tile.
			TileInstance tile = instanceTiles[y, x];
			// Update the tile's underlay type.
			tile.Underlay = underlayType;
			// Reassign the tile.
			instanceTiles[y, x] = tile;
		}
		/// <summary>
		/// Breaks the tile at the given location- setting it to air, making particles, and playing a sound.
		/// </summary>
		/// <param name="coordinates">The (X,Y) grid-coordinates of the tile to break.</param>
		/// <returns>The tile that was broken.</returns>
		public TileInstance BreakTile((uint, uint) coordinates)
		{
			// Unpack the coordinates.
			(uint x, uint y) = coordinates;
			// Get the tile at the given coordinates.
			TileInstance tile = _instanceTiles[y, x];
			
			// Check if the tile is already air.
			if (tile.TypeID == 0)
			{
				return tile;
			}
			
			// Set the tile to air in the map.
			_instanceTiles[y, x].Set(tile);
			// Play the break sound.
			//AudioManager.PlaySound("Break", ); // Todo: Play sound in world
			// Create the break particles.
			//ParticleManager.CreateParticles("Break", GetWorldCoordinates((x, y)));

			return tile;
		}
		
		
		// METHODS - MATH
		/// <summary>
		/// Clamps and de-crosses the given padding.
		/// </summary>
		/// <param name="padding">The padding to clamp and de-cross.</param>
		public static void ClampPadding(ref Vector4 padding)
		{
			// Clamp the padding to [0,1].
			padding.x = Mathf.Clamp01(padding.x);
			padding.y = Mathf.Clamp01(padding.y);
			padding.z = Mathf.Clamp01(padding.z);
			padding.w = Mathf.Clamp01(padding.w);
			// Clamp the padding to not cross itself.
			padding.x = Mathf.Min(padding.x, padding.y);
			padding.y = Mathf.Max(padding.x, padding.y);
			padding.z = Mathf.Min(padding.z, padding.w);
			padding.w = Mathf.Max(padding.z, padding.w);
		}
		/// <summary>
		/// Computes and returns the calculated limits from the given maximums and paddings.
		/// </summary>
		/// <param name="padding">The amount to pad each direction (where X:Left, Y:Right, Y:Bottom, W:Top).</param>
		/// <param name="maximums">The maximum X (left/right) and Y (top/bottom) values.</param>
		/// <returns>The limits from the given maximums and padding, (where X:Left, Y:Right, Y:Bottom, W:Top)</returns>
		public static (uint, uint, uint, uint) ApplyPadding(Vector4 padding, Vector2 maximums)
		{
			// Compute the limits.
			uint left = (uint)(padding.x * maximums.x);
			uint right = (uint)(padding.y * maximums.x);
			uint bottom = (uint)(padding.z * maximums.y);
			uint top = (uint)(padding.w * maximums.y);
			
			// Return normally if the padding is not overlapping itself.
			if (left != right)
				return (left, right, bottom, top);
			
			// Fix padding sitting on itself.
			if (left == 0)
			{
				right += 1;
			}
			else
			{
				left -= 1;
			}
			return (left, right, bottom, top);
		}
		
		
		// METHODS - UNITY
		private void Start()
		{
			// Set the singleton instance.
			if (Instance != null && Instance != this)
			{
				Debug.LogError("Cannot have more than one Map script!");
				return;
			}
			Instance = this;
			
			ConfigureMap(newMapDimensions: (30, 120), layerSize: 20, layerCount: 6);
			ConfigureRenderTiles(25, 12);
			GenerateMap();
			MoveMap(Vector2.zero);
			RerenderObjectTiles();

			Debug.Log($"Map size: {MapWidth}, {MapHeight}");
		}

		private void Update()
		{
			if (isDebugCameraMoveEnabled)
				DoDebugCameraMovement();
		}

		private static Camera _mainCamera;
		private static bool _isPanning;
		private static Vector2 _panPosition;
		[SerializeField]
		private bool isDebugCameraMoveEnabled;
		private void DoDebugCameraMovement()
		{
			if (Input.GetKeyDown(KeyCode.Mouse0))
			{
				_mainCamera = Camera.main;
				_panPosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
				_isPanning = true;
			}
			else if (Input.GetKeyUp(KeyCode.Mouse0))
			{
				_isPanning = false;
			}
			if (_isPanning)
			{
				// Compute the camera's new position.
				Vector3 newPosition = _mainCamera.transform.position + (Vector3) (_panPosition - (Vector2) _mainCamera.ScreenToWorldPoint(Input.mousePosition));
				Vector3 cameraCornerOffset = new Vector3(_mainCamera.orthographicSize * _mainCamera.aspect, _mainCamera.orthographicSize);
				Vector3 newPositionBLCorner = newPosition - cameraCornerOffset;
				
				// Compute the bounds of the map.
				Vector2 bottomleftCorner = GetWorldCoordinates((0, 0));
				Vector2 toprightCorner = GetWorldCoordinates((MapWidth - 1, MapHeight - 1));
				
				// Clamp the new position to within the bounds of the map.
				Vector3 clampedPosition = new Vector3(x: Math.Clamp(newPositionBLCorner.x, bottomleftCorner.x, toprightCorner.x), 
				                                      y: Math.Clamp(newPositionBLCorner.y, bottomleftCorner.y, toprightCorner.y), 
				                                      z: newPositionBLCorner.z);
				_mainCamera.transform.position = clampedPosition + cameraCornerOffset;
				// Update the corner.
				SetRenderArea(GetTileCoordinates(clampedPosition));
			}
			
			// TODO: DEBUG/DEMONSTRATION CODE, REMOVE
			if (Input.GetKey(KeyCode.UpArrow))
			{
				MoveRenderArea((0, 1));
			}
			if (Input.GetKey(KeyCode.DownArrow))
			{
				MoveRenderArea((0, -1));
			}
			if (Input.GetKey(KeyCode.LeftArrow))
			{
				MoveRenderArea((-1, 0));
			}
			if (Input.GetKey(KeyCode.RightArrow))
			{
				MoveRenderArea((1, 0));
			}
		}
	}
}