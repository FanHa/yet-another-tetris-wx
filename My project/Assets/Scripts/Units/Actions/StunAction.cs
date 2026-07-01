namespace Units.Actions
{
	public sealed class StunAction : UnitAction
	{
		public override int Priority => 100;

		public StunAction(IUnitActionContext context) : base(context, UnitActionType.Stun)
		{
		}

		public override bool CanStart()
		{
			return Context.IsStunned;
		}

		public override bool CanPreempt(UnitAction currentAction)
		{
			return currentAction != null && currentAction.Type != UnitActionType.Stun;
		}

		protected override void OnEnter()
		{
			Context.PauseNavigation();
		}

		protected override void OnTick(global::Units.Actions.ActionTickContext context)
		{
			if (!Context.IsStunned)
			{
				Complete();
			}
		}

		protected override void OnExit()
		{
			Context.ResumeNavigation();
		}
	}
}
