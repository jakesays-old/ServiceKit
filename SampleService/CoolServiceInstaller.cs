using System.ComponentModel;
using Std.ServicesKit;

namespace SampleService
{
	/// <summary>
	/// This class is required to support /install and /uninstall.
	/// All that is required is for the RunInstaller attribute to be present.
	/// Everything else is provided by ServiceHostInstaller
	/// </summary>
	[RunInstaller(true)]
	public class CoolServiceInstaller : ServiceHostInstaller
	{
	}
}