using UnityEngine;
using System.Collections;

public class Hero : MonoBehaviour {

    static public Hero S; //Singleton

    public float gameRestartDelay = 2f;

    //These fields control the movement of the ship
    public float speed = 30;
    public float rollMult = -45;
    public float pitchMult = 30;

    //Ship status information
    [SerializeField]
    public float _shieldLevel = 1;  //Added an underscore

    public bool ____________________________;
    public Bounds bounds;

    void Awake()
    {
        S = this; //Set the Singleton
        bounds = Utils.CombineBoundsOfChildren(this.gameObject);
    }
    void Update ()
    {
        //Pull in information from the Input class
        float xAxis = Input.GetAxis("Horizontal");  //1
        float yAxis = Input.GetAxis("Vertical");    //1

        //Change transform.position based on the axes
        Vector3 pos = transform.position;
        pos.x += xAxis * speed * Time.deltaTime;
        pos.y += yAxis * speed * Time.deltaTime;
        transform.position = pos;

        bounds.center = transform.position; //1

        //Keep the ship constrained to the screen bounds
        Vector3 off = Utils.ScreenBoundsCheck(bounds, BoundsTest.onScreen); //2
        if(off != Vector3.zero) //3
        {
            pos -= off;
            transform.position = pos;
        }

        //Rotate the ship to make it feel more dynamic  //2
        transform.rotation = Quaternion.Euler(yAxis * pitchMult, xAxis * rollMult, 0);
    }

    //This variable holds a refernce to the last triggering GameObject
    public GameObject lastTriggerGo = null;   //1

    void OnTriggerEnter(Collider other)
    {
        //Find the tag of other.gameObject or its parent GameObjects
        GameObject go = Utils.FindTaggedParent(other.gameObject);
        //If there is a parent with a tag
        if(go != null)
        {
            //Make sure it's not the same triggering go as last time
            if(go == lastTriggerGo) //2
            {
                return;
            }
            lastTriggerGo = go; //3

            if(go.tag == "Enemy")
            {
                //If the shield was triggered by an enemy
                //Decrease the level of the shield by 1
                shieldLevel--;
                //Destroy the enemy
                Destroy(go);    //4
            }
            else
            {
                print("Triggered: " + go.name); //moved line
            }
        }
        else
        {
            //Otherwise announce the original other.gameObject
            print("Triggered: " + other.gameObject.name); //Test line moved
        }
    }
    public float shieldLevel
    {
        get
        {
            return (_shieldLevel);  //1
        }
        set
        {
            _shieldLevel = Mathf.Min(value, 4); //2
            //If the shield is going to be set to less than zero
            if(value < 0)   //3
            {
                Destroy(this.gameObject);
                //Tell Main.S to restart the game after a delay
                Main.S.DelayedRestart(gameRestartDelay);
            }
        }
    }
}
