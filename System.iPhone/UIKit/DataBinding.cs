using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
namespace System.iPhone.UIKit
{
	public sealed class DataBinding
	{		
		public DataBinding ()
		{
			SourceGetProperty = () => Source;
		}
		
		public UIView View
		{
			get;
			set;
		}
		
		public Func<object> ViewGetProperty
		{
			get;
			set;
		}
		
		public Action<object> ViewSetProperty
		{
			get;
			set;
		}
		
		public object Source
		{
			get;
			set;
		}
		
		public Func<object> SourceGetProperty
		{
			get;
			set;
		}
		
		public Action<object> SourceSetProperty
		{
			get;
			set;
		}
		
		public DataBinding SetDefaultsFromView()
		{
			if (View != null)	
			{
				if (View is UISwitch)
				{
					var @switch = (UISwitch)View;
					
					ViewGetProperty = () => @switch.On;
					ViewSetProperty = value => @switch.On = (bool)value;
				}
				else if (View is UITextField)
				{
					var text = (UITextField)View;
					
					ViewGetProperty = () => text.Text;
					ViewSetProperty = value => text.Text = (string)value;
				}
				else if (View is UILabel)
				{
					var label = (UILabel)View;
					
					ViewGetProperty = () => label.Text;
					ViewSetProperty = value => label.Text = (string)value;
				}
				else if (View is UIActivityIndicatorView)
				{
					var indicator = (UIActivityIndicatorView)View;
					
					ViewGetProperty = () => indicator.Hidden;
					ViewSetProperty = value => indicator.Hidden = (bool)value;
				}
				else if (View is UITextView)
				{
					var text = (UITextView)View;
					
					ViewGetProperty = () => text.Text;
					ViewSetProperty = value => 
					{
						text.Text = (string)value;
						text.ScrollRangeToVisible(new NSRange(0, 0));
					};
				}
				else if (View is UIDatePicker)
				{
					var date = (UIDatePicker)View;
					
					ViewGetProperty = () => (DateTime)date.Date;
					ViewSetProperty = value => date.SetDate(value == null ? NSDate.Now : (NSDate)(DateTime)value, false);
				}
			}
			
			return this;
		}
		
		public DataBinding Bind()
		{
			SourceChanged();
			
			return this;
		}
		
		public void ViewChanged()
		{
			if (SourceSetProperty != null && ViewGetProperty != null)
			{
				SourceSetProperty(ViewGetProperty());
			}
		}
		
		public void SourceChanged()
		{
			if (ViewSetProperty != null && SourceGetProperty != null)
			{
				ViewSetProperty(SourceGetProperty());	
			}
		}
	}
}

