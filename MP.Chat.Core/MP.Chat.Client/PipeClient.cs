using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Security.Principal;
using System.Diagnostics;
using System.Threading;


namespace MP.Chat.Client
{
    //public class PipeClient
    //{
    //    private int numClients = 4;

    //    public PipeClient(string[] Args)
    //    {
    //        if (Args.Length > 0)
    //        {
    //            if (Args[0] == "spawnclient")
    //            {
    //                NamedPipeClientStream pipeClient =
    //                    new NamedPipeClientStream(".", "testpipe",
    //                        PipeDirection.InOut, PipeOptions.None,
    //                        TokenImpersonationLevel.Impersonation);

    //                Console.WriteLine("Connecting to server...\n");
    //                pipeClient.Connect();

    //                StreamString ss = new StreamString(pipeClient);
    //                // Validate the server's signature string
    //                if (ss.ReadString() == "I am the one true server!")
    //                {
    //                    // The client security token is sent with the first write.
    //                    // Send the name of the file whose contents are returned
    //                    // by the server.
    //                    ss.WriteString(@"C:\Users\Aliaksei_Kazlou\Documents\textfile.txt");

    //                    // Print the file to the screen.
    //                    Console.Write(ss.ReadString());
    //                }
    //                else
    //                {
    //                    Console.WriteLine("Server could not be verified.");
    //                }
    //                pipeClient.Close();
    //                // Give the client process some time to display results before exiting.
    //                Thread.Sleep(4000);
    //            }
    //        }
    //        else
    //        {
    //            Console.WriteLine("\n*** Named pipe client stream with impersonation example ***\n");
    //            StartClients();
    //        }
    //    }

    //    // Helper function to create pipe client processes
    //    private void StartClients()
    //    {
    //        int i;
    //        string currentProcessName = Environment.CommandLine;
    //        Process[] plist = new Process[numClients];

    //        Console.WriteLine("Spawning client processes...\n");

    //        if (currentProcessName.Contains(Environment.CurrentDirectory))
    //        {
    //            currentProcessName = currentProcessName.Replace(Environment.CurrentDirectory, String.Empty);
    //        }

    //        // Remove extra characters when launched from Visual Studio
    //        currentProcessName = currentProcessName.Replace("\\", String.Empty);
    //        currentProcessName = currentProcessName.Replace("\"", String.Empty);

    //        for (i = 0; i < numClients; i++)
    //        {
    //            // Start 'this' program but spawn a named pipe client.
    //            plist[i] = Process.Start(currentProcessName, "spawnclient");
    //        }
    //        while (i > 0)
    //        {
    //            for (int j = 0; j < numClients; j++)
    //            {
    //                if (plist[j] != null)
    //                {
    //                    if (plist[j].HasExited)
    //                    {
    //                        Console.WriteLine("Client process[{0}] has exited.",
    //                            plist[j].Id);
    //                        plist[j] = null;
    //                        i--;    // decrement the process watch count
    //                    }
    //                    else
    //                    {
    //                        Thread.Sleep(250);
    //                    }
    //                }
    //            }
    //        }
    //        Console.WriteLine("\nClient processes finished, exiting.");
    //    }
    //}
}
