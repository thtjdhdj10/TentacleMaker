using UnityEngine;
using System.Collections;
using System.Diagnostics;

//using Debug = UnityEngine.Debug;

public class TentacleBody : MonoBehaviour
{


    public float ro;

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
            float _theta = theta;
            theta = value % 360.0f;
            float deltaTheta = _theta - theta;

            if (next != null)
            {
                if (next.Theta > theta + 180.0f)
                {
                    next.Theta -= deltaTheta;
                }
                else if (next.Theta <= theta - 180.0f)
                {
                    next.Theta -= deltaTheta;
                }
            }

            if (prev != null)
            {
                if (prev.Theta > theta + 180.0f)
                {
                    prev.Theta -= deltaTheta;
                }
                else if (prev.Theta <= theta - 180.0f)
                {
                    prev.Theta -= deltaTheta;
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
            float _phi = phi;
            phi = value % 360.0f;
            float deltaPhi = _phi - phi;

            if (next != null)
            {
                if (next.Phi > phi + 180.0f)
                {
                    next.Phi -= deltaPhi;
                }
                else if (next.Phi <= phi - 180.0f)
                {
                    next.Phi -= deltaPhi;
                }
            }

            if (prev != null)
            {
                if (prev.Phi > phi + 180.0f)
                {
                    prev.Phi -= deltaPhi;
                }
                else if (prev.Phi <= phi - 180.0f)
                {
                    prev.Phi -= deltaPhi;
                }
            }
        }
    }

    Vector3 followPosition = new Vector3();

    float devide90 = 1.0f / 90.0f;

    float deltaTheta = 0.0f;
    float deltaPhi = 0.0f;

    public TentacleRoot root;
    public TentacleBody next;
    public TentacleBody prev;
    public GameObject cylinder;

    void Start()
    {
        followPosition = transform.position;

        ro = TentacleRoot.states[root.currentState].minLength;
        theta = 0.0f;
        phi = 90.0f;


    }

    void Update()
    {
        //if(TentacleRoot.states[root.currentState].useCylinder == true)
        //{
        //    if(cylinder == null)
        //    {
        //        cylinder = ObjectPoolManager.GetObjectRequest(TentacleRoot.states[root.currentState].cylinder);
        //    }
        //}
        // TODO
    }

    void FixedUpdate()
    {
        if (TentacleRoot.states[root.currentState].solid == true)
            return;

        if (prev != null)
        {
            float _theta = prev.theta;
            float _phi = prev.phi;

            deltaTheta = Mathf.Abs(_theta - theta);
            deltaPhi = Mathf.Abs(_phi - phi);

            // 각도차가 비탄성정도보다 클 경우에만 각도변경
            if (deltaTheta > TentacleRoot.states[root.currentState].bendingDegrees)
            {
                theta = theta * (1 - TentacleRoot.states[root.currentState].bendingRecoverySpeed) + _theta * TentacleRoot.states[root.currentState].bendingRecoverySpeed;
            }
            if (deltaPhi > TentacleRoot.states[root.currentState].bendingDegrees)
            {
                phi = phi * (1 - TentacleRoot.states[root.currentState].bendingRecoverySpeed) + _phi * TentacleRoot.states[root.currentState].bendingRecoverySpeed;
            }
        }

        float phiAngleConformity = 1.0f - Mathf.Abs(((theta + 360.0f) * devide90) % 2 - 1.0f);

        ro = (deltaPhi * phiAngleConformity + deltaTheta) * devide90 * TentacleRoot.states[root.currentState].lengthenRatio * TentacleRoot.states[root.currentState].minLength + TentacleRoot.states[root.currentState].minLength;

        followPosition = followPosition * (1 - TentacleRoot.states[root.currentState].modifyRecoverySpeed) + transform.position * TentacleRoot.states[root.currentState].modifyRecoverySpeed;

        float theta2Rad = theta * Mathf.Deg2Rad;
        float phi2Rad = phi * Mathf.Deg2Rad;

        Vector3 addPosition = new Vector3();
        addPosition.x += ro * Mathf.Sin(theta2Rad) * Mathf.Cos(phi2Rad);
        addPosition.y += ro * Mathf.Cos(theta2Rad);
        addPosition.z += ro * Mathf.Sin(theta2Rad) * Mathf.Sin(phi2Rad);

        Vector3 directionToNext = (followPosition + addPosition) - transform.position;

        if (next != null)
        {
            next.transform.position = followPosition + addPosition;

            transform.up = directionToNext.normalized;

            if (cylinder != null)
            {
                cylinder.transform.up = directionToNext.normalized;

                float distanceToNextHalf = Vector3.Distance(transform.position, next.transform.position) * 0.5f;
                cylinder.transform.localScale = new Vector3(
                    TentacleRoot.states[root.currentState].thickness,
                    distanceToNextHalf,
                    TentacleRoot.states[root.currentState].thickness);

                cylinder.transform.position = transform.position + cylinder.transform.up * distanceToNextHalf;
            }
        }

    }

    void SetPosition(Vector3 pos)
    {
        transform.position = pos;
        ReverseOperation();
    }

    public void ReverseOperation()
    {
        Vector3 deltaPos = (next.transform.position - transform.position).normalized;

        float _phi = Mathf.Atan2(deltaPos.z, deltaPos.x) * Mathf.Rad2Deg;

        if (_phi < phi - 180.0f)
        {
            Phi = phi * (1 - TentacleRoot.states[root.currentState].rotationSpeedByModify) + (_phi + 360.0f) * TentacleRoot.states[root.currentState].rotationSpeedByModify;
        }
        else if (_phi > phi + 180.0f)
        {
            Phi = phi * (1 - TentacleRoot.states[root.currentState].rotationSpeedByModify) + (_phi - 360.0f) * TentacleRoot.states[root.currentState].rotationSpeedByModify;
        }
        else
        {
            Phi = phi * (1 - TentacleRoot.states[root.currentState].rotationSpeedByModify) + _phi * TentacleRoot.states[root.currentState].rotationSpeedByModify;
        }

        float _theta = Mathf.Acos(deltaPos.y) * Mathf.Rad2Deg;

        if (_theta < theta - 180.0f)
        {
            Theta = theta * (1 - TentacleRoot.states[root.currentState].rotationSpeedByModify) + (_theta + 360.0f) * TentacleRoot.states[root.currentState].rotationSpeedByModify;
        }
        else if (_theta > theta + 180.0f)
        {
            Theta = theta * (1 - TentacleRoot.states[root.currentState].rotationSpeedByModify) + (_theta - 360.0f) * TentacleRoot.states[root.currentState].rotationSpeedByModify;
        }
        else
        {
            Theta = theta * (1 - TentacleRoot.states[root.currentState].rotationSpeedByModify) + _theta * TentacleRoot.states[root.currentState].rotationSpeedByModify;
        }
    }

    public void DestroyTentacle()
    {
        if (cylinder != null)
            Destroy(cylinder);
        Destroy(gameObject);
    }
}
