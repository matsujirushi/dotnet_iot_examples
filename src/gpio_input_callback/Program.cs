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
controller.RegisterCallbackForPinValueChangedEvent(DI_PIN, PinEventTypes.Rising | PinEventTypes.Falling, (object sender, PinValueChangedEventArgs pinValueChangedEventArgs) => {
    Console.Write(pinValueChangedEventArgs.ChangeType == PinEventTypes.Rising ? '~' : '_');
});

while (!ct.IsCancellationRequested)
{
    if (ct.WaitHandle.WaitOne()) break;
}

Console.WriteLine();
