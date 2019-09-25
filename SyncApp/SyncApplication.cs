using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Collections;
using LiteDB;

public class SyncApplication
{

    public static void Main(string[] args)
    {
        using (var db = new LiteDatabase(@"E:\Creds.db"))
        {

        // Authentication

        log:
            Console.WriteLine("Welcome. Please enter your username.");
            string username = Console.ReadLine();
            var users = db.GetCollection<Credentials>("users");
            users.EnsureIndex(x => x.Username);
            var uResults = users.FindOne(x => x.Username == username);

            if (uResults != null)
                {
                Console.WriteLine($"Welcome back {username}!");

                pw:
                    Console.WriteLine("Please enter your password.");
                    string password = Console.ReadLine();
                    bool authentic = PasswordHash.ValidatePassword(password, uResults.Password);
                    if (authentic == true) 
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
                    string passwordHash = PasswordHash.HashPassword(newPass);
                
                    var user = new Credentials
                    {
                        Username = newUser,
                        Password = passwordHash,
                    };
                    users.Insert(user);

                    string newLocal = $@"E:\{newUser}-local";
                    string newServer = $@"E:\{newUser}-server";
                    Directory.CreateDirectory(newLocal);
                    Directory.CreateDirectory(newServer);
                    Console.WriteLine("Account created! Returning you to login~");
                    goto log;
                }

        }
    }
}


