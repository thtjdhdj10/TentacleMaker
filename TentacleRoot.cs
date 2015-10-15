using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TentacleRoot : MonoBehaviour
{
    public bool removeCylinderAtRemoveBody = true;

    //    List<TentacleLeg> bodyList = new List<TentacleLeg>();
    List<TentacleBody> bodyList = new List<TentacleBody>();

    [SerializeField]
    private int tentacleCount;
    public int TentacleCount
    {
        get
        {
            return tentacleCount;
        }
        set
        {

        }
    }

    public static Dictionary<string, TentacleProperties> propertiesDictionary = new Dictionary<string, TentacleProperties>();

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
            theta = value % 360.0f;
            if (theta < 0.0f) theta += 360.0f;

            if ((theta < value && value > 360.0f) ||
               (theta > value && value < 0.0f))
            {
                float varyingDegree = theta - value;

                for(int i = 0; i < bodyList.Count; ++i)
                {
                    bodyList[i].theta += varyingDegree;
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
            phi = value % 360.0f;
            if (phi < 0.0f) phi += 360.0f;

            if ((phi < value && value > 360.0f) ||
               (phi > value && value < 0.0f))
            {
                float varyingDegree = phi - value;

                for (int i = 0; i < bodyList.Count; ++i)
                {
                    bodyList[i].phi += varyingDegree;
                }
            }
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

    /*
   // void FixedUpdate()
  //  {
        //for (int i = 0; i < bodyList.Count; ++i)
        //{

        //    if (bodyList[i].properties.solid == true)
        //        return;

        //    float _theta, _phi;

        //    if (bodyList[i].prev == null)
        //    {
        //        _theta = bodyList[i].root.Theta;
        //        _phi = bodyList[i].root.Phi;
        //    }
        //    else
        //    {
        //        _theta = bodyList[i].prev.theta;
        //        _phi = bodyList[i].prev.phi;
        //    }

        //    bodyList[i].deltaTheta = Mathf.Abs(_theta - bodyList[i].theta);
        //    bodyList[i].deltaPhi = Mathf.Abs(_phi - bodyList[i].phi);

        //    // 각도차가 비탄성정도보다 클 경우에만 각도변경
        //    if (bodyList[i].deltaTheta > bodyList[i].properties.bendingDegrees)
        //    {
        //        bodyList[i].theta = bodyList[i].theta * (1 - bodyList[i].properties.bendingRecoverySpeed) + _theta * bodyList[i].properties.bendingRecoverySpeed;
        //    }
        //    if (bodyList[i].deltaPhi > bodyList[i].properties.bendingDegrees)
        //    {
        //        bodyList[i].phi = bodyList[i].phi * (1 - bodyList[i].properties.bendingRecoverySpeed) + _phi * bodyList[i].properties.bendingRecoverySpeed;
        //    }

        //    //

        //    float phiAngleConformity = 1.0f - Mathf.Abs(((bodyList[i].theta + 360.0f) * bodyList[i].devide90) % 2 - 1.0f);

        //    bodyList[i].ro = (bodyList[i].deltaPhi * phiAngleConformity + bodyList[i].deltaTheta) * bodyList[i].devide90 * bodyList[i].properties.lengthenRatio * bodyList[i].properties.minLength + bodyList[i].properties.minLength;

        //    bodyList[i].followPosition = bodyList[i].followPosition * (1 - bodyList[i].properties.modifyRecoverySpeed) + bodyList[i].transform.position * bodyList[i].properties.modifyRecoverySpeed;

        //    //

        //    float theta2Rad = bodyList[i].theta * Mathf.Deg2Rad;
        //    float phi2Rad = bodyList[i].phi * Mathf.Deg2Rad;

        //    Vector3 addPosition = new Vector3();
        //    addPosition.x += bodyList[i].ro * Mathf.Sin(theta2Rad) * Mathf.Cos(phi2Rad);
        //    addPosition.y += bodyList[i].ro * Mathf.Cos(theta2Rad);
        //    addPosition.z += bodyList[i].ro * Mathf.Sin(theta2Rad) * Mathf.Sin(phi2Rad);

        //    Vector3 directionToNext = (bodyList[i].followPosition + addPosition) - bodyList[i].transform.position;

        //    //

        //    if (Vector3.Distance(bodyList[i].transform.position, bodyList[i].beforePosition) > TentacleBody.minDistanceForReverseOperation)
        //    {
        //        bodyList[i].ReverseOperation();
        //        bodyList[i].beforePosition = bodyList[i].transform.position;
        //    }

        //    //

        //    if (bodyList[i].next != null)
        //    {
        //        bodyList[i].next.transform.position = bodyList[i].followPosition + addPosition;

        //        bodyList[i].transform.up = directionToNext.normalized;

        //        if (bodyList[i].cylinder != null)
        //        {
        //            bodyList[i].cylinder.transform.up = directionToNext.normalized;

        //            float distanceToNextHalf = Vector3.Distance(bodyList[i].transform.position, bodyList[i].next.transform.position) * 0.5f;
        //            bodyList[i].cylinder.transform.localScale = new Vector3(
        //                bodyList[i].properties.thickness,
        //                distanceToNextHalf,
        //                bodyList[i].properties.thickness);

        //            bodyList[i].cylinder.transform.position = bodyList[i].transform.position + bodyList[i].cylinder.transform.up * distanceToNextHalf;
        //        }
        //    }
        //}
  //  }
  */

    void Awake()
    {
        TentacleProperties drop = new TentacleProperties();

        propertiesDictionary["drop"] = drop;

        TentacleProperties swing = new TentacleProperties();

        swing.rotationSpeedByModify = 0.2f;
        swing.lengthenRatio = 5.0f;
        swing.useCylinder = false;

        propertiesDictionary["swing"] = swing;

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

        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    float _x = Random.Range(-1.0f, 1.0f);
        //    float _y = Random.Range(-1.0f, 1.0f);
        //    float _z = Random.Range(-1.0f, 1.0f);

        //    float _theta = Random.Range(0.0f, 360.0f);
        //    float _phi = Random.Range(0.0f, 360.0f);

        //    propertiesDictionary[currentState] = new TentacleState();
        //    // state 를 조작

        //    Whipping(new Vector3(_x, _y, _z), _theta, _phi, _theta + 270.0f, _phi + 90.0f, 0.5f,-1.0f);
        //}
    }

    public void SetTentacleProperties(params string[] propertiesName)
    {
        string nonvalidatedName = IsValidKey(propertiesName);
        if (nonvalidatedName != null)
        {
            Debug.LogWarning(nonvalidatedName + " is undeclared propertiesDictionary name");
            return;
        }

        int nameCount = propertiesName.Length;

        for (int i = 0; i < bodyList.Count; ++i)
        {
            string key = propertiesName[i % nameCount];
            bodyList[i].properties = new TentacleProperties(propertiesDictionary[key]);
        }

    }

    string IsValidKey(params string[] propertiesName)
    {
        for(int i = 0; i < propertiesName.Length; ++i)
        {
            if (propertiesDictionary.ContainsKey(propertiesName[i]) == false)
                return propertiesName[i];
        }

        return null;
    }

    void SetDefaultTentacleState()
    {
        TentacleProperties state = new TentacleProperties();
        state.bendingRecoverySpeed = 1.0f;
        state.bendingDegrees = 0.0f;
        state.modifyRecoverySpeed = 1.0f; // 0 ~ 1
        state.rotationSpeedByModify = 0.0f; // 0 ~ 1
        state.lengthenRatio = 0.0f;

        propertiesDictionary["default"] = state;
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

        for (int i = 0; i < obj.Count; ++i)
        {
            TentacleBody tb = obj[i].AddComponent<TentacleBody>();
            TentacleBody prevTb = null;
            
            insertBodyList.Add(tb);

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

            if(i == obj.Count - 1)
            {
                if(bodyList.Count > 0)
                    bodyList[idx + obj.Count].prev = tb;
            }

            tb.root = this;
            tb.prev = prevTb;
            if (prevTb != null)
                prevTb.next = tb;
            if (i == obj.Count - 1)
                tb.next = null;

            prevTb = tb;
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
