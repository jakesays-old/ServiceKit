using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;

namespace Std.ServicesKit
{
	public class ServiceProcessHost : ServiceBase
	{
		private static OptionSet _options;
		private static bool _install;
		private static bool _uninstall;

		private static Service _implementation;

        public ServiceProcessHost()
		{
		}

		public static void ServiceMain(Service implementation)
		{
			_implementation = implementation;
			_options = new OptionSet
			{
				{"install", "Install as a service (might need to run as admin.)", _ => _install = true},
				{"uninstall", "Uninstall service.", _ => _uninstall = true}
			};

			var extraArgs = _options.Parse();

			if (extraArgs.Count > 0 &&
				_install == false)
			{
				if (implementation.Options != null)
				{
					extraArgs = implementation.Options.Parse(extraArgs);
				}

				if (extraArgs.Count > 0)
				{
					if (!Environment.UserInteractive)
					{
						new ServiceProcessHost().EventLog.WriteEntry("Invalid parameters specified on service command line.");
					}
					else
					{
						Console.WriteLine(@"Usage:");
						_options.WriteOptionDescriptions(Console.Out);

						if (implementation.Options != null)
						{
							Console.WriteLine(@"Application specific options:");
							implementation.Options.WriteOptionDescriptions(Console.Out);
						}

						Environment.Exit(1);
					}
				}
			}

			if (!Environment.UserInteractive)
			{
				Run(new ServiceBase[] { new ServiceProcessHost() });
				return;
			}

			if (_install)
			{
				Console.WriteLine(@"Installing as a service");
				ServiceHostInstaller.InstallService(implementation.Configuration, extraArgs);

				return;
			}

			if (_uninstall)
			{
				Console.WriteLine(@"Uninstalling service");
				ServiceHostInstaller.UninstallService(implementation.Configuration);

				return;
			}

			RunInConsole();
		}

		private static ManualResetEvent _exitWaiter;

		private static void RunInConsole()
		{
			Console.WriteLine(@"*** Entering console mode ***");
			Console.CancelKeyPress += ControlCHandler;

			_exitWaiter = new ManualResetEvent(false);
			RunService();
			_exitWaiter.WaitOne();

			_implementation.Stop();

			Console.WriteLine(@"GoodbyE!");
		}

		private static bool _exiting = false;

		static void ControlCHandler(object sender, ConsoleCancelEventArgs e)
		{
			e.Cancel = true;

			Shutdown();
		}

		private static void RunService()
		{
			var thr = new Thread(_implementation.Start)
			{
				IsBackground = true
			};
			thr.Start();
		}

		private static void Shutdown()
		{
			if (_exiting)
			{
				return;
			}

			Console.WriteLine(@"*** Initiating shutdown ***");

			_implementation.Stop();
			_exiting = true;
			_exitWaiter?.Set();
		}

		protected override void OnStart(string[] args)
		{
			RunService();
		}

		protected override void OnStop()
		{
			_implementation.Stop();
		}
	}
}
