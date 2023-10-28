﻿using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using UITest.Core;

namespace UITest.Appium.NUnit
{
	//#if ANDROID
	//	[TestFixture(TestDevice.Android)]
	//#elif IOSUITEST
	//	[TestFixture(TestDevice.iOS)]
	//#elif MACUITEST
	//	[TestFixture(TestDevice.Mac)]
	//#elif WINTEST
	//	[TestFixture(TestDevice.Windows)]
	//#else
	//    [TestFixture(TestDevice.iOS)]
	//    [TestFixture(TestDevice.Mac)]
	//    [TestFixture(TestDevice.Windows)]
	//    [TestFixture(TestDevice.Android)]
	//#endif
	public abstract class UITestBase : UITestContextBase
	{
		public UITestBase(TestDevice testDevice, bool useBrowserStack)
			: base(testDevice, useBrowserStack)
		{
		}

		[SetUp]
		public void RecordTestSetup()
		{
			var name = TestContext.CurrentContext.Test.MethodName ?? TestContext.CurrentContext.Test.Name;
			TestContext.Progress.WriteLine($">>>>> {DateTime.Now} {name} Start");

			// For BrowserStack, InitialSetup is called for each test as BrowserStack expects the driver to be
			// recreated for each test, so each test has its own session
			if (_useBrowserStack)
			{
				InitialSetup(UITestContextSetupFixture.ServerContext);
			}
		}

		[TearDown]
		public void RecordTestTeardown()
		{
			if (_useBrowserStack)
			{
				TearDown();
			}

			var name = TestContext.CurrentContext.Test.MethodName ?? TestContext.CurrentContext.Test.Name;
			TestContext.Progress.WriteLine($">>>>> {DateTime.Now} {name} Stop");
		}

		protected virtual void FixtureSetup()
		{
			var name = TestContext.CurrentContext.Test.MethodName ?? TestContext.CurrentContext.Test.Name;
			TestContext.Progress.WriteLine($">>>>> {DateTime.Now} {nameof(FixtureSetup)} for {name}");
		}

		protected virtual void FixtureTeardown()
		{
			var name = TestContext.CurrentContext.Test.MethodName ?? TestContext.CurrentContext.Test.Name;
			TestContext.Progress.WriteLine($">>>>> {DateTime.Now} {nameof(FixtureTeardown)} for {name}");
		}

		[TearDown]
		public void UITestBaseTearDown()
		{
			// With BrowserStack, checking AppState isn't supported currently, producing the error below,
			// so skip this on BrowserStack
			// BrowserStack error: System.NotImplementedException : Unknown mobile command "queryAppState". Only shell,scrollBackTo,viewportScreenshot,deepLink,startLogsBroadcast,stopLogsBroadcast,acceptAlert,dismissAlert,batteryInfo,deviceInfo,changePermissions,getPermissions,performEditorAction,startScreenStreaming,stopScreenStreaming,getNotifications,listSms,type commands are supported
			if (!_useBrowserStack)
			{
				if (App.AppState == ApplicationState.Not_Running)
				{
					// Assert.Fail will immediately exit the test which is desirable as the app is not
					// running anymore so we don't want to log diagnostic data as there is nothing to collect from
					Reset();
					FixtureSetup();
					Assert.Fail("The app was expected to be running still, investigate as possible crash");
				}
			}

			var testOutcome = TestContext.CurrentContext.Result.Outcome;
			if (testOutcome == ResultState.Error ||
				testOutcome == ResultState.Failure)
			{
				SaveDiagnosticLogs("UITestBaseTearDown");
			}
		}

		[OneTimeSetUp]
		public void OneTimeSetup()
		{
			InitialSetup(UITestContextSetupFixture.ServerContext);
			try
			{
				//SaveDiagnosticLogs("BeforeFixtureSetup");
				FixtureSetup();
			}
			catch
			{
				SaveDiagnosticLogs("FixtureSetup");
				throw;
			}
		}

		[OneTimeTearDown]
		public void OneTimeTearDown()
		{
			var outcome = TestContext.CurrentContext.Result.Outcome;

			// We only care about setup failures as regular test failures will already do logging
			if (outcome.Status == ResultState.SetUpFailure.Status &&
				outcome.Site == ResultState.SetUpFailure.Site)
			{
				SaveDiagnosticLogs("OneTimeTearDown");
			}

			if (_useBrowserStack)
			{
				// FixtureTeardown normally navigates back to the home page, and needs an Appium connection
				// to do that. For BrowserStack, since the connection is recreated for each test, it needs to be
				// recreated for the teardown
				InitialSetup(UITestContextSetupFixture.ServerContext);
				FixtureTeardown();
				TearDown();
			}
			else
			{
				FixtureTeardown();
			}
		}

		void SaveDiagnosticLogs(string? note = null)
		{
			if (string.IsNullOrEmpty(note))
				note = "-";
			else
				note = $"-{note}-";

			var logDir = (Path.GetDirectoryName(Environment.GetEnvironmentVariable("APPIUM_LOG_FILE")) ?? Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))!;

			// App could be null if UITestContext was not able to connect to the test process (e.g. port already in use etc...)
			if (UITestContext is not null)
			{
				string name = TestContext.CurrentContext.Test.MethodName ?? TestContext.CurrentContext.Test.Name;

				var screenshotPath = Path.Combine(logDir, $"{name}-{_testDevice}{note}ScreenShot.png");
				_ = App.Screenshot(screenshotPath);
				TestContext.AddTestAttachment(screenshotPath, Path.GetFileName(screenshotPath));

				var pageSourcePath = Path.Combine(logDir, $"{name}-{_testDevice}{note}PageSource.txt");
				File.WriteAllText(pageSourcePath, App.ElementTree);
				TestContext.AddTestAttachment(pageSourcePath, Path.GetFileName(pageSourcePath));
			}
		}
	}
}