using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
  class Connessione
  {
    /* VARIABILI */
    private static Connessione istanza; // Istanza Singleton
    public string indirizzoIP { get; private set; } // Proprietà per l'indirizzo IP
    public int porta { get; private set; } // Proprietå per la porta utilizzata
    private TcpListener ascoltatore; // Listener in ascolto delle connessioni
    private Thread accettaClient; // Thread che si occupa di accettare i client

    /* Dizionario contenente i dati relativi alle connessioni stabilite     *
     * Campo chiave : Oggetto Cliente                                       *
     * Campo valore : Stringa Username (solo in caso di accesso al Servizio */
    public Dictionary<Ricettore, string> ClientConnessi { get; private set; }

    /* COSTRUTTORE */
    private Connessione()
    {
      /* Assegnazione Indirizzo IP e Porta del Server */
      indirizzoIP = "127.0.0.1";
      porta = 5302;

      /* Assegnazione di Dizionario */
      ClientConnessi = new Dictionary<Ricettore,string>();

      /* Assegnazione e Avvio del Listener */
      try
      {
        ascoltatore = new TcpListener(IPAddress.Parse(indirizzoIP), porta);
        ascoltatore.Start();

        /* Assegnazione e Avvio del thread relegato ad accettare i Client */
        accettaClient = new Thread(connessioniInEntrata);
        accettaClient.Start();
      }
      catch (SocketException)
      {
        Interfaccia.stampaMessaggio(" <!> Non è possibile stabilire la connessione <!>\n");
        Interfaccia.stampaMessaggio(" <!> Controlla che un'istanza del Server non sia gia' Online <!>\n");
        Interfaccia.stampaMessaggio(" -> Chiusura automatica in 10 secondi");
        Thread.Sleep(10000);
        Environment.Exit(0);
      }
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
   
    /* Metodo per il controllo delle connessioni in ingresso */
    private void connessioniInEntrata()
    {
      do
      {
        try
        {
          /* Con una chiamata Bloccante attendo la connessione di nuovi Client *
            * Creo un oggetto che fungerà da STUB                               */
          Ricettore client = new Ricettore(ascoltatore.AcceptTcpClient());

          clientConnesso(client); // Aggiungo il client nel dizionario
        }
        catch (Exception)
        {
          Interfaccia.stampaMessaggio(" (!) Si e' verificato un errore con l'accettazione di un client\n");
        }
      } while (true);
    }

    /* Metodo per l'inserimento del Client connesso nel Dizionario */
    private void clientConnesso(Ricettore client)
    {
      /* Se il client non è già stato inserito nel Dizionario, Inseriscilo    *
        * Se il client risulta già nel dizionario, segnala un malfunzionamento */
      if (!ClientConnessi.ContainsKey(client))
      {
        ClientConnessi.Add(client, null);
        Interfaccia.stampaMessaggio(" (Connesso) Client Connesso: " + client.TcpClient.Client.RemoteEndPoint + "\n"); 
      }
      else
        Interfaccia.stampaMessaggio("Errore in fase di inserimento nel dizionario");           
    }

    /* Metodo per la rimozione di un Client dal dizionario              *
      * public perchè l'avviso di rimozione giungerà dall'oggetto Client */
    public void clientDisconnesso(Ricettore client)
    {
      /* Lock per maneggiare correttamente il Dizionario */
      lock (ClientConnessi)
        /* Rimozione del Client se esisistente, altrimenti errore */
        if (ClientConnessi.ContainsKey(client))
        {
          ClientConnessi.Remove(client);
          Interfaccia.stampaMessaggio(" (Disconnesso) Client Disconnesso: " + client.TcpClient.Client.RemoteEndPoint + "\n");
        }
        else
          Interfaccia.stampaMessaggio("Il Client" + client.TcpClient.Client.RemoteEndPoint + "ha richiesto la rimozione, ma non esiste!");
    }

    /* Metodo per l'aggiornamento del dizionario se il Client accede al Servizio*/
    public bool utenteOnline(Ricettore client, string username)
    {
      bool esito;

      /* Lock per maneggiare correttamente il Dizionario */
      lock (ClientConnessi)
        /* Aggiornamento del Client se esistente */
        if (ClientConnessi.ContainsKey(client))
          if (!ClientConnessi.ContainsValue(username))
          {
            ClientConnessi[client] = username;
            esito = true;
          }
          else
            esito = false;
        else
          esito = false;

      return esito;
    }

    /* Metodo per la ricerca nel Dizionario per valore (non per chiave) */
    public Ricettore cercaInDizionario(string username)
    {
      Ricettore clientRitorno = null;

      /* Lock per maneggiare correttamente il Dizionario */
      lock (ClientConnessi)
        /* Ricerco per valore */
        if (ClientConnessi.ContainsValue(username))
          /* Estraggo la chiave */
          clientRitorno = ClientConnessi.FirstOrDefault(x => x.Value == username).Key;         

      return clientRitorno;
    }
  }
}
