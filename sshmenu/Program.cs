using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
namespace sshmenu
{
    class sshmenu
    {
        private static string ConfigFile =  Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/.ssh/menu.json";
        static void Main(string[] args)
        {
            if (!File.Exists(ConfigFile))
            {
                MenuOptions options = new MenuOptions();
                options.NiceName = "My Computer!";
                options.IP = "127.0.0.0.1";
                options.Port = "22";
                options.Username = "example";
                options.useSSHPass = false;
                options.passwordIfUsing = "password for if using SSHPass";
                MenuOptions options2 = options;
                Menu menuOptions = new Menu();
                menuOptions.Options.Add(options);
                menuOptions.Options.Add(options2);
                File.WriteAllText(ConfigFile,JsonConvert.SerializeObject(menuOptions, Formatting.Indented));
                Console.WriteLine(ConfigFile);
                Console.WriteLine("Wrote new config file to " + ConfigFile + ". You should edit it.");
                Environment.Exit(0);
            }

            ProcessStartInfo sshConnection = new ProcessStartInfo();
            
            Console.Title = "SSH Menu";
            Console.Clear();
            var Menu = JsonConvert.DeserializeObject<Menu>(File.ReadAllText(ConfigFile));
            int index = 1;
            Console.WriteLine("Please choose an option:");
            foreach (var option in Menu.Options)
            {
                Console.WriteLine(index + ": " + option.NiceName);
                index += 1;
            }
            Retry:
            try
            {
                ConsoleKeyInfo input = Console.ReadKey();
                int Selection = 0;
                if (char.IsDigit(input.KeyChar))
                {
                    Selection = int.Parse(input.KeyChar.ToString()); // use Parse if it's a Digit
                }
                else
                {
                    Console.WriteLine("Invalid Input");
                    goto Retry;
                }
                Console.Clear();
                if (Selection <= Menu.Options.Count)
                {
                    MenuOptions option = Menu.Options[Selection - 1];
                    if (option.useSSHPass == true)
                    {
                        sshConnection.FileName = "sshpass";

                        sshConnection.Arguments = "-p " + option.passwordIfUsing + " ssh " + option.Username + "@" +
                                                  option.IP + " -p " + option.Port;
                        var connection = Process.Start(sshConnection);
                        connection.WaitForExit();
                    }
                    else
                    {
                        sshConnection.FileName = "ssh";
                        sshConnection.Arguments = option.Username + "@" + option.IP + " -p " + option.Port;
                        var connection = Process.Start(sshConnection);
                        connection.WaitForExit();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Environment.Exit(2);
            }
            
            
        }

        public class Menu
        {
            public List<MenuOptions> Options = new List<MenuOptions>();
        }
         public class MenuOptions
         { 
             public string NiceName;
             public string IP;
             public string Port;
             public string Username;
             public bool useSSHPass;
             public string passwordIfUsing;
         }
    }
}