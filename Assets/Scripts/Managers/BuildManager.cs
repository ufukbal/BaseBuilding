using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class BuildManager : MonoBehaviour
{

	string buildObjectName;

	public bool objectBuilding = false;
	GameObject buildingPreview;
	InputManager inputManager;

	bool buldozePreviewHided;

	void Awake ()
	{
		inputManager = GameObject.FindObjectOfType<InputManager> ();

		if (buildingPreview == null) {
			buildingPreview = new GameObject ();
			buildingPreview.transform.SetParent (this.transform);
			buildingPreview.transform.name = "BuildingPreview";
			buildingPreview.AddComponent<SpriteRenderer> ();
			buildingPreview.SetActive (false);
		}
	}

	void Update ()
	{

		if (objectBuilding) {
			ShowBuildingPreview (buildObjectName, inputManager.GetTileAtMouse ());
		} else
			HideBuildingPreview ();

		if (inputManager.currentMode == InputManager.MouseMode.BULDOZE)
			ShowBuldozePreview (inputManager.GetTileAtMouse ());
		else {
			if (!buldozePreviewHided) {
				HideBuldozePreview ();
			}
		}
	}

	void ShowBuildingPreview (string buildingName, Tile t)
	{  //show building sprite on cursor

		if (t == null)
			return;
	
		buildingPreview.SetActive (true);
		SpriteRenderer sr = buildingPreview.GetComponent<SpriteRenderer> ();

		if (MapManager.Instance.map.IsBuildingPlacementValid (buildingName, t)) {
			sr.color = new Color (0.2f, 1f, 0.2f, 0.25f); //invalid placement color for cursor: red
		} else
			sr.color = new Color (1f, 0.2f, 0.2f, 0.25f); //valid placement color for cursor: green
		
		string path = "Images/Buildings/" + buildingName + "Sprite";
		sr.sprite = Resources.Load<Sprite> (path);
		buildingPreview.transform.position = new Vector3 (t.X, t.Y, 1);

	}

	void HideBuildingPreview ()
	{
		buildingPreview.SetActive (false);
	}

	void ShowBuldozePreview (Tile t)
	{

		if (t == null)
			return;

		if (t.building == null) {
			if (!buldozePreviewHided)
				foreach (var item in MapManager.Instance.spawnedBuildings) {
					item.Value.GetComponentInChildren<SpriteRenderer> ().color = Color.white;
					buldozePreviewHided = true;
					//FIXME TOO MUCH ITERATION. USE COMPONENT SYSTEM FOR BUILDINGS WITH ON HOVER.
				}
			return;
		}
		
		Tile masterTile = t.building.tile;
		if (!buldozePreviewHided)
			foreach (var item in MapManager.Instance.spawnedBuildings) {
				item.Value.GetComponentInChildren<SpriteRenderer> ().color = Color.white;
				buldozePreviewHided = true;
				//FIXME TOO MUCH ITERATION. USE COMPONENT SYSTEM FOR BUILDINGS WITH ON HOVER.
			}
		MapManager.Instance.spawnedBuildings [masterTile].GetComponentInChildren<SpriteRenderer> ().color = new Color (1f, 0.2f, 0.2f, 0.25f);
		buldozePreviewHided = false;

	}

	void HideBuldozePreview ()
	{
		if (MapManager.Instance.map == null)
			return;
		foreach (var item in MapManager.Instance.spawnedBuildings) {
			item.Value.GetComponentInChildren<SpriteRenderer> ().color = Color.white;
			buldozePreviewHided = true;
			//FIXME TOO MUCH ITERATION. USE COMPONENT SYSTEM FOR BUILDINGS WITH ON HOVER.
		}
	}

	public void BuildingMode (string objectName)
	{
		buildObjectName = objectName;
		Building obj = MapManager.Instance.map.buildingPrototypes [objectName];
		objectBuilding = true;
		inputManager.currentMode = InputManager.MouseMode.BUILD;
	}

	public void ResetBuildingMode ()
	{
		objectBuilding = false;
		if (inputManager == null) //FIXME: may be null after loading.
			inputManager = GameObject.FindObjectOfType<InputManager> (); 
		inputManager.currentMode = InputManager.MouseMode.SELECT;
	}


	public void BuldozeMode ()
	{
		objectBuilding = false;
		inputManager.currentMode = InputManager.MouseMode.BULDOZE;
	}

	public void Build (Tile t)
	{					
		if (objectBuilding) {
			MapManager.Instance.map.PlaceBuilding (buildObjectName, t);		
		} 
	}

	public void Buldoze (Tile t)
	{		
		if (t.building == null)
			return;
		
		t.DestroyObject (t); //TODO do not destroy-instantiate. IMPLEMENT POOLING SYSTEM
		ResetBuildingMode ();
	}

}
