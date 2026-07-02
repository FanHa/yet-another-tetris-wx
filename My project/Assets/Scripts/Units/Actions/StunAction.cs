namespace Units.Actions
{
	public sealed class StunAction : UnitAction<IStatusActionContext>
	{
		public override int Priority => 100;

		public StunAction(IStatusActionContext context) : base(context, UnitActionType.Stun)
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
			Context.SuspendAutoMovement();
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
			Context.ResumeAutoMovement();
		}
	}
}
