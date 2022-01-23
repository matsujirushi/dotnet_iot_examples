using System.Device.Gpio;

Console.WriteLine("Press the enter key to execute.");
Console.ReadLine();

CancellationTokenSource cts = new();
CancellationToken ct = cts.Token;
Console.CancelKeyPress += (s, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

const int DI_PIN = 17;

using GpioController controller = new();
controller.OpenPin(DI_PIN, PinMode.Input);

while (!ct.IsCancellationRequested)
{
    Console.Write(controller.Read(DI_PIN) == PinValue.High ? '~' : '_');
    if (ct.WaitHandle.WaitOne(TimeSpan.FromMilliseconds(200))) break;
}

Console.WriteLine();
