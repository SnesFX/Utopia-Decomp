using System;

public class ActorEventArgs : EventArgs
{
	protected Context _context;

	public Context context
	{
		get
		{
			return _context;
		}
	}
}
