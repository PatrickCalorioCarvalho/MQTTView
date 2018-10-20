using MQTTView.Helpers;
using MQTTView.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MQTTView.View
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DetailPage : ContentPage
	{
        MqttClient client;
        int Dimension = Device.RuntimePlatform == Device.iOS ||
                Device.RuntimePlatform == Device.Android ? 240 : 280;
        public DetailPage (string titulo)
		{
			InitializeComponent ();
            this.Title = titulo;
            GetConfig(titulo);
        }
        public async void GetConfig(string titulo)
        {
            List<MQTT> configs = new List<MQTT>();
            if (await Storage.FileExistAsync("MQTTMobileConfig"))
            {
                var Json = await Storage.ReadAllTextAsync("MQTTMobileConfig");
                configs = JsonConvert.DeserializeObject<List<MQTT>>(Json);
            }
            MQTT selecionado = configs.Where(x => x.Usar).First();

            client = new MqttClient(selecionado.Servidor, selecionado.Porta, false, null, null, MqttSslProtocols.None);
            client.Connect(Guid.NewGuid().ToString(), selecionado.Usuario, selecionado.Senha);
            client.Subscribe(new string[] { $"/{titulo}/#" }, new byte[] { 0 });
            client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
        }

        private void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            Regex rgx = new Regex(@"/[A-Za-z0-9]*/[A-Za-z0-9]*/[A-Za-z0-9]*");
            if (rgx.IsMatch(e.Topic))
            {
                var conteudo = Encoding.UTF8.GetString(e.Message);
                Device.BeginInvokeOnMainThread(() =>
                {
                    GerarQuadrado(e.Topic, conteudo);
                });
            }
        }

        void GerarQuadrado(string topico,string body)
        {
            var haed = topico.Split('/')[2];
            var tipo = topico.Split('/')[3];
            var atual = flexLayout.Children.ToList();
            Xamarin.Forms.View excluirAtual = null;

            foreach (var item in atual)
            {
                flexLayout.Children.Remove(item);
                if (item.ClassId == haed)
                    excluirAtual = item;
            }

            if (excluirAtual != null)
                atual.Remove(excluirAtual);


            Frame quadrado = new Frame()
            {
                Padding = new Thickness(5, 5, 5, 5),
                WidthRequest = Dimension,
                HeightRequest = Dimension,
                OutlineColor = Color.Silver
            };
            quadrado.ClassId = haed;
            StackLayout SL = new StackLayout();
            Label t = new Label();
            t.Text = haed;
            t.FontSize = 20;
            t.HorizontalOptions = LayoutOptions.CenterAndExpand;
            Label c = new Label();
            if (tipo == "switch")
                c.Text = body == "0" ? "OFF" : "ON";
            else
                c.Text = body;

            c.HorizontalOptions = LayoutOptions.CenterAndExpand;
            c.VerticalOptions = LayoutOptions.CenterAndExpand;
            c.FontSize = 70;
            c.ClassId = body;
            Label time = new Label();
            time.Text = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
            time.HorizontalOptions = LayoutOptions.CenterAndExpand;
            time.VerticalOptions = LayoutOptions.EndAndExpand;
            SL.Children.Add(t);
            SL.Children.Add(c);
            SL.Children.Add(time);
            quadrado.Content = SL;
            if(tipo == "switch")
            {
                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += (s, e) => {
                    if(body == "0" )
                    {
                        client.Publish(topico,
                              Encoding.UTF8.GetBytes("1"),
                              MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE,
                              true);
                    }else if(body == "1")
                    {
                        client.Publish(topico,
                          Encoding.UTF8.GetBytes("0"),
                          MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE,
                          true);
                    }
                };
                quadrado.GestureRecognizers.Add(tapGestureRecognizer);
            }
            atual.Add(quadrado);
            var order = atual.OrderBy(x => x.ClassId);
            foreach (var item in order)
            {
                flexLayout.Children.Add(item);
            }
            
        }
    }
}