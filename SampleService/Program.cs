using System;
using System.Collections.Generic;
using System.Text;
using Std.ServicesKit;

namespace SampleService
{
	static class Program
	{
		static void Main(string[] args)
		{
			ServiceProcessHost.ServiceMain(new CoolService());
		}
	}
}
