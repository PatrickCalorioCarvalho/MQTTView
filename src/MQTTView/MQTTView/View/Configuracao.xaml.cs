using MQTTView.Helpers;
using MQTTView.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MQTTView.View
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Configuracao : ContentPage
	{
        MQTT mqttConfig;
        public Configuracao (MQTT mqtt)
		{
			InitializeComponent ();
            mqttConfig = mqtt;
            txtPorta.Text = mqtt.Porta.ToString();
            txtSenha.Text = mqtt.Senha;
            txtServidor.Text = mqtt.Servidor;
            txtUsuario.Text = mqtt.Usuario;
            ckUsar.IsToggled = mqttConfig.Usar;
        }

        private async void btnSalvar_Clicked(object sender, EventArgs e)
        {
            mqttConfig.Porta = int.Parse(txtPorta.Text);
            mqttConfig.Servidor = txtServidor.Text;
            mqttConfig.Usuario = txtUsuario.Text;
            mqttConfig.Senha = txtSenha.Text;
            mqttConfig.Usar = ckUsar.IsToggled;
            List<MQTT> configs = new List<MQTT>();

            if (await Storage.FileExistAsync("MQTTMobileConfig"))
            {
                var Json = await Storage.ReadAllTextAsync("MQTTMobileConfig");
                configs = JsonConvert.DeserializeObject<List<MQTT>>(Json);
            }

            MQTT remover = null;

            foreach (var item in configs)
            {
                if (mqttConfig.Usar)
                    item.Usar = false;
                if (mqttConfig.ID == item.ID)
                    remover = item;
            }

            if (remover != null)
                configs.Remove(remover);

            configs.Add(mqttConfig);
            await Storage.WriteTextAllAsync("MQTTMobileConfig", JsonConvert.SerializeObject(configs));
            await DisplayAlert("Configuração", "Servidor MQTT salvo com Sucesso!!!!!!", "OK");
            await Navigation.PopModalAsync();
        }
    }
}