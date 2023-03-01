public class ActorDeathEventArgs : ActorEventArgs
{
	protected DeathType _deathType;

	public DeathType deathType
	{
		get
		{
			return _deathType;
		}
	}

	public ActorDeathEventArgs(ContextDeath c, DeathType oType)
	{
		_context = c;
		_deathType = oType;
	}
}
