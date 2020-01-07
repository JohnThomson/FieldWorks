// Copyright (c) 2005-2020 SIL International
// This software is licensed under the LGPL, version 2.1 or later
// (http://www.gnu.org/licenses/lgpl-2.1.html)

using System;
using System.Diagnostics;

namespace LanguageExplorer.Controls.XMLViews
{
	/// <summary>
	/// Class representing the information we know about an item in a target/source field combo
	/// </summary>
	internal class FieldComboItem : IDisposable
	{
		private string m_name; // string to show in menu

		internal FieldComboItem(string name, int icol, FieldReadWriter accessor)
		{
			m_name = name;
			ColumnIndex = icol;
			Accessor = accessor;
		}

		#region Disposable stuff

		/// <summary />
		~FieldComboItem()
		{
			Dispose(false);
		}

		/// <summary />
		private bool IsDisposed { get; set; }

		/// <inheritdoc />
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary />
		protected virtual void Dispose(bool disposing)
		{
			Debug.WriteLineIf(!disposing, "****** Missing Dispose() call for " + GetType() + " *******");
			if (IsDisposed)
			{
				// No need to run it more than once.
				return;
			}
			if (disposing)
			{
				// dispose managed and unmanaged objects
				(Accessor as IDisposable)?.Dispose();
			}
			IsDisposed = true;
		}
		#endregion

		public override string ToString()
		{
			return m_name;
		}

		/// <summary>
		/// Gets the column index to show preview.
		/// </summary>
		public int ColumnIndex { get; }

		/// <summary>
		/// Gets the thing that can read/write strings.
		/// </summary>
		public FieldReadWriter Accessor { get; }
	}
}