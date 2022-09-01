using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.ServiceProcess;
using System.Text;
using System.Threading;

namespace FileSystemWatcherService
{
    public partial class Service1 : ServiceBase
    {
        private static String messageBodyTotal;
        private static bool isChanged;
        private static Timer timer;
        private static MailMessage mailMessage = new MailMessage();

        public Service1()
        {
            InitializeComponent();
        }
        public static void Time()
        {
            Service1 ex = new Service1();
            ex.StartTimer(10000);

        }
        public void StartTimer(int dueTime)
        {
            Timer t = new Timer(new TimerCallback(myCallBack));
            t.Change(dueTime, 0);
        }
        private static void myCallBack(object state)
        {
            Timer t = (Timer)state;
            t.Dispose();
            SendMail();
        }
        protected override void OnStart(string[] args)
        {
            var watcher = new FileSystemWatcher(@"C:\a");

            watcher.NotifyFilter = NotifyFilters.Attributes
                                 | NotifyFilters.CreationTime
                                 | NotifyFilters.DirectoryName
                                 | NotifyFilters.FileName
                                 | NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.Security
                                 | NotifyFilters.Size;

            watcher.Changed += OnChanged;
            watcher.Created += OnCreated;
            watcher.Deleted += OnDeleted;
            watcher.Renamed += OnRenamed;
            watcher.Error += OnError;

            watcher.Filter = "*.txt";
            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;
        }

        protected override void OnStop()
        {
        }
        private static void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
            {
                return;
            }

            if (messageBodyTotal == null)
            {
                messageBodyTotal = $"Changed: {e.FullPath}";
            }
            else
            {
                messageBodyTotal = messageBodyTotal + " and " + $"Changed: {e.FullPath}";
            }
            Console.WriteLine($"Changed: {e.FullPath}");
            isChanged = true;
            Time();
        }

        private static void OnCreated(object sender, FileSystemEventArgs e)
        {
            if (messageBodyTotal == null)
            {
                messageBodyTotal = $"Created: {e.FullPath}";

            }
            else
            {
                messageBodyTotal = messageBodyTotal + " and " + $"Created: {e.FullPath}";
            }
            string value = $"Created: {e.FullPath}";
            Console.WriteLine(value);
            isChanged = true;
            Time();
        }

        private static void OnDeleted(object sender, FileSystemEventArgs e)
        {
            if (messageBodyTotal == null)
            {
                messageBodyTotal = $"Deleted: {e.FullPath}";

            }
            else
            {
                messageBodyTotal = messageBodyTotal + " and " + $"Deleted: {e.FullPath}";
            }
            Console.WriteLine($"Deleted: {e.FullPath}");
            isChanged = true;
            Time();
        }

        private static void OnRenamed(object sender, RenamedEventArgs e)
        {

            if (messageBodyTotal == null)
            {
                messageBodyTotal = $"Renamed: Old Path is {e.OldFullPath}, New Path is {e.FullPath}";

            }
            else
            {
                messageBodyTotal = messageBodyTotal + " and " + $"Renamed: Old Path is {e.OldFullPath}, New Path is {e.FullPath}";
            }
            Console.WriteLine($"Renamed:");
            Console.WriteLine($"    Old: {e.OldFullPath}");
            Console.WriteLine($"    New: {e.FullPath}");
            isChanged = true;
            Time();
        }

        private static void OnError(object sender, ErrorEventArgs e) =>
            PrintException(e.GetException());

        private static void PrintException(Exception ex)
        {
            if (ex != null)
            {
                Console.WriteLine($"Message: {ex.Message}");
                Console.WriteLine("Stacktrace:");
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine();
                PrintException(ex.InnerException);
            }
        }
        public static bool SendMail()
        {
            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Host = "smtp.gmail.com";
            smtpClient.Port = 587;
            smtpClient.EnableSsl = true;
            smtpClient.Credentials = new NetworkCredential("teststaj1@gmail.com", "nmtjhfahswbeblqu");

            mailMessage.From = new MailAddress("teststaj1@gmail.com");

            mailMessage.To.Add("teststaj1@gmail.com");

            mailMessage.Subject = "WINDOWS SERVICE from CBA";
            mailMessage.Body = messageBodyTotal;

            try
            {
                smtpClient.Send(mailMessage);
                messageBodyTotal = null;
                Console.WriteLine("Mail Sent!");
                isChanged = false;
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}

