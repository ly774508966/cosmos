﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cosmos.Tool;
namespace Cosmos.Actor
{
    /// <summary>
    /// Actor In Task State Share
    /// </summary>
    public enum ActorRunState
    {
        None,
        Running,
    }

    /// <summary>
    /// Control a actor's running in process
    /// 
    /// A observer
    /// </summary>
    public class ActorRunner
    {
        public static Dictionary<string, ActorRunner> Runners = new Dictionary<string, ActorRunner>();

        public DateTime StartTime = DateTime.UtcNow;

        public int SecondsTick = 0;
        private ActorRunner()
        {
        }

        public ActorRunState State = ActorRunState.None;
        private Task ActorThread;
        private ActorConf Conf;
        public Actor Actor;

        public object ActorName => Conf.Name;

        private ActorRunner(ActorConf conf)
        {
            Conf = conf;
            ActorThread = Task.Run(() =>
            {
                Actor = ActorFactory.Create(conf);
                State = ActorRunState.Running;
                while (Actor.IsActive)
                {
                    Thread.Sleep(1000);
                    SecondsTick++;
                }
            });

            while (State == ActorRunState.None)
            {
                // block
            }

            Runners[conf.Name] = this;
        }



        public static ActorRunner Run(ActorConf conf)
        {
            ActorRunner runner = new ActorRunner(conf);
            return runner;
        }

        public static ActorRunner GetActorStateByName(string actorName)
        {
            return Runners[actorName];
        }
    }
}
