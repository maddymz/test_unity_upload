﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class s_LaunchPlayer : sb_PlayerConstruction
{
    GameObject model;
    GameObject arrow; GameObject target; GameObject missed;
    Slider steering; Text dist;
    Color sky; Color space; public Material skybox;
    float rotation = 0f; bool setSkybox = false;
    void Awake()
    {//Start is called before the first frame update
        model = this.transform.GetChild(0).gameObject; //camera = GameObject.Find("PlayerCamera").GetComponent<Camera>();
        ctl_PlayerAwake();
        ctl_UpdatePlayerPrefab();
        playerRb = gameObject.GetComponent<Rigidbody2D>();
        steering = GameObject.Find("slider_Steer").GetComponent<Slider>();
        arrow = GameObject.Find("Arrow"); target = GameObject.Find("Target");
        arrow.transform.position = new Vector3(-7f, 2f, 0f); arrow.transform.parent = this.transform;
        dist = GameObject.Find("distance").GetComponent<Text>(); missed = GameObject.Find("btn_Miss"); missed.SetActive(false);
        sky = new Color(.85f,.95f,.95f,1f); space = new Color(0f,0f,0f,1f);
        ctl_SetCameraPosition(new Vector3(0f, 25f, -40f));
    }

    void Update()
    {//Update is called once per frame

    }
    void FixedUpdate()
    {//Fixed Update is when physics are calculated so want to do physics stuff here...
        rotation = steering.value * -1; //Invert value so slider left is steer left.
        model.transform.rotation = Quaternion.Euler(new Vector3(0f,0f,rotation));
        playerRb.AddForce(model.transform.up * movementSpeed);
        Vector3 dir = target.transform.position - arrow.transform.position;
        Quaternion rot = Quaternion.AngleAxis(Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg, Vector3.up);
        arrow.transform.rotation = Quaternion.Euler(new Vector3(rot.eulerAngles.x, rot.eulerAngles.z, rot.eulerAngles.y-90f));
        dist.text = "Distance: " + (int)Vector3.Distance(model.transform.position, target.transform.position);
        //arrow.transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(arrow.transform.position, target.transform.position, 1f, 1f));
        if(this.transform.position.y > target.transform.position.y + 20f)
        {//You passed it, and rockets don't like to go down...
            Debug.Log("You Missed!");
            missed.SetActive(true);
        }

        if(setSkybox)
        {//Gradually fade in the skybox.
            RenderSettings.skybox.SetColor("_Tint", Color.Lerp(space, Color.white, model.transform.position.y/450f));
        }
        else if (model.transform.position.y > 240f)
        {//Keep from setting every update.
            RenderSettings.skybox = skybox;
            setSkybox = true;
            RenderSettings.skybox.SetColor("_Tint", Color.black);
        }
        else
        {//Use regular color skybox.
            camera.backgroundColor = Color.Lerp(sky, space, model.transform.position.y/240f);
        }
    }
    public void ctl_UpdatePlayerPrefab()
    {
        this.ctl_UsePlayerPrefs(new Vector3(0f, 10.7f, 0f));
        //GameObject rocketObj = Instantiate(prefabRocket, this.transform.position + new Vector3(0f, -3f, 0f), this.transform.rotation);
        //rocketObj.transform.parent = modelChild;
        //cFH testing
        GameObject rocketObj = Instantiate(cFH, this.transform.position + new Vector3(0f, 2.07f, 0f), Quaternion.Euler(0f, 90f, 0f));
        rocketObj.transform.parent = modelChild;
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("You hit the target +Score!");
        PlayerPrefs.SetInt("Score", PlayerPrefs.GetInt("Score")+100);
        SceneManager.LoadScene(4);
    }
    public void RestartLevel()
    {
        SceneManager.LoadScene(3);
    }
    /*
    void OnCollisionEnter2D(Collision2D col)
    {
        Debug.Log("You hit the something!");
        SceneManager.LoadScene(4);
    }*/
}
