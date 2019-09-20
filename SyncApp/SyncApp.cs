using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Collections;

public class SyncApp
{
    static List<Credentials> creds = new List<Credentials>();

    public static void Main(string[] args)
    {
        // Authentication

    log:
        Console.WriteLine("Welcome. Please enter your username.");
        string username = Console.ReadLine();

        Credentials user = creds.Find(cred =>
        {
            return cred.username == username;
        });

            if (user != null)
            {
                Console.WriteLine($"Welcome back {username}!");

            pw:
                Console.WriteLine("Please enter your password.");
                string password = Console.ReadLine();
                if (user.password == password)
                {
                    Console.WriteLine("Authenticated!");
                    DirectoryInfo local = new DirectoryInfo($@"E:\{username}-local");
                    DirectoryInfo server = new DirectoryInfo($@"E:\{username}-server");
                    SyncUp syncer = new SyncUp(local, server);
                    Console.WriteLine("Beginning sync process...");
                    syncer.StartTimer();

                }
                else
                {
                    Console.WriteLine("Incorrect password. Please try again.");
                    goto pw;
                }

            }
            else
            {
                Console.WriteLine("Username not found. Please create a new account. What would you like your username to be?");
                string newUser = Console.ReadLine();
                Console.WriteLine($"Your username is now {newUser}! Please enter a password.");
                string newPass = Console.ReadLine();
                creds.Add(new Credentials(newUser, newPass));
                string newLocal = $@"E:\{newUser}-local";
                string newServer = $@"E:\{newUser}-server";
                Directory.CreateDirectory(newLocal);
                Directory.CreateDirectory(newServer);
                Console.WriteLine("Account created! Returning you to login~");
                goto log;
            }
        
    }
}


