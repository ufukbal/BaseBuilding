using UnityEngine;
using System.Collections;
using System;

public class UniqueBuilding : Building
{

	public UniqueBuilding (UniqueBuilding other)
	{
		this.objectName = other.objectName;
		this.Width = other.Width;
		this.Height = other.Height;
		this.color = other.color;
		this.tile = other.tile;
	}

	public UniqueBuilding (string objectName, Color color)
	{
		this.objectName = objectName;
		this.color = color;
		this.Width = 2;
		this.Height = 2;
		this.placementValidation = this.isValidPlacement;
	}

	static public UniqueBuilding SpawnObject (UniqueBuilding selected, Tile tile)
	{

		if (selected.placementValidation (tile) == false) {
			Debug.Log ("Not valid.");
			return null;
		}

		UniqueBuilding obj = new UniqueBuilding (selected);
		obj.tile = tile;

		if (!tile.PlaceObject (obj)) {
			return null;
		}

		MapManager.Instance.map.buildings.Add (obj);
		return obj;
	}

	override public bool isValidPlacement (Tile t)
	{
		
		foreach (var item in MapManager.Instance.map.buildings) {
			if (item.objectName == this.objectName)
				return false;
		}


		if (t == null)
			return false;
		
		for (int xOffset = t.X; xOffset < t.X + Width; xOffset++) { // control all tiles that unique building occupies

			for (int yOffset = t.Y; yOffset < t.Y + Height; yOffset++) {
				
				Tile tOffset = t.map.GetTile (xOffset, yOffset);
				if (tOffset == null)
					return false;

				if (tOffset.Type != TileType.Floor) {
					return false;
				}

				if (tOffset.building != null) {
					return false;
				}
			}

		}
	
		return true;
	}

	override public void ShowInfo ()
	{
		UIManager.Instance.OpenMenu (this.objectName, this.color);
	}



}
