using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Environment
{
	/// <summary>
	/// A container of a tile's fixed/characteristic information.
	/// </summary>
	[Serializable]
	public struct TileType
	{
		public TileType(ushort typeID, ushort spriteID, byte durabilityMax, byte durabilityHardness, bool isSolid)
		{
			this.typeID = typeID;
			this.spriteID = spriteID;
			this.durabilityMax = durabilityMax;
			this.durabilityHardness = durabilityHardness;
			this.isSolid = isSolid;
		}
		
		/// <summary>
		/// The unique ID of this tile type.
		/// </summary>
		public ushort typeID;
		
		/// <summary>
		/// The ID of the tile's texture.
		/// </summary>
		public ushort spriteID;
		
		/// <summary>
		/// The maximum/initial tile durability (health).
		/// </summary>
		public byte durabilityMax;
		/// <summary>
		/// The hardness modifier of this tile's durability.
		/// </summary>
		public byte durabilityHardness;

		/// <summary>
		/// Whether the tile can be stood on or not.
		/// </summary>
		public bool isSolid;
	}
	/// <summary>
	/// A granular instance of a block in the world; the type ID and state of a tile.
	/// </summary>
	public struct TileInstance
	{
		/// <summary>
		/// Creates a tile of the given type.
		/// Not optimized for repeated use!
		/// </summary>
		/// <param name="type">The type to initialize this type to.</param>
		public TileInstance(TileType type)
		{
			this.TypeID = type.typeID;
			this.IsSolid = type.isSolid;
			this.Overlay = 0;
			this.Underlay = 0;
		}
		/// <summary>
		/// Creates a tile instance using the given instance.
		/// </summary>
		/// <param name="instance">The instance to create from.</param>
		public TileInstance(TileInstance instance)
		{
			this = instance;
		}
		
		/// <summary>
		/// The type-ID of this tile; used for looking up the immutable data of the tile.
		/// </summary>
		public ushort TypeID;
		
		/// <summary>
		/// Whether this tile is solid or not (can be passed through).
		/// </summary>
		public bool IsSolid;

		/// <summary>
		/// Refers to a type of overlay for a tile.
		/// </summary>
		public enum OverlayType : ushort
		{
			None = 0,
			Iron = 1,
			Copper = 2,
			Silver = 3,
			Gold = 4,
			Diamond = 5,
			Azurite = 6,
			CloudOne = 7,
			CloudTwo = 8,
			CloudThree = 9,
		}
		/// <summary>
		/// The type of the overlay on the tile.
		/// </summary>
		public OverlayType Overlay;
		
		/// <summary>
		/// Refers to a type of background for a tile.
		/// </summary>
		public enum UnderlayType : ushort
		{
			Sky = 0,
			Dirt = 1,
			Dirter = 2,
			Dirtest = 3,
			Stone = 4,
			DeepStone = 5,
			Lava = 6,
		}
		/// <summary>
		/// The type of the underlay (background) for the tile.
		/// </summary>
		public UnderlayType Underlay;

		/// <summary>
		/// Sets this tile to be as the given one.
		/// </summary>
		/// <param name="newValue">The new tile to set this to.</param>
		public void Set(TileInstance newValue)
		{
			this = newValue;
		}
	}
	/// <summary>
	/// A GameObject representing a TileInstance in the Scene.
	/// Used as the interface for tile data to interact with the Scene.
	/// </summary>
	public class TileObject : MonoBehaviour
	{
		/// <summary>
		/// The renderer of this tile's primary graphic.
		/// </summary>
		public new SpriteRenderer renderer;
		/// <summary>
		/// The renderer of this tile's secondary graphic (overlay).
		/// </summary>
		public SpriteRenderer overlayRenderer;
		/// <summary>
		/// The renderer of this tile's tertiary graphic (background).
		/// </summary>
		public SpriteRenderer underlayRenderer;
		/// <summary>
		/// The scene collider for this tile. 
		/// </summary>
		public new Collider2D collider;

		/// <summary>
		/// Configure this tile GameObject to the given settings.
		/// </summary>
		/// <param name="position">The position to move to, in world coordinates.</param>
		/// <param name="sprite">The sprite to set.</param>
		/// <param name="overlaySprite">The sprite to place overlaid the base sprite.</param>
		/// <param name="isSolid">Whether this tile should collide or not.</param>
		public void Configure(Vector2 position, Sprite sprite, Sprite overlaySprite, Sprite underlaySprite, bool isSolid)
		{
			// Cache the transform for tiny performance gains.
			Transform localTransform = transform;
			// Update the size/scale of the transform.
			localTransform.position = new Vector3(position.x, position.y, localTransform.position.z);
			// Update the sprite.
			renderer.sprite = sprite;
			// Update the overlay sprite.
			overlayRenderer.sprite = overlaySprite;
			// Update the underlay sprite.
			underlayRenderer.sprite = underlaySprite;
			// Update the collider.
			collider.enabled = isSolid;
		}

		private void Awake()
		{
			renderer = GetComponent<SpriteRenderer>();
			overlayRenderer = transform.Find("Tile Sprite Overlay").GetComponent<SpriteRenderer>();
			underlayRenderer = transform.Find("Tile Sprite Underlay").GetComponent<SpriteRenderer>();
			collider = GetComponent<Collider2D>();
		}
	}
}

