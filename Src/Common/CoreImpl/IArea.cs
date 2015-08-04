// Copyright (c) 2015 SIL International
// This software is licensed under the LGPL, version 2.1 or later
// (http://www.gnu.org/licenses/lgpl-2.1.html)

using System.Collections.Generic;
using System.Drawing;

namespace SIL.CoreImpl
{
	/// <summary>
	/// Interface for each Area in the main IFwMainWnd
	/// </summary>
	public interface IArea : IMajorFlexUiComponent
	{
		/// <summary>
		/// Get all installed tools for the area.
		/// </summary>
		List<ITool> AllToolsInOrder { get; }

		/// <summary>
		/// Get the image for the area.
		/// </summary>
		Image Icon { get; }
	}
}