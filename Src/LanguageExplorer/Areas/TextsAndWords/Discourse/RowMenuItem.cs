// Copyright (c) 2008-2020 SIL International
// This software is licensed under the LGPL, version 2.1 or later
// (http://www.gnu.org/licenses/lgpl-2.1.html)

using SIL.LCModel;

namespace LanguageExplorer.Areas.TextsAndWords.Discourse
{
	internal sealed class RowMenuItem
	{
		internal RowMenuItem(IConstChartRow row)
		{
			Row = row;
		}

		// Return the ChartRow's row label (1a, 1b, etc.) as a string
		public override string ToString()
		{
			return Row.Label.Text;
		}

		internal IConstChartRow Row { get; }
	}
}