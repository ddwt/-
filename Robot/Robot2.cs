using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Robot2 : AIRobot {

    public static Robot2 _instance;
    public Transform searchTarget01;
    public GameObject waterDes;

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
            //灭火功能
            robotNav.stoppingDistance = 1;
            robotNav.SetDestination(selectFire.position);
            if (robotNav.remainingDistance < 0.8f) {
                StartCoroutine(UpdataLookAt());
                PutOutFire();
            }
        } else if (status == Status.isSearching) {
            //探查
            robotNav.SetDestination(searchTarget01.position);
            if (robotNav.remainingDistance < 3) {
                OnUpdateUI(3, 1, 4);
            }
        } else {
            //初始状态
        }
    }

    IEnumerator UpdataLookAt() {
        yield return new WaitForSeconds(1);
        transform.LookAt(selecedTarget);
    }

}
