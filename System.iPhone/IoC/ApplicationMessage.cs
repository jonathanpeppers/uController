using System;
namespace System.iPhone.IoC
{
	public enum ApplicationMessageType
	{
		Loaded,
		Backgrounded,
		Foregrounded,
	}

	public class ApplicationMessage : GenericTinyMessage<ApplicationMessageType>
	{
		public ApplicationMessage (object sender, ApplicationMessageType content)
			: base(sender, content)
		{ }
	}
}

