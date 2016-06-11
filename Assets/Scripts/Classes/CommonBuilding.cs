using UnityEngine;
using System.Collections;
using System;


public class CommonBuilding : Building
{

	public CommonBuilding (CommonBuilding other)
	{
		this.objectName = other.objectName;
		this.Width = other.Width;
		this.Height = other.Height;
		this.color = other.color;
		this.tile = other.tile;
	}

	public CommonBuilding (string objectName)
	{
		this.objectName = objectName;
		this.color = Color.gray;
		this.Width = 1;
		this.Height = 1;
		this.placementValidation = this.isValidPlacement;
	}

	static public CommonBuilding SpawnObject (CommonBuilding selected, Tile tile)
	{	
		if (selected.placementValidation (tile) == false) {
			Debug.Log ("Not valid.");
			return null;
		}
			
		CommonBuilding obj = new CommonBuilding (selected);
		obj.tile = tile;

		if (!tile.PlaceObject (obj)) {
			return null;
		}
		MapManager.Instance.map.buildings.Add (obj);
		return obj;
	}

	override public bool isValidPlacement (Tile t)
	{

		if (t == null)
			return false;

		if (t.Type != TileType.Floor) {
			return false;
		}

		if (t.building != null) {
			return false;
		}

		return true;
	}

	override public void ShowInfo ()
	{
		UIManager.Instance.OpenMenu (this.GetType ().ToString (), this.color);
	}

}

