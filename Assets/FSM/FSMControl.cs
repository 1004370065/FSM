using UnityEngine;
using System.Collections;
public class FSMControl :MonoBehaviour{
	public FSM FSMAI = new FSM();
	StatePath ActiveIdle = new StatePath("Idle");
	StatePath ActiveRun = new StatePath("Run");
	public void Start(){
		//添加成员变量
		FSMAI.AddParameter ("ActiveRun",ParametersType.Bool);

		//添加状态 ActiveIdle 并设置状态进入回调 状态更新回调
		FSMAI.AddState (ActiveIdle,OnIdleEnter);
		//添加状态 ActiveWalk 并设置状态进入回调 状态更新回调
		FSMAI.AddState (ActiveRun,OnRunEnter);

		//添加任意状态转移 ->ActiveRun  条件 ActiveRun == true
		FSMAI.AddAnyTranstion(new StateTranstion(ActiveRun,"ActiveRun",ParametersType.Bool,true));
		//添加任意状态转移 ->ActiveIdle  条件 ActiveRun == false
		FSMAI.AddAnyTranstion(new StateTranstion(ActiveIdle,"ActiveRun",ParametersType.Bool,false));

		//设置默认状态
		FSMAI.SetDefualt (ActiveIdle);
	}
	public void OnRunEnter(){
		Debug.Log ("OnRunEnter");
	}
	public void OnIdleEnter(){
		Debug.Log ("OnIdleEnter");
	}
	public void Update(){
		FSMAI.UpdateState (Time.deltaTime);
		if (Input.GetKeyDown ("1"))
			FSMAI.SetBool ("ActiveRun", true);
		if (Input.GetKeyDown ("2"))
			FSMAI.SetBool ("ActiveRun", false);
	}
}
