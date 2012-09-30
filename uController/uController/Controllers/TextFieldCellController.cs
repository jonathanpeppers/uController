using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.iPhone.UIKit;
namespace uController.Controllers
{
	[Register("TextFieldCellController")]
	public class TextFieldCellController : UIViewController
	{
		#region Constructors

		// The IntPtr and initWithCoder constructors are required for controllers that need 
		// to be able to be created from a xib rather than from managed code

		public TextFieldCellController (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		[Export("initWithCoder:")]
		public TextFieldCellController (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		public TextFieldCellController () : base("TextFieldCell", null)
		{
			Initialize ();
		}

		void Initialize ()
		{
			NSBundle.MainBundle.LoadNib("TextFieldCell", this, null);

			_textField.EditingChanged += (sender, e) =>
			{
				if (Binding != null)
				{
					Binding.ViewChanged();
				}
			};
			_textField.ShouldReturn = t => 
			{
				t.ResignFirstResponder();
				return true;
			};
		}
		
		#endregion
		
		[Outlet]
		private UITableViewCell _cell
		{
			get; set;
		}

		[Outlet]
		private UILabel _label
		{
			get; set;
		}

		[Outlet]
		private UITextField _textField
		{
			get; set;
		}
		
		public DataBinding Binding
		{
			get;
			set;
		}
		
		public UITableViewCell Cell
		{
			get { return _cell; }
		}

		public UILabel Label
		{
			get { return _label; }
		}

		public UITextField TextField
		{
			get { return _textField; }
		}
		
		public TextFieldCellController Bind()
		{
			if (Binding != null && Binding.View != _textField)
			{
				Binding.View = _textField;
				Binding.SetDefaultsFromView().Bind();
			}
			return this;
		}
	}

}

