//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cosmos.Actor;
using Cosmos.Framework.Components;

namespace ExampleProjectLib
{
	class GateServices
	{
        //Dictionary<int, ActorRunner> SubActors = new Dictionary<int, ActorRunner>();

        GateActor _actor;
		public GateServices (GateActor actor)
		{
			_actor = actor;
		}

		public Task<PlayerSession> UserLogin()
		{
			var player = new Player(new PlayerEntity());
			var pSession = new PlayerSession(player);

			return null;
		}

		/// <summary>
		/// 返回一个动态创建的Actor，供客户端连接
		/// </summary>
		/// <param name="session">Session.</param>
		//public ActorNodeConfig GetGameActor(PlayerSession session, int mapId)
		//{
		//    ActorRunner actorRunner;
		//    if (!SubActors.TryGetValue(mapId, out actorRunner))
		//    {
  //              var actorNodeConf = _actor.Conf.Clone();
  //              actorRunner = SubActors[mapId] = ActorRunner.Run(actorNodeConf); // 启动一个Actor
  //          }

  //          return actorRunner.Actor.Conf;
		//}

		public Task<bool> UserLogout(PlayerSession session)
		{
			return null;
		}
	}
}

