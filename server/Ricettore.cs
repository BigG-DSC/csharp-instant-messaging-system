using Server.Servizi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Server
{
  class Ricettore
  {
    /* VARIABILI */
    public TcpClient TcpClient { get; private set; } // Posizione del Client
    private Thread ascoltaClient; // Thread per attività 
    private NetworkStream canale; // Canale trasmissione dati
    private StreamReader lettore; // Lettore del canale
    private StreamWriter scrittore; // Scrittore del canale

    /* COSTRUTTORE */
    public Ricettore(TcpClient client)
    {
      /* Memorizzazione della posizione del Client */
      TcpClient = client;

      /* Apertura Canale e assegnazione strumenti di Lettura e Scrittura nello stesso */
      canale = client.GetStream();
      lettore = new StreamReader(canale);
      scrittore = new StreamWriter(canale);

      /* Creazione e Avvio di un Thread dedicato alla comunicazione Client Remoto associato */
      ascoltaClient = new Thread(ricezione);
      ascoltaClient.Start();
    }

    /* METODI */
    /* Metodo per la ricezione dei dati nel canale dal Client Remoto*/
    private void ricezione()
    {
      bool condizione = true; // Condizione per il ciclo
      List<int> listaCaratteri = new List<int>(); // Lista dei caratteri acquisiti in ricezione
      int valore = new int(); // Variabile per valore acquisito nel canale
            
      /* Esegui le istruzioni se il Client è connesso             *
        * Interrompi il ciclo in caso di Disconnessione del Client */
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
            for (int i = 0 ; i < listaCaratteri.Count ; i++)
              messaggio.Append(Convert.ToChar(listaCaratteri[i]));

            /* Invio del messaggio ricevuto per l'elaborazione */
            elaboraRichiesta(messaggio.ToString());

            /* Svuoto la lista degli interi */
            listaCaratteri.Clear();
          }
        }
        catch (IOException) // Se il Client si disconnette
        {
          /* Rimozione Client dal Dizionario */
          Connessione.Istanza.clientDisconnesso(this);
          /* Uscita dal Ciclo */
          condizione = false;
        }
      } while (condizione);
    }

    /* Metodo per l'elaborazione della richiesta ricevuta dal Client Remoto */
    private void elaboraRichiesta(string data)
    {
      try
      {
        /* Lettura del comando */
        var pacchetto = new Pacchetto(data);
        string comando = pacchetto.Comando;

        /* Conversione del messaggio in lista */
        Convertitore converti = new Convertitore();
        var lista = converti.stringaALista(pacchetto.Contenuto);

        /* Smistamento della richiesta al servizio */
        switch (comando)
        {
          case "Login":
            new Login().fornisciServizio(this, lista);
            break;
          case "Registrazione":
            new Registrazione().fornisciServizio(this, lista);
            break;
          case "AggiungiAmico":
            new Amicizia().fornisciServizio(this, lista);
            break;
          case "Messaggio":
            new Messaggistica().fornisciServizio(this, lista);
            break;
          case "ListaAmici":
            new Lista().fornisciServizio(this, lista);
            break;
          case "ListaUtentiOnline":
            new ListaUtenti().fornisciServizio(this, lista);
            break;
          case "Segreteria":
            new Segreteria().fornisciServizio(this, lista);
            break;
          default:
            Interfaccia.stampaMessaggio("Pacchetto Invalido");
            break;
        }
      }
      catch (Exception)
      {
        Interfaccia.stampaMessaggio(" (!) Il pacchetto ricevuto è Invalido\n");
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
