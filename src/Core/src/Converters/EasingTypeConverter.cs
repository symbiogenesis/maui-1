using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using static Microsoft.Maui.Easing;

#nullable disable

namespace Microsoft.Maui.Converters
{
	/// <inheritdoc/>
	public class EasingTypeConverter : TypeConverter
	{
		private static readonly Dictionary<string, Easing> StringToEasingMap = new(StringComparer.OrdinalIgnoreCase)
		{
			{ nameof(Linear), Linear },
			{ nameof(SinIn), SinIn },
			{ nameof(SinOut), SinOut },
			{ nameof(SinInOut), SinInOut },
			{ nameof(CubicIn), CubicIn },
			{ nameof(CubicOut), CubicOut },
			{ nameof(CubicInOut), CubicInOut },
			{ nameof(BounceIn), BounceIn },
			{ nameof(BounceOut), BounceOut },
			{ nameof(SpringIn), SpringIn },
			{ nameof(SpringOut), SpringOut }
		};

		private static readonly Dictionary<Easing, string> EasingToStringMap = StringToEasingMap.ToDictionary(pair => pair.Value, pair => pair.Key);

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
			=> sourceType == typeof(string);

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
			=> destinationType == typeof(string);

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			var strValue = value?.ToString();

			if (string.IsNullOrWhiteSpace(strValue))
				return null;

			strValue = strValue?.Trim() ?? "";
			var parts = strValue.Split('.');

			if (parts.Length == 2 && parts[0] == nameof(Easing))
				strValue = parts[parts.Length - 1];

			if (StringToEasingMap.TryGetValue(strValue, out var easing))
				return easing;

			var fallbackValue = typeof(Easing)
				.GetTypeInfo()
				.DeclaredFields
				.FirstOrDefault(f => f.Name.Equals(strValue, StringComparison.OrdinalIgnoreCase))
				?.GetValue(null);

			if (fallbackValue is Easing fallbackEasing)
				return fallbackEasing;

			throw new InvalidOperationException($"Cannot convert \"{strValue}\" into {typeof(Easing)}");
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (value is not Easing easing)
				throw new NotSupportedException();

			if (EasingToStringMap.TryGetValue(easing, out var strValue))
				return strValue;

			throw new NotSupportedException();
		}

		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
			=> true;

		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
			=> false;

		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
			=> new(StringToEasingMap.Keys);
	}
}