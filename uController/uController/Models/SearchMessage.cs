using System;
using System.iPhone.IoC;
namespace uController.Models
{
	public class SearchMessage : TinyMessageBase
	{
		public string SearchText
		{
			get;
			private set;
		}

		public int SearchScope
		{
			get;
			private set;
		}

		public SearchMessage (object sender, string searchText, int searchScope)
			: base (sender)
		{
			SearchText = searchText;
			SearchScope = searchScope;
		}
	}
}

