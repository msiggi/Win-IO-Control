using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;
using Windows.System.Threading;
using Restup.Webserver.Rest;
using System.Threading.Tasks;
using Restup.Webserver.Models.Schemas;
using Restup.Webserver.Attributes;
using Restup.Webserver.Models.Contracts;
using Restup.Webserver.Http;

namespace WinIoRc.BackgroundService
{
    public sealed class StartupTask : IBackgroundTask
    {
        private BackgroundTaskDeferral _deferral;


        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            _deferral = taskInstance.GetDeferral();

            var restRouteHandler = new RestRouteHandler();
            restRouteHandler.RegisterController<ParameterController>();

            var configuration = new HttpServerConfiguration()
              .ListenOnPort(8800)
              .RegisterRoute("api", restRouteHandler)
              .EnableCors();

            var httpServer = new HttpServer(configuration);
            await httpServer.StartServerAsync();

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
        [UriFormat("/setgpio/{gpio}/{state}")]
        public IGetResponse Setgpio(int gpio, bool state)
        {
            return new GetResponse(GetResponse.ResponseStatus.OK, new DataReceived() { Gpio = gpio, State = state });
        }
    }
}
