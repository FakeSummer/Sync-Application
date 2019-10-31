using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Newtonsoft.Json;

public class Credentials
{
    public string Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }

    public static async Task UserLog()
    {
        
        // Authentication Loop

        int loginAttempts;
        for (loginAttempts = 0; loginAttempts < 3;)
        {
            Console.WriteLine("Please enter your username.");
            string user = Console.ReadLine();
            Console.WriteLine("Please enter your password.");
            string pass = Console.ReadLine();
            var User = new Credentials
            {
                Username = user,
                Password = pass
            };

            var httpClient = new HttpClient();
            var payload = JsonConvert.SerializeObject(User);
            var content = new StringContent(payload, Encoding.UTF8, "application/json");
            var result = await httpClient.PostAsync("HTTP://localhost:5000/api/verify", content);

            if (result.IsSuccessStatusCode)
            {
                Console.WriteLine("Login success!");
                string newLocal = $@".\{user}-local";
                var localDirectory = Directory.CreateDirectory(newLocal);
                var guidResult = await result.Content.ReadAsStringAsync();
                User.Id = guidResult;
                var sync = new SyncUp(localDirectory, guidResult);
                sync.StartTimer();
                break;
            }
                Console.WriteLine("Login attempt failed. Please try again.");
                loginAttempts++;
        }

        // Login Attempt Lockout

        if (loginAttempts >= 3)
        {
            Console.WriteLine("Login attempts exceeded. Please try again later.");
            Console.WriteLine("Press any key to terminate the program.");
        }

        Console.ReadLine();

    }
}

