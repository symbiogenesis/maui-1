using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace Maui.Controls.Sample
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
			//entry.Completed += Entry_Completed;
		}

		int i = 0;
		void Entry_Completed(object? sender, EventArgs e)
		{
			System.Diagnostics.Debug.WriteLine("DONE");
			i++;
			if (i > 10)
			{

			}
		}
	}
}