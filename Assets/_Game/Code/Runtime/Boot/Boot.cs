using System;
using UnityEngine;
using ProxFramework.StateMachine;

namespace Prox.GameName.Runtime
{
    public class Boot : MonoBehaviour
    {
        private StateMachine _bootStateMachine;

        private void Awake()
        {
#if ENABLE_HCLR
            DisStripCode();
#endif
        }

        private void Start()
        {
            InitBootState();
            _bootStateMachine.Start<StateLaunch>();
        }

        private void InitBootState()
        {
            _bootStateMachine = StateMachine.Create(this);
            _bootStateMachine.AddState<StateLaunch>();
            _bootStateMachine.AddState<StateSplash>();
            _bootStateMachine.AddState<StateInitPackage>();
            _bootStateMachine.AddState<StateUpdateVersion>();
            _bootStateMachine.AddState<StateUpdateManifest>();
            _bootStateMachine.AddState<StateCreateDownloader>();
            _bootStateMachine.AddState<StateDownloadFile>();
            _bootStateMachine.AddState<StatePatchDone>();
            _bootStateMachine.AddState<StateLoadAssembly>();
            _bootStateMachine.AddState<StateStartGame>();
        }

        //防止代码被裁剪
        //如果在主工程无引用，link.xml的防裁剪也无效。
        private void DisStripCode()
        {
            //UnityEngine.Physics
            RegisterType<Collider>();
            RegisterType<Collider2D>();
            RegisterType<Collision>();
            RegisterType<Collision2D>();
            RegisterType<CapsuleCollider2D>();

            RegisterType<Rigidbody>();
            RegisterType<Rigidbody2D>();
        
            RegisterType<Ray>();
            RegisterType<Ray2D>();

            //UnityEngine.Graphics
            RegisterType<Mesh>();
            RegisterType<MeshRenderer>();

            //UnityEngine.Animation
            RegisterType<AnimationClip>();
            RegisterType<AnimationCurve>();
            RegisterType<AnimationEvent>();
            RegisterType<AnimationState>();
            RegisterType<Animator>();
            RegisterType<Animation>(); 
        }

        private void RegisterType<T>()
        {
#if UNITY_EDITOR
            Debug.Log($"DisStripCode RegisterType :{typeof(T)}");
#endif
        }
    }
}