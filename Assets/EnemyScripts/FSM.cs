using System;
using System.Linq;
using System.Collections.Generic;

public delegate bool FSMCondition();

public delegate void FSMAction();

public class FSMTransition
{

	public FSMCondition myCondition;

	private List<FSMAction> myActions = new List<FSMAction>();

	public FSMTransition(FSMCondition condition, FSMAction[] actions = null)
	{
		myCondition = condition;
		if (actions != null) myActions.AddRange(actions);
	}

	public void Fire()
	{
		foreach (FSMAction action in myActions) action();
	}
}

public class FSMState
{

	public List<FSMAction> enterActions = new List<FSMAction>();
	public List<FSMAction> stayActions = new List<FSMAction>();
	public List<FSMAction> exitActions = new List<FSMAction>();

	private Dictionary<FSMTransition, FSMState> links;

	public FSMState()
	{
		links = new Dictionary<FSMTransition, FSMState>();
	}

	public void AddTransition(FSMTransition transition, FSMState target)
	{
		links[transition] = target;
	}

	public FSMTransition VerifyTransitions()
	{
		foreach (FSMTransition t in links.Keys)
		{
			if (t.myCondition()) return t;
		}
		return null;
	}

	public FSMState NextState(FSMTransition t)
	{
		return links[t];
	}

	public void Enter() { foreach (FSMAction a in enterActions) a(); }
	public void Stay() { foreach (FSMAction a in stayActions) a(); }
	public void Exit() { foreach (FSMAction a in exitActions) a(); }

}

public class FSM
{

	public FSMState current;

	public FSM(FSMState state)
	{
		current = state;
		current.Enter();
	}


	public void Update()
	{ 
		FSMTransition transition = current.VerifyTransitions();
		if (transition != null)
		{
			current.Exit();     
			transition.Fire(); 
			current = current.NextState(transition);   
			current.Enter(); 
		}
		else
		{
			current.Stay();   
		}
	}
}