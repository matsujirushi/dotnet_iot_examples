using System.Device.I2c;

Console.WriteLine("Press the enter key to execute.");
Console.ReadLine();

CancellationTokenSource cts = new();
CancellationToken ct = cts.Token;
Console.CancelKeyPress += (s, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

const int ADXL345_I2C_ADDRESS = 0x1d;

using I2cDevice device = I2cDevice.Create(new I2cConnectionSettings(1, ADXL345_I2C_ADDRESS));

#region ADXL345 Functions

static double Adxl345OutputDataToG(short value) => value * 2.0 / (1 << (10 - 1));

void Adxl345StartMeasurement()
{
    device.Write(new byte[] { 0x2d, 0x08, });
}

void Adxl345StopMeasurement()
{
    device.Write(new byte[] { 0x2d, 0x00, });
}

(double X, double Y, double Z) Adxl345ReadAcceleration()
{
    byte[] readData = new byte[6];
    device.WriteRead(new byte[] { 0x32, }, readData);

    var x = BitConverter.ToInt16(readData, 0);
    var y = BitConverter.ToInt16(readData, 2);
    var z = BitConverter.ToInt16(readData, 4);

    return (Adxl345OutputDataToG(x), Adxl345OutputDataToG(y), Adxl345OutputDataToG(z));
}

#endregion

Adxl345StartMeasurement();

while (!ct.IsCancellationRequested)
{
    var accel = Adxl345ReadAcceleration();
    Console.WriteLine($"{accel.X:f1} {accel.Y:f1} {accel.Z:f1}");

    if (ct.WaitHandle.WaitOne(TimeSpan.FromMilliseconds(200))) break;
}

Adxl345StopMeasurement();
