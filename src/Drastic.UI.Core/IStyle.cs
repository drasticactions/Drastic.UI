using System;

namespace Drastic.UI
{
	interface IStyle
	{
		Type TargetType { get; }

		void Apply(BindableObject bindable);
		void UnApply(BindableObject bindable);
	}
}