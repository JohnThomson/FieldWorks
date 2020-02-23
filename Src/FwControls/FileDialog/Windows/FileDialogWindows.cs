// Copyright (c) 2011-2020 SIL International
// This software is licensed under the LGPL, version 2.1 or later
// (http://www.gnu.org/licenses/lgpl-2.1.html)

using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace SIL.FieldWorks.Common.Controls.FileDialog.Windows
{
	internal abstract class FileDialogWindows : IFileDialog, IDisposable
	{
		protected System.Windows.Forms.FileDialog m_dlg;

		#region IFileDialog implementation
		public event EventHandler Disposed
		{
			add => m_dlg.Disposed += value;
			remove => m_dlg.Disposed -= value;
		}

		public event CancelEventHandler FileOk
		{
			add => m_dlg.FileOk += value;
			remove => m_dlg.FileOk -= value;
		}

		public event EventHandler HelpRequest
		{
			add => m_dlg.HelpRequest += value;
			remove => m_dlg.HelpRequest -= value;
		}

		public void Reset()
		{
			m_dlg.Reset();
		}

		public DialogResult ShowDialog()
		{
			return m_dlg.ShowDialog();
		}

		public DialogResult ShowDialog(IWin32Window owner)
		{
			return m_dlg.ShowDialog(owner);
		}

		public bool AddExtension
		{
			get => m_dlg.AddExtension;
			set => m_dlg.AddExtension = value;
		}

		public bool CheckFileExists
		{
			get => m_dlg.CheckFileExists;
			set => m_dlg.CheckFileExists = value;
		}

		public bool CheckPathExists
		{
			get => m_dlg.CheckPathExists;
			set => m_dlg.CheckPathExists = value;
		}

		public string DefaultExt
		{
			get => m_dlg.DefaultExt;
			set => m_dlg.DefaultExt = value;
		}

		public string FileName
		{
			get => m_dlg.FileName;
			set => m_dlg.FileName = value;
		}

		public string[] FileNames => m_dlg.FileNames;

		public string Filter
		{
			get => m_dlg.Filter;
			set => m_dlg.Filter = value;
		}

		public int FilterIndex
		{
			get => m_dlg.FilterIndex;
			set => m_dlg.FilterIndex = value;
		}

		public string InitialDirectory
		{
			get => m_dlg.InitialDirectory;
			set => m_dlg.InitialDirectory = value;
		}

		public bool RestoreDirectory
		{
			get => m_dlg.RestoreDirectory;
			set => m_dlg.RestoreDirectory = value;
		}

		public bool ShowHelp
		{
			get => m_dlg.ShowHelp;
			set => m_dlg.ShowHelp = value;
		}

		public bool SupportMultiDottedExtensions
		{
			get => m_dlg.SupportMultiDottedExtensions;
			set => m_dlg.SupportMultiDottedExtensions = value;
		}

		public string Title
		{
			get => m_dlg.Title;
			set => m_dlg.Title = value;
		}

		public bool ValidateNames
		{
			get => m_dlg.ValidateNames;
			set => m_dlg.ValidateNames = value;
		}
		#endregion

		#region Disposable stuff

		/// <summary />
		~FileDialogWindows()
		{
			Dispose(false);
		}

		/// <inheritdoc />
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary/>
		protected virtual void Dispose(bool disposing)
		{
			System.Diagnostics.Debug.WriteLineIf(!disposing, "****** Missing Dispose() call for " + GetType() + ". *******");

			if (disposing)
			{
				m_dlg?.Dispose();
			}
			m_dlg = null;
		}
		#endregion
	}
}