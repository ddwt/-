using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Robot : MonoBehaviour {

    private GameObject waterPrefab;
    private Transform waterTarget;
    private NavMeshAgent robotNav;
    private Transform[] wayPoints;
    private int index = 0;
    private bool isWorking = false;
    private GameObject water;
    private Transform wayPoints_2floor;
    private Vector3 prePosition;

    void Awake() {
        wayPoints_2floor = GameObject.FindGameObjectWithTag("WayPoints").transform.FindChild("WayPoints_2floor");
        robotNav = transform.GetComponent<NavMeshAgent>();
        wayPoints = new Transform[wayPoints_2floor.childCount];
        waterPrefab = Resources.Load("Water/Prefabs/Water") as GameObject;
        waterTarget = transform.GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0);
        prePosition = transform.position;
        
    }

    // Use this for initialization
    void Start() {
        for (int i = 0; i < wayPoints.Length; i++) {
            wayPoints[i] = wayPoints_2floor.GetChild(i);
        }
        //Debug.Log(wayPoints.Length);
    }

    // Update is called once per frame
    void Update() {
        Move();
    }
    /// <summary>
    /// 按安排的顺序移动
    /// </summary>
    public void Move() {
        if (index < wayPoints.Length) {
            robotNav.SetDestination(wayPoints[index].position);
        } else {
            robotNav.SetDestination(prePosition);
            if (robotNav.remainingDistance < 3) {
                robotNav.Stop();
            }

        }
        //MoveToWards(wayPoints[index].localPosition);
        if (robotNav.remainingDistance < 3) {
            transform.LookAt(wayPoints[index]);
            PutOutFire();
        }
        
    }

    public void PutOutFire() {
        //1 获得该火 通过射线检测
        //2 实例化水 灭火
        //3 灭火完毕 寻找下一个
        //robotNav.Stop();
        Ray ray = new Ray(waterTarget.position, waterTarget.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 10.0f)) {
            if (hit.collider.tag == "fire") {
                //transform.LookAt(hit.collider.transform);
                if (water == null) {
                    water = GameObject.Instantiate(waterPrefab, waterTarget.position, waterTarget.rotation) as GameObject;
                    water.transform.parent = waterTarget;
                }
                //hit.collider.transform.localScale = new Vector3((x -= 0.001f),(y -= 0.001f), (z -= 0.001f));
                hit.collider.transform.GetChild(0).GetComponent<ParticleSystem>().startSize -= 0.2f * Time.deltaTime;
                if (hit.collider.transform.GetChild(0).GetComponent<ParticleSystem>().startSize < 0.1) {
                    hit.collider.gameObject.SetActive(false);
                    isWorking = false;
                    Destroy(water, 1);
                    water = null;
                    robotNav.Resume();
                    index++;
                    Debug.Log(index);
                    if (index < wayPoints.Length) {
                        robotNav.SetDestination(wayPoints[index].position);
                    } else {
                        robotNav.SetDestination(prePosition);
                    }
                }
            }
        }
    }
    void OnTriggerEnter(Collider other) {
        if (other.tag == "door") {
            other.gameObject.transform.Rotate(transform.up * 90);
            other.gameObject.transform.GetComponent<BoxCollider>().enabled = false;
        }
    }
}
