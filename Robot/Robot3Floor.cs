using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Robot3Floor : MonoBehaviour {

    //public int numNextFloor = 2;
    public static Robot3Floor _instance;
    public int index = 0;
    public Transform player;

    private GameObject waterPrefab;
    private Transform waterTarget;
    private NavMeshAgent robotNav;
    private Transform[] wayPoints;
    private bool isWorking = false;
    private GameObject water;
    private Transform wayPoints_3floor;
    private Transform wayPoints_2floor;
    private Vector3 prePosition;
   
    

    void Awake() {
        _instance = this;
        robotNav = transform.GetComponent<NavMeshAgent>();
        //wayPoints = new Transform[GameObject.FindGameObjectWithTag("WayPoints").transform.FindChild("WayPoints_3floor").childCount + numNextFloor];
        wayPoints = new Transform[GameObject.FindGameObjectWithTag("WayPoints").transform.FindChild("WayPoints_3floor").childCount];
        waterPrefab = Resources.Load("Water/Prefabs/Water") as GameObject;
        waterTarget = transform.GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0);
        wayPoints_3floor = GameObject.FindGameObjectWithTag("WayPoints").transform.FindChild("WayPoints_3floor");
        prePosition = transform.position;
    }

    // Use this for initialization
    void Start() {
        for (int i = 0; i < wayPoints.Length; i++) {
            wayPoints[i] = wayPoints_3floor.GetChild(i);
        }
    }

    void Update() {
        Move();
    }
    /// <summary>
    /// 按安排的顺序移动
    /// </summary>
    public void Move() {
        robotNav.SetDestination(wayPoints[index].position);
        if (robotNav.remainingDistance < 3) {
            transform.LookAt(wayPoints[index]);
            if (wayPoints[index].childCount == 2) {
                wayPoints[index].FindChild("block").gameObject.SetActive(true);
            }
            PutOutFire();
        } else {
            //transform.LookAt(wayPoints[index]);
        }

    }

    public void PutOutFire() {
        //1 获得该火 通过射线检测
        //2 实例化水 灭火
        //3 灭火完毕 寻找下一个
        //robotNav.Stop();
        Ray ray = new Ray(waterTarget.position, waterTarget.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 3.0f)) {
            if (hit.collider.tag == "fire") {
                //transform.LookAt(hit.collider.transform);
                if (water == null) {
                    water = GameObject.Instantiate(waterPrefab, waterTarget.position, waterTarget.rotation) as GameObject;
                    water.transform.parent = waterTarget;
                }
                //hit.collider.transform.localScale = new Vector3((x -= 0.001f),(y -= 0.001f), (z -= 0.001f));
                hit.collider.transform.GetChild(0).GetComponent<ParticleSystem>().startSize -= 0.2f * Time.deltaTime;
                if (hit.collider.transform.childCount > 2) {
                    hit.collider.transform.GetChild(1).GetComponent<ParticleSystem>().startSize -= 0.2f * Time.deltaTime;
                }
                if (hit.collider.transform.GetChild(0).GetComponent<ParticleSystem>().startSize < 0.1) {
                    if (wayPoints[index].childCount == 2) {
                        wayPoints[index].FindChild("block").gameObject.SetActive(false);
                    }
                    hit.collider.gameObject.SetActive(false);
                    isWorking = false;
                    Destroy(water, 1);
                    water = null;
                    robotNav.Resume();
                    index++;
                    if (index < wayPoints.Length) {
                        robotNav.SetDestination(wayPoints[index].position);
                    }
                    else {
                        robotNav.SetDestination(prePosition);
                    }
                }
            } else if (hit.collider.tag == "wayPre") {
                index++;
                robotNav.Resume();
                robotNav.SetDestination(wayPoints[index].position);
                //Destroy(hit.collider.gameObject);
                hit.collider.gameObject.GetComponent<BoxCollider>().enabled = false;
                //Debug.Log(wayPoints[index].name);
                //Debug.Log(index);
            } else if (hit.collider.tag == "startUI"){
                robotNav.Stop();
                StartCoroutine(UIAppearAfterTime(1, 0));
                StartCoroutine(UIAppearAfterTime(5, 1));
                hit.transform.GetComponent<BoxCollider>().enabled = false;
            }
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.tag == "door") {
            //Debug.Log("碰撞到门");
            other.gameObject.transform.Rotate(new Vector3(0, 90, 0));
            other.gameObject.transform.GetComponent<BoxCollider>().enabled = false;
        } if (other.tag == "blockCube") {
            //Debug.Log("检测到不可灭火源");
            index++;
            other.gameObject.transform.GetComponent<BoxCollider>().enabled = false;
        }
    }

    IEnumerator UIAppearAfterTime(float time, int uiIndex) {
        yield return new WaitForSeconds(time);
        transform.GetChild(2).GetChild(uiIndex).gameObject.SetActive(true);
        //transform.GetChild(2).GetChild(uiIndex).GetComponent<InterfaceAnimManager>().startAppear();
        //UI关闭
        if (uiIndex != 1) {
            StartCoroutine(UIDisAppearAtertime(3f, uiIndex));
        }
    } 

    IEnumerator UIDisAppearAtertime(float time, int uiIndex) {
        yield return new WaitForSeconds(time);
        transform.GetChild(2).GetChild(uiIndex).GetComponent<InterfaceAnimManager>().startDisappear();
    }

    public void OnOkButtonDown() {
        StartCoroutine(UIDisAppearAtertime(0.3f, 1));
        index++;
        robotNav.Resume();
    }
}
