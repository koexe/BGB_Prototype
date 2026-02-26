using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;
    [SerializeField] Camera currentCamera;
    [SerializeField] Transform targetTr;

    [Header("Follow")]
    [SerializeField] float followSmooth = 5f;

    [Header("Shake")]
    [SerializeField] float defaultShakePower = 0.3f;

    Vector3 velocity;
    Vector3 shakeOffset;

    CancellationTokenSource shakeCTS;

    private void Awake()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }

    public void Initialization(Camera _camera , Transform _targetTr)
    {
        this.currentCamera = _camera;
        this.targetTr = _targetTr;
    }

    void LateUpdate()
    {
        if (this.targetTr == null)
            return;

        FollowTarget();
    }

    void FollowTarget()
    {
        Vector3 t_targetPos = this.targetTr.position;
        t_targetPos.z = this.currentCamera.transform.position.z;

        // 부드러운 이동 (SmoothDamp)
        Vector3 t_smoothPos = Vector3.SmoothDamp(
            this.currentCamera.transform.position,
            t_targetPos,
            ref this.velocity,
            1f / this.followSmooth
        );

        this.currentCamera.transform.position = t_smoothPos + this.shakeOffset;
    }

    public void Shake(float _power, float _duration)
    {
        this.shakeCTS?.Cancel();
        this.shakeCTS?.Dispose();

        this.shakeCTS = new CancellationTokenSource();
        ShakeAsync(_power, _duration, this.shakeCTS.Token).Forget();
    }

    public void ShakeDefault(float _duration)
    {
        Shake(this.defaultShakePower, _duration);
    }

    async UniTaskVoid ShakeAsync(float _power, float _duration, CancellationToken _ct)
    {
        float t_elapsed = 0f;

        while (t_elapsed < _duration)
        {
            _ct.ThrowIfCancellationRequested();

            t_elapsed += Time.deltaTime;

            float t_damper = 1f - (t_elapsed / _duration); // 점점 약해짐

            this.shakeOffset = Random.insideUnitSphere * _power * t_damper;
            this.shakeOffset.z = 0f;

            await UniTask.Yield(PlayerLoopTiming.Update, _ct);
        }

        this.shakeOffset = Vector3.zero;
    }
}