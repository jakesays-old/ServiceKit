namespace Std.ServicesKit
{
	public class ServiceConfiguration
	{
		public string Description { get; set; }
		public string DisplayName { get; set; }
		public string ServiceName { get; set; }
		public StartupMode StartMode { get; set; }
		public ServiceAccountType AccountType { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
	}
}