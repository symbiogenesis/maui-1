using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Handlers;
using UIKit;
using Xunit;

namespace Microsoft.Maui.DeviceTests
{
	public partial class LabelTests
	{

		[Fact(DisplayName = "Using TailTruncation LineBreakMode changes MaxLines")]
		public async Task UsingTailTruncationSetMaxLines()
		{
			var label = new Label()
			{
				Text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit",
				LineBreakMode = LineBreakMode.TailTruncation,
			};

			var handler = await CreateHandlerAsync<LabelHandler>(label);

			var platformLabel = GetPlatformLabel(handler);

			await InvokeOnMainThreadAsync((System.Action)(() =>
			{
				Assert.Equal(1, GetPlatformMaxLines(handler));
				Assert.Equal(LineBreakMode.TailTruncation.ToPlatform(), GetPlatformLineBreakMode(handler));
			}));
		}
				
		UILabel GetPlatformLabel(LabelHandler labelHandler) =>
			(UILabel)labelHandler.PlatformView;

		UILineBreakMode GetPlatformLineBreakMode(LabelHandler labelHandler) =>
			GetPlatformLabel(labelHandler).LineBreakMode;

		int GetPlatformMaxLines(LabelHandler labelHandler) =>
 			(int)GetPlatformLabel(labelHandler).Lines;
	}
}
