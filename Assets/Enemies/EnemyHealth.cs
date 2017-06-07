using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Player;

public class EnemyHealth : MonoBehaviour {

    private GameObject Enemy;
    private GameObject MainCamera;
    private PUC PlayerUserController;
    public GameObject CanvasObject;
    public GameObject SliderObject;
    public GameObject DeathObject;

    private Slider ESlider;
   // private bool dead = false;

    // Use this for initialization
    void Start () {
        MainCamera = FindObjectOfType<CameraControl>().gameObject;
        Enemy = this.gameObject;
        ESlider = SliderObject.GetComponent<Slider>();
        PlayerUserController = FindObjectOfType<PUC>().GetComponent<PUC>();
    }
	
	// Update is called once per frame
	void Update () {

         SliderObject.transform.LookAt(MainCamera.transform);
         CanvasObject.transform.position = new Vector3(Enemy.transform.position.x, Enemy.transform.position.y , Enemy.transform.position.z);

    }

    public void Hit(float Damage)
    {
        ESlider.value -= Damage;
        if (ESlider.value <= 0)
        {
            Death();
        }
    }

    private void Death()
    {
        GetComponent<MeshRenderer>().enabled = false;
       // BroadcastMessage("Dead");
        Instantiate(DeathObject, transform.position, transform.rotation);
        PlayerUserController.LostLock(this.gameObject);
        //GetComponent<Enemy_AI_Control>().enabled = false;
        Destroy(SliderObject);
        Destroy(gameObject);
    }


}
