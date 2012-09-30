using System;
using System.Collections.Generic;
using System.iPhone.IoC;
using System.iPhone.UIKit;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using uController.API;
using uController.API.Search;
using uController.Controllers;
using uController.Models;
namespace uController
{
	public class Application
	{
		static void Main (string[] args)
		{	
			//IoC Registration
			var container = TinyIoCContainer.Current;
			//UI
			container.Register<BusyIndicator>().AsSingleton();
			//Models
			container.Register<Settings>().AsSingleton();
			container.Register<Server>().AsSingleton();
			container.RegisterMultiple<ISearchProvider>(new Type[] { typeof(PirateBayProvider) }).AsSingleton();
			
			AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
			{
				Console.WriteLine("Unhandled Error: " + Unwind(e.ExceptionObject as Exception));	
			};
			
			try
			{
				UIApplication.Main (args);
			}
			catch (Exception exc)
			{
				Console.WriteLine("Unhandled Error: " + Unwind(exc));	
			}
		}
		
		static Exception Unwind(Exception exc)
		{
			if (exc.InnerException == null)	
			{
				return exc;	
			}
			return Unwind(exc.InnerException);
		}
	}
}

