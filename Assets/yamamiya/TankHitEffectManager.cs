using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class TankHitEffectManager : MonoBehaviour
{
    [SerializeField] private ParticleSystem _hitEffectPrefab;
    [SerializeField] private int poolSize = 50;
    [SerializeField] private int maxPoolSize = 100;

    private Stack<ParticleSystem> _hitEffectPool = new Stack<ParticleSystem>();
    private CancellationTokenSource _cancellationTokenSource;

    private void Awake()
    {
        _cancellationTokenSource = new CancellationTokenSource();
    }

    private void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            ParticleSystem effect = Instantiate(_hitEffectPrefab, transform);
            effect.gameObject.SetActive(false);
            _hitEffectPool.Push(effect);
        }
    }

    private void OnDestroy()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
    }

    /// <summary>
    /// 指定された位置でヒットエフェクトを再生する。
    /// </summary>
    /// <param name="position">ヒットエフェクトを表示するべき位置</param>
    public void PlayHitEffect(Vector3 position)
    {
        // プールからエフェクトを取得、または新規作成
        ParticleSystem effect = _hitEffectPool.Count > 0
            ? _hitEffectPool.Pop()
            : Instantiate(_hitEffectPrefab);

        // エフェクトの位置を設定して再生
        effect.transform.position = position;
        effect.gameObject.SetActive(true);
        effect.Play();

        ReturnToPool(effect, _cancellationTokenSource.Token).Forget();
    }

    /// <summary>
    /// エフェクトが終了したらプールに戻す。
    /// </summary>
    /// <param name="effect">プールに戻すエフェクト</param>
    /// <param name="cancellationToken">キャンセルトークン</param>
    /// <returns></returns>
    private async UniTask ReturnToPool(ParticleSystem effect, CancellationToken cancellationToken)
    {
        try
        {
            // エフェクトの再生時間を待機
            await UniTask.Delay(TimeSpan.FromSeconds(effect.main.duration), 
                cancellationToken: cancellationToken);

            if (effect != null && !cancellationToken.IsCancellationRequested)
            {
                // エフェクトを停止してプールに戻す
                effect.gameObject.SetActive(false);

                // プールのサイズが上限に達していない場合は戻す
                if (_hitEffectPool.Count < maxPoolSize)
                {
                    _hitEffectPool.Push(effect);
                }
                else
                {
                    Destroy(effect.gameObject);
                }
            }
        }
        catch (OperationCanceledException)
        {
            Debug.Log("エフェクトがキャンセルされました");
        }
    }

    public void Dispose()
    {
        _hitEffectPool.Clear();
    }
}
