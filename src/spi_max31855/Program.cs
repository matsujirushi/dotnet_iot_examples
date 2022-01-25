using System.Device.Spi;

Console.WriteLine("Press the enter key to execute.");
Console.ReadLine();

CancellationTokenSource cts = new();
CancellationToken ct = cts.Token;
Console.CancelKeyPress += (s, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

using SpiDevice device = SpiDevice.Create(new SpiConnectionSettings(0, 0) { ClockFrequency = 5000000, Mode = SpiMode.Mode3, });

#region MAX31855 Functions

double Max31855ReadThermocoupleTemperature()
{
    Span<byte> writeData = stackalloc byte[4];
    Span<byte> readData = stackalloc byte[4];
    device.TransferFullDuplex(writeData, readData);

    Span<byte> readDataThermocouple = stackalloc byte[] { readData[0], readData[1], };
    if (BitConverter.IsLittleEndian) MemoryExtensions.Reverse(readDataThermocouple);
    return (double)(BitConverter.ToInt16(readDataThermocouple) >> 2) / 4;
}

#endregion

while (!ct.IsCancellationRequested)
{
    var temperature = Max31855ReadThermocoupleTemperature();
    Console.WriteLine($"{temperature:f2}");

    if (ct.WaitHandle.WaitOne(TimeSpan.FromMilliseconds(200))) break;
}
