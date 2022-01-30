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
    WaitForEventResult waitResult;
    try
    {
        waitResult = controller.WaitForEvent(DI_PIN, PinEventTypes.Rising | PinEventTypes.Falling, TimeSpan.FromMilliseconds(200));
    }
    catch (IOException)
    {
        break;
    }
    Console.Write((waitResult.TimedOut, waitResult.EventTypes) switch
    {
        (true, _) => '.',
        (false, PinEventTypes.Rising) => '~',
        (false, PinEventTypes.Falling) => '_',
        _ => throw new InvalidOperationException()
    });
}

Console.WriteLine();
