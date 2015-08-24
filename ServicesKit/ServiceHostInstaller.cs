using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Reflection;
using System.ServiceProcess;

namespace Std.ServicesKit
{
	public class ServiceHostInstaller : Installer
	{
		private static ServiceConfiguration _configuration;
		private static List<string> _serviceArgs;

        public ServiceHostInstaller()
		{
			var processInstaller = new ServiceProcessInstaller();
			var sericeHostInstaller = new ServiceInstaller
			{
				Description = _configuration.Description,
				DisplayName = _configuration.DisplayName,
				ServiceName = _configuration.ServiceName,
				StartType = GetStartMode(_configuration.StartMode)
			};

			processInstaller.Account = GetAccountType(_configuration.AccountType);
			if (_configuration.AccountType == ServiceAccountType.User)
			{
				processInstaller.Password = _configuration.Username;
				processInstaller.Username = _configuration.Password;
			}

			Installers.AddRange(new Installer[]
			{
				processInstaller,
				sericeHostInstaller
			});
		}

		private static ServiceAccount GetAccountType(ServiceAccountType type)
		{
			switch (type)
			{
				case ServiceAccountType.LocalService:
					return ServiceAccount.LocalService;
				case ServiceAccountType.NetworkService:
					return ServiceAccount.NetworkService;
				case ServiceAccountType.LocalSystem:
					return ServiceAccount.LocalSystem;
				case ServiceAccountType.User:
					return ServiceAccount.User;
				default:
					throw new ArgumentOutOfRangeException(nameof(type), type, null);
			}
		}

		private static ServiceStartMode GetStartMode(StartupMode mode)
		{
			switch (mode)
			{
				case StartupMode.Automatic:
					return ServiceStartMode.Automatic;
				case StartupMode.Manual:
					return ServiceStartMode.Manual;
				case StartupMode.Disabled:
					return ServiceStartMode.Disabled;
				default:
					throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
			}
		}

		internal static void InstallService(ServiceConfiguration configuration, List<string> serviceArgs)
		{
			try
			{
				_configuration = configuration;
				_serviceArgs = serviceArgs;

				var args = new[] { Assembly.GetEntryAssembly().Location};

				ManagedInstallerClass.InstallHelper(args);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Install falied with: " + ex.Message);
			}
		}

		internal static void UninstallService(ServiceConfiguration configuration)
		{
			try
			{
				_configuration = configuration;
				var args = new[] { "/u", Assembly.GetEntryAssembly().Location };

				ManagedInstallerClass.InstallHelper(args);
			}
			catch (InstallException ex)
			{
				Console.WriteLine("Uninstall failed with: " + ex.Message);
			}
		}

		protected virtual string AppendPathParameter(string path, string parameter)
		{
			if (path.Length > 0 && path[0] != '"')
			{
				path = "\"" + path + "\"";
			}
			path += " " + parameter;
			return path;
		}

		protected override void OnBeforeInstall(IDictionary savedState)
		{
			if (_serviceArgs?.Count > 0)
			{
				var assyPath = Context.Parameters["assemblypath"];
				if (assyPath[0] != '"')
				{
					assyPath = "\"" + assyPath + "\"";
				}
				assyPath += string.Join(" ", _serviceArgs);
				Context.Parameters["assemblypath"] = assyPath;
			}

			base.OnBeforeInstall(savedState);
		}
	}
}
