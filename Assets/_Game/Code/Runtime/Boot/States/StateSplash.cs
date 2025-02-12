﻿using ProxFramework.StateMachine;

namespace Prox.GameName.Runtime
{
    public class StateSplash : State
    {
        public override void Init()
        {
        }

        public override void Enter()
        {
            ChangeState<StateInitPackage>();
        }

        public override void Exit()
        {
        }
    }
}