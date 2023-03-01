public class ActorChangedEventArgs : ActorEventArgs
{
	protected int _value;

	public int value
	{
		get
		{
			return _value;
		}
	}

	public ActorChangedEventArgs(ContextChanged c, int iValue)
	{
		_context = c;
		_value = iValue;
	}
}
