//#define FSMDEBUG
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public enum UndoMode{
	/// <summary>
	/// 撤销直接返回上一级  比如 Base/A  ->  Base/B  -> Base/A   撤销返回到 Base
	/// </summary>
	ToBase,
	/// <summary>
	/// 撤销的时候根据列表  比如 Base/A  ->  Base/B  -> Base/A   撤销返回到 Base/B  再次撤销返回 Base/A界面
	/// </summary>
	ToUodoList,
	/// <summary>
	/// 撤销的时候根据列表 并且不存储同级信息   比如 Base/A  ->  Base/B  -> Base/A   撤销返回到 Base界面
	/// </summary>
	ToUndoListOnlyUnEqualStateMacth,
	Count,
}
[System.Serializable]
public class State{
	public delegate void Action();
	public delegate void Action<T>(T arg);
	public delegate void Action<T1,T2>(T1 arg1,T2 arg2);
	public Action onEnter;
	public Action onLeave;
	public Action<float> onUpdate;
	public StatePath statePath;
	/// <summary>
	/// 状态转移
	/// </summary>
	public List<StateTranstion> Transtions = new List<StateTranstion>();
	public void AddTranstion(StateTranstion target){
		Transtions.Add (target);
	}
	/// <summary>
	/// 经过多久后切换到下一个状态 
	/// time = 0 的时候为下一帧切换
	/// </summary>
	/// <param name="state">State.</param>
	/// <param name="time">Time.</param>
	public void ByStateTimeTranstionsState(StatePath state,float time = 0){
		AddTranstion (new StateTranstion (state, "StateTime", ParametersType.Float, time, StateTranstion.TranstionType.Greater));
	}
	public void ByPauseTimeTranstionState(StatePath state,float time = 0){
		AddTranstion (new StateTranstion (state, "PauseTime", ParametersType.Float, time, StateTranstion.TranstionType.Greater));
	}
}
/// <summary>
/// 一个状态机集合
/// </summary>
public class StateMachine{
	public delegate void Action();
	public delegate void Action<T>(T arg);
	public delegate void Action<T1,T2>(T1 arg1,T2 arg2);
	public Action onStateMachineEnter;
	public Action onStateMachineLeave;
	public Action<float> onStateMachineUpdate;
	/// <summary>
	/// 拥有多少状态机
	/// </summary>
	public Dictionary<string,State> states = new Dictionary<string,State>();
	/// <summary>
	/// 拥有多个状态机集合
	/// </summary>
	public Dictionary<string,StateMachine> stateMachines = new Dictionary<string,StateMachine>();
	public string StatePathName;
	public StateMachine(string name){
		StatePathName = name;
	}
	public StateMachine getStateMachine(List<string> path,int count = 0){
		if (path.Count - count == 1) {
			return this;
		} else {
			count++;
			string statemachinesname = path[count-1];
			return _getStateMachines(statemachinesname).getStateMachine(path,count);
		}
	}
	private StateMachine _getStateMachines(string stateMachineName){
		if (!stateMachines.ContainsKey (stateMachineName)) {
			stateMachines.Add(stateMachineName,new StateMachine(stateMachineName));
		}
		return stateMachines [stateMachineName];
	}
	public void AddState(string statename,State state){
		if (states.ContainsKey (statename))
			Debug.Log ("已经存在状态了:"+statename);
		else
			states.Add (statename, state);
	}
	public State getState(string statename){
		return states [statename];
	}
	public void OnEnter(){
		if (onStateMachineEnter != null) {
			onStateMachineEnter();
		}
	}
	public void OnUpdate(float dt){
		if (onStateMachineUpdate != null)
			onStateMachineUpdate (dt);
	}
	public void OnLeave(){
		if (onStateMachineLeave != null) {
			onStateMachineLeave();
		}
	}
}
[System.Serializable]
/// <summary>
/// 状态转移
/// </summary>
public class StateTranstion{
	public enum TranstionType
	{
		ExitTime,
		Greater,
		Less,
		Equals,
		UnEquals,
	}
	public TranstionType transtrionType;
	public ParametersType parametersType;
	public string parametersName;
	public int Value_int;
	public float Value_Float;
	public bool Value_Bool;
	//目标状态
	public StatePath TargetState;
	public StateTranstion(StatePath targetState,string _parametersName,ParametersType _parametersType,float valueFloat,TranstionType _transtrionType){
		parametersName = _parametersName;
		parametersType = _parametersType;
		TargetState = targetState;
		if(_parametersType == ParametersType.Float)
			Value_Float = valueFloat;
		else if(_parametersType == ParametersType.Int)
			Value_int = (int)valueFloat;
		transtrionType = _transtrionType;
	}
	public StateTranstion(StatePath targetState,string _parametersName,ParametersType _parametersType,bool valueBool){
		parametersName = _parametersName;
		parametersType = _parametersType;
		TargetState = targetState;
		Value_Bool = valueBool;
	}
	public StateTranstion(StatePath targetState,string _parametersName,ParametersType _parametersType,int ValueInt,TranstionType _transtrionType = TranstionType.Equals){
		parametersName = _parametersName;
		parametersType = _parametersType;
		TargetState = targetState;
		if(_parametersType == ParametersType.Float)
			Value_Float = (int)ValueInt;
		else if(_parametersType == ParametersType.Int)
			Value_int = ValueInt;
		transtrionType = _transtrionType;
	}
	public StateTranstion(StatePath targetState,string _parametersName,ParametersType _parametersType){
		parametersName = _parametersName;
		parametersType = _parametersType;
		TargetState = targetState;
	}
	public bool CheckStateParameters(StateParameters target){
		switch(parametersType){
		case ParametersType.Bool:
			return target.AsBool == Value_Bool;
		case ParametersType.Int:
			switch(transtrionType){
			case TranstionType.Greater:
				return target.AsInt>Value_int;
			case TranstionType.Less:
				return target.AsInt<Value_int;	
			case TranstionType.Equals:
				return Value_int == target.AsInt;	
			case TranstionType.UnEquals:
				return Value_int!=target.AsInt;	
			}
			break;
		case ParametersType.Float:
			switch(transtrionType){
			case TranstionType.Greater:
				return target.AsFloat>Value_Float;
				case TranstionType.Less:
				return target.AsFloat<Value_Float;	
			}
			return false;
		case ParametersType.Trigger:
			if(target.AsTrigger)
			{
				target.SetBool(false);
				return true;
			}
			break;
		}
		return false;
	}
}
public enum ParametersType{
	Int,
	Float,
	Bool,
	Trigger,
}
[System.Serializable]
/// <summary>
/// 状态机参数
/// </summary>
public class StateParameters{
	float valuefloat = 0;
	int valueint = 0;
	bool valuebool = false;
	public ParametersType parametersType;
	public StateParameters(ParametersType type){
		parametersType = type;
	}
	public int AsInt{
		get{
			return valueint;
		}
	}
	public float AsFloat{
		get{ 
			return valuefloat;
		}
	}
	public bool AsBool{
		get{

			return valuebool;
		}
	}
	public bool AsTrigger{
		get{
			return valuebool;
		}
	}
	public void SetBool(bool _value){
		valuebool = _value;
	}
	public void SetInt(int _value){
		valueint = _value;
	}
	public void SetFloat(float _value){
		valuefloat = _value;
	}
}

public interface IFSM{
	/// <summary>
	/// 初始化状态机
	/// </summary>
	void InitState();
}
[System.Serializable]
public class FSM {
	/// <summary>
	/// 状态机集合
	/// </summary>
	public StateMachine stateMachine = new StateMachine("Base");
	public Dictionary<string,StateParameters> FSMParameters = new Dictionary<string, StateParameters> ();
	public List<StateTranstion> AnyTranstrions = new List<StateTranstion> ();
	public State LastState;
	protected StateMachine lastSM;
	protected StateMachine currentStateMachine;
	public State _nextState;
	private State _state;
	private StateParameters _stateTimeParameter;
	private StateParameters _pauseTimeParameter;
	public string StateName;
	public string stateMacthName;
	public List<State> _undoStatePath = new List<State>();
	public UndoMode _undoMode;
	public void InitTimeParameter(){
		_stateTimeParameter = AddParameter ("StateTime", ParametersType.Float);
		_pauseTimeParameter = AddParameter ("PauseTime", ParametersType.Float);
	}
	public System.Enum stateEnum{
		set{ 
			SetState (value);
		}
	}
	public State state
	{
		// 获取当前状态
		get { return _state; }
		// 切换状态
		set
		{
			if (_state == value)
				return;
			
			if(_state != null)
				LastState = _state;

			if (currentStateMachine != null)
				lastSM = currentStateMachine;

			_nextState = value;

			stateTime = 0;
			StateName = value.statePath.StateName;
			stateMacthName = value.statePath.StateMachineName;
			_state = value;

			bool isnewSM = false;
			//GetNewSM
			if (LastState == null || _state.statePath.StateMachineName != LastState.statePath.StateMachineName) {
				isnewSM = true;
				currentStateMachine = stateMachine.getStateMachine (_state.statePath.AsListPath);
			}

			//ST Leave
			if (LastState != null && LastState.onLeave !=null ){
				LastState.onLeave();
			}
			//SM Leave
			if(isnewSM && lastSM!=null && lastSM.onStateMachineUpdate != null){
				lastSM.OnLeave();
			}

			//SM Enter
			if (isnewSM ) {
				if(currentStateMachine.onStateMachineEnter!=null)
					currentStateMachine.OnEnter();
			}
			//ST Enter
			if (_state != null && _state.onEnter != null){
				_state.onEnter();
			}
		}
	}
	bool _pause;
	float _pauseTime;
	public bool IsPause{
		get{ return _pause; }
	}
	public float PauseTime{
		set{
			_pauseTime = value;
			if(_pauseTimeParameter!=null)
				_pauseTimeParameter.SetFloat (value);
		}
		get{ return _pauseTime; }
	}
	// 状态时间
	private float _stateTime;
	public float stateTime { 
		get { return _stateTime; } 
		set { 
				
			_stateTime = value;
			if(_stateTimeParameter!=null)
				_stateTimeParameter.SetFloat(value);
		}
	}
	// 更新状态（在 FixedUpdate，Update 或 LateUpdate 中调用）
	public void UpdateState(float deltaTime)
	{
		if (_pause) {
			PauseTime =PauseTime+ deltaTime;
			if (_state != null && _pauseTimeParameter != null) {
				CheckStateTranstion();
			}
			return;
		}
		stateTime = stateTime+deltaTime;
		if (_state != null) {
			if (_state.onUpdate != null) {
				if (currentStateMachine != null && currentStateMachine.onStateMachineUpdate != null)
					currentStateMachine.OnUpdate (deltaTime);
				_state.onUpdate (deltaTime);
			}
			CheckStateTranstion();
		}
	}
	public bool IsNestState(StatePath path){
		return path.StateName == _nextState.statePath.StateName;
	}
	public bool IsState(StatePath path){
		return StateName == path.StateName;
	}
	public bool IsState(System.Enum path){
		return StateName == path.ToString ();
	}
	/// <summary>
	/// 状态转移
	/// </summary>
	public void CheckStateTranstion(){
		for (int i = 0; i < state.Transtions.Count; i++) {
			if (!FSMParameters.ContainsKey (state.Transtions [i].parametersName)) {
				StateParameters sp = new StateParameters (state.Transtions [i].parametersType);
				FSMParameters.Add (state.Transtions [i].parametersName,sp );
				if (_state.Transtions [i].parametersName == "StateTime")
					_stateTimeParameter = sp;
				else if (_state.Transtions [i].parametersName == "PauseTime")
					_pauseTimeParameter = sp;
			}
			if (state.Transtions [i].CheckStateParameters (FSMParameters [state.Transtions [i].parametersName])) {
				state = getState (state.Transtions [i].TargetState);
				return;
			}
		}
		for (int i = 0; i < AnyTranstrions.Count; i++) {
			if (!FSMParameters.ContainsKey (AnyTranstrions [i].parametersName)) {
				StateParameters sp = new StateParameters (AnyTranstrions [i].parametersType);
				FSMParameters.Add (AnyTranstrions[i].parametersName,sp );
				if (AnyTranstrions [i].parametersName == "StateTime")
					_stateTimeParameter = sp;
				else if (AnyTranstrions [i].parametersName == "PauseTime")
					_pauseTimeParameter = sp;
			}
			if (AnyTranstrions [i].CheckStateParameters (FSMParameters [AnyTranstrions [i].parametersName]) && state.statePath != AnyTranstrions [i].TargetState) {
				state = getState (AnyTranstrions [i].TargetState);
				return;
			}
		}
	}
	#region Start
	/// <summary>
	/// 添加一个任意事件转移
	/// </summary>
	/// <param name="target">Target.</param>
	public void AddAnyTranstion(StateTranstion target){
		AnyTranstrions.Add (target);
	}
	/// <summary>
	/// 新增成员变量
	/// 变量可能包含 浮点型 整数型 布尔型 触发器
	/// </summary>
	/// <param name="parametertype">Parametertype.</param>
	public StateParameters AddParameter(string parametertype,ParametersType type){
		StateParameters stateParameters = new StateParameters (type);
		if (!FSMParameters.ContainsKey (parametertype)) {
			FSMParameters.Add (parametertype, stateParameters);
		}
		return stateParameters;
	}
	/// <summary>
	/// 创建一个状态集合
	/// statePath 状态集合地址
	/// onenter 进入这个状态集合回调
	/// onUpdate 在这个状态集合更新的时候
	/// onLeave 离开这个状态集合的时候
	/// </summary>
	/// <returns>The state machine.</returns>
	/// <param name="statepath">Statepath.</param>
	/// <param name="onenter">Onenter.</param>
	/// <param name="onUpdate">On update.</param>
	/// <param name="onLeave">On leave.</param>
	public StateMachine AddStateMachine(StatePath statepath,StateMachine.Action onenter = null,StateMachine.Action<float> onUpdate = null,StateMachine.Action onLeave = null){
		if (statepath.Path [statepath.Path.Length - 1] != '/')
			statepath.Path += "/";
		StateMachine newstatemachine = stateMachine.getStateMachine (statepath.AsListPath);
		newstatemachine.onStateMachineEnter = onenter;
		newstatemachine.onStateMachineLeave = onLeave;
		newstatemachine.onStateMachineUpdate = onUpdate;
		if (!newstatemachine.states.ContainsKey ("base")){
			StatePath sp = new StatePath (statepath.Path + "base");
			AddState (sp);
		}
		return newstatemachine;
	}
	public State AddState(System.Enum statepath,State.Action onenter = null,State.Action<float> onupdate = null,State.Action onleave = null){
		return AddState (new StatePath(statepath.ToString ()), onenter, onupdate, onleave);
	}
	/// <summary>
	/// 创建一个状态
	/// 传入状态地址 比如 Action/Run  表示 创建一个Run状态  放在Action 集合里
	/// OnEnter 状态进入回调
	/// OnUpdate 状态更新回调
	/// OnLeave 状态离开回调
	/// </summary>
	/// <returns>The state.</returns>
	/// <param name="statepath">Statepath.</param>
	/// <param name="onenter">Onenter.</param>
	/// <param name="onUpdate">On update.</param>
	/// <param name="onLeave">On leave.</param>
	public State AddState(StatePath statepath,State.Action onenter = null,State.Action<float> onUpdate = null,State.Action onLeave = null){
		string statename = statepath.StateName;
		State newstate = new State ();
		newstate.onEnter = onenter;
		newstate.onUpdate = onUpdate;
		newstate.onLeave = onLeave;
		newstate.statePath = statepath;
		stateMachine.getStateMachine (statepath.AsListPath).AddState (statename,newstate);
		#if FSMDEBUG
		Debug.Log("PATH:"+statepath+  "准备添加状态:"+statepath.Path+"状态数"+stateMachine.getStateMachine (statepath.AsListPath).states.Count);
		#endif
		return newstate;
	}
	public State AddState(StatePath statepath,State.Action<float> onUpdate){
		return AddState (statepath, null, onUpdate, null);
	}
	/// <summary>
	/// 通过路径获取状态机
	/// </summary>
	/// <returns>The state.</returns>
	/// <param name="statepath">Statepath.</param>
	public State getState(StatePath statepath){
		try{
			if(statepath.StateName == "")
				return stateMachine.getStateMachine(statepath.AsListPath).getState("base");
			return stateMachine.getStateMachine(statepath.AsListPath).getState(statepath.StateName);
		}
		catch{
				Debug.LogError("PATH:"+statepath+  "   Get StateMachine Error :::"+statepath.Path+"::andname::"+statepath.StateName+"Base状态机状态数"+stateMachine.states.Count+"Base状态机集合数:"+stateMachine.stateMachines.Count);
				StateMachine machine = stateMachine.getStateMachine(statepath.AsListPath);
				return machine.getState("base");
		}
	}
	public void Recove(){
		_pause = false;
	}
	public void GoBackState(){
		if (_undoMode == UndoMode.ToUodoList || _undoMode == UndoMode.ToUndoListOnlyUnEqualStateMacth) {
			if (_undoStatePath.Count > 0) {
				State undostate = _undoStatePath [_undoStatePath.Count - 1];
				_undoStatePath.RemoveAt (_undoStatePath.Count - 1);
				state = undostate;
			}
		} else if (_undoMode == UndoMode.ToBase) {
			StatePath sp = state.statePath.Copy();
			sp.AsListPath [sp.AsListPath.Count - 1] = "base";
			SetState (sp);
		}
	}
	public void Pause(){
		PauseTime = 0;
		_pause = true;	
	}
	public void StartAnew(){
		if(state.onEnter != null)
			state.onEnter();
		stateTime = 0;
		Recove ();
	}
	public void RefreshStateTime(){
		stateTime = 0;
	}
	/// <summary>
	/// 设置初始状态
	/// </summary>
	/// <param name="target">Target.</param>
	public void SetDefualt(State target){
		state = target;
	}
	/// <summary>
	/// 设置初始状态
	/// </summary>
	/// <param name="target">Target.</param>
	public void SetDefualt(StatePath statepath){
		SetDefualt (getState (statepath));
	}
	/// <summary>
	/// 强制设置状态
	/// </summary>
	/// <param name="target">Target.</param>
	public void SetState(State target){
		if (_undoMode == UndoMode.ToUodoList || _undoMode == UndoMode.ToUndoListOnlyUnEqualStateMacth) {
			if (state != null && target != state) {
				if (_undoMode == UndoMode.ToUodoList || target.statePath.StateMachineName != state.statePath.StateMachineName || state.statePath.StateName == "base")
					_undoStatePath.Add (state);
			}
		}

		state = target;
	}
	public void SetState(System.Enum target){
		SetState(new StatePath(target.ToString()));
	}
	/// <summary>
	/// 强制设置状态
	/// </summary>
	/// <param name="target">Target.</param>
	public void SetState(StatePath statepath){
		SetState (getState (statepath));
	}
	/// <summary>
	/// 设置触发器
	/// </summary>
	/// <param name="triggername">Triggername.</param>
	public void SetTrigger(string key){
		if(!FSMParameters.ContainsKey(key))
			FSMParameters.Add(key,new StateParameters(ParametersType.Bool));
		FSMParameters [key].SetBool(true);
	}
	/// <summary>
	/// 设置名为 boolname  的布尔值变量
	/// </summary>
	/// <param name="boolname">Boolname.</param>
	/// <param name="value">If set to <c>true</c> value.</param>
	public void SetBool(string key,bool value){
		if(!FSMParameters.ContainsKey(key))
			FSMParameters.Add(key,new StateParameters(ParametersType.Bool));
		FSMParameters [key].SetBool(value);
	}
	/// <summary>
	/// 设置名为 boolname  的浮点型变量
	/// </summary>
	/// <param name="boolname">Boolname.</param>
	/// <param name="value">If set to <c>true</c> value.</param>
	public void SetFloat(string key,float value){
		if(!FSMParameters.ContainsKey(key))
			FSMParameters.Add(key,new StateParameters(ParametersType.Float));
		FSMParameters [key].SetFloat(value);
	}
	/// <summary>
	/// 设置名为 boolname  的整数型变量
	/// </summary>
	/// <param name="boolname">Boolname.</param>
	/// <param name="value">If set to <c>true</c> value.</param>
	public void SetInt(string key,int value){
		if(!FSMParameters.ContainsKey(key))
			FSMParameters.Add(key,new StateParameters(ParametersType.Int));
		FSMParameters [key].SetInt(value);
	}

	#endregion
	public void DebugStates(){
		DebugSM (stateMachine);
	}

	void DebugSM(StateMachine SM){
		foreach (KeyValuePair<string,State> value in SM.states) {
			DebugState (value.Value);
		}
		foreach (KeyValuePair<string,StateMachine> value in SM.stateMachines) {
			Debug.Log ("SM:" + value.Value.StatePathName);
			DebugSM (value.Value);
		}
	}
	void DebugState(State state){
		Debug.Log("State:" + state.statePath.Path);
	}
	public void OnGUIParameters(){
		GUILayout.Label ("FSM StateParameters :");
		foreach (KeyValuePair<string,StateParameters> value in FSMParameters) {
			GUILayout.Label (value.Key + " --- " + value.Value.AsFloat);
		}
	}
}
[System.Serializable]
public class StatePath{
	public StatePath(string path){
		Path = path;
	}
	public string Path;
	public List<string> paths;
	public string StateMachineName;
	public StatePath Copy(){
		StatePath sp = new StatePath (Path);
		return sp;
	}
	public string StateName{
		get{ return AsListPath [AsListPath.Count - 1];}
	}

	public List<string> AsListPath{
		get{
			if(paths == null){
				string[] path = Path.Split ('/');
				paths = new List<string>();
				StateMachineName = "";
				if(path.Length == 1){
					StateMachineName = "Base";
				}
				else{
					StateMachineName = "Base/";
				}
				for(int i = 0;i < path.Length;i++){
					if(i<path.Length - 2){
						StateMachineName+=path[i]+"/";
					}
					else if(i == path.Length - 2){
						StateMachineName+=path[i];
					}
					paths.Add(path[i]);
				}
			}
			return paths;
		}
	}
}


