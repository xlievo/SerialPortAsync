using SerialPortAsync;
using Spectre.Console;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Demo
{
    internal class Program
    {
        static readonly Table table = new Table() { Border = TableBorder.HeavyHead }.AddColumn(new TableColumn("TIME").Centered()).AddColumn(new TableColumn("DATA").Centered()).AddRow("");

        static SerialPortAsyncClient COM1 = new(new SerialPortStreamClient("COM1", 9600, 8, RJCP.IO.Ports.Parity.None, RJCP.IO.Ports.StopBits.One) { }, open: true);

        static SerialPortAsyncClient COM2 = new(new SerialPortStreamClient("COM2", 9600, 8, RJCP.IO.Ports.Parity.None, RJCP.IO.Ports.StopBits.One) { }, open: true);

        static readonly CancellationTokenSource cancel = new();

        static readonly BlockingCollection<string> receivedQueue = new(1);

        static Timer timer;


        static async Task Main(string[] args)
        {
            try
            {
                //AnsiConsole.Write(new Rule("-----------").RuleStyle(new Style(Color.Yellow)));
                AnsiConsole.Write(table);//"[green]Qux[/]"

                HandleReceived();

                timer = new(c =>
                {
                    if (COM1 is null || !COM1.IsOpen)
                    {
                        return;
                    }

                    COM1.WriteLine($"{DateTimeOffset.Now}");
                }, default, 0, 100);

                COM2.DataReceived += (object? sender, SerialDataReceivedEventArgs e) =>
                {
                    //var buffer = new byte[20];
                    //COM2.Read(buffer, 0, buffer.Length);
                    //var data = Convert.ToHexString(buffer);

                    receivedQueue.TryAdd(COM2.ReadExisting().Trim());
                };
            }
            catch (Exception ex) when (!Debugger.IsAttached)
            {
                AnsiConsole.WriteException(ex);
            }

            Console.Read();
        }

        static void HandleReceived()
        {
            Task.Factory.StartNew(async c =>
            {
                try
                {
                    foreach (var source in receivedQueue.GetConsumingEnumerable(cancel.Token))
                    {
                        cancel.Token.ThrowIfCancellationRequested();

                        try
                        {
                            await Task.Delay(5000);
                            table.UpdateCell(0, 0, new Text(DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                            table.UpdateCell(0, 1, new Text($"COM2: {source}"));
                            AnsiConsole.Console.Clear(true);
                            AnsiConsole.Write(table);
                        }
                        catch (Exception ex)
                        {
                            AnsiConsole.WriteException(ex);
                        }
                    }
                }
                //catch (ObjectDisposedException) { }
                //catch (OperationCanceledException) { }
                catch (Exception ex)
                {
                    AnsiConsole.WriteException(ex);
                }
            }, null, cancel.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }
    }
}
