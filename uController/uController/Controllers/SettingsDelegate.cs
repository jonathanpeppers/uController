using System;
using System.iPhone.IoC;
using System.iPhone.UIKit;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using uController.Models;
namespace uController.Controllers
{
	[Register("SettingsDelegate")]
	public class SettingsDelegate : UITableViewSource
	{
		private Settings _settings = null;
		private TextFieldCellController[] _cells = null;
		private UITableViewCell _aboutCell = null;

		#region Constructors
	
		public SettingsDelegate (IntPtr handle) : base (handle)
		{
			Initialize();
		}
	
		[Export("initWithCoder:")]
		public SettingsDelegate (NSCoder coder) : base(coder)
		{
			Initialize();
		}
	
		protected virtual void Initialize()
		{
			_settings = TinyIoCContainer.Current.Resolve<Settings>();
			_settings.Synchronized += (sender, e) => 
			{
				foreach (TextFieldCellController cell in _cells)
				{
					cell.Binding.SourceChanged();
				}
			};

			var url = new TextFieldCellController
			{
				Binding = new DataBinding
				{
					Source = _settings,
					SourceGetProperty = () => _settings.Url,
					SourceSetProperty = value => _settings.Url = (string)value,
				}
			};
			url.Label.Text = "uTorrent URL";
			url.TextField.KeyboardType = UIKeyboardType.Url;
			url.TextField.AutocorrectionType = UITextAutocorrectionType.No;
			url.TextField.AutocapitalizationType = UITextAutocapitalizationType.None;

			var port = new TextFieldCellController
			{
				Binding = new DataBinding
				{
					Source = _settings,
					SourceGetProperty = () => _settings.Port.ToString(),
					SourceSetProperty = value => _settings.Port = ((string)value).ToInt(),
				}
			};
			port.Label.Text = "uTorrent Port";
			port.TextField.KeyboardType = UIKeyboardType.NumberPad;

			var username = new TextFieldCellController
			{
				Binding = new DataBinding
				{
					Source = _settings,
					SourceGetProperty = () => _settings.Username,
					SourceSetProperty = value => _settings.Username = (string)value,
				}
			};
			username.Label.Text = "Username";
			username.TextField.AutocorrectionType = UITextAutocorrectionType.No;
			username.TextField.AutocapitalizationType = UITextAutocapitalizationType.None;

			var password = new TextFieldCellController
			{
				Binding = new DataBinding
				{
					Source = _settings,
					SourceGetProperty = () => _settings.Password,
					SourceSetProperty = value => _settings.Password = (string)value,
				}
			};
			password.Label.Text = "Password";
			password.TextField.SecureTextEntry = true;
		
			_cells = new TextFieldCellController[]
			{
				url,
				port,
				username,
				password,
			};
			
			_aboutCell = new UITableViewCell();
			_aboutCell.TextLabel.Text = "About uController";
			_aboutCell.SelectionStyle = UITableViewCellSelectionStyle.Gray;
			_aboutCell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
		}
	
		#endregion
		
		[Outlet]
		private UINavigationController _navigationController
		{
			get; set;
		}
		
		[Outlet]
		private AboutController _aboutController
		{
			get; set;
		}
		
		public override string TitleForHeader (UITableView tableView, int section)
		{
			if (section == 0)
			{
				return "Settings";	
			}
			return "Credits";
		}
		
		public override int NumberOfSections (UITableView tableView)
		{
			return 2;
		}
		
		public override int RowsInSection (UITableView tableView, int section)
		{
			if (section == 0)
			{
				return _cells.Length;
			}
			return 1;
		}
		
		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			if (indexPath.Section == 0)
			{
				return _cells[indexPath.Row].Bind().Cell;
			}
			return _aboutCell;
		}
		
		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			if (indexPath.Section == 1 && indexPath.Row == 0)
			{
				_navigationController.PushViewController(_aboutController, true);
			}
		}
	}
}

