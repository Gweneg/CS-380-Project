using System.Collections;
using System.Collections.Generic;
using Resources;
using UnityEngine;

namespace Environment
{
	/// <summary>
	/// A container of a tile's fixed/characteristic information.
	/// </summary>
	public struct TileType
	{
		/// <summary>
		/// The unique ID of this tile type.
		/// </summary>
		public ushort TypeID;
		
		/// <summary>
		/// The ID of the tile's texture.
		/// </summary>
		public ushort TextureID;
		
		/// <summary>
		/// The maximum/initial tile durability (health).
		/// </summary>
		public byte DurabilityMax;
		/// <summary>
		/// The hardness modifier of this tile's durability.
		/// </summary>
		public float DurabilityHardness;
	}
	/// <summary>
	/// A granular instance of a block in the world; the type ID and state of a tile.
	/// </summary>
	public struct TileInstance
	{
		/// <summary>
		/// The type-ID of this tile; used for looking up the immutable data of the tile.
		/// </summary>
		public ushort TypeID;
		
		/// <summary>
		/// The X-position (coordinate) of this tile. Relative to the map array, such that 0 is the left of the map.
		/// </summary>
		public ushort PositionX;
		/// <summary>
		/// The Y-position (coordinate) of this tile. Relative to the map array, such that 0 is the bottom of the map.
		/// </summary>
		public ushort PositionY;
		
		/// <summary>
		/// The remaining durability of this tile.
		/// </summary>
		public byte DurabilityRemaining;
		/// <summary>
		/// Whether this tile is solid or not (can be passed through).
		/// </summary>
		public bool IsSolid;
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
		/// The scene collider for this tile. 
		/// </summary>
		public new Collider2D collider;

		/// <summary>
		/// Configure this tile GameObject to the given settings.
		/// </summary>
		/// <param name="position">The position to move to, in world coordinates.</param>
		/// <param name="sprite">The sprite to set.</param>
		/// <param name="isSolid">Whether this tile should collide or not.</param>
		public void Configure(Vector2 position, Sprite sprite, bool isSolid)
		{
			// Cache the transform for tiny performance gains.
			Transform localTransform = transform;
			// Update the size/scale of the transform.
			localTransform.localPosition = localTransform.InverseTransformPoint(position);
			// Update the sprite.
			renderer.sprite = sprite;
			// Update the collider.
			collider.enabled = isSolid;
		}
	}
}

