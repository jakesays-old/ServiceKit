using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Std.ServicesKit;

namespace SampleService
{
	class CoolService : Service
	{
		private HttpListener _httpServer;
		private readonly ManualResetEvent _waiter = new ManualResetEvent(false);
		private bool _running = true;
		private readonly OptionSet _options;
		private readonly List<string> _prefixes;

		public CoolService()
		{
			_prefixes = new List<string>();

			_options = new OptionSet
			{
				{"prefix=", "Specify a prefix to listen on (ex: http://localhost:7777/cool/)", p => _prefixes.Add(p)}
			};
		}

		public override OptionSet Options
		{
			get { return _options; }
		}

		public override ServiceConfiguration Configuration
		{
			get
			{
				return new ServiceConfiguration
				{
					AccountType = ServiceAccountType.LocalSystem,
					Description = "Way cool service",
					DisplayName = "Cool Service",
					ServiceName = "CoolService"
				};
			}
		}

		public override void Start()
		{
			_httpServer = new HttpListener();
			foreach (var prefix in _prefixes)
			{
				_httpServer.Prefixes.Add(prefix);
			}

			_httpServer.Start();
			while (_running)
			{
				HttpListenerContext context;
				try
				{
					context = _httpServer.GetContext();
				}
				catch (Exception)
				{
					_running = false;
					break;
				}
				using (var response = context.Response)
				{
					var buffer = System.Text.Encoding.UTF8.GetBytes("<HTML><BODY>Cool service here!</BODY></HTML>");
					response.ContentLength64 = buffer.Length;
					response.ContentType = "text/html";
					using (var output = response.OutputStream)
					{
						output.Write(buffer, 0, buffer.Length);
					}
				}
			}

			if (_httpServer.IsListening)
			{
				_httpServer.Stop();
			}

			_waiter.Set();
		}

		public override void Stop()
		{
			if (!_running)
			{
				return;
			}
			_running = false;
			_httpServer.Stop();
			_waiter.WaitOne();
		}
	}
}