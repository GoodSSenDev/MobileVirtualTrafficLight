using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace VirtualTrafficLightServer
{
    public class Startup
    {
        ChannelManager _channelManager = new ChannelManager();
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ChannelManager>(provider => _channelManager);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseWebSockets();

            app.Use(async (context, next) =>
            {

                if (context.WebSockets.IsWebSocketRequest)
                {
                    using (WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync())
                    {

                        var sourceFinishedTcs = new TaskCompletionSource<bool>();
                        _channelManager.AddConnection(webSocket, context.Request.Path, sourceFinishedTcs);

                        //to keep this pipeline running for the websocket.
                        await sourceFinishedTcs.Task;
                    }
                }
                else
                {
                    //if not websocket request than it is bad Request
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                }

            });

        }
    }
}
