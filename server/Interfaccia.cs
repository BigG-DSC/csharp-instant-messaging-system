using System;
using System.Threading;

namespace Server
{
  /* Tutta la comunicazione con l'amministratore è contenuta in questa classe */
  static class Interfaccia
  {
    /* Funzione Main del programma */
    static int Main(string[] args)
    {
      /* Creazione evento di cattura CTRL-C */
      Console.CancelKeyPress += new ConsoleCancelEventHandler(trappola);

      /* Messaggio di Benvenuto all'Amministratore */
      testoApertura();

      /* Stabilimento connessione Server */
      connessioneServer();

      return 0;
    }

    /* METODI */
    /* Metodo contenente il messaggio di Benvenuto */
    static void testoApertura()
    {
      Console.WriteLine("\t\t  * * * * * * * * * * * * * * * * * * * * * *");
      Console.WriteLine("\t\t  *   Progetto di Ingegneria del Software   *");
      Console.WriteLine("\t\t  *    Software Messaggistica Istantanea    *");
      Console.WriteLine("\t\t  *    Bigini Gioele - Matricola: 261990    *");
      Console.WriteLine("\t\t  *          Versione Software: 1.0         *");
      Console.WriteLine("\t\t  * * * * * * * * * * * * * * * * * * * * * *\n\n");
    }

    /* Metodo relegato allo stabilimento della connessione del Server presso un IP e una Porta*/
    static void connessioneServer()
    {
      Console.WriteLine(" -> Stabilisco la connessione al Database\n");
      /* Creazione dell'istanza di Database */
      Database database = Database.Istanza;
      Console.WriteLine(" <> Connessione al Database stabilita\n");

      Console.WriteLine(" -> Stabilisco la connessione del Server\n");
      /* Connessione è una classe Singleton con Lazy Initialization     *
          * Pertanto la prima chiamata istanza l'oggetto (una volta sola ) */
      Console.WriteLine(" <> Connessione stabilita presso {0}:{1}\n\n", 
                                        Connessione.Istanza.indirizzoIP, 
                                        Connessione.Istanza.porta);

      Console.WriteLine("\t\t *  Benvenuto Amministratore - Server Online  *\n\n");
      Console.WriteLine(" -> Ricorda! La procedura di spegnimento si attiva con CTRL-C\n");
      Console.WriteLine(" -> Resto in attesa di Connessioni...\n\n");
    }

    /* Metodo eseguito in seguito alla cattura della combinazione CTRL-C */
    static void trappola(object sender, ConsoleCancelEventArgs pressione)
    {
      /* Messaggio a schermo di conferma spegimento */
      Console.Clear();
      testoApertura();
      Console.WriteLine(" <!> Rilevata richiesta di spegnimento del Server <!>\n");
      Console.WriteLine(" -> Vuoi davvero spegnere il Server? [S/N]\n");
          
      ConsoleKeyInfo cki = new ConsoleKeyInfo();

      /* Cattura della scelta dell'utente                                                   *
       * Questo procedimento consente di prelevare gli input senza che l'utente prema invio *
       * Se l'utente preme S, comincia la procedura di spegnimento                          *
       * Se l'utente preme N, la procedura viene interrotta,il server continua a funzionare */
      while (cki.Key != ConsoleKey.S && cki.Key != ConsoleKey.N)
      {
        cki = Console.ReadKey(true);

        switch (cki.Key)
        {
          /* S */
          case ConsoleKey.S:
              Console.WriteLine(" <!> Attendere prego. Procedura di spegnimento avviata <!>\n");
              Console.WriteLine(" -> Fatto. Arrivederci Amministratore =D");
              Thread.Sleep(1000);
              Environment.Exit(0);
              break;
          /* N */
          case ConsoleKey.N:
              Console.Clear();
              testoApertura();
              Console.WriteLine(" -> Stabilisco la connessione del Server\n");
              Console.WriteLine(" <> Connessione stabilita presso {0}:{1}\n\n", 
                                                Connessione.Istanza.indirizzoIP, 
                                                Connessione.Istanza.porta);
              Console.WriteLine("\t\t *  Benvenuto Amministratore - Server Online  *\n\n");
              Console.WriteLine(" -> Ricorda! La procedura di spegnimento si attiva con CTRL-C\n");
              Console.WriteLine(" -> Resto in attesa di Connessioni...\n\n");
              pressione.Cancel = true;
              break;
        }
      }
    }

    /* Metodo per la stampa di un eccezione in qualsiasi zona del codice */
    public static void stampaMessaggio(string messaggio)
    {
      Console.WriteLine(messaggio);
    }
  }
}
