// Copyright (c) 2011-2020 SIL International
// This software is licensed under the LGPL, version 2.1 or later
// (http://www.gnu.org/licenses/lgpl-2.1.html)

using System;
using System.Collections.Generic;
using LanguageExplorer.Areas.TextsAndWords.Interlinear;
using NUnit.Framework;
using SIL.LCModel;
using SIL.LCModel.Application;
using SIL.LCModel.Core.KernelInterfaces;
using SIL.LCModel.Core.Text;
using SIL.LCModel.DomainServices;

namespace LanguageExplorerTests.Areas.TextsAndWords.Interlinear
{
	/// <summary>
	/// Test a class which decorates StTxtPara.Contents to make zero-width spaces visible.
	/// </summary>
	[TestFixture]
	public class ShowSpaceDecoratorTests
	{
		private const string zws = AnalysisOccurrence.KstrZws;

		[Test]
		public void DecoratorDoesNothingWhenTurnedOff()
		{
			var mockDa = new MockDa();
			const string underlyingValue = "hello" + zws + "world";
			mockDa.StringValues[new Tuple<int, int>(27, StTxtParaTags.kflidContents)] = TsStringUtils.MakeString(underlyingValue, 77);
			var decorator = new ShowSpaceDecorator(mockDa);

			var tss = decorator.get_StringProp(27, StTxtParaTags.kflidContents);
			Assert.That(tss.Text, Is.EqualTo(underlyingValue));
			VerifyNoBackColor(tss);
		}

		[Test]
		public void DecoratorGetHandlesEmptyStringWhenTurnedOn()
		{
			var mockDa = new MockDa();
			const string underlyingValue = "";
			mockDa.StringValues[new Tuple<int, int>(27, StTxtParaTags.kflidContents)] = TsStringUtils.MakeString(underlyingValue, 77);
			var decorator = new ShowSpaceDecorator(mockDa);
			decorator.ShowSpaces = true;

			var tss = decorator.get_StringProp(27, StTxtParaTags.kflidContents);
			Assert.That(string.IsNullOrEmpty(tss.Text));
			VerifyNoBackColor(tss);
		}

		[Test]
		public void DecoratorReplacesZwsWithGreySpaceWhenTurnedOn()
		{
			var mockDa = new MockDa();
			const string underlyingValue = zws + "hello" + zws + "world" + zws + "today";
			mockDa.StringValues[new Tuple<int, int>(27, StTxtParaTags.kflidContents)] = TsStringUtils.MakeString(underlyingValue, 77);
			var decorator = new ShowSpaceDecorator(mockDa);
			decorator.ShowSpaces = true;

			var tss = decorator.get_StringProp(27, StTxtParaTags.kflidContents);
			Assert.That(tss.Text, Is.EqualTo(" hello world today"));
			VerifyBackColor(tss, new[] { ShowSpaceDecorator.KzwsBackColor, -1, ShowSpaceDecorator.KzwsBackColor, -1, ShowSpaceDecorator.KzwsBackColor, -1 });
		}

		[Test]
		public void DecoratorReplacesGreySpaceWithZwsWhenTurnedOn()
		{
			var mockDa = new MockDa();
			const string underlyingValue = "hello world today keep these spaces";
			var bldr = TsStringUtils.MakeString(underlyingValue, 77).GetBldr();
			bldr.SetIntPropValues("hello world".Length, "hello world".Length + 1, (int)FwTextPropType.ktptBackColor, (int)FwTextPropVar.ktpvDefault, ShowSpaceDecorator.KzwsBackColor);
			bldr.SetIntPropValues("hello".Length, "hello".Length + 1, (int)FwTextPropType.ktptBackColor, (int)FwTextPropVar.ktpvDefault, ShowSpaceDecorator.KzwsBackColor);
			var decorator = new ShowSpaceDecorator(mockDa);
			decorator.ShowSpaces = true;
			decorator.SetString(27, StTxtParaTags.kflidContents, bldr.GetString());
			var tss = mockDa.StringValues[new Tuple<int, int>(27, StTxtParaTags.kflidContents)];
			Assert.That(tss.Text, Is.EqualTo("hello" + zws + "world" + zws + "today keep these spaces"));
			VerifyNoBackColor(tss);
		}

		[Test]
		public void DecoratorSetHandlesEmptyStringWhenTurnedOn()
		{
			var mockDa = new MockDa();
			const string underlyingValue = "";
			var bldr = TsStringUtils.MakeString(underlyingValue, 77).GetBldr();
			var decorator = new ShowSpaceDecorator(mockDa);
			decorator.ShowSpaces = true;
			decorator.SetString(27, StTxtParaTags.kflidContents, bldr.GetString());
			var tss = mockDa.StringValues[new Tuple<int, int>(27, StTxtParaTags.kflidContents)];
			Assert.That(string.IsNullOrEmpty(tss.Text));
			VerifyNoBackColor(tss);
		}

		private static void VerifyNoBackColor(ITsString tss)
		{
			for (var irun = 0; irun < tss.RunCount; irun++)
			{
				int nVar;
				Assert.That(tss.get_Properties(irun).GetIntPropValues((int)FwTextPropType.ktptBackColor, out nVar), Is.EqualTo(-1));
				Assert.That(nVar, Is.EqualTo(-1));
			}
		}

		private static void VerifyBackColor(ITsString tss, int[] colors)
		{
			Assert.That(tss.RunCount, Is.EqualTo(colors.Length));
			for (var irun = 0; irun < tss.RunCount; irun++)
			{
				int nVar;
				Assert.That(tss.get_Properties(irun).GetIntPropValues((int)FwTextPropType.ktptBackColor, out nVar), Is.EqualTo(colors[irun]));
				Assert.That(nVar, colors[irun] == -1 ? Is.EqualTo(-1) : Is.EqualTo((int)FwTextPropVar.ktpvDefault));
			}
		}

		private sealed class MockDa : SilDataAccessManagedBase
		{
			public readonly Dictionary<Tuple<int, int>, ITsString> StringValues = new Dictionary<Tuple<int, int>, ITsString>();
			public override ITsString get_StringProp(int hvo, int tag)
			{
				return StringValues[new Tuple<int, int>(hvo, tag)];
			}

			public override void SetString(int hvo, int tag, ITsString tss)
			{
				StringValues[new Tuple<int, int>(hvo, tag)] = tss;
			}
		}
	}
}