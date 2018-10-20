using MQTTView.Helpers;
using MQTTView.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MQTTView.View
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MenuPage : MasterDetailPage
    {
        List<string> ItemsMenu = new List<string>();
        string status = string.Empty;
        public MenuPage ()
		{
            InitializeComponent();
            getConfig();
            Detail = new NavigationPage(new HomePage(status))
            {
                BarTextColor = Color.White,
                BarBackgroundColor = Color.FromHex("#171336")
            };
            IsPresented = false;
        }

        private void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            Device.BeginInvokeOnMainThread(() => {
                var titulo = e.Topic.Split('/')[1];
                if (!ItemsMenu.Contains(titulo))
                {
                    ItemsMenu.Add(titulo);
                    Button itemmenu = new Button();
                    itemmenu.Text = titulo;
                    itemmenu.BackgroundColor = Color.Transparent;
                    itemmenu.TextColor = Color.WhiteSmoke;
                    itemmenu.Clicked += ButtonMenu_Clicked;
                    itemmenu.FontSize = 20;
                    itemmenu.BackgroundColor = Color.FromHex("#BF171336");
                    PainelMenu.Children.Add(itemmenu);
                }
            });
        }

        private async void getConfig()
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
                client.Subscribe(new string[] { "#" }, new byte[] { 0 });
                client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
                status = "Servidor Conectado";

            }
            catch (Exception)
            {
                status = "Nao foi Possivel Conectar ao Servidor";
            }

        }

        private void ButtonMenu_Clicked(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            Detail = new NavigationPage(new DetailPage(b.Text))
            {
                BarTextColor = Color.White,
                BarBackgroundColor = Color.FromHex("#171336")
            };
            IsPresented = false;
        }

        void OnImageNameTapped(object sender, EventArgs args)
        {
            try
            {
                Detail = new NavigationPage(new HomePage(status))
                {
                    BarTextColor = Color.White,
                    BarBackgroundColor = Color.FromHex("#171336")
                };
                IsPresented = false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}