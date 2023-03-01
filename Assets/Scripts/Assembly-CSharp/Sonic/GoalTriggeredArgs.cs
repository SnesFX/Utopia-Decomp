namespace Sonic
{
	public class GoalTriggeredArgs : ActorEventArgs
	{
		private Pawn _pawn;

		public Pawn pawn
		{
			get
			{
				return _pawn;
			}
		}

		public GoalTriggeredArgs(Context c, Pawn oPawn)
		{
			_context = c;
			_pawn = oPawn;
		}
	}
}
