using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Collections;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;

public class SyncUp
{
    private DirectoryInfo local;
    private string userToken;
    public SyncUp(DirectoryInfo l, string token)
    {
        local = l;
        userToken = token;
    }
    Task SyncFolders()
    {
        return CopyAll(local);
    }

    void SyncClock_Tick(object sender, System.Timers.ElapsedEventArgs e)
    {
        Console.WriteLine("Beginning sync!");
        Task.Run(() => SyncFolders());
    }

    public void StartTimer()
    {
        System.Timers.Timer syncClock = new System.Timers.Timer();
        syncClock.Interval = 10000;
        syncClock.Elapsed += new System.Timers.ElapsedEventHandler(SyncClock_Tick);
        syncClock.AutoReset = true;
        syncClock.Enabled = true;
        Console.WriteLine("Timer started!");
        Console.WriteLine("Press the Enter key to exit the program at any time...");
        Console.ReadLine();
        System.Environment.Exit(1);

    }


    // File Copy (Creates list of local file names, paths, lm dates, and created dates)

    public async Task CopyAll(DirectoryInfo source)
    {
        var clientFiles = new List<SyncFileInfo>();
        foreach (FileInfo fi in source.GetFiles())
        {
            var clientFile = new SyncFileInfo
            {
                created = fi.CreationTime,
                lastModified = fi.LastWriteTime,
                filePath = fi.FullName,
                fileName = fi.Name,
            };

            clientFiles.Add(clientFile);
            


            /*if (File.Exists(Path.Combine(target.FullName, fi.Name)))
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
                }*/

            }

        // JSON payload and HTTP routing

        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userToken);
        var payload = JsonConvert.SerializeObject(clientFiles);
        var content = new StringContent(payload, Encoding.UTF8, "application/json");
        var result = await httpClient.PostAsync("HTTP://localhost:5000/api/sync/filecompare", content);
        Console.WriteLine(result.StatusCode);

            /*else
            {
                Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

        }

        foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
        {
            DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
            CopyAll(diSourceSubDir, nextTargetSubDir);
        }*/
        }
    }
