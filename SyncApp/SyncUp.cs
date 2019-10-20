using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Collections;

public class SyncUp
{
    private DirectoryInfo local, server;
    public SyncUp(DirectoryInfo l, DirectoryInfo s)
    {
        local = l;
        server = s;
    }
        void SyncFolders()
        {
            CopyAll(local, server);
            CopyAll(server, local);
        }

        void SyncClock_Tick(object sender, System.Timers.ElapsedEventArgs e)
        {
            Console.WriteLine("Beginning sync!");
            SyncFolders();
        }

        public void StartTimer()
        {
            System.Timers.Timer syncClock = new System.Timers.Timer();
            syncClock.Interval = 60000;
            syncClock.Elapsed += new System.Timers.ElapsedEventHandler(SyncClock_Tick);
            syncClock.AutoReset = true;
            syncClock.Enabled = true;
            Console.WriteLine("Timer started!");
            Console.WriteLine("Press the Enter key to exit the program at any time...");
            Console.ReadLine();
            System.Environment.Exit(1);

    }
    

        // File Copy

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {

            foreach (FileInfo fi in source.GetFiles())
            {
                DateTime created = fi.CreationTime;
                DateTime lastModified = fi.LastWriteTime;

                if (File.Exists(Path.Combine(target.FullName, fi.Name)))
                {
                    string tFileName = Path.Combine(target.FullName, fi.Name);
                    FileInfo f2 = new FileInfo(tFileName);
                    DateTime lm = f2.LastWriteTime;
                    Console.WriteLine(@"File {0}\{1} already exists. Last modified {3}.", target.FullName, fi.Name, tFileName, lm);

                    try
                    {
                        if (lastModified > lm)
                        {
                            Console.WriteLine(@"Source file {0}\{1} last modified {2} is newer than the target file {3}\{4} last modified {5}. Updating...", fi.DirectoryName, fi.Name, lastModified.ToString(), target.FullName, fi.Name, lm.ToString());
                            fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
                        }
                        else
                        {
                            Console.WriteLine(@"Destination File {0}\{1} Skipped", target.FullName, fi.Name);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                }
                else
                {
                    Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                    fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
                }

            }

            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }
    }

