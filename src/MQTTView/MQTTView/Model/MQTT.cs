using System;
using System.Collections.Generic;
using System.Text;

namespace MQTTView.Model
{
    public class MQTT
    {
        public MQTT()
        {
            ID = Guid.NewGuid();
            Servidor = string.Empty;
            Porta = 1883;
            Usuario = string.Empty;
            Senha = string.Empty;
            Usar = false;
        }
        public Guid ID { get; set; }

        public string Servidor { get; set; }

        public int Porta { get; set; }

        public string Usuario { get; set; }

        public string Senha { get; set; }

        public bool Usar { get; set; }

        public override string ToString()
        {
            return Servidor;
        }
    }
}
