using System;
using uController.API;
namespace uController.Models
{
	public class Server : TorrentServer
	{
		private readonly Settings _settings;
	
		public Server (Settings settings)
		{
			_settings = settings;
		
			Timeout = 30000;
			PollInterval = 10000;
		}
		
		public override string Url 
		{
			get { return _settings.Url;	}
			set { _settings.Url = value; }
		}

		public override int Port 
		{
			get { return _settings.Port;	}
			set { _settings.Port = value; }
		}

		public override string Username 
		{
			get { return _settings.Username;	}
			set { _settings.Username = value; }
		}

		public override string Password 
		{
			get { return _settings.Password;	}
			set { _settings.Password = value; }
		}
	}
}

