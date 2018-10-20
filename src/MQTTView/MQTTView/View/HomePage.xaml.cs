using MQTTView.Helpers;
using MQTTView.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MQTTView.View
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class HomePage : ContentPage
	{
		public HomePage (string status)
		{
			InitializeComponent ();
            Title = "Home";
            txtstatus.Text = status;
            Carregar();
        }

        private async void btnConfig_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new Configuracao(new MQTT()));
        }

        private async void Carregar()
        {
            List<MQTT> configs = new List<MQTT>();
            if (await Storage.FileExistAsync("MQTTMobileConfig"))
            {
                var Json = await Storage.ReadAllTextAsync("MQTTMobileConfig");
                configs = JsonConvert.DeserializeObject<List<MQTT>>(Json);
            }
            Servidores.ItemsSource = configs.OrderBy(x=>x.Servidor);
        }

        private async void Servidores_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var selecionado = (MQTT)e.SelectedItem; 
            await Navigation.PushModalAsync(new Configuracao(selecionado));
        }

        private void btnAtualizar_Clicked(object sender, EventArgs e)
        {
            Carregar();
        }

        private async void btnTestar_Clicked(object sender, EventArgs e)
        {
            try
            {
                List<MQTT> configs = new List<MQTT>();
                if (await Storage.FileExistAsync("MQTTMobileConfig"))
                {
                    var Json = await Storage.ReadAllTextAsync("MQTTMobileConfig");
                    configs = JsonConvert.DeserializeObject<List<MQTT>>(Json);
                }
                MQTT selecionado = configs.Where(x => x.Usar).First();

                MqttClient client = new MqttClient(selecionado.Servidor, selecionado.Porta, false, null, null, MqttSslProtocols.None);
                client.Connect(Guid.NewGuid().ToString(), selecionado.Usuario, selecionado.Senha);
                txtstatus.Text = "Servidor Conectado";
            }
            catch (Exception)
            {
                txtstatus.Text = "Nao foi Possivel Conectar ao Servidor";
            }
        }
    }
}