using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SerialPortAsync
{
    /// <summary>
    /// SerialPortClient to SerialPortStream.
    /// </summary>
    public class SerialPortStreamClient : RJCP.IO.Ports.SerialPortStream, ISerialPort
    {
        public SerialPortStreamClient(string port) : base(port) { }

        public SerialPortStreamClient(string port, int baud) : base(port, baud) { }

        public SerialPortStreamClient(string port, int baud, int data, RJCP.IO.Ports.Parity parity, RJCP.IO.Ports.StopBits stopbits) : base(port, baud, data, parity, stopbits) { }

        public new event EventHandler<SerialDataReceivedEventArgs> DataReceived
        {
            add => base.DataReceived += (object sender, RJCP.IO.Ports.SerialDataReceivedEventArgs e) => value?.Invoke(sender, new SerialDataReceivedEventArgs((SerialData)e.EventType));
            remove => SerialPortAsyncClient.RemoveEvents<RJCP.IO.Ports.SerialPortStream>(this, nameof(DataReceived), (handler) => base.DataReceived -= handler as EventHandler<RJCP.IO.Ports.SerialDataReceivedEventArgs>);
        }

        public new event EventHandler<SerialErrorReceivedEventArgs> ErrorReceived
        {
            add => base.ErrorReceived += (object sender, RJCP.IO.Ports.SerialErrorReceivedEventArgs e) => value?.Invoke(sender, new SerialErrorReceivedEventArgs((SerialError)e.EventType));
            remove => SerialPortAsyncClient.RemoveEvents<RJCP.IO.Ports.SerialPortStream>(this, nameof(ErrorReceived), (handler) => base.ErrorReceived -= handler as EventHandler<RJCP.IO.Ports.SerialErrorReceivedEventArgs>);
        }

        public new event EventHandler<SerialPinChangedEventArgs> PinChanged
        {
            add => base.PinChanged += (object sender, RJCP.IO.Ports.SerialPinChangedEventArgs e) => value?.Invoke(sender, new SerialPinChangedEventArgs((SerialPinChange)e.EventType));
            remove => SerialPortAsyncClient.RemoveEvents<RJCP.IO.Ports.SerialPortStream>(this, nameof(PinChanged), (handler) => base.PinChanged -= handler as EventHandler<RJCP.IO.Ports.SerialPinChangedEventArgs>);
        }
    }

    /// <summary>
    /// SerialPortClient to System.IO.Ports.SerialPort.
    /// </summary>
    public class SystemIOSerialPortClient : System.IO.Ports.SerialPort, ISerialPort
    {
        public SystemIOSerialPortClient(string portName) : base(portName) { }

        public SystemIOSerialPortClient(string portName, int baudRate) : base(portName, baudRate) { }

        public SystemIOSerialPortClient(string portName, int baudRate, System.IO.Ports.Parity parity, int dataBits, System.IO.Ports.StopBits stopBits) : base(portName, baudRate, parity, dataBits, stopBits) { }

        bool isDisposed;

        public bool IsDisposed => isDisposed;

        public bool CanTimeout => true;

        public new event EventHandler<SerialDataReceivedEventArgs> DataReceived
        {
            add => base.DataReceived += (object sender, System.IO.Ports.SerialDataReceivedEventArgs e) => value?.Invoke(sender, new SerialDataReceivedEventArgs((SerialData)e.EventType));
            remove => SerialPortAsyncClient.RemoveEvents<System.IO.Ports.SerialPort>(this, "_dataReceived", (handler) => base.DataReceived -= handler as System.IO.Ports.SerialDataReceivedEventHandler);
        }

        public new event EventHandler<SerialErrorReceivedEventArgs> ErrorReceived
        {
            add => base.ErrorReceived += (object sender, System.IO.Ports.SerialErrorReceivedEventArgs e) => value?.Invoke(sender, new SerialErrorReceivedEventArgs((SerialError)e.EventType));
            remove => SerialPortAsyncClient.RemoveEvents<System.IO.Ports.SerialPort>(this, nameof(ErrorReceived), (handler) => base.ErrorReceived -= handler as System.IO.Ports.SerialErrorReceivedEventHandler);
        }

        public new event EventHandler<SerialPinChangedEventArgs> PinChanged
        {
            add => base.PinChanged += (object sender, System.IO.Ports.SerialPinChangedEventArgs e) => value?.Invoke(sender, new SerialPinChangedEventArgs((SerialPinChange)e.EventType));
            remove => SerialPortAsyncClient.RemoveEvents<System.IO.Ports.SerialPort>(this, nameof(PinChanged), (handler) => base.PinChanged -= handler as System.IO.Ports.SerialPinChangedEventHandler);
        }

        public async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) => Write(buffer, offset, count);

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            isDisposed = true;
        }
    }

    public interface ISerialPort : IDisposable
    {
        Encoding Encoding { get; set; }

        string NewLine { get; set; }

        //bool CanTimeout { get; }

        //int WriteTimeout { get; set; }

        //int ReadTimeout { get; set; }

        bool IsOpen { get; }

        bool IsDisposed { get; }

        void Open();

        void Close();

        event EventHandler<SerialDataReceivedEventArgs> DataReceived;

        event EventHandler<SerialErrorReceivedEventArgs> ErrorReceived;

        event EventHandler<SerialPinChangedEventArgs> PinChanged;

        string ReadTo(string text);

        string ReadLine();

        string ReadExisting();

        int Read(byte[] buffer, int offset, int count);

        Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken);

        void WriteLine(string text);
    }

    #region Events

    /// <summary>
    /// Event related data on PinChanged.
    /// </summary>
    [Flags]
    public enum SerialPinChange
    {
        /// <summary>
        /// Indicates no pin change detected.
        /// </summary>
        NoChange = 0,
        /// <summary>
        /// Clear To Send signal has changed.
        /// </summary>
        CtsChanged = 8,
        /// <summary>
        /// Data Set Ready signal has changed.
        /// </summary>
        DsrChanged = 16,
        /// <summary>
        /// Carrier Detect signal has changed.
        /// </summary>
        CDChanged = 32,
        /// <summary>
        /// Break detected.
        /// </summary>
        Break = 64,
        /// <summary>
        /// Ring signal has changed.
        /// </summary>
        Ring = 256
    }

    /// <summary>
    /// Event related information on ErrorReceived.
    /// </summary>
    [Flags]
    public enum SerialError
    {
        /// <summary>
        /// Indicates no error.
        /// </summary>
        NoError = 0,
        /// <summary>
        /// Driver buffer has reached 80% full.
        /// </summary>
        RXOver = 1,
        /// <summary>
        /// Driver has detected an overflow.
        /// </summary>
        Overrun = 2,
        /// <summary>
        /// Parity error detected.
        /// </summary>
        RXParity = 4,
        /// <summary>
        /// Frame error detected.
        /// </summary>
        Frame = 8,
        /// <summary>
        /// Transmit buffer is full.
        /// </summary>
        TXFull = 256
    }

    /// <summary>
    /// Event related information on DataReceived
    /// </summary>
    [Flags]
    public enum SerialData
    {
        /// <summary>
        /// Indicates no data received
        /// </summary>
        NoData = 0,
        /// <summary>
        /// At least a single byte has been received
        /// </summary>
        Chars = 1,
        /// <summary>
        /// The EOF character has been detected
        /// </summary>
        Eof = 2
    }

    /// <summary>
    /// EventArgs for PinChanged.
    /// </summary>
    public class SerialPinChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="eventType">Event that occurred.</param>
        public SerialPinChangedEventArgs(SerialPinChange eventType) => EventType = eventType;

        /// <summary>
        /// The event type for ErrorReceived.
        /// </summary>
        public SerialPinChange EventType { get; }
    }

    /// <summary>
    /// EventArgs for ErrorReceived.
    /// </summary>
    public class SerialErrorReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="eventType">Event that occurred.</param>
        public SerialErrorReceivedEventArgs(SerialError eventType) => EventType = eventType;

        /// <summary>
        /// The event type for ErrorReceived.
        /// </summary>
        public SerialError EventType { get; }
    }

    /// <summary>
    /// EventArgs for DataReceived.
    /// </summary>
    public class SerialDataReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="eventType">Event that occurred.</param>
        public SerialDataReceivedEventArgs(SerialData eventType) => EventType = eventType;

        /// <summary>
        /// The event type for DataReceived.
        /// </summary>
        public SerialData EventType { get; }
    }

    #endregion

    /// <summary>
    /// SerialPortClient.
    /// </summary>
    public class SerialPortAsyncClient : IDisposable
    {
        readonly ISerialPort serial;

        readonly BlockingCollection<Source> sendQueue = new();

        readonly BlockingCollection<Source> receivedQueue = new(1);

        readonly CancellationTokenSource cancel = new();

        volatile CancellationTokenSource clear = new();

        readonly TimeSpan timeout = default;

        readonly static TimeSpan timeoutDefault = TimeSpan.FromSeconds(10);

        /// <summary>
        /// Indicates if this object has already been disposed.
        /// </summary>
        public volatile bool IsDisposed;

        readonly struct Source
        {
            public Source(Func<ValueTask> send, Func<SerialData, ValueTask<dynamic>> read, TaskCompletionSource<dynamic> signal)
            {
                Send = send;
                Read = read;
                Signal = signal;
            }

            public Func<ValueTask> Send { get; }
            public Func<SerialData, ValueTask<dynamic>> Read { get; }
            public TaskCompletionSource<dynamic> Signal { get; }
        }

        #region SerialPortClient

        #region SystemIOSerialPortClient

        //public SerialPortAsyncClient(string port, bool open = false) : this(new SystemIOSerialPortClient(port), timeoutDefault, open) { }

        //public SerialPortAsyncClient(string port, TimeSpan timeout = default, bool open = false) : this(new SystemIOSerialPortClient(port), timeout, open) { }

        //public SerialPortAsyncClient(string port, int baud, bool open = false) : this(new SystemIOSerialPortClient(port, baud), timeoutDefault, open) { }

        //public SerialPortAsyncClient(string port, int baud, TimeSpan timeout = default, bool open = false) : this(new SystemIOSerialPortClient(port, baud), timeout, open) { }

        /// <summary>
        /// timeout default value 10 seconds.
        /// </summary>
        /// <param name="port"></param>
        /// <param name="baud"></param>
        /// <param name="parity"></param>
        /// <param name="data"></param>
        /// <param name="stopbits"></param>
        /// <param name="open"></param>
        public SerialPortAsyncClient(string port, int baud, System.IO.Ports.Parity parity, int data, System.IO.Ports.StopBits stopbits, bool open = false) : this(new SystemIOSerialPortClient(port, baud, parity, data, stopbits), timeoutDefault, open) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="baud"></param>
        /// <param name="parity"></param>
        /// <param name="data"></param>
        /// <param name="stopbits"></param>
        /// <param name="timeout"></param>
        /// <param name="open"></param>
        public SerialPortAsyncClient(string port, int baud, System.IO.Ports.Parity parity, int data, System.IO.Ports.StopBits stopbits, TimeSpan timeout = default, bool open = false) : this(new SystemIOSerialPortClient(port, baud, parity, data, stopbits), timeout, open) { }

        #endregion

        #region SerialPortStreamClient

        //public SerialPortAsyncClient(string port, bool open = false) : this(new SerialPortStreamClient(port), timeoutDefault, open) { }

        //public SerialPortAsyncClient(string port, TimeSpan timeout = default, bool open = false) : this(new SerialPortStreamClient(port), timeout, open) { }

        //public SerialPortAsyncClient(string port, int baud, bool open = false) : this(new SerialPortStreamClient(port, baud), timeoutDefault, open) { }

        //public SerialPortAsyncClient(string port, int baud, TimeSpan timeout = default, bool open = false) : this(new SerialPortStreamClient(port, baud), timeout, open) { }

        /// <summary>
        /// timeout default value 10 seconds.
        /// </summary>
        /// <param name="port"></param>
        /// <param name="baud"></param>
        /// <param name="data"></param>
        /// <param name="parity"></param>
        /// <param name="stopbits"></param>
        /// <param name="open"></param>
        public SerialPortAsyncClient(string port, int baud, int data, RJCP.IO.Ports.Parity parity, RJCP.IO.Ports.StopBits stopbits, bool open = false) : this(new SerialPortStreamClient(port, baud, data, parity, stopbits), timeoutDefault, open) { }

        public SerialPortAsyncClient(string port, int baud, int data, RJCP.IO.Ports.Parity parity, RJCP.IO.Ports.StopBits stopbits, TimeSpan timeout = default, bool open = false) : this(new SerialPortStreamClient(port, baud, data, parity, stopbits), timeout, open) { }

        #endregion

        public SerialPortAsyncClient(ISerialPort serial, TimeSpan timeout = default, bool open = false)
        {
            this.serial = serial ?? throw new ArgumentNullException(nameof(serial));

            //if (this.serial.CanTimeout)
            //{
            //    this.serial.ReadTimeout = (int)timeout.TotalSeconds;
            //    this.serial.WriteTimeout = (int)timeout.TotalSeconds;
            //}

            this.timeout = timeout;

            if (open)
            {
                Open();
            }

            Task.Factory.StartNew(async c =>
            {
                try
                {
                    foreach (var source in sendQueue.GetConsumingEnumerable(cancel.Token))
                    {
                        cancel.Token.ThrowIfCancellationRequested();

                        if (clear.IsCancellationRequested)
                        {
                            source.Signal.TrySetCanceled();
                            continue;
                        }

                        try
                        {
                            await source.Send();
                            receivedQueue.Add(source, cancel.Token);
                        }
                        catch (Exception ex)
                        {
                            source.Signal.TrySetException(ex);
                            continue;
                        }

                        try
                        {
                            if (System.Threading.Timeout.InfiniteTimeSpan == timeout || default == timeout)
                            {
                                await source.Signal.Task;
                            }

                            await Timeout(source.Signal, timeout);
                        }
                        catch (Exception ex)
                        {
                            receivedQueue.TryTake(out _);
                            source.Signal.TrySetException(ex);
                        }

                        await Task.Delay(1);
                    }
                }
                //catch (ObjectDisposedException) { }
                //catch (OperationCanceledException) { }
                catch
                {
                    //throw ex;
                }
            }, null, cancel.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

            this.serial.ErrorReceived += ErrorReceived;

            this.serial.PinChanged += PinChanged;

            this.serial.DataReceived += async (object sender, SerialDataReceivedEventArgs e) =>
            {
                //if (cancel.IsCancellationRequested) { return; }

                try
                {
                    cancel.Token.ThrowIfCancellationRequested();

                    if (receivedQueue.TryTake(out Source source))
                    {
                        //if (cancel.IsCancellationRequested) { return; }

                        //if (this.serial.IsDisposed || !this.serial.IsOpen) { source.Signal.TrySetCanceled(); }
                        if (!IsOpen) { source.Signal.TrySetCanceled(); }

                        var result = await source.Read(e.EventType);

                        source.Signal.TrySetResult(result);
                    }
                    else
                    {
                        DataReceived?.Invoke(sender, new SerialDataReceivedEventArgs(e.EventType));
                    }
                }
                //catch (ObjectDisposedException) { }
                //catch (OperationCanceledException) { }
                catch //(Exception)
                {
                    //throw ex;
                }
            };
        }

        #endregion

        #region SerialPortStream

        /// <summary>
        /// Read buffer length
        /// </summary>
        public int BufferLength { get; set; } = 4096;

        /// <summary>
        /// Gets or sets the byte encoding for pre- and post-transmission conversion of text.
        /// <para>The encoding is used for encoding string information to byte format when sending over the serial port, or receiving data via the serial port. It is only used with the read/write functions that accept strings (and not used for byte based reading and writing).</para>
        /// </summary>
        public Encoding Encoding { get { CheckDisposed(); return serial.Encoding; } set { CheckDisposed(); serial.Encoding = value; } }

        /// <summary>
        /// Gets or sets the value used to interpret the end of a call to the RJCP.IO.Ports.SerialPortStream.ReadLine and RJCP.IO.Ports.SerialPortStream.WriteLine(System.String) methods.
        /// <para>A value that represents the end of a line. The default is a line feed, (NewLine).</para>
        /// </summary>
        public string NewLine { get { CheckDisposed(); return serial.NewLine; } set { CheckDisposed(); serial.NewLine = value; } }

        /// <summary>
        /// Gets a value indicating the open or closed status of the SerialPortStream object.
        /// </summary>
        public bool IsOpen { get { CheckDisposed(); return serial.IsDisposed ? false : serial.IsOpen; } }

        /// <summary>
        /// Opens a new serial port connection.
        /// <para>Opens a connection to the serial port provided by the constructor or the Port property. If this object is already managing a serial port, this object raises an exception. When opening the port, only the settings explicitly applied will be given to the port. That is, if you read the default BaudRate as 115200, this value will only be applied if you explicitly set it to 115200. Else the default baud rate of the serial port when its opened will be used. Normally when you instantiate this stream on a COM port, it is opened for a brief time and queried for the capabilities and default settings. This allows your application to use the settings that were already available (such as defined by the windows user in the Control Panel, or the last open application). If you require to open the COM port without briefly opening it to query its status, then you need to instantiate this object through the default constructor. Set the property UpdateOnPortSet to false and then set the Port property. Provide all the other properties you require then call the RJCP.IO.Ports.SerialPortStream.Open method. The port will be opened using the default properties providing you with a consistent environment (independent of the state of the Operating System or the driver beforehand).</para>
        /// </summary>
        public void Open()
        {
            CheckDisposed();

            if (!serial.IsDisposed && !serial.IsOpen)
            {
                serial.Open();
            }
        }

        /// <summary>
        /// Closes the port connection, sets the RJCP.IO.Ports.SerialPortStream.IsOpen property to false. Does not dispose the object.
        /// <para>This method will clean up the object so far as to close the port. Internal buffers remain active that the stream can continue to read. Writes will throw an exception.</para>
        /// </summary>
        public void Close()
        {
            CheckDisposed();

            if (!serial.IsDisposed && serial.IsOpen) { serial.Close(); }
        }

        public event EventHandler<SerialPinChangedEventArgs> PinChanged;

        public event EventHandler<SerialDataReceivedEventArgs> DataReceived;

        public event EventHandler<SerialErrorReceivedEventArgs> ErrorReceived;

        #endregion

        /// <summary>
        /// Clear send queue.
        /// </summary>
        public void Clear()
        {
            CheckDisposed();

            clear.Cancel(false);

            SpinWait.SpinUntil(() => 0 == sendQueue.Count && 0 == receivedQueue.Count, timeout);
        }

        /// <summary>
        /// Clear send queue.
        /// </summary>
        /// <returns></returns>
        public async Task ClearAsync() => await Task.Run(() => Clear());

        void CheckDisposed()
        {
            if (IsDisposed) { throw new ObjectDisposedException("SerialPortClient"); }
        }

        public void Dispose()
        {
            if (IsDisposed) { return; }

            Clear();

            cancel.Cancel(false);
            sendQueue.Dispose();
            receivedQueue.Dispose();
            serial.DataReceived -= null;
            serial.PinChanged -= null;
            serial.ErrorReceived -= null;
            serial.Dispose();
            clear.Dispose();
            cancel.Dispose();

            if (null != DataReceived)
            {
                foreach (var item in DataReceived.GetInvocationList())
                {
                    DataReceived -= item as EventHandler<SerialDataReceivedEventArgs>;
                }
            }

            if (null != PinChanged)
            {
                foreach (var item in PinChanged.GetInvocationList())
                {
                    PinChanged -= item as EventHandler<SerialPinChangedEventArgs>;
                }
            }

            if (null != ErrorReceived)
            {
                foreach (var item in ErrorReceived.GetInvocationList())
                {
                    ErrorReceived -= item as EventHandler<SerialErrorReceivedEventArgs>;
                }
            }

            IsDisposed = true;
        }

        static async ValueTask<TResult> Timeout<TResult>(TaskCompletionSource<TResult> tcs, TimeSpan timeout)
        {
            using (var cancel = new CancellationTokenSource())
            {
                var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(timeout, cancel.Token));

                if (completedTask == tcs.Task)
                {
                    cancel.Cancel(); // Very important in order to propagate exceptions
                }
                else
                {
                    tcs.TrySetException(new TimeoutException($"{nameof(Timeout)}: The operation has timed out after {timeout:mm\\:ss}"));
                }

                return await tcs.Task;
            }
        }

        public async Task<TResult> SendAsync<TResult>(Func<ValueTask> send, Func<SerialData, ValueTask<dynamic>> read)
        {
            CheckDisposed();

            var signal = new TaskCompletionSource<dynamic>();

            if (clear.IsCancellationRequested)
            {
                clear = new();
            }

            //_isDisposed = true

            sendQueue.TryAdd(new Source(async () =>
            {
                if (serial.IsDisposed || !serial.IsOpen) { return; }

                await send();

            }, read, signal));

            return await signal.Task;
        }

        public async Task<string> SendAsync(string text, bool hexString = false, TimeSpan readInterval = default)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException(nameof(text));
            }

            return await SendAsync<string>(async () =>
            {
                if (hexString)
                {
                    var buffer = Convert.FromHexString(text.Replace(" ", string.Empty));

                    await serial.WriteAsync(buffer, 0, buffer.Length, cancel.Token);
                }
                else
                {
                    serial.WriteLine(text);
                }
            }, async eventType =>
            {
                if (System.Threading.Timeout.InfiniteTimeSpan != readInterval && default != readInterval)
                {
                    await Task.Delay(readInterval);
                }

                if (SerialData.Chars != eventType) { return null; }

                return ReadExisting();
            });
        }

        //public async Task<string> SendAsync(byte[] data, TimeSpan readInterval = default)
        //{
        //    if (data is null)
        //    {
        //        throw new ArgumentNullException(nameof(data));
        //    }

        //    return await SendAsync<string>(async () => await serial.WriteAsync(data, 0, data.Length, cancel.Token), async eventType =>
        //    {
        //        if (System.Threading.Timeout.InfiniteTimeSpan != readInterval && default != readInterval)
        //        {
        //            await Task.Delay(readInterval);
        //        }

        //        if (SerialData.Chars != eventType) { return null; }

        //        return !serial.IsDisposed && serial.IsOpen ? serial.ReadExisting() : null;
        //    });
        //}

        public async Task<byte[]> SendAsync(byte[] data, TimeSpan readInterval = default)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            return await SendAsync<byte[]>(async () => await serial.WriteAsync(data, 0, data.Length, cancel.Token), async eventType =>
            {
                if (System.Threading.Timeout.InfiniteTimeSpan != readInterval && default != readInterval)
                {
                    await Task.Delay(readInterval);
                }

                if (SerialData.Chars != eventType) { return null; }

                var buffer = new byte[BufferLength];

                var count = Read(buffer, 0, buffer.Length);

                if (0 >= count)
                {
                    return Array.Empty<byte>();
                }

                var result = new byte[count];

                Array.Copy(buffer, result, count);

                return result;
            });
        }

        /// <summary>
        /// Reads up to the NewLine value in the input buffer.
        /// </summary>
        /// <returns>
        /// The contents of the input buffer up to the first occurrence of a NewLine value.
        /// <para>
        /// System.TimeoutException: Data was not available in the timeout specified.
        /// </para>
        /// <para>
        /// System.IO.IOException: Device Error (e.g. device removed).
        /// </para>
        /// <para>
        /// System.ObjectDisposedException:
        /// </para>
        /// </returns>
        public string ReadLine()
        {
            CheckDisposed();

            return !serial.IsDisposed && serial.IsOpen ? serial.ReadLine() : null;
        }

        /// <summary>
        /// Reads all immediately available bytes.
        /// <para>Reads all data in the current buffer. If there is no data available, then no
        /// data is returned. This is different to the Microsoft implementation, that will
        /// read all data, and if there is no data, then it waits for data based on the time
        /// outs. This method employs no time outs.
        /// Because this method returns only the data that is currently in the cached buffer
        /// and ignores the data that is actually buffered by the driver itself, there may
        /// be a slight discrepancy between the value returned by BytesToRead and the actual
        /// length of the string returned.
        /// This method differs slightly from the Microsoft implementation in that this function
        /// doesn't initiate a read operation, as we have a dedicated thread to reading data
        /// that is running independently.</para>
        /// </summary>
        /// <returns>The contents of the stream and the input buffer of the RJCP.IO.Ports.SerialPortStream.</returns>
        public string ReadExisting()
        {
            CheckDisposed();

            return !serial.IsDisposed && serial.IsOpen ? serial.ReadExisting() : null;
        }

        /// <summary>
        /// Reads a string up to the specified text in the input buffer.
        /// <para>
        /// The ReadTo() function will read text from the byte buffer up to a predetermined limit (1024 characters) when looking for the string text. If text is not found within this limit, data is thrown away and more data is read (effectively consuming the earlier bytes).
        /// </para>
        /// <para>
        /// This method is provided as compatibility with the Microsoft implementation. There are some important differences however. This method attempts to fix a minor pathological problem with the Microsoft implementation. If the string text is not found, the MS implementation may modify the internal state of the decoder. As a workaround, it pushes all decoded characters back into its internal byte buffer, which fixes the problem that a second call to the ReadTo() method returns the consistent results, but a call to Read(byte[], ..) may return data that was not actually transmitted by the DCE. This would happen in case that an invalid byte sequence was found, converted to a fall back character. The original byte sequence is removed and replaced with the byte equivalent of the fall back character.
        /// </para>
        /// <para>
        /// This method is rather slow, because it tries to preserve the byte buffer in case of failure.
        /// </para>
        /// <para>
        /// In case the data cannot be read, an exception is always thrown. So you may assume that if this method returns, you have valid data.
        /// </para>
        /// </summary>
        /// <param name="text">The text to indicate where the read operation stops.</param>
        /// <returns>
        /// The contents of the input buffer up to the specified text.
        /// <para>
        /// System.TimeoutException: Data was not available in the timeout specified.
        /// </para>
        /// <para>
        /// System.IO.IOException: Device Error (e.g. device removed).
        /// </para>
        /// <para>
        /// System.ObjectDisposedException:
        /// </para>
        /// </returns>
        public string ReadTo(string text)
        {
            CheckDisposed();

            return !serial.IsDisposed && serial.IsOpen ? serial.ReadTo(text) : null;
        }

        /// <summary>
        /// Reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read.
        /// </summary>
        /// <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified byte array with the values between offset and (offset + count - 1) replaced by the bytes read from the current source.</param>
        /// <param name="offset">The zero-based byte offset in buffer at which to begin storing the data read from the current stream.</param>
        /// <param name="count">The maximum number of bytes to be read from the current stream.</param>
        /// <returns>
        /// The total number of bytes read into the buffer. This can be less than the number of bytes requested if that many bytes are not currently available, or zero (0) if the end of the stream has been reached.
        /// <para>
        /// System.ObjectDisposedException:
        /// </para>
        /// System.ArgumentNullException: null buffer provided.
        /// <para>
        /// System.ArgumentOutOfRangeException: Negative offset provided, or negative count provided.
        /// </para>
        /// <para>
        /// System.ArgumentException: Offset and count exceed buffer boundaries.
        /// </para>
        /// <para>
        /// System.IO.IOException: Device Error (e.g. device removed).
        /// </para>
        /// </returns>
        public int Read(byte[] buffer, int offset, int count)
        {
            CheckDisposed();

            return !serial.IsDisposed && serial.IsOpen ? serial.Read(buffer, offset, count) : -1;
        }

        /// <summary>
        /// Gets an array of serial port names for the current computer.
        /// </summary>
        /// <returns>An array of serial port names for the current computer.</returns>
        public static string[] GetPortNames() => RJCP.IO.Ports.SerialPortStream.GetPortNames();

        public static void RemoveEvents<T>(T target, string eventName, Action<Delegate> remove) where T : class
        {
            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (string.IsNullOrEmpty(eventName))
            {
                throw new ArgumentNullException(nameof(target));
            }

            var field = typeof(T).GetField(eventName, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            var fieldValue = field.GetValue(target) as Delegate;

            remove(fieldValue);
        }
    }
}
