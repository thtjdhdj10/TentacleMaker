using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TentacleRoot : MonoBehaviour
{
    // upVector, cylinder object, connect, body object, 

    //    List<TentacleLeg> bodyList = new List<TentacleLeg>();
    public List<TentacleBody> bodyList = new List<TentacleBody>();

    public static Dictionary<string, TentacleState> states = new Dictionary<string, TentacleState>();

    public string currentState = "drop";

    public class TentacleState
    {
        // 휜 정도의 회복 속도
        // recovery speed of bending
        public float bendingRecoverySpeed = 0.2f; // 0 ~ 1
        // 최대로 휘는 정도
        // max bending degrees of the two objects
        public float bendingDegrees = 7.5f;

        // 변형 회복 속도
        // form recovery speed by movement
        public float modifyRecoverySpeed = 0.2f; // 0 ~ 1
        // 변형에 의한 회전속도
        // rotation speed by modify
        public float rotationSpeedByModify = 0.5f; // 0 ~ 1

        // 최대 길이는 deltaTheta, deltaPhi 에 따라
        // baseLength + baseLength * lengthenDegrees 가 된다.
        // 길어지는 비율
        public float lengthenRatio = 2.0f;
        // object 사이의 최소 간격( lengthenRatio = 0 일때 최소 )
        public float minLength = 0.5f;

        public bool solid = false;

        // 일단 임시로 모든 실린더 on off 여기서 관리.
        // TODO body 에서 관리하게 수정
        public bool useCylinder = true;

        public string cylinder = "Cylinder";

        // TODO 얇아지는 비율

        // 굵기
        public float thickness = 1.0f;

        public float headSize = 1.0f;
        public float tailSize = 1.0f;
    }

    [SerializeField]
    private int count;

    Hashtable whipTweenTheta = new Hashtable();
    Hashtable whipTweenPhi = new Hashtable();
    float theta;
    float phi;

    void Awake()
    {
        TentacleState drop = new TentacleState();

        states["drop"] = drop;

        TentacleState swing = new TentacleState();

        swing.rotationSpeedByModify = 0.2f;
        swing.lengthenRatio = 5.0f;
        swing.useCylinder = false;

        states["swing"] = swing;

        whipTweenTheta.Add("from", 0.0f);
        whipTweenTheta.Add("to", 0.0f);
        whipTweenTheta.Add("time", 0.0f);
        whipTweenTheta.Add("easetype", iTween.EaseType.easeOutSine);
        whipTweenTheta.Add("onupdate", "SetTheta");

        whipTweenPhi.Add("from", 0.0f);
        whipTweenPhi.Add("to", 0.0f);
        whipTweenPhi.Add("time", 0.0f);
        whipTweenPhi.Add("easetype", iTween.EaseType.easeOutSine);
        whipTweenPhi.Add("onupdate", "SetPhi");

    }

    private void SetTheta(float value)
    {
        theta = value;
    }

    private void SetPhi(float value)
    {
        phi = value;
    }

    // 회전 속도
    public static float rotationSpeed = 200.0f;
    // 이동속도
    public static float moveSpeed = 3.0f;

    Vector3 beforePosition = new Vector3();

    void Update()
    {
        //if (Input.GetKey(KeyCode.A))
        //    bodyList[0].Theta += Time.deltaTime * rotationSpeed;
        //if (Input.GetKey(KeyCode.S))
        //    bodyList[0].Phi += Time.deltaTime * rotationSpeed;
        //if (Input.GetKey(KeyCode.D))
        //    bodyList[0].Theta += Time.deltaTime * -rotationSpeed;
        //if (Input.GetKey(KeyCode.F))
        //    bodyList[0].Phi += Time.deltaTime * -rotationSpeed;

        //if (Input.GetKey(KeyCode.LeftArrow))
        //    transform.position = transform.position + new Vector3(Time.deltaTime * moveSpeed, 0.0f, Time.deltaTime * -moveSpeed);
        //if (Input.GetKey(KeyCode.RightArrow))
        //    transform.position = transform.position + new Vector3(Time.deltaTime * -moveSpeed, 0.0f, Time.deltaTime * moveSpeed);
        //if (Input.GetKey(KeyCode.UpArrow))
        //    transform.position = transform.position + new Vector3(Time.deltaTime * -moveSpeed, 0.0f, Time.deltaTime * -moveSpeed);
        //if (Input.GetKey(KeyCode.DownArrow))
        //    transform.position = transform.position + new Vector3(Time.deltaTime * moveSpeed, 0.0f, Time.deltaTime * moveSpeed);
        //if (Input.GetKey(KeyCode.LeftShift))
        //    transform.position = transform.position + new Vector3(0.0f, Time.deltaTime * -moveSpeed, 0.0f);
        //if (Input.GetKey(KeyCode.Space))
        //    transform.position = transform.position + new Vector3(0.0f, Time.deltaTime * moveSpeed, 0.0f);


        bodyList[0].transform.position = transform.position;

        if (Vector3.Distance(transform.position, beforePosition) > 0.01f)
        {
            bodyList[0].ReverseOperation();
        }
        beforePosition = transform.position;

        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    float _x = Random.Range(-1.0f, 1.0f);
        //    float _y = Random.Range(-1.0f, 1.0f);
        //    float _z = Random.Range(-1.0f, 1.0f);

        //    float _theta = Random.Range(0.0f, 360.0f);
        //    float _phi = Random.Range(0.0f, 360.0f);

        //    states[currentState] = new TentacleState();
        //    // state 를 조작

        //    Whipping(new Vector3(_x, _y, _z), _theta, _phi, _theta + 270.0f, _phi + 90.0f, 0.5f,-1.0f);
        //}
    }

    //public List<GameObject> SetBodyObjects(GameObject obj)
    //{
    //    List<GameObject> ret = new List<GameObject>(count);

    //    for(int i = 0; i < count; ++i)
    //    {
    //        ret.Add(Instantiate(obj) as GameObject);

    //        ret[i].
    //    }
    //}

    public void SetCylinder(string name)
    {
        for (int i = 0; i < bodyList.Count - 1; ++i)
        {
            bodyList[i].cylinder = VEasyPoolerManager.GetObjectRequest(name);
        }
    }

    void SetDefaultTentacleState()
    {
        TentacleState state = new TentacleState();
        state.bendingRecoverySpeed = 1.0f;
        state.bendingDegrees = 0.0f;
        state.modifyRecoverySpeed = 1.0f; // 0 ~ 1
        state.rotationSpeedByModify = 0.0f; // 0 ~ 1
        state.lengthenRatio = 0.0f;

        states["default"] = state;
    }

    public void FiniteWhipping(Vector3 pos, float beginTheta, float beginPhi, float endTheta, float endPhi, float whipTime, float lifeTime)
    {
        string backupState = currentState;

        SetDefaultTentacleState();
        currentState = "default";

        Vector3 backupPos = transform.position;
        //        transform.position = pos;

        bodyList[0].Theta = beginTheta;
        bodyList[0].Phi = beginPhi;

        whipTweenTheta["from"] = beginTheta;
        whipTweenTheta["to"] = endTheta;
        whipTweenTheta["time"] = whipTime;

        whipTweenPhi["from"] = beginPhi;
        whipTweenPhi["to"] = endPhi;
        whipTweenPhi["time"] = whipTime;

        iTween.ValueTo(gameObject, whipTweenTheta);
        iTween.ValueTo(gameObject, whipTweenPhi);

        StopAllCoroutines();
        StartCoroutine(FiniteWhipTentacle(backupPos, backupState, whipTime, lifeTime));
    }

    IEnumerator FiniteWhipTentacle(Vector3 pos, string state, float limitTime, float lifeTime)
    {

        yield return new WaitForSeconds(0.1f);

        currentState = state;

        float time = 0.0f;

        while (time < limitTime)
        {
            time += Time.deltaTime;

            bodyList[0].Theta = theta;
            bodyList[0].Phi = phi;

            yield return null;
        }

        //        transform.position = pos;

        if (lifeTime < 0.0f) yield break;

        yield return new WaitForSeconds(lifeTime);

        DestroyTentacle();
    }

    public void Whipping(Vector3 pos, float beginTheta, float beginPhi, float endTheta, float endPhi, float whipTime)
    {
        string backupState = currentState;

        SetDefaultTentacleState();
        currentState = "default";

        Vector3 backupPos = transform.position;
        //        transform.position = pos;

        bodyList[0].Theta = beginTheta;
        bodyList[0].Phi = beginPhi;

        whipTweenTheta["from"] = beginTheta;
        whipTweenTheta["to"] = endTheta;
        whipTweenTheta["time"] = whipTime;

        whipTweenPhi["from"] = beginPhi;
        whipTweenPhi["to"] = endPhi;
        whipTweenPhi["time"] = whipTime;

        iTween.ValueTo(gameObject, whipTweenTheta);
        iTween.ValueTo(gameObject, whipTweenPhi);

        StopAllCoroutines();
        StartCoroutine(WhipTentacle(backupPos, backupState, whipTime));
    }

    IEnumerator WhipTentacle(Vector3 pos, string state, float limitTime)
    {

        yield return new WaitForSeconds(0.01f);

        currentState = state;

        float time = 0.0f;

        while (time < limitTime)
        {
            time += Time.deltaTime;

            bodyList[0].Theta = theta;
            bodyList[0].Phi = phi;

            yield return null;
        }


    }

    public void SetBodyObjects(List<GameObject> obj)
    {
        // TODO 이미 set 된 경우 기존의 obj 를 바꾸는 식으로.

        bodyList.Clear();

        for (int i = 0; i < obj.Count; ++i)
        {
            float size = states[currentState].headSize + (states[currentState].tailSize - states[currentState].headSize) * i / obj.Count;
            obj[i].transform.localScale = new Vector3(size, size, size);

            bodyList.Add(obj[i].AddComponent<TentacleBody>());

            // TODO 각각의 초기위치를 설정할 방법 생각할 것
            obj[i].transform.position = transform.position;

            bodyList[i].root = this;

            if (i == 0)
            {
                bodyList[i].prev = null;
            }
            else if (i == obj.Count - 1)
            {
                bodyList[i].next = null;
                bodyList[i].prev = bodyList[i - 1];
                bodyList[i - 1].next = bodyList[i];
            }
            else
            {
                bodyList[i].prev = bodyList[i - 1];
                bodyList[i - 1].next = bodyList[i];
            }
        }
    }

    public void AddBodyObject(List<GameObject> obj)
    {
        if (bodyList.Count == 0)
        {
            SetBodyObjects(obj);
            return;
        }

        for (int i = 0; i < obj.Count; ++i)
        {
            bodyList.Add(obj[i].AddComponent<TentacleBody>());

            int idx = bodyList.Count - 1;

            obj[i].transform.position = bodyList[idx - 1].transform.position;

            bodyList[idx].root = this;

            if (i == obj.Count - 1)
            {
                bodyList[idx].next = null;
                bodyList[idx].prev = bodyList[idx - 1];
                bodyList[idx - 1].next = bodyList[idx];
            }
            else
            {
                bodyList[idx].prev = bodyList[idx - 1];
                bodyList[idx - 1].next = bodyList[idx];
            }
        }

        for (int i = 0; i < bodyList.Count; ++i)
        {
            float size = states[currentState].headSize + (states[currentState].tailSize - states[currentState].headSize) * i / obj.Count;
            bodyList[i].transform.localScale = new Vector3(size, size, size);
        }
    }

    public void RemoveBodyObject(int count)
    {
        while (count > 0)
        {
            count--;

            bodyList[bodyList.Count - 1].DestroyTentacle();
            bodyList.RemoveAt(bodyList.Count - 1);
        }

        bodyList[bodyList.Count - 1].next = null;
    }

    public void SetBodyObjects(GameObject[] obj)
    {
        for (int i = 0; i < obj.Length; ++i)
        {
            //            obj[i].AddComponent<TentacleBody>();
        }
    }

    public void DestroyTentacle()
    {
        while (bodyList.Count > 0)
        {
            bodyList[bodyList.Count - 1].DestroyTentacle();

            bodyList.RemoveAt(bodyList.Count - 1);
        }

        Destroy(this);
    }

}
