using UnityEngine;
using System.Collections;

public class FSMControlEx :MonoBehaviour{
	public FSM FSMAI = new FSM();
	StatePath ActiveIdle = new StatePath("Idle");
	StatePath ActiveRun = new StatePath("Active/Run");
	StatePath ActiveWalk = new StatePath("Active/Walk");
	public void Start(){
		InitState ();
	}
	/// <summary>
	/// Inits the state.
	/// </summary>
	public void InitState(){
		//如果 状态本身很复杂 则创建状态机集合 
		//		FSMAI.AddStateMachine (ActiveIdle,onIdleMachineEnter);
		//		FSMAI.AddStateMachine (ActiveRun,onActiveMachineEnter);
		
		//添加成员变量
		FSMAI.AddParameter ("ActiveRun",ParametersType.Int);
		FSMAI.AddParameter ("ActiveIdle",ParametersType.Int);
		FSMAI.AddParameter ("ActiveWalk",ParametersType.Int);
		FSMAI.AddParameter ("ActiveState",ParametersType.Int);
		
		//添加状态 ActiveIdle 并设置状态进入回调 状态更新回调 和 一个ActiveRun 状态转移【当触发 ActiveRun 的时候转移到 ActiveRun】
		FSMAI.AddState (ActiveIdle,OnIdleEnter,OnIdleUpdaet).AddTranstion (new StateTranstion (ActiveRun,"ActiveRun",ParametersType.Trigger));
		//添加状态 ActiveIdle 并设置状态进入回调 状态更新回调 和 一个ActiveRun 状态转移【当触发 ActiveWalk 的时候转移到 ActiveWalk】
		FSMAI.AddState (ActiveRun,OnRunEnter,OnRunUpdate).AddTranstion (new StateTranstion (ActiveWalk,"ActiveWalk",ParametersType.Trigger));
		//添加状态 ActiveIdle 并设置状态进入回调 状态更新回调 和 一个ActiveRun 状态转移【当触发 ActiveIdle 的时候转移到 ActiveIdle】
		FSMAI.AddState (ActiveWalk,OnWalkEnter,OnRunUpdate).AddTranstion (new StateTranstion (ActiveIdle,"ActiveIdle",ParametersType.Trigger));
		
		//添加任意状态转移 ->ActiveRun  条件 ActiveState == 1
		FSMAI.AddAnyTranstion(new StateTranstion(ActiveRun,"ActiveState",ParametersType.Int,1));
		//添加任意状态转移 ->ActiveWalk  条件 ActiveState == 2
		FSMAI.AddAnyTranstion(new StateTranstion(ActiveWalk,"ActiveState",ParametersType.Int,2));
		//添加任意状态转移 ->ActiveIdle  条件 ActiveState == 3
		FSMAI.AddAnyTranstion(new StateTranstion(ActiveIdle,"ActiveState",ParametersType.Int,3));
		
		//设置默认状态
		FSMAI.SetDefualt (ActiveIdle);
	}
	public void onActiveMachineEnter(){
		//		Debug.Log ("进入ActiveMachine");
	}
	public void onIdleMachineEnter(){
		//		Debug.Log ("进入IdleMachine");
	}
	public void OnRunUpdate(float dt){
	}
	public void OnRunEnter(){
		//		Debug.Log ("RunEnter");
	}
	public void OnWalkEnter(){
		//		Debug.Log ("OnWalkEnter");
	}
	public void OnIdleUpdaet(float dt){
	}
	public void OnIdleEnter(){
		//		Debug.Log ("待机进入");
	}
	public void Update(){
		FSMAI.UpdateState (Time.deltaTime);
		if (Input.GetKeyDown ("1"))
			FSMAI.SetInt ("ActiveState", 1);
		//			FSMAI.SetTrigger ("ActiveRun");
		//			FSMAI.SetState (ActiveRun);
		if (Input.GetKeyDown ("2"))
			FSMAI.SetInt ("ActiveState", 2);
		//			FSMAI.SetTrigger ("ActiveWalk");
		//			FSMAI.SetState (ActiveWalk);
		if (Input.GetKeyDown ("3"))
			FSMAI.SetInt ("ActiveState", 3);
		//			FSMAI.SetTrigger ("ActiveIdle");
		//			FSMAI.SetState (idle);
	}
}
