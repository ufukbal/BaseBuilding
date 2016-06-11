using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{

	Vector3 lastFrame;
	Vector3 currentFrame;
	BuildManager buildManager;

	public enum MouseMode
	{
		SELECT,
		BUILD,
		BULDOZE
	}

	public MouseMode currentMode = MouseMode.SELECT;

	void Start ()
	{
		buildManager = GameObject.FindObjectOfType<BuildManager> ();
	}

	void Update ()
	{
		currentFrame = Camera.main.ScreenToWorldPoint (Input.mousePosition);

		if (Input.GetKeyUp (KeyCode.Escape)) {
			if (currentMode != MouseMode.SELECT) {
				currentMode = MouseMode.SELECT;
				buildManager.objectBuilding = false;

			} else if (currentMode == MouseMode.SELECT) {
				Debug.Log ("Open Menu");
			}
		}

		UpdateCamera ();

		if (!EventSystem.current.IsPointerOverGameObject ()) {
			
			if (Input.GetMouseButtonDown (0)) {
				
				if (currentMode == MouseMode.BUILD) {
					
					Tile tileUnderMouse = GetTileAtMouse ();
					if (tileUnderMouse != null) {
						buildManager.Build (tileUnderMouse);
					}

				} else if (currentMode == MouseMode.SELECT) {
					
					Tile tileUnderMouse = GetTileAtMouse ();

					if (tileUnderMouse != null && tileUnderMouse.building != null) {
						UIManager.Instance.ShowBuildingInfo (tileUnderMouse.building);
					}
					
				} else if (currentMode == MouseMode.BULDOZE) {
					Tile tileUnderMouse = GetTileAtMouse ();
					buildManager.Buldoze (tileUnderMouse);
				}

			}
		}

		lastFrame = Camera.main.ScreenToWorldPoint (Input.mousePosition);

	}

	public Tile GetTileAtMouse ()
	{
		return MapManager.Instance.GetTileAtWorldCoordinate (currentFrame);
	}

	void UpdateCamera ()
	{
		// use raycasts instead of screentoworldpoint for 3d setup
		if (Input.GetMouseButton (1)) {
			Vector3 diff = lastFrame - currentFrame;
			Camera.main.transform.Translate (diff);
			//TODO clamp position with map size
		}

		Camera.main.orthographicSize -= Camera.main.orthographicSize * Input.GetAxis ("Mouse ScrollWheel");
		Camera.main.orthographicSize = Mathf.Clamp (Camera.main.orthographicSize, 3, 13);
	}



}
