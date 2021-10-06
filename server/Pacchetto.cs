using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
  /* Classe per la generazione dei pacchetti */
  class Pacchetto
  {
    /* VARIABILI */
    /* I pacchetti contengono i parametri                *
      * Comando   : Nome del servizio richiesto o offerto *
      * Contenuto : Contenuto devoluto                    */
    public string Comando { get; private set; }   
    public string Contenuto { get; private set; } 

    /* COSTRUTTORI */
    /* 2 Costruttori da utilizzare secondo l'uso */
    /* Costruzione di un pacchetto direttamente con i parametri assegnati */
    public Pacchetto(string comando, string contenuto)
    {
      Comando = comando;
      Contenuto = contenuto;
    }

    /* Costruttore di un pacchetto attraverso una stringa */
    public Pacchetto(string dati)
    {
      int separatore = dati.IndexOf(":", StringComparison.Ordinal);
      Comando = dati.Substring(0, separatore);
      Contenuto = dati.Substring(Comando.Length + 1);
    }
  }
}
