using UnityEngine;
using System.Collections.Generic;
using CriWare;
using static CriWare.CriAtomEx;
using System.Linq;
using System;
using Cysharp.Threading.Tasks;


/// <summary>
/// サウンドの種類
/// NOTE: チャンネルと同義
/// </summary>
public enum SoundType
{
    BGM,
    SE,
    VOICE
}


/// <summary>
/// サウンド再生管理クラス
/// </summary>
public class CRIAudioManager
{
    static CRIAudioManager _instance = new CRIAudioManager();
    public static CRIAudioManager Instance => _instance;

    // サウンドプレイヤーの数はプロジェクトに応じて変えても良い
    const int SoundTypeCount = 3;
    const int AtomSourceBuffer = 10;
    private SoundPlayer[] _player = new SoundPlayer[SoundTypeCount];
    private BGMPlayer _bgmplayer;
    private SEPlayerWith3D _seplayer;
    private SoundPlayer _voiceplayer;

    CRIAudioManager()
    {
        _player[(int)SoundType.BGM] = _bgmplayer = new BGMPlayer();
        _player[(int)SoundType.SE] = _seplayer = new SEPlayerWith3D();
        _player[(int)SoundType.VOICE] = _voiceplayer = new SoundPlayer(SoundType.VOICE);
    }

    static public BGMPlayer BGM => _instance._bgmplayer;
    static public SEPlayerWith3D SE => _instance._seplayer;
    static public SoundPlayer VOICE => _instance._voiceplayer;


    bool _isReady = false;
    private Dictionary<string, SoundDic> _soundDic = new Dictionary<string, SoundDic>();
    private List<Tuple<SoundType, string, string>> _defferPlaySoundList = new List<Tuple<SoundType, string, string>>();


    static public void Initialize()
    {
        _instance.LoadCueSheet();
    }

    async void LoadCueSheet()
    {
        //CriAtomの取得
        //NOTE: シーンにおいてある必要がある
        var criAtom = GameObject.FindAnyObjectByType<CriAtom>();
        if (criAtom == null)
        {
            _isReady = false;
            // CRIPrefabをResourcesフォルダーからロード
            var criAtomPrefab = (GameObject)Resources.Load("CRI");
            // ロードしたCRIPrefabを生成
            var newCriAtom = GameObject.Instantiate(criAtomPrefab);

            criAtom = GameObject.FindAnyObjectByType<CriAtom>();
        }

        if (_isReady) return;

        // キューシートファイルのロード待ち
        await UniTask.WaitUntil(() => criAtom.cueSheets.All(cs => cs.IsLoading == false));

        // Cue情報の取得
        foreach (var sheet in criAtom.cueSheets)
        {
            _soundDic.Add(sheet.name, new SoundDic(sheet.acb));
        }

        _isReady = true;

        foreach (var player in _player)
        {
            player.Setup();
            player.SetVolume(1.0f);
        }

        foreach (var s in _defferPlaySoundList)
        {
            _player[(int)s.Item1].Play(s.Item2, s.Item3);
        }
        _defferPlaySoundList.Clear();
    }

    private void OnDestroy()
    {
        foreach (var player in _player)
        {
            player.Dispose();
        }
    }

    static void PlayQueue(SoundType type, string acb, string name)
    {
        _instance._defferPlaySoundList.Add(new Tuple<SoundType, string, string>(type, acb, name));
    }

    static public void Update3D(Vector3 listenerPos)
    {
        //TBD
    }


    class SoundDic
    {
        CriAtomExAcb _atomExAcb;
        private Dictionary<string, CueInfo> _cueInfoDic = new Dictionary<string, CueInfo>();

        public SoundDic(CriAtomExAcb acb)
        {
            _atomExAcb = acb;
            foreach (var cueInfo in acb.GetCueInfoList())
            {
                _cueInfoDic.Add(cueInfo.name, cueInfo);
            }
        }

        public CriAtomExAcb GetAcb()
        {
            return _atomExAcb;
        }

        public CueInfo GetCueInfo(string cueName)
        {
            return _cueInfoDic[cueName];
        }
    }

    /// <summary>
    /// チャンネル別で管理するためのサウンドプレイヤークラス
    /// </summary>
    public class SoundPlayer
    {
        SoundType _type;
        protected float _volume = 1.0f;
        protected CriAtomExPlayer _atomExPlayer;

        public bool IsPlaying => _atomExPlayer.GetStatus() == CriAtomExPlayer.Status.Playing;


        public SoundPlayer(SoundType type)
        {
            _type = type;
        }

        public virtual void Setup()
        {
            _atomExPlayer = new CriAtomExPlayer();
        }

        public virtual void Dispose()
        {
            _atomExPlayer.Dispose();
        }

        public virtual void SetVolume(float vol)
        {
            _volume = vol;
            _atomExPlayer.SetVolume(_volume);
        }

        public virtual CriAtomExPlayback Play(string cueSheet, string cueName, float delay = 0.0f)
        {
            //準備待ちの時は準備終わり次第再生
            if (!_instance._isReady)
            {
                PlayQueue(_type, cueSheet, cueName);
                return default;
            }

            CueInfo info = _instance._soundDic[cueSheet].GetCueInfo(cueName);
            _atomExPlayer.SetCue(_instance._soundDic[cueSheet].GetAcb(), info.id);
            _atomExPlayer.SetPreDelayTime(delay);
            return _atomExPlayer.Start();
        }

        public virtual void Stop()
        {
            _atomExPlayer.Stop();
        }
    }

    /// <summary>
    /// BGM再生用のカスタムプレーヤー
    /// NOTE: 何もしていないが拡張はできるサンプル
    /// </summary>
    public class BGMPlayer : SoundPlayer
    {
        public BGMPlayer() : base(SoundType.BGM) { }
    }

    /// <summary>
    /// 3Dサウンド再生もできるSEプレーヤー
    /// </summary>
    public class SEPlayerWith3D : SoundPlayer
    {
        /// <summary>
        /// 3Dサウンド再生用(単一)
        /// </summary>
        public class Sound3D
        {
            protected CriAtomEx3dSource _source = new CriAtomEx3dSource();
            protected CriAtomExPlayer _atomExPlayer3D = new CriAtomExPlayer();

            public bool IsBusy => _atomExPlayer3D.GetStatus() == CriAtomExPlayer.Status.Playing;

            public void Dispose()
            {
                _atomExPlayer3D.Dispose();
                _source.Dispose();
            }

            public void Play3D(Vector3 playPos, string cueSheet, string cueName)
            {
                //_source.SetMinMaxDistance(minDistance, maxDistance);
                //_source.SetDopplerFactor(dopplerFactor);
                _source.SetPosition(playPos.x, playPos.y, playPos.z);
                _source.Update();

                CueInfo info = _instance._soundDic[cueSheet].GetCueInfo(cueName);
                _atomExPlayer3D.SetCue(_instance._soundDic[cueSheet].GetAcb(), info.id);
                _atomExPlayer3D.SetPanType(CriAtomEx.PanType.Pos3d);
                _atomExPlayer3D.Set3dSource(_source);
                _atomExPlayer3D.UpdateAll();
                //_atomExPlayer3D.Set3dListener(_instance._listener.3d as CriAtomEx3dListener);
                _atomExPlayer3D.Start();
            }
        }

        CriAtomEx3dListener _lintener;
        Sound3D[] _sound3Ds = new Sound3D[AtomSourceBuffer];

        public SEPlayerWith3D() : base(SoundType.SE)
        {
        }

        public override void Setup()
        {
            _lintener = new CriAtomEx3dListener();
            _atomExPlayer = new CriAtomExPlayer();
            for (int i = 0; i < AtomSourceBuffer; ++i)
            {
                _sound3Ds[i] = new Sound3D();
            }
        }
        public override void Dispose()
        {
            base.Dispose();
            for (int i = 0; i < AtomSourceBuffer; ++i)
            {
                _sound3Ds[i].Dispose();
            }
        }

        Sound3D GetPlayer()
        {
            for (int i = 0; i < AtomSourceBuffer; ++i)
            {
                if (_sound3Ds[i].IsBusy) continue;
                return _sound3Ds[i];
            }

            return null;
        }

        public void Play3D(Vector3 playPos, string cueSheet, string cueName)
        {
            Sound3D player = GetPlayer();
            if (player == null)
            {
                Debug.LogWarning("3D音声の再生上限です");
                return;
            }

            player.Play3D(playPos, cueSheet, cueName);
        }
    }
}