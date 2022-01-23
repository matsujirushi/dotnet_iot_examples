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

const int DO_PIN = 18;

using GpioController controller = new();
controller.OpenPin(DO_PIN, PinMode.Output, PinValue.Low);

while (!ct.IsCancellationRequested)
{
    controller.Write(DO_PIN, PinValue.High);
    if (ct.WaitHandle.WaitOne(TimeSpan.FromMilliseconds(200))) break;

    controller.Write(DO_PIN, PinValue.Low);
    if (ct.WaitHandle.WaitOne(TimeSpan.FromMilliseconds(800))) break;
}

controller.Write(DO_PIN, PinValue.Low);
