using System;
using System.Timers;

namespace CopyApp
{
    class Copy
    {
        public DirectoryCpoier DC { get; set; }
        System.Threading.Thread trd;
        Timer T;

        public void Main()
        {
            trd = new System.Threading.Thread(Run);
            T = new Timer(60000);
            T.Elapsed += T_Elapsed;
            T.Start();
        }

        private void T_Elapsed(object sender, ElapsedEventArgs e)
        {
            DC = Config.Directory_Read();

            if (DC != null && !trd.IsAlive && trd.ThreadState == System.Threading.ThreadState.Stopped)
            {
                trd = new System.Threading.Thread(Run);
                trd.Start();
            }

            if (DC != null && !trd.IsAlive && trd.ThreadState == System.Threading.ThreadState.Unstarted)
            {
                trd.Start();
            }
        }

        public void Run()
        {
            DateTime dt = DateTime.Now;

            switch (DC.ScheduledCopyType.ToString())
            {
                case "Watching":
                    FileHash fh = new FileHash(DC);
                    fh.Main();
                    break;
                case "Hourly":
                    if (dt.Minute == DC.CopyTime.Minute)
                        DC.StartCopy();
                    break;
                case "Daily":
                    if (dt.Hour == DC.CopyTime.Hour && dt.Minute == DC.CopyTime.Minute)
                        DC.StartCopy();
                    break;
                case "Weekly":
                    if (dt.DayOfWeek == DC.DayOfWeek && dt.Hour == DC.CopyTime.Hour && dt.Minute == DC.CopyTime.Minute)
                        DC.StartCopy();
                    break;
                case "Monthly":
                    if (dt.Day == DC.DayOfMonth && dt.Hour == DC.CopyTime.Hour && dt.Minute == DC.CopyTime.Minute)
                        DC.StartCopy();
                    break;
            }
        }
    }
}