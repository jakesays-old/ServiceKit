using System;

namespace Std.ServicesKit
{
	public abstract class Service
	{
		public abstract ServiceConfiguration Configuration { get; }

		public virtual OptionSet Options
		{
			get { return null; }
		}

		public abstract void Start();

		public abstract void Stop();
	}
}