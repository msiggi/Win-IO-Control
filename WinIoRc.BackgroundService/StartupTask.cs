using NLog;
using NLog.Config;
using NLog.Targets;
using Restup.Webserver.Attributes;
using Restup.Webserver.Http;
using Restup.Webserver.Models.Contracts;
using Restup.Webserver.Models.Schemas;
using Restup.Webserver.Rest;
using System;
using System.IO;
using Windows.ApplicationModel.Background;
using WinIoRc.HardwareHandler;

namespace WinIoRc.BackgroundService
{
    public sealed class StartupTask : IBackgroundTask
    {
        private BackgroundTaskDeferral _deferral;
        private Logger _logger;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            _deferral = taskInstance.GetDeferral();

            // Log-Config:
            Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            var config = new LoggingConfiguration();
            var fileTarget = new FileTarget();
            config.AddTarget("file", fileTarget);
            fileTarget.FileName = Path.Combine(storageFolder.Path, "${shortdate}.log");
            fileTarget.Layout = "${message}";

            var rule = new LoggingRule("*", LogLevel.Trace, fileTarget);
            config.LoggingRules.Add(rule);

            LogManager.Configuration = config;

            _logger = LogManager.GetCurrentClassLogger();
            _logger.Trace("WinIoRc.BackgroundService started");

            HardwareHandler.GpioHandler.ErrorOccured += OnErrorOccured;
            HardwareHandler.GpioHandler.Init();

            var restRouteHandler = new RestRouteHandler();

            try
            {
                restRouteHandler.RegisterController<ParameterController>();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error starting WinIoRc.BackgroundService");
            }

            var configuration = new HttpServerConfiguration()
              .ListenOnPort(8800)
              .RegisterRoute("api", restRouteHandler)
              .EnableCors();

            var httpServer = new HttpServer(configuration);

            await httpServer.StartServerAsync();
        }
        private void OnErrorOccured(object sender, ErrorEventArgs e)
        {
            _logger.Error(e.Exception, e.MessageText);
        }
    }


    public sealed class DataReceived
    {
        public int Gpio { get; set; }
        public bool State { get; set; }
    }

    [RestController(InstanceCreationType.Singleton)]
    public sealed class ParameterController
    {
        [UriFormat("/setgpio/{gpioPin}/{state}")]
        public IGetResponse SetGpio(int gpioPin, bool state)
        {
            try
            {
                HardwareHandler.GpioHandler.Set(gpioPin, state);

                return new GetResponse(GetResponse.ResponseStatus.OK, new DataReceived() { Gpio = gpioPin, State = state });
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [UriFormat("/togglegpio/{gpioPin}")]
        public IGetResponse ToggleGpio(int gpioPin)
        {
            try
            {
               var state = HardwareHandler.GpioHandler.Toggle(gpioPin);

                return new GetResponse(GetResponse.ResponseStatus.OK, new DataReceived() { Gpio = gpioPin, State = state });
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}