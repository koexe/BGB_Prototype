using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsModule : MonoBehaviour
{
    [SerializeField] Transform targetTr;
    [SerializeField] Vector2 inputForce;

    [SerializeField] List<Force> currentForces;


    public void Initialization()
    {
        if (this.targetTr == null)
        {
            this.enabled = true;
            LogUtil.LogError($"Target Tranform Null{this.name}");
        }
        this.currentForces = new List<Force>();
    }

    public void SetInputForce(Vector2 _force)
    {
        this.inputForce += _force;
    }
    public void AddForce(Force _force)
    {
        this.currentForces.Add(_force);
    }

    private void FixedUpdate()
    {
        foreach (var t_force in this.currentForces)
        {
            if (t_force.IsFinished()) continue;
            this.targetTr.position += (Vector3)(t_force.Update(Time.fixedDeltaTime) * Time.fixedDeltaTime);
        }


        this.targetTr.position += (Vector3)(this.inputForce * Time.fixedDeltaTime);
        this.inputForce = Vector3.zero;
    }
}

[System.Serializable]
public class Force
{
    public enum ForceType
    {
        Constant,          // 일정하게 유지
        LinearDecrease,    // 선형 감소
        LinearIncrease,    // 선형 증가
        EaseIn,            // 점점 강해짐 (가속)
        EaseOut,           // 점점 약해짐 (감속)
        EaseInOut,         // 처음/끝 완만
        CustomCurve        // AnimationCurve 사용
    }

    public ForceType forceType;

    public Vector2 direction;    // 정규화된 방향
    public float magnitude;      // 최대 힘
    public float duration;       // 지속 시간 (0이면 즉시)

    float elapsedTime;

    // CustomCurve용
    AnimationCurve curve;

    public Force DuplacateForce(Vector2 _dir)
    {
        return new Force(this.forceType, _dir, this.magnitude, this.duration, this.curve);
    }


    public Force(
        ForceType _type,
        Vector2 _direction,
        float _magnitude,
        float _duration,
        AnimationCurve _curve = null)
    {
        forceType = _type;
        direction = _direction.normalized;
        magnitude = _magnitude;
        duration = Mathf.Max(_duration, 0.0001f);
        curve = _curve;
        elapsedTime = 0f;
    }

    /// <summary>
    /// 매 프레임 호출
    /// </summary>
    public Vector2 Update(float _deltaTime)
    {
        elapsedTime += _deltaTime;

        float t = Mathf.Clamp01(elapsedTime / duration);
        float strength = EvaluateStrength(t);

        return direction * (magnitude * strength);
    }

    public bool IsFinished()
    {
        return elapsedTime >= duration;
    }

    float EvaluateStrength(float _t)
    {
        switch (forceType)
        {
            case ForceType.Constant:
                return 1f;

            case ForceType.LinearDecrease:
                return 1f - _t;

            case ForceType.LinearIncrease:
                return _t;

            case ForceType.EaseIn:
                return _t * _t;

            case ForceType.EaseOut:
                return 1f - Mathf.Pow(1f - _t, 2f);

            case ForceType.EaseInOut:
                return Mathf.SmoothStep(0f, 1f, _t);

            case ForceType.CustomCurve:
                if (curve != null)
                    return curve.Evaluate(_t);
                return 1f;

            default:
                return 1f;
        }
    }
}