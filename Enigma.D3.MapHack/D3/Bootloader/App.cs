﻿using Enigma.D3.MapHack;
using Enigma.Wpf;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

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
            MainWindow = MitmeoShell.Instance;
            MainWindow.Show();
            ShutdownMode = ShutdownMode.OnMainWindowClose;
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    using (var engine = CreateEngine())
                    using (var watcher = new WatcherThread(engine))
                    {
                        MitmeoShell.Instance.IsAttached = true;
                        //Minimap minimap = null;
                        OverlayWindow overlay = null;
                        Execute.OnUIThread(() =>
                        {
                            MitmeoShell.Instance.Init(engine);

                            //Canvas canvas = new Canvas();
                            //overlay = OverlayWindow.Create(engine.Process, canvas);
                            //overlay.Show();
                            //minimap = new Minimap(canvas);
                        });
                        //watcher.AddTask(minimap.Update);
                        watcher.Start();
                        engine.Process.WaitForExit();
                        //Execute.OnUIThread(() => overlay.Close());
                    }
                    MitmeoShell.Instance.IsAttached = false;
                    //Execute.OnUIThread(() => MainWindow.Close());
                }
            }, TaskCreationOptions.LongRunning);
        }

        private Engine CreateEngine()
        {
            Engine engine = Engine.Create();
            while (engine == null)
            {
                Thread.Sleep(1000);
                engine = Engine.Create();
            }
            while (engine.ApplicationLoopCount == 0)
            {
                Thread.Sleep(1000);
            }
            return engine;
        }
    }
}
