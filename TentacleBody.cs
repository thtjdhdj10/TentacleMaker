using UnityEngine;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;

using Debug = UnityEngine.Debug;

//using Debug = UnityEngine.Debug;

public class TentacleBody : MonoBehaviour
{
    // 일단 전부 public

    public TentacleRoot.TentacleProperties properties = new TentacleRoot.TentacleProperties();

    public float ro;

    public float theta;
    public float phi;

    Vector3 followPosition = new Vector3();
    Vector3 beforePosition = new Vector3();

    float deltaTheta = 0.0f;
    float deltaPhi = 0.0f;

    const float minDistanceForReverseOperation = 0.01f;
    float devide90 = 1.0f / 90.0f;

    public TentacleRoot root;
    public TentacleBody next;
    public TentacleBody prev;
    public GameObject cylinder;

    void Awake()
    {

    }

    void Start()
    {
        followPosition = transform.position;

        ro = properties.minLength;
        theta = 0.0f;
        phi = 90.0f;
        
    }

    void Update()
    {

        if (properties.useCylinder)
        {
            if (cylinder == null && next != null)
            {
                cylinder = VEasyPoolerManager.GetObjectRequest(properties.cylinder);
            }
        }
        else
        {
            if (cylinder != null)
            {
                VEasyPoolerManager.ReleaseObjectRequest(cylinder);
            }
        }
    }

    void OnDestroy()
    {
        if(prev != null)
            prev.next = next;

        if(next != null)
            next.prev = prev;

        if(root.removeCylinderAtRemoveBody == true)
            Destroy(cylinder);
    }

    void FixedUpdate()
    {
        if (properties.solid == true)
            return;

        if (prev == null)
        {
            theta = root.Theta;
            phi = root.Phi;
        }
        else
        {
            float _theta = prev.theta;
            float _phi = prev.phi;
            
            deltaTheta = Mathf.Abs(_theta - theta);
            deltaPhi = Mathf.Abs(_phi - phi);

            // 각도차가 비탄성정도보다 클 경우에만 각도변경
            if (deltaTheta > properties.bendingDegrees)
            {
                theta = theta * (1 - properties.bendingRecoverySpeed) + _theta * properties.bendingRecoverySpeed;
            }
            if (deltaPhi > properties.bendingDegrees)
            {
                phi = phi * (1 - properties.bendingRecoverySpeed) + _phi * properties.bendingRecoverySpeed;
            }
        }

        if (Vector3.Distance(transform.position, beforePosition) > minDistanceForReverseOperation)
        {
            ReverseOperation();
        }

        //

        float phiAngleConformity = 1.0f - Mathf.Abs(((theta + 360.0f) * devide90) % 2 - 1.0f);

        ro = (deltaPhi * phiAngleConformity + deltaTheta) * devide90 * properties.lengthenRatio * properties.minLength + properties.minLength;

        followPosition = followPosition * (1 - properties.modifyRecoverySpeed) + transform.position * properties.modifyRecoverySpeed;

        //

        float theta2Rad = theta * Mathf.Deg2Rad;
        float phi2Rad = phi * Mathf.Deg2Rad;

        Vector3 addPosition = new Vector3();
        addPosition.x += ro * Mathf.Sin(theta2Rad) * Mathf.Cos(phi2Rad);
        addPosition.y += ro * Mathf.Cos(theta2Rad);
        addPosition.z += ro * Mathf.Sin(theta2Rad) * Mathf.Sin(phi2Rad);

        //

        if (next != null)
        {
            Vector3 nextPosition = followPosition + addPosition;

            next.transform.position = next.transform.position * (1 - properties.modifyRecoverySpeed) + nextPosition * properties.modifyRecoverySpeed;

            Vector3 directionToNext = next.transform.position - transform.position;

            if (properties.setUpVector == true)
                transform.up = directionToNext.normalized;

            if (cylinder != null)
            {
                cylinder.transform.up = directionToNext.normalized;

                float distanceToNextHalf = Vector3.Distance(transform.position, next.transform.position) * 0.5f;
                cylinder.transform.localScale = new Vector3(
                    properties.thickness,
                    distanceToNextHalf,
                    properties.thickness);

                cylinder.transform.position = transform.position + cylinder.transform.up * distanceToNextHalf;
            }
        }

    }

    public void ReverseOperation()
    {
        if (next == null) return;

        Vector3 deltaPos = (next.transform.position - transform.position).normalized;

        //
        
        float _phi = Mathf.Atan2(deltaPos.z, deltaPos.x) * Mathf.Rad2Deg;

        float perPhi = phi % 360.0f;
        if (perPhi < 0.0f) perPhi += 360.0f;
        float varyingDegree = phi - perPhi;

        _phi += varyingDegree;

        //if (prev != null)
        //{


        //    if (Mathf.Abs(_phi - prev.phi) > Mathf.Abs(_phi - prev.phi - 360.0f))

        //        //            if (phi > _phi + 180.0f)
        //        _phi += 360.0f;
        //}
        //else
        //{
        //    if (phi > _phi + 180.0f)
        //        _phi += 360.0f;
        //}

        if (phi > _phi + 180.0f)
            _phi += 360.0f;

        phi = phi * (1 - properties.rotationSpeedByModify) +
        _phi * properties.rotationSpeedByModify;

        //

        float _theta = Mathf.Acos(deltaPos.y) * Mathf.Rad2Deg;

        float pertheta = theta % 360.0f;
        if (pertheta < 0.0f) pertheta += 360.0f;
        varyingDegree = theta - pertheta;

        _theta += varyingDegree;

        if(prev != null)
        {
            if (Mathf.Abs(_theta - prev.theta) > Mathf.Abs(_theta - prev.theta - 360.0f))

                //            if (theta > _theta + 180.0f)
                _theta += 360.0f;
        }
        else
        {
            if (theta > _theta + 180.0f)
                _theta += 360.0f;
        }

        theta = theta * (1 - properties.rotationSpeedByModify) +
            _theta * properties.rotationSpeedByModify;
    }
}
