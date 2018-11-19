using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerThreeController : MonoBehaviour {
    public AudioSource audiosource;

    public GameObject Robots2L;
    public Canvas FinalPanel;
    public Camera mainCamera;
    public GameObject fire003;
    public float moveSpeed = 5;
    public float RotateSpeed = 100;
    public float JumpSpeed = 10;
    public Transform HeadTransform;
    public bool isMoving = true;
    public GameObject FinalFire;
    public static PlayerThreeController _instance;

    private Animator playerAni;
    private Image image;
    private Image warning;
    private Image precure;
    private Image cure;
    private bool IsReady = true;
    private Slider slider_NPC;
    private float cureTime = 10f;
    private Transform npc_cure;
    private Transform informationImage;
    private Transform uiPickUp;
    private Transform robot1SelectTarget;

    void Awake() {
        _instance = this;
        playerAni = transform.GetComponent<Animator>();
        image = FinalPanel.transform.GetChild(0).GetComponent<Image>();
        warning = FinalPanel.transform.GetChild(1).GetComponent<Image>();
        precure = FinalPanel.transform.GetChild(3).GetComponent<Image>();
        cure = FinalPanel.transform.GetChild(2).GetComponent<Image>();
        informationImage = FinalPanel.transform.GetChild(4);
        uiPickUp = FinalPanel.transform.GetChild(5);
        FinalFire.SetActive(false);
    }

	// Use this for initialization
	void Start () {
        image.gameObject.SetActive(false);
        warning.gameObject.SetActive(false);
        slider_NPC = precure.transform.GetChild(1).GetComponent<Slider>();
        precure.gameObject.SetActive(false);
        cure.gameObject.SetActive(false);
        informationImage.gameObject.SetActive(false);
        uiPickUp.gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
        if (isMoving) {
            Move();
        }
        OnCheck();
        OnRaycast();
	}

    public void Move() {
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");

        transform.Translate(HeadTransform.forward * moveSpeed * Time.deltaTime * vertical);
        transform.Translate(HeadTransform.right * moveSpeed * Time.deltaTime * horizontal);

    }

    void ScrollRotate() {
        if (Input.GetMouseButton(0)) {
            transform.Rotate(transform.up * Input.GetAxis("Mouse X") * 10f);
        }
    }
    /// <summary>
    /// 射线检测
    /// </summary>
    void OnRaycast() {
        Ray ray = new Ray(HeadTransform.position, HeadTransform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)) {
            if (hit.transform.tag == "FinalButton") {
                //显示UI提示
                UpdateUIInformation("确定");
                if (Input.GetKeyDown(KeyCode.Y) || Input.GetKeyDown(KeyCode.JoystickButton1)) {
                    FinalPanel.GetComponent<FinalUIManger>().OnDownButtonDown();
                }
            } else if (hit.transform.tag == "Towel") {
                //显示UI
                UpdateUIInformation("拾取");
                if (Input.GetKeyDown(KeyCode.Y) || Input.GetKeyDown(KeyCode.JoystickButton1)) {
                    uiPickUp.gameObject.SetActive(true);
                    StartCoroutine(UIPanelDisAppearAftertime(1, uiPickUp.GetComponent<Image>()));
                    Destroy(hit.transform.gameObject);

                }
            } else if (hit.transform.tag == "elevator") {
                UpdateUIInformation("打开电梯");
                if (Input.GetKeyDown(KeyCode.Y) || Input.GetKeyDown(KeyCode.JoystickButton1)) {
                    Robot1._instance.selecedTarget = hit.transform.GetChild(0);
                    Robot1._instance.status = Status.isSearching;
                    Robot1._instance.OnUpdateUI(3, 2, 7);
                }
            } else {
                informationImage.gameObject.SetActive(false);
            }  
        }
    }

    /// <summary>
    /// 碰撞检测
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter(Collider other) {
        if (other.name == "2LRobotsTrigger") {
            //Robots2L.SetActive(true);
        } else if (other.tag == "fire") {
            mainCamera.GetComponent<CameraFilterPack_AAA_Blood>().Blood2 = 5f;
            if (IsReady) {
                IsReady = false;
                StartCoroutine(ShowBlood(1f));
                audiosource.Play();
            }
        } else if (other.tag == "door") {
            other.transform.Rotate(new Vector3(0, -90, 0));
            other.transform.GetComponent<BoxCollider>().enabled = false;
        } else if (other.name == "step_toliet") {
            Robot1._instance.selecedTarget = other.transform.GetChild(0);
            Robot1._instance.status = Status.isSearching;
            Robot1._instance.OnUpdateUI(3, 2, 2);
            other.GetComponent<BoxCollider>().enabled = false;
        } else if (other.name == "step_1Lwarning01_Trigger" || other.name == "step_1Lwarning02") {
            warning.gameObject.SetActive(true);
            StartCoroutine(UIPanelDisAppearAftertime(3, warning));
            Robot2._instance.status = Status.isFollowing;
            Robot3._instance.status = Status.isFollowing;
            fire003.transform.GetChild(1).gameObject.SetActive(true);   //全界面UI提示
            FinalFire.SetActive(true);
        } else if (other.tag == "FireTrigger") {
            robot1SelectTarget = other.transform;
            Robot1._instance.OnUpdateUI(1f, 2f, 9);
            StartCoroutine(Robot1UpdateAfterTime(3));
            Robot2._instance.selectFire = other.transform.GetChild(0);
            Robot2._instance.status = Status.isPutting;
            Robot3._instance.selectFire = other.transform.GetChild(0);
            Robot3._instance.status = Status.isPutting;
            other.GetComponent<BoxCollider>().enabled = false;
        } else if (other.tag == "elevator") {            //射线
            Robot1._instance.OnUpdateUI(0.1f, 1f, 7);
        } else if (other.tag == "NPC") {                //射线
            //UI提示是否救援
            //高亮
            //Slider进度条
            //isMoving = false;
            npc_cure = other.transform;
            precure.gameObject.SetActive(true);
            StartCoroutine(UIPanelDisAppearAftertime(5, precure));
            //precure.GetComponent<CureManger>().isCure = true;
            //other.transform.GetComponent<BoxCollider>().enabled = false;
            //Robot4._instance.OnUpdateUI(100, 100, 4);
        } else if (other.tag == "block_waitting") {
            Robot1._instance.OnUpdateUI(0.1f, 1f, 3);
            Robot1._instance.OnUpdateUI(3, 2, 4);
            Robot1._instance.selecedTarget = other.transform.GetChild(0);
            Robot1._instance.status = Status.isSearching;
            StartCoroutine(StatusChaneAtertime(1, Robot1._instance));
            other.GetComponent<BoxCollider>().enabled = false;
        } else if (other.name == "doorPlayer") {
            other.transform.Rotate(new Vector3(0, -90, 0));
            other.transform.GetComponent<BoxCollider>().enabled = false;
        } else {
            precure.gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// 碰撞检测2
    /// </summary>
    /// <param name="other"></param>
    void OnCollisionEnter(Collision other) {
        if (other.gameObject.name == "block") {
            mainCamera.GetComponent<CameraFilterPack_AAA_Blood>().Blood2 = 5f;
            StartCoroutine(ShowBlood(1f));
        } else if (other.transform.tag == "block") {
            Robot1._instance.OnUpdateUI(3, 2, 4);
            Robot2._instance.OnUpdateUI(3, 2, 4);
            Robot3._instance.OnUpdateUI(3, 2, 4);
        }
    }

    void UpdateUIInformation(string text) {
        informationImage.gameObject.SetActive(true);
        informationImage.GetChild(0).GetComponent<Text>().text = text;
    }

    IEnumerator ShowBlood(float time) {
        while (time > 0) {
            mainCamera.GetComponent<CameraFilterPack_AAA_Blood>().Blood2 -= 0.5f;
            time -= 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
        IsReady = true;
    }

    IEnumerator UIPanelDisAppearAftertime(float time, Image image) {
        yield return new WaitForSeconds(time);
        image.gameObject.SetActive(false);
    }

    IEnumerator StatusChaneAtertime(float time, Robot1 robot1) {
        yield return new WaitForSeconds(time);
        robot1.status = Status.isSearching;
    }

    IEnumerator Robot1UpdateAfterTime(float time) {
        yield return new WaitForSeconds(time);
        Robot1._instance.selectFire = robot1SelectTarget.GetChild(0);
        Robot1._instance.status = Status.isPutting;
    }

    public void OnCheck() {
        if (Input.GetKeyDown(KeyCode.X)) {
            Robot1._instance.OnUpdateUI(1, 3, 0);
        }
        if (Input.GetKeyDown(KeyCode.Y)) {
            //cure.gameObject.SetActive(true);
            //StartCoroutine(CureNPC());
            //cure.gameObject.SetActive(true);
            //cure.GetComponent<CureManger>().isCure = true;
            //StartCoroutine(UIPanelDisAppearAftertime(6, cure));
            //warning.gameObject.SetActive(true);
            //StartCoroutine(UIPanelDisAppearAftertime(3, warning));
            //Globe.nextSceneName = "DownScene";
            //SceneManager.LoadScene("LoadScene");
            Robot1._instance.OnUpdateUI(1f, 2f, 9);
        }
    }
}
