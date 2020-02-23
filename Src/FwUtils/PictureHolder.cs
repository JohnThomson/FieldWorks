// Copyright (c) 2011-2020 SIL International
// This software is licensed under the LGPL, version 2.1 or later
// (http://www.gnu.org/licenses/lgpl-2.1.html)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using SIL.FieldWorks.Common.ViewsInterfaces;
using SIL.LCModel.Utils;

namespace SIL.FieldWorks.Common.FwUtils
{
	/// <summary>
	/// PictureHolder is a holding place for pictures that are useful to various windows through the lifetime of the
	/// application, and which should be disposed only when the whole application shuts down.
	/// </summary>
	public class PictureHolder : IDisposable
	{
		private Dictionary<string, IPicture> m_previousPictures;

		/// <summary>
		/// Get the image stored at the specified path as an IPicture.
		/// The PictureHolder remains responsible to dispose or release the picture as needed,
		/// typically when the application exits, so the client may keep and use it indefinitely.
		/// </summary>
		public IPicture GetComPicture(string imagePath)
		{
			if (m_previousPictures == null)
			{
				m_previousPictures = new Dictionary<string, IPicture>();
			}
			if (m_previousPictures.TryGetValue(imagePath, out var comPicture))
			{
				return comPicture;
			}
			try
			{
				var actualFilePath = FileUtils.ActualFilePath(imagePath);
				using (var image = Image.FromFile(actualFilePath))
				{
					comPicture = (IPicture)OLEConvert.ToOLE_IPictureDisp(image);
				}
			}
			catch (Exception e)
			{
				Debug.WriteLine("Failed to create picture from path " + imagePath + " exception: " + e.Message);
				comPicture = null; // if we can't get the picture too bad.
			}
			m_previousPictures[imagePath] = comPicture;
			return comPicture;
		}

		/// <summary>
		/// Get an IPicture corresponding to the specified key. If one is not known,
		/// obtain it from the source image, and save it for next time. The PictureHolder
		/// remains responsible for disposing or releasing the picture in either case.
		/// </summary>
		public IPicture GetPicture(string key, Image source)
		{
			if (m_previousPictures == null)
			{
				m_previousPictures = new Dictionary<string, IPicture>();
			}
			if (!m_previousPictures.TryGetValue(key, out var comPicture))
			{
				comPicture = (IPicture)OLEConvert.ToOLE_IPictureDisp(source);
				m_previousPictures[key] = comPicture;
			}
			return comPicture;
		}

		/// <summary />
		~PictureHolder()
		{
			Dispose(false);
		}

		/// <summary/>
		private bool IsDisposed { get; set; }

		/// <inheritdoc />
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// As a special case, this class does not HAVE to be disposed if it does not allow pictures.
		/// </summary>
		protected virtual void Dispose(bool disposing)
		{
			Debug.WriteLineIf(!disposing, "****************** Missing Dispose() call for " + GetType().Name + ". ******************");
			if (IsDisposed)
			{
				// No need to run it more than once.
				return;
			}

			if (disposing)
			{
				if (m_previousPictures != null)
				{
					foreach (var picture in m_previousPictures.Select(kvp => kvp.Value))
					{
						ReleasePicture(picture);
					}
					m_previousPictures.Clear();
					m_previousPictures = null;
				}
			}
		}

		/// <summary>
		/// Release any data that might prevent deleting the specified picture file
		/// </summary>
		public void ReleasePicture(string key)
		{
			if (m_previousPictures == null || !m_previousPictures.TryGetValue(key, out var val))
			{
				return;
			}
			ReleasePicture(val);
			m_previousPictures.Remove(key);
		}

		private static void ReleasePicture(IPicture picture)
		{
			if (picture is IDisposable)
			{
				((IDisposable)picture).Dispose(); // typically Linux
			}
			else if (picture != null && Marshal.IsComObject(picture))
			{
				Marshal.ReleaseComObject(picture); // typically Windows
			}
		}
	}
}