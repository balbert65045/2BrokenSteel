using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour {

    public GameObject Enemy;
    public Camera MainCamera;
    public Canvas Canvas;

    private RectTransform CanvasRect;

    // Use this for initialization
    void Start () {

        CanvasRect = Canvas.GetComponent<RectTransform>();
    }
	
	// Update is called once per frame
	void Update () {

        transform.rotation = Quaternion.identity;

        Vector2 ViewPortPosition = RectTransformUtility.WorldToScreenPoint(MainCamera, Enemy.transform.position);
        Vector2 WorldObject_ScreenPosition = new Vector2(
        ((ViewPortPosition.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f)),
        ((ViewPortPosition.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f)));


        transform.position = ViewPortPosition;


    }



}
