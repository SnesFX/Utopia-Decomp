public class ActorTickEventArgs : ActorEventArgs
{
	protected float _deltaTime;

	public float deltaTime
	{
		get
		{
			return _deltaTime;
		}
	}

	public ActorTickEventArgs(ContextTick c, float fDeltaTime)
	{
		_context = c;
		_deltaTime = fDeltaTime;
	}
}
