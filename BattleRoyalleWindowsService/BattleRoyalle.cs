using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using System.Management.Automation;

namespace BattleRoyalleWindowsService
{
    public partial class BattleRoyalle : ServiceBase
    {
        private static HubConnection connection;

        public BattleRoyalle()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
        }

        protected override void OnStop()
        {
        }

        public async Task escutarComandosAsync()
        {
            connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:44325/commandhub")
                .Build();

            await connection.StartAsync();

            MaquinaService maquinaService = new MaquinaService();

            var resultado = await maquinaService.solicitarTokenAsync();


            connection.On<string, string>("ReceberComando",
            (string user, string message) =>
            {
                Console.WriteLine($"Message from {user}: {message}");

                var list = message.Split(' ');

                PowerShell ps = PowerShell.Create();
                ps.AddCommand(list[0]);
                ps.AddArgument(list[1]);

                // Execute PowerShell script
                var results = ps.Invoke();
            });

            Console.ReadLine();
        }
    }
}
