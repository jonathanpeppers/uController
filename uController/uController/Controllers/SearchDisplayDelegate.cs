using System;
using System.iPhone.IoC;
using System.iPhone.UIKit;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using uController.Models;
namespace uController.Controllers
{
	[Register("SearchDisplayDelegate")]
	public class SearchDisplayDelegate : UISearchDisplayDelegate
	{
		private ITinyMessengerHub _hub = null;
		private UISearchDisplayController _controller = null;
		private string _searchText = null;
		private int _searchScope = 0;

		#region Constructors
	
		public SearchDisplayDelegate (IntPtr handle) : base (handle)
		{
			Initialize();
		}
	
		[Export("initWithCoder:")]
		public SearchDisplayDelegate (NSCoder coder) : base(coder)
		{
			Initialize();
		}
	
		protected virtual void Initialize()
		{
			_hub = TinyIoCContainer.Current.Resolve<ITinyMessengerHub>();
		}

		#endregion

		public override void WillBeginSearch (UISearchDisplayController controller)
		{
			if (_controller == null)
			{
				//Has to be stored in a member variable, GC collects and breaks something
				_controller = controller;
				_controller.SearchBar.SearchButtonClicked += (sender, e) =>
				{
					_hub.Publish(new SearchMessage(this, _searchText, _searchScope));
				};
			}
		}

		public override bool ShouldReloadForSearchString (UISearchDisplayController controller, string forSearchString)
		{
			if (_searchText != forSearchString)
			{
				_searchText = forSearchString;
				
				//Send message to wipe out TableView
				_hub.Publish(new SearchMessage(this, null, 0));
			}
			return false;
		}

		public override bool ShouldReloadForSearchScope (UISearchDisplayController controller, int forSearchOption)
		{
			_searchScope = forSearchOption;
			return false;
		}
	}
}

