using Microsoft.AspNetCore.ResponseCompression;
using vNekoChatUI.Web.BlazorServer.Hubs;
using vNekoChatUI.Web.BlazorServer.WebUI.MainViewModel;
using vNekoChatUI.Web.BlazorServer.WebUI.Service;

namespace vNekoChatUI.Web.BlazorServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //
            var builder = WebApplication.CreateBuilder();

            {
                //ChatRoom×¨ÓÃ
                builder.Services.AddSingleton<IChatRoomModel, ChatRoomModel>();
                builder.Services.AddSingleton<JsonService>();
                builder.Services.AddSingleton<MessageService>();
                builder.Services.AddSingleton<TimerService>();
            }

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();
            builder.Services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });//¡ï

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
            }


            app.UseStaticFiles();

            app.UseRouting();

            app.MapBlazorHub();
            app.MapHub<ChatHub>("/chathub");//¡ï
            app.MapFallbackToPage("/_Host");

            app.Run();
        }
    }
}