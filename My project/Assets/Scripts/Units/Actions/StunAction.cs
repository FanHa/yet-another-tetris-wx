namespace Units.Actions
{
	public sealed class StunAction : UnitAction
	{
		public override int Priority => 100;

		public StunAction(Unit owner) : base(owner, UnitActionType.Stun)
		{
		}

		public override bool CanStart()
		{
			return Owner.IsStunned;
		}

		public override bool CanPreempt(UnitAction currentAction)
		{
			return currentAction != null && currentAction.Type != UnitActionType.Stun;
		}

		protected override void OnEnter()
		{
			Owner.PauseNavigation();
		}

		protected override void OnTick(global::Units.Actions.ActionTickContext context)
		{
			if (!Owner.IsStunned)
			{
				Complete();
			}
		}

		protected override void OnExit()
		{
			Owner.ResumeNavigation();
		}
	}
}
