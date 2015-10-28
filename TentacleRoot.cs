using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TentacleRoot : MonoBehaviour
{
    public bool removeCylinderAtRemoveBody = true;

    //    List<TentacleLeg> bodyList = new List<TentacleLeg>();
    List<TentacleBody> bodyList = new List<TentacleBody>();

    public int BodyCount
    {
        get
        {
            return bodyList.Count;
        }
    }

    public static Dictionary<string, TentacleProperties> propertiesDic = new Dictionary<string, TentacleProperties>();

    public class TentacleProperties
    {
        // 휜 정도의 회복 속도
        // recovery speed of bending degrees
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

        public bool setUpVector = true;

        public TentacleProperties()
        {

        }

        public TentacleProperties(TentacleProperties tp)
        {
            bendingDegrees = tp.bendingDegrees;
            bendingRecoverySpeed = tp.bendingRecoverySpeed;

            modifyRecoverySpeed = tp.modifyRecoverySpeed;
            rotationSpeedByModify = tp.rotationSpeedByModify;

            lengthenRatio = tp.lengthenRatio;
            minLength = tp.minLength;

            solid = tp.solid;

            useCylinder = tp.useCylinder;
            cylinder = tp.cylinder;

            thickness = tp.thickness;
            headSize = tp.headSize;
            tailSize = tp.tailSize;
        }
    }

    Hashtable whipTweenTheta = new Hashtable();
    Hashtable whipTweenPhi = new Hashtable();
    
    [SerializeField]
    private float theta;
    public float Theta
    {
        get
        {
            return theta;
        }
        set
        {
            //    float deltaTheta = theta;

            //    theta = value % 360.0f;
            //    if (theta < 0.0f) theta += 360.0f;

            //    deltaTheta -= theta;

            //    float varyingDegree = theta - value;

            //    if ((theta < value && value > 360.0f  && deltaTheta > 0.0f && deltaTheta < 180.0f) ||
            //       (theta > value && value < -0.0f  && deltaTheta > 180.0f && deltaTheta < 360.0f))
            //    {
            //        for (int i = 1; i < bodyList.Count; ++i)
            //        {
            //            bodyList[i].theta += varyingDegree;
            //        }
            //    }
            //}

            float _theta = value % 360.0f;
            if (_theta < 0.0f) _theta += 360.0f;
            float deltaTheta = theta - _theta;
            theta = _theta;

            if(deltaTheta > 180.0f && deltaTheta < 360.0f && value > 360.0f)
            {
                for(int i = 0; i < bodyList.Count; ++i)
                {
                    bodyList[i].theta -= 360.0f;
                }
            }
            else if(deltaTheta < -180.0f && deltaTheta > -360.0f && value < 0.0f)
            {
                for(int i = 0; i < bodyList.Count; ++i)
                {
                    bodyList[i].theta += 360.0f;
                }
            }
        }
    }

    [SerializeField]
    private float phi;
    public float Phi
    {
        get
        {
            return phi;
        }
        set
        {

            float _phi = value % 360.0f;
            if (_phi < 0.0f) _phi += 360.0f;
            float deltaPhi = phi - _phi;
            phi = _phi;

            if (deltaPhi > 180.0f && deltaPhi < 360.0f && value > 360.0f)
            {
                for (int i = 0; i < bodyList.Count; ++i)
                {
                    bodyList[i].phi -= 360.0f;
                }
            }
            else if (deltaPhi < -180.0f && deltaPhi > -360.0f && value < 0.0f)
            {
                for (int i = 0; i < bodyList.Count; ++i)
                {
                    bodyList[i].phi += 360.0f;
                }
            }

            //float deltaPhi = phi;

            //phi = value % 360.0f;
            //if (phi < 0.0f) phi += 360.0f;

            //deltaPhi -= phi;

            //float varyingDegree = phi - value;

            //if ((phi < value && value > 360.0f && deltaPhi > 0.0f && deltaPhi < 180.0f) ||
            //   (phi > value && value < -0.0f && deltaPhi > 180.0f && deltaPhi < 360.0f))
            //{
            //    for (int i = 1; i < bodyList.Count; ++i)
            //    {
            //        bodyList[i].phi += varyingDegree;
            //    }
            //}
        }
    }

    private void SetTheta(float value)
    {
        Theta = value;
    }

    private void SetPhi(float value)
    {
        Phi = value;
    }

    // 회전 속도
    public static float rotationSpeed = 200.0f;
    // 이동속도
    public static float moveSpeed = 3.0f;

    //float dd = Random.Range(1.0f, 5.0f);
    //float ff = Random.Range(1.0f, 5.0f);
    //void FixedUpdate()
    //{
    //    Theta += dd;
    //    Phi += ff;
    //}

    void Awake()
    {
        TentacleProperties defaultPropertiex = new TentacleProperties();

        propertiesDic["default"] = defaultPropertiex;

    }

    void Start()
    {
        whipTweenTheta.Add("from", 0.0f);
        whipTweenTheta.Add("to", 360.0f);
        whipTweenTheta.Add("time", 2.0f);
        whipTweenTheta.Add("easetype", iTween.EaseType.easeOutSine);
        whipTweenTheta.Add("onupdate", "SetTheta");

        whipTweenPhi.Add("from", 0.0f);
        whipTweenPhi.Add("to", 0.0f);
        whipTweenPhi.Add("time", 0.0f);
        whipTweenPhi.Add("easetype", iTween.EaseType.easeOutSine);
        whipTweenPhi.Add("onupdate", "SetPhi");
    }

    void OnDestroy()
    {
        bodyList.Clear();
    }

    void Update()
    {

        if (bodyList.Count > 0)
        {
            bodyList[0].transform.position = transform.position;
            Phi = bodyList[0].phi;
            Theta = bodyList[0].theta;
        }

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

        if (Input.GetKeyDown(KeyCode.Q))
        {
            whipTweenTheta["from"] = theta;
            whipTweenTheta["to"] = theta + 360.0f;

            iTween.ValueTo(gameObject, whipTweenTheta);
        }
    }

    public void SetTentacleProperties(params string[] propertiesName)
    {
        string nonvalidatedName = IsValidKey(propertiesName);
        if (nonvalidatedName != null)
        {
            Debug.LogWarning(nonvalidatedName + " is undeclared propertiesDic name");
            return;
        }

        int nameCount = propertiesName.Length;

        for (int i = 0; i < bodyList.Count; ++i)
        {
            string key = propertiesName[i % nameCount];
            bodyList[i].properties = new TentacleProperties(propertiesDic[key]);
        }

    }

    string IsValidKey(params string[] propertiesName)
    {
        for(int i = 0; i < propertiesName.Length; ++i)
        {
            if (propertiesDic.ContainsKey(propertiesName[i]) == false)
                return propertiesName[i];
        }

        return null;
    }

    //

    public void AddBodyObject(GameObject obj)
    {
        List<GameObject> list = new List<GameObject>(1);
        list.Add(obj);
        AddBodyObject(list);
    }

    public void AddBodyObject(List<GameObject> obj)
    {
        InsertBodyObject(bodyList.Count, obj);
    }

    public void InsertBodyObject(int idx, GameObject obj)
    {
        List<GameObject> list = new List<GameObject>(1);
        list.Add(obj);
        InsertBodyObject(idx, list);
    }

    public void InsertBodyObject(int idx, List<GameObject> obj)
    {
        if (idx < 0 || idx > bodyList.Count)
        {
            Debug.LogWarning("wrong index");
            return;
        }

        for(int i = 0; i < obj.Count; ++i)
        {
            if(obj[i] == null)
            {
                Debug.LogWarning("wrong GameObject");
                return;
            }
        }
        
        List<TentacleBody> insertBodyList = new List<TentacleBody>(obj.Count);

        TentacleBody prevTb = null;

        for (int i = 0; i < obj.Count; ++i)
        {
            TentacleBody tb = obj[i].AddComponent<TentacleBody>();
            
            insertBodyList.Add(tb);

            tb.root = this;
            tb.prev = prevTb;
            if (prevTb != null)
                prevTb.next = tb;
            if (i == obj.Count - 1)
                tb.next = null;

            prevTb = tb;

            if (i == 0)
            {
                if (idx == 0)
                    tb.transform.position = transform.position;
                else
                {
                    tb.transform.position = bodyList[idx - 1].transform.position;

                    bodyList[idx - 1].next = tb;
                }
            }
            else
                tb.transform.position = obj[i - 1].transform.position;

            if (i == obj.Count - 1)
            {
                if (bodyList.Count > 0)
                    bodyList[idx + obj.Count].prev = tb;
            }
        }

        bodyList.InsertRange(idx, insertBodyList);
    }

    //

    public bool RemoveBodyObject(TentacleBody tb)
    {
        return bodyList.Remove(tb);
    }

    public void RemoveBodyObject(int idx)
    {
        bodyList.RemoveAt(idx);
    }

    public void RemoveBodyObject(int idx, int count)
    {
        bodyList.RemoveRange(idx, count);
    }

    public void ClearBodyObject()
    {
        bodyList.Clear();
    }

    //public void FiniteWhipping(Vector3 pos, float beginTheta, float beginPhi, float endTheta, float endPhi, float whipTime, float lifeTime)
    //{
    //    string backupState = currentState;

    //    SetDefaultTentacleState();
    //    currentState = "default";

    //    Vector3 backupPos = transform.position;
    //    //        transform.position = pos;

    //    bodyList[0].theta = beginTheta;
    //    bodyList[0].phi = beginPhi;

    //    whipTweenTheta["from"] = beginTheta;
    //    whipTweenTheta["to"] = endTheta;
    //    whipTweenTheta["time"] = whipTime;

    //    whipTweenPhi["from"] = beginPhi;
    //    whipTweenPhi["to"] = endPhi;
    //    whipTweenPhi["time"] = whipTime;

    //    iTween.ValueTo(gameObject, whipTweenTheta);
    //    iTween.ValueTo(gameObject, whipTweenPhi);

    //    StopAllCoroutines();
    //    StartCoroutine(FiniteWhipTentacle(backupPos, backupState, whipTime, lifeTime));
    //}

    //IEnumerator FiniteWhipTentacle(Vector3 pos, string state, float limitTime, float lifeTime)
    //{

    //    yield return new WaitForSeconds(0.1f);

    //    currentState = state;

    //    float time = 0.0f;

    //    while (time < limitTime)
    //    {
    //        time += Time.deltaTime;

    //        bodyList[0].theta = theta;
    //        bodyList[0].phi = phi;

    //        yield return null;
    //    }

    //    //        transform.position = pos;

    //    if (lifeTime < 0.0f) yield break;

    //    yield return new WaitForSeconds(lifeTime);

    //    DestroyTentacle();
    //}

    //public void Whipping(Vector3 pos, float beginTheta, float beginPhi, float endTheta, float endPhi, float whipTime)
    //{
    //    string backupState = currentState;

    //    SetDefaultTentacleState();
    //    currentState = "default";

    //    Vector3 backupPos = transform.position;
    //    //        transform.position = pos;

    //    bodyList[0].theta = beginTheta;
    //    bodyList[0].phi = beginPhi;

    //    whipTweenTheta["from"] = beginTheta;
    //    whipTweenTheta["to"] = endTheta;
    //    whipTweenTheta["time"] = whipTime;

    //    whipTweenPhi["from"] = beginPhi;
    //    whipTweenPhi["to"] = endPhi;
    //    whipTweenPhi["time"] = whipTime;

    //    iTween.ValueTo(gameObject, whipTweenTheta);
    //    iTween.ValueTo(gameObject, whipTweenPhi);

    //    StopAllCoroutines();
    //    StartCoroutine(WhipTentacle(backupPos, backupState, whipTime));
    //}

    //IEnumerator WhipTentacle(Vector3 pos, string state, float limitTime)
    //{

    //    yield return new WaitForSeconds(0.01f);

    //    currentState = state;

    //    float time = 0.0f;

    //    while (time < limitTime)
    //    {
    //        time += Time.deltaTime;

    //        bodyList[0].theta = theta;
    //        bodyList[0].phi = phi;

    //        yield return null;
    //    }


    //}


}
