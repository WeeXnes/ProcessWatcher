using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows;

namespace ProcessWatcher
{
    public class ProcessWatcher
    {
        public EventHandler ProcessStarted { get; set; }
        public EventHandler ProcessStopped { get; set; }
        public string ProcessName { get; set; }
        private BackgroundWorker BackgroundThread { get; set; }
        public int ProcessCheckDelay { get; set; }
        private bool isRunning { get; set; }
        public ProcessWatcher(string processName)
        {
            this.ProcessName = processName;
            this.ProcessCheckDelay = 3000;
            this.isRunning = false;
            this.BackgroundThread = new BackgroundWorker();
            BackgroundThread.WorkerSupportsCancellation = true;
            BackgroundThread.DoWork += BackgroundThreadOnDoWork;
            BackgroundThread.RunWorkerAsync();
        }

        private void m_processStarted()
        {
            this.isRunning = true;
            this.ProcessStarted.Invoke(this, EventArgs.Empty);
        }
        private void m_processStopped()
        {
            this.isRunning = false;
            this.ProcessStopped.Invoke(this, EventArgs.Empty);
        }

        private void BackgroundThreadOnDoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                bool procFound = false;
                Process[] processes = Process.GetProcesses();
                foreach (var process in processes)
                {
                    if (process.ProcessName == this.ProcessName)
                        procFound = true;
                }

                if (this.isRunning)
                {
                    if (!procFound)
                    {
                        m_processStopped();
                    }
                }
                else
                {
                    if (procFound)
                    {
                        m_processStarted();
                    }
                }

                Thread.Sleep(this.ProcessCheckDelay);
            }
        }
    }
}