using System;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Views.InputMethods;
using AndroidX.AppCompat.Widget;

namespace Microsoft.Maui.Platform
{
	internal class MauiAppCompatEditText : AppCompatEditText
	{
		public event EventHandler? SelectionChanged;

		public MauiAppCompatEditText(Context context) : base(context)
		{
		}

		protected override void OnSelectionChanged(int selStart, int selEnd)
		{
			base.OnSelectionChanged(selStart, selEnd);

			SelectionChanged?.Invoke(this, EventArgs.Empty);
		}

		public override void SetOnEditorActionListener(IOnEditorActionListener? l)
		{
			base.SetOnEditorActionListener(l);
		}

		public override bool OnKeyDown([GeneratedEnum] Keycode keyCode, KeyEvent? e)
		{

			System.Diagnostics.Debug.WriteLine($"OnKeyDown 1");
			var result =  base.OnKeyDown(keyCode, e);
			System.Diagnostics.Debug.WriteLine($"OnKeyDown 2");

			//if (keyCode == Keycode.Enter && e?.Action == KeyEventActions.Down)
			//	return true;

			return false;
		}

		public override bool OnKeyUp([GeneratedEnum] Keycode keyCode, KeyEvent? e)
		{
			System.Diagnostics.Debug.WriteLine($"OnKeyUp 1");
			var result = base.OnKeyUp(keyCode, e);
			System.Diagnostics.Debug.WriteLine($"OnKeyUp 2");
			return false;
		}

		public override void OnEditorAction([GeneratedEnum] ImeAction actionCode)
		{
			System.Diagnostics.Debug.WriteLine($"OnEditorAction 1: {actionCode} 1");
			base.OnEditorAction(actionCode);
			System.Diagnostics.Debug.WriteLine($"OnEditorAction 2: {actionCode} 2");
		}
	}
}
