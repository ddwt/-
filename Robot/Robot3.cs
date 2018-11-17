using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot3 : AIRobot {

    public GameObject waterDes;
    public static Robot3 _instance;

    private Transform startUI;
    private bool uiflag = true;
    
    // Use this for initialization
    void Start () {
        _instance = this;
        status = Status.isWaitting;
        startUI = transform.GetChild(1).GetChild(0);
    }
	
	// Update is called once per frame
	void Update () {
        OnMoving();
	}

    public void OnMoving() {
        if (status == Status.isFollowing) {
            FollowPlayer();
            if (uiflag) {
                startUI.gameObject.SetActive(false);
                uiflag = false;
            }
            if (waterDes != null) {
                Destroy(waterDes);
            }
        } else if (status == Status.isPutting) {
            robotNav.stoppingDistance = 1;
            robotNav.SetDestination(selectFire.position);
            if (robotNav.remainingDistance < 0.8f) {
                StartCoroutine(UpdataLookAt());
                PutOutFire();
            }
        } else if (status == Status.isWaitting) {
            //等待
            //OnUpdateUI(2, 2, 2);
        } else if (status == Status.isEnding) {
            //OnUpdateUI(10, 10, 5);

        }
    }
    IEnumerator UpdataLookAt() {
        yield return new WaitForSeconds(1);
        transform.LookAt(selecedTarget);
    }
}
