using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
 
namespace Client
{
  class Connessione
  {
    /* VARIABILI */
    private static Connessione istanza; // Istanza Singleton
    public TcpClient connessioneTCP { get; private set; } // Connessione TCP
    private NetworkStream canale; // Canale di comunicazione
    private StreamReader lettore; // Lettore del canale
    private StreamWriter scrittore; // Scrittore del canale
    private Thread ascoltaServer; // Thread in ascolto al Server

    /* Variabile nella quale viene depositata la risposta ricevuta SINCRONA */
    public string chiamataSincrona { get; private set; }
    /* Variabile nelle quali vengono depositate le risposte ricevute ASINCRONE */
    public Queue<Pacchetto> chiamateAsincrone = new Queue<Pacchetto>();

    /* COSTRUTTORE */
    private Connessione()
    {
      /* Creazione della connessione TCP con il Server */
      connessioneTCP = new TcpClient();
      connessioneTCP.Connect(IPAddress.Parse("127.0.0.1"), 5302);

      /* Assegnamento strumenti di dialogo nel canale stabilito con il Server */
      canale = connessioneTCP.GetStream();
      scrittore = new StreamWriter(canale);
      lettore = new StreamReader(canale);

      /* Creazione e Avvio di un Thread dedicato al'ascolto del Server */
      ascoltaServer = new Thread(ricezione);
      ascoltaServer.Start();
    }

    /* METODI */
    /* Metodo per creazione dell'istanza Singleton*/
    public static Connessione Istanza
    {
      get
      {
        if (istanza == null)
        {
          istanza = new Connessione();
        }
        return istanza;
      }
    }

    /* Metodo per la ricezione dei dati nel canale dal Server Remoto */
    private void ricezione()
    {
      bool condizione = true; // Condizione per il ciclo
      List<int> listaCaratteri = new List<int>(); // Lista dei caratteri acquisiti in ricezione
      int valore = new int(); // Variabile per valore acquisito nel canale

      /* Esegui le istruzioni se il Server è connesso             *
        * Interrompi il ciclo in caso di Disconnessione del Server */
      do
      {
        try
        {
          /* Acquisizione del valore */
          valore = lettore.Read();

          /* Se non è il valore terminatore */
          if (valore != 0)
            listaCaratteri.Add(valore); // Aggiunta del valore alla lista
          /* Altrimenti, è il valore terminatore, composizione del messaggio ricevuto */
          else
          {
            var messaggio = new StringBuilder();

            /* Costruzione del messaggio ricevuto - Conversione degli interi in caratteri */
            for (int i = 0; i < listaCaratteri.Count; i++)
                messaggio.Append(Convert.ToChar(listaCaratteri[i]));

            /* Invio del messaggio ricevuto per l'elaborazione */
            elaboraRichiesta(messaggio.ToString());

            /* Svuoto la lista degli interi */
            listaCaratteri.Clear();
          }
        }
        catch (IOException) // Se il Server si disconnette
        {
          /* Esegui una routine */
          Interfaccia.stampaErrore("Il Server e' OFFLINE");
          /* Uscita dal Ciclo */
          condizione = false;
        }
      } while (condizione);
    }

    /* Metodo per l'elaborazione dei messaggi ricevuti dal Server */
    private void elaboraRichiesta(string richiesta)
    {
      /* Generazione di un pacchetto interpretabile */
      Pacchetto pacchetto = new Pacchetto(richiesta);
      /* Estrazione del comando */
      string comando = pacchetto.Comando;

      /* Comportamento relativo al comando ricevuto */
      switch (comando)
      {
        /* Chiamate sincrone */
        case "Login":
        case "Registrazione":
        case "AggiungiAmico":
        case "Messaggio":
        case "ListaAmici":
        case "ListaUtentiOnline":
        case "Segreteria":
            chiamataSincrona = pacchetto.Contenuto;
            Interfaccia.liberaMutex();
            break;

        /* Chiamate asincrone */
        case "InoltroAmicizia":
        case "InoltroMsg":
            chiamateAsincrone.Enqueue(pacchetto);
            break;
                
        /* Se il server spedisce pacchetti anomali */
        default:
            Interfaccia.stampaErrore("Il server ti ha inviato un pacchetto anomalo");
            break;
      }
    }

    /* Questo Metodo, con il successivo, permette l'invio dei pacchetti nel NetworkStream */
    public void InviaPacchetto(Pacchetto pacchetto)
    {
      /* Invio nel NetworkStream */
      scrittore.Write("{0}:{1}\0", pacchetto.Comando, pacchetto.Contenuto);
      /* Svuoto lo Stream */
      scrittore.Flush();
    }
  }
}
