using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Controls;
using Enigma.Wpf;
using Enigma.D3.MapHack;
using Enigma.D3.MemoryModel;
using System.Diagnostics;
using Enigma.Memory;

namespace Enigma.D3.Bootloader
{
	internal class App : Application
	{
		[STAThread]
		public static void Main()
		{
			var app = new App();
			app.Run();
		}

		protected override void OnStartup(StartupEventArgs e)
		{
			MainWindow = Shell.Instance;
			MainWindow.Show();
			ShutdownMode = ShutdownMode.OnMainWindowClose;

			Task.Factory.StartNew(() =>
			{
				while (true)
				{
					using (var engine = CreateEngine())
					using (var watcher = new WatcherThread(engine))
					{
						Shell.Instance.IsAttached = true;
						Minimap minimap = null;
						OverlayWindow overlay = null;
						Execute.OnUIThread(() =>
						{
							Canvas canvas = new Canvas();
							overlay = OverlayWindow.Create((engine.Context.Memory as ProcessMemoryReader).Process, canvas);
							overlay.Show();
							minimap = new Minimap(canvas);
						});
						watcher.AddTask(minimap.Update);
						watcher.Start();
						(engine.Context.Memory as ProcessMemoryReader).Process.WaitForExit();
						Execute.OnUIThread(() => overlay.Close());
					}
					Shell.Instance.IsAttached = false;
					//Execute.OnUIThread(() => MainWindow.Close());
				}
			}, TaskCreationOptions.LongRunning);
		}

		private Engine CreateEngine()
		{
			while (true)
			{
				var process = Process.GetProcessesByName("Diablo III").FirstOrDefault();
				if (process != null)
				{
					var engine = new Engine(process);
					while (engine.Context.DataSegment.ApplicationLoopCount == 0)
						Thread.Sleep(1000);
					return engine;
				}
				Thread.Sleep(1000);
			}
		}
	}
}
