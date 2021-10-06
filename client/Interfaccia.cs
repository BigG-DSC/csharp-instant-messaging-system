using System;
using System.IO;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text.RegularExpressions;

namespace Client
{
  static class Interfaccia
  {
    /* VARIABILI */
    /* Istanza della classe collegamento */
    static Connessione collegamento;
    /* Istanza della classe richiesta */
    static Richiesta richiesta = new Richiesta();
    /* Mutex per la corretta sincronizzazione chiamate sincrone */
    static Semaphore mutex = new Semaphore(0,1); 

    /* Avvio per Argomenti                       *
     * Argomento mancante - Avvio Normale        *
     * Argomento presente - Errore               */
    static void Main(string[] args)
    {
      /* Testo di Apertura */
      testoApertura();

      /* Controllo del numero di argomenti */
      if (args.Length < 1)
      {
        /* Stabilimento della connessione con il Server */
        stabilisciConnessione("127.0.0.1", 5302);

        /* Lancia il Servizio di Login e Registrazione */
        login();
      }
      else
      {
        /* Messaggio d'errore */
        Console.WriteLine(" <!> Hai inserito argomenti non necessari <!>");
        Console.WriteLine(" <!> Premi Invio e riavvia l'applicazione senza argomenti <!>");
        Console.ReadLine();
      }

      /* Chiusura ambiente */
      Environment.Exit(0);
    }

    /* METODI */
    /* Metodo per la stampa del messaggio di Benvenuto a schermo */
    static void testoApertura()
    {
      Console.WriteLine("\t\t  * * * * * * * * * * * * * * * * * * * * * *");
      Console.WriteLine("\t\t  *   Progetto di Ingegneria del Software   *");
      Console.WriteLine("\t\t  *    Software Messaggistica Istantanea    *");
      Console.WriteLine("\t\t  *    Bigini Gioele - Matricola: 261990    *");
      Console.WriteLine("\t\t  *          Versione Software: 1.0         *");
      Console.WriteLine("\t\t  * * * * * * * * * * * * * * * * * * * * * *\n\n");
    }

    /* Metodo per la stampa del messaggio di Connessione al Server */
    static void stabilisciConnessione(string indirizzoIP, int porta)
    {
      try
      {
        Console.WriteLine(" -> Tento di stabilire la connessione con il Server\n");

        /* Istanziazione dell'oggetto Connessione (Singleton) */
        collegamento = Connessione.Istanza;

        Console.WriteLine(" <> Connessione stabilita\n\n");
      }
      catch (Exception)
      {
        Console.WriteLine(" (!) Impossibile connettersi al Server. Il Server è OFFLINE\n");
        Console.WriteLine(" -> Premere Invio per chiudere l'applicazione");
        Console.ReadLine();
        Environment.Exit(0);
      }
    }

    /* Metodo per la stampa dei Servizi di Accesso ai Servizi veri e propri */
    static void testoLogin()
    {
      Console.WriteLine("\t\t       *  Unisciti ora alla Community  *\n\n");
      Console.WriteLine(" 1. Effettua il Login");
      Console.WriteLine(" 2. Registrati ora!");
      Console.WriteLine(" 3. Chiudi l'Applicazione\n");
    }

    /* Metodo per la stampa dei Servizi di Accesso ai Servizi veri e propri *
      * I Servizi sono:                                                      *
      * - Login                                                              *
      * - Registrazione                                                      */
    static void login()
    {
      string username, // Stringa di memorizzazione dello Username dell'utente
              password; // Stringa di memorizzazione della Password dell'utente

      testoLogin(); // Stampa a schermo dei Servizi di Accesso

      /* Variabile che descrive i tasti premuti sulla tastiera */
      ConsoleKeyInfo pressione = new ConsoleKeyInfo(); 
            
      /* Ciclo che resta in attesa della pressione dei tasti [1,2,3] *
        * 1. Login                                                    *
        * 2. Registrazione                                            *
        * 3. Chiudi Applicazione                                      */
      while (pressione.Key != ConsoleKey.D1 && 
              pressione.Key != ConsoleKey.D2 && 
              pressione.Key != ConsoleKey.D3)
      {
        pressione = Console.ReadKey(true);

        switch (pressione.Key)
        {
            /* 1. Login */
            case ConsoleKey.D1:
                Console.Clear();
                /* Acquisizione dati da Utente */
                testoApertura();
                Console.WriteLine(" -> Tento di stabilire la connessione con il Server\n");
                Console.WriteLine(" <> Connessione stabilita\n\n");
                testoLogin();
                Console.WriteLine("\t\t\t\t  * Login *\n");
                Console.Write(" Inserisci la tua Username: ");
                username = Console.ReadLine();
                Console.Write(" Inserisci la tua Password: ");
                password = Console.ReadLine();

                /* Se l'inserimento rispetta le seguenti regole *
                  * 1. Limite 24 Caratteri                       *
                  * 2. Solo lettere maiuscole e minuscole        */
                if (Regex.IsMatch(username, @"^[a-zA-Z]+$") && 
                    Regex.IsMatch(password, @"^[a-zA-Z]+$") &&
                    username.Length <= 24 && 
                    password.Length <= 24)
                {
                  /* Chiama il Servizio */
                  richiesta.login(collegamento, username, password);

                  /* Attendi la risposta dal Server */
                  Console.WriteLine("\n -> Verifica delle credenziali di accesso\n");
                  mutex.WaitOne();

                  /* Se la risposta del Server non e' OK */
                  if (collegamento.chiamataSincrona != "OK")
                  {
                    /* Messaggio di Errore dal Server */
                    Console.WriteLine(" [ERRORE] {0}\n", collegamento.chiamataSincrona.ToString());
                    pressione = new ConsoleKeyInfo();
                  }
                  else
                      servizi(username); // Avvia i Servizi per il Client
                }
                else
                {
                  /* Messaggio di Errore */
                  Console.WriteLine("\n (!) Sono consentiti non piu' di 24 caratteri minuscoli e maiuscoli");

                  /* Reset della pressione */
                  pressione = new ConsoleKeyInfo();
                }
                break;

            /* 2. Registrazione */
            case ConsoleKey.D2:
                Console.Clear();
                testoApertura();
                Console.WriteLine(" -> Tento di stabilire la connessione con il Server\n");
                Console.WriteLine(" <> Connessione stabilita\n\n");
                testoLogin();
                /* Acquisizione dati da Utente */
                Console.WriteLine("\t\t\t      *  Registrazione *\n");
                Console.Write(" Inserisci la tua Username: ");
                username = Console.ReadLine();
                Console.Write(" Inserisci la tua Password: ");
                password = Console.ReadLine();

                /* Se l'inserimento (username e password) rispetta le seguenti regole: *
                  * 1. Limite 24 Caratteri                                              *
                  * 2. Solo lettere maiuscole e minuscole                               */
                if (Regex.IsMatch(username, @"^[a-zA-Z]+$") && 
                    Regex.IsMatch(password, @"^[a-zA-Z]+$") &&
                    username.Length <= 24 && 
                    password.Length <= 24)
                {
                  /* Chiama il Servizio */
                  richiesta.registrazione(collegamento, username, password);

                  /* Attendi la risposta dal Server */
                  Console.WriteLine("\n -> Verifica delle credenziali di accesso\n");
                  mutex.WaitOne();

                  /* Se la risposta del Server non e' OK */
                  if (collegamento.chiamataSincrona != "OK")
                      /* Messaggio di errore dal Server */
                      Console.WriteLine(" [ERRORE] {0}\n", collegamento.chiamataSincrona.ToString());
                  else
                      /* Messaggio registrazione avvenuta */
                      Console.WriteLine(" [OK] Sei registrato. Ora puoi effettuare il Login\n");
                }
                else
                  /* Messaggio di Errore */
                  Console.WriteLine("\n (!) Sono consentiti non piu' di 24 caratteri minuscoli e maiuscoli");

                /* Reset della pressione */
                pressione = new ConsoleKeyInfo();
                break;
                    
            /* 3. Chiusura del Programma */
            case ConsoleKey.D3:
                break;
                    
            /* Altra Pressione */
            default:
                Console.Clear();
                testoApertura();
                Console.WriteLine(" -> Tento di stabilire la connessione con il Server\n");
                Console.WriteLine(" <> Connessione stabilita\n\n");
                testoLogin();
                Console.WriteLine(" (!) Errore nella scelta del Servizio\n");
                break;
        }
      }
    }

    /* Metodo per la stampa dei Servizi di Accesso ai Servizi veri e propri */
    static void testoServizi(string username)
    {
      testoApertura();
      Console.WriteLine("\t\t\t\t  * Servizi *\n");
      Console.WriteLine(" [{0}]\n", username);
      Console.WriteLine(" 1. Segreteria");
      Console.WriteLine(" 2. Invia un messaggio");
      Console.WriteLine(" 3. Ottieni la lista dei tuoi amici");
      Console.WriteLine(" 4. Aggiungi un Amico");
      Console.WriteLine(" 5. Utenti Online");
      Console.WriteLine(" 6. Manuale");
      Console.WriteLine(" 7. Chiudi l'Applicazione\n");
    }

    /* Metodo per la stampa dei Servizi in seguito al Login *
     * I Servizi sono:                                      *
     * - Segreteria                                         *
     * - Invia un messaggio                                 *
     * - Ottieni la lista dei tuoi amici                    *
     * - Aggiungi un amico                                  *
     * - Utenti Online                                      *
     * - Manuale                                            */
    static void servizi(string username)
    {
      /* Thread che si occupa di stampare a video le chiamate asincrone *
       * Esso viene interrotto nel momento in cui un utente accede ad   *
       * un servizio e riattivato solo in seguito                       */
      Thread controllaCoda = new Thread(stampaChiamataAsincrona);
      controllaCoda.Start();

      /* Messaggio a schermo dei Servizi */
      Console.Clear();
      testoServizi(username);
            
      /* Variabile che descrive i tasti premuti sulla tastiera */
      ConsoleKeyInfo pressione = new ConsoleKeyInfo();

      /* Istanza di Convertitore per convertire le stringhe */
      Convertitore converti = new Convertitore();
      /* Lista per il contenuto convertito in liste */
      List<string> lista = new List<string>();

      /* Ciclo che resta in attesa della pressione dei tasti [1,2,3,4,5] *
       * 1. Segreteria                                                   *
       * 2. Invia un Messaggio                                           *
       * 3. Lista degli Amici                                            *             
       * 4. Aggiungi un amico                                            *
       * 5. Utenti Online                                                *
       * 6. Manuale                                                      *
       * 7. Chiudi Applicazione                                          */
      while (pressione.Key != ConsoleKey.D1 && 
             pressione.Key != ConsoleKey.D2 &&
             pressione.Key != ConsoleKey.D3 && 
             pressione.Key != ConsoleKey.D4 &&
             pressione.Key != ConsoleKey.D5 &&
             pressione.Key != ConsoleKey.D6 &&
             pressione.Key != ConsoleKey.D7)
      {
        pressione = Console.ReadKey(true);

        switch (pressione.Key)
        {
          /* 1. Segreteria */
          case ConsoleKey.D1:
              /* Interruzione del Server */
              controllaCoda.Abort();
              /* Acquisizione dati da utente */
              Console.Clear();
              testoApertura();
              Console.WriteLine("\t\t\t\t* Segreteria *\n");

              do
              {
                /* Invio della richiesta al Server */
                richiesta.segreteria(collegamento,username);

                /* Attesa della riposta da parte del Server */
                Console.WriteLine(" <> Prelievo dei dati in Segreteria...\n");
                mutex.WaitOne();

                /* Scrivi la risposta a schermo */
                Console.WriteLine(" -> {0}", Connessione.Istanza.chiamataSincrona);

                Console.WriteLine("\n Continuo a verificare?S/N\n");

                /* Variabile che descrive i tasti premuti sulla tastiera */
                pressione = new ConsoleKeyInfo();

                /* Cattura della pressione */
                while (pressione.Key != ConsoleKey.S && pressione.Key != ConsoleKey.N)
                {
                  pressione = Console.ReadKey(true);

                  switch (pressione.Key)
                  {
                    case ConsoleKey.S:
                        break;
                    case ConsoleKey.N:
                        break;
                  }
                }
              } while(pressione.Key == ConsoleKey.S);
                        
              /* Messaggio di ritorno ai servizi */
              Console.WriteLine(" <> Premi Invio per tornare ai Servizi\n");
              Console.ReadLine();
              Console.Clear();
              testoServizi(username);

              /* Reset della pressione */
              pressione = new ConsoleKeyInfo();

              /* Riavvio Thread chiamate asincrone */
              controllaCoda = new Thread(stampaChiamataAsincrona);
              controllaCoda.Start();
              break;

          /* 2. Invia un Messaggio */
          case ConsoleKey.D2:
              /* Interruzione del Server */
              controllaCoda.Abort();
              /* Acquisizione dati da utente */
              Console.Clear();
              testoApertura();
              Console.WriteLine(" [SUGGERIMENTO]");
              Console.WriteLine(" Per chattare devi essere amico della persona con cui desideri farlo!\n\n");
              Console.WriteLine("\t\t\t     * Invia un Messaggio *\n");
              Console.Write(" A chi vuoi inviare il messaggio: ");
              string destinatario = Console.ReadLine().ToString();
              Console.Write(" Messaggio: ");
              string messaggio = Console.ReadLine().ToString();

              /* Se l'inserimento rispetta le seguenti regole *
                * 1. Limite 24 Caratteri                       *
                * 2. Solo lettere maiuscole e minuscole        *
                * 3. Solo altri utenti (non se stessi)         */
              if (Regex.IsMatch(destinatario, @"^[a-zA-Z]+$") &&
                  Regex.IsMatch(messaggio, @"^[^,]+$") &&
                  destinatario.Length <= 24 &&
                  destinatario != username &&
                  messaggio.Length <= 256)
              {
                richiesta.inviaMessaggio(collegamento, username, destinatario, messaggio);

                /* Attesa della riposta da parte del Server */
                Console.WriteLine("\n -> Attendere prego\n");
                mutex.WaitOne();

                /* Risposta del Server */
                if (collegamento.chiamataSincrona != "OK")
                    Console.WriteLine(" [ERRORE] {0}\n", collegamento.chiamataSincrona);
                else
                    Console.WriteLine(" [OK] Messaggio Inviato\n");
              }
              else
              {
                /* Messaggio di Errore */
                Console.WriteLine("\n [MESSAGGIO NON INVIATO]");
                Console.WriteLine(" (!) Non puoi scrivere a te stesso");
                Console.WriteLine(" (!) In questa versione la virgola non e' ammessa\n");
              }

              /* Messaggio di ritorno ai Servizi */
              Console.WriteLine(" <> Premi Invio per tornare ai Servizi\n");
              Console.ReadLine();
              Console.Clear();
              testoServizi(username);

              /* Reset della pressione */
              pressione = new ConsoleKeyInfo();

              /* Riavvio Thread chiamate asincrone */
              controllaCoda = new Thread(stampaChiamataAsincrona);
              controllaCoda.Start();
              break;

          /* 3. Lista degli Amici */             
          case ConsoleKey.D3:
              /* Interruzione del Server */
              controllaCoda.Abort();
              /* Acquisizione dati da utente */
              Console.Clear();
              testoApertura();
              Console.WriteLine("\t\t\t       * Lista Amicizie *\n", username);
              /* Invia la richiesta */
              richiesta.listaAmici(collegamento,username);

              /* Attendi la risposta del Server */
              Console.WriteLine(" <> Recupero della lista di amici...\n");
              mutex.WaitOne();

              try
              {
                /* Scrivi la risposta a schermo */
                lista = converti.StringaALista(Connessione.Istanza.chiamataSincrona);

                foreach (string amico in lista)
                    Console.WriteLine(" -> {0}", amico);
              }
              catch (Exception)
              {
                Console.WriteLine(" (!) Si e' verificato un errore nella richiesta. Riprova\n");
                lista = new List<string>();
              }

              /* Rimanda ai Servizi se l'utente preme invio */
              Console.WriteLine("\n <> Premi Invio per tornare ai Servizi\n");
              Console.ReadLine();
              Console.Clear();
              testoServizi(username);

              /* Reset della pressione */
              pressione = new ConsoleKeyInfo();

              /* Riavvio Thread chiamate asincrone */
              controllaCoda = new Thread(stampaChiamataAsincrona);
              controllaCoda.Start();
              break;
        
          /* 4. Aggiungi un amico */
          case ConsoleKey.D4:
              /* Interruzione del Server */
              controllaCoda.Abort();
              /* Acquisizione dati da utente */
              Console.Clear();
              testoApertura();
              Console.WriteLine(" [SUGGERIMENTO]");
              Console.WriteLine(" Se sei stato aggiunto, per confermare l'amicizia e' sufficiente che anche");
              Console.WriteLine(" tu aggiungi l'Utente!\n\n");
              Console.WriteLine("\t\t\t     * Aggiungi un amico *\n");
              Console.Write(" Username dell'amico: ");
              string amicoAggiunto = Console.ReadLine();

              /* Se l'inserimento rispetta le seguenti regole *
                * 1. Limite 24 Caratteri                       *
                * 2. Solo lettere maiuscole e minuscole        *
                * 3. Solo altri utenti (non se stessi)         */
              if (Regex.IsMatch(amicoAggiunto, @"^[a-zA-Z]+$") && amicoAggiunto.Length <= 24 &&
                  amicoAggiunto != username)
              {
                richiesta.aggiungiAmico(collegamento, username, amicoAggiunto);
                Console.WriteLine();

                /* Attesa della risposta dal Server */
                Console.WriteLine(" -> Attendere prego\n");
                mutex.WaitOne();

                /* Se la risposta dal Server non e' OK */
                if (collegamento.chiamataSincrona != "OK")
                    /* Comunicazione dell'errore del Server */
                    Console.WriteLine(" [ERRORE] {0}\n", collegamento.chiamataSincrona);                           
                else // altrimenti
                    /* Conferma che il servizio richiesto e' andato a buon fine */
                    Console.WriteLine(" [OK] Amico aggiunto. Contattalo se lo vedi nella tua lista di amici\n");                           
              }
              else // altrimenti
              {
                /* Messaggio di Errore */
                Console.WriteLine("\n [AMICO NON AGGIUNTO]");
                Console.WriteLine(" (!) Non puoi aggiungere te stesso"); 
                Console.WriteLine(" (!) Sono ammessi caratteri alfabetici maiuscoli e minuscoli\n");
              }

              /* Ritorna alla schermata dei Servizi */
              Console.WriteLine(" <> Premi Invio per tornare ai Servizi\n");
              Console.ReadLine();
              Console.Clear();
              testoServizi(username);

              /* Reset della pressione */
              pressione = new ConsoleKeyInfo();

              /* Riavvio Thread chiamate asincrone */
              controllaCoda = new Thread(stampaChiamataAsincrona);
              controllaCoda.Start();
              break;

          /* 5. Utenti Online */
          case ConsoleKey.D5:
              /* Interruzione del Server */
              controllaCoda.Abort();
              /* Acquisizione dati da utente */
              Console.Clear();
              testoApertura();
              Console.WriteLine("\t\t\t       * Utenti Online *\n");

              /* Invia la richiesta */
              richiesta.listaUtentiOnline(collegamento, username);

              /* Attendi la risposta del Server */
              Console.WriteLine(" <> Recupero della lista di utenti Online...\n");
              mutex.WaitOne();

              try
              {
                /* Scrivi la risposta a schermo */
                lista = converti.StringaALista(Connessione.Istanza.chiamataSincrona);

                foreach (string utente in lista)
                  if(utente.Length > 0)
                    Console.WriteLine(" -> {0}", utente);
              }
              catch (Exception)
              {
                Console.WriteLine(" (!) Si e' verificato un errore nella richiesta. Riprova\n");
                lista = new List<string>();
              }

              /* Rimanda ai Servizi se l'utente preme invio */
              Console.WriteLine("\n <> Premi Invio per tornare ai Servizi\n");
              Console.ReadLine();
              Console.Clear();
              testoServizi(username);

              /* Reset della pressione */
              pressione = new ConsoleKeyInfo();

              /* Riavvio Thread chiamate asincrone */
              controllaCoda = new Thread(stampaChiamataAsincrona);
              controllaCoda.Start();
              break;

          /* 6. Aiuto */
          case ConsoleKey.D6:
              /* Interruzione del Server */
              controllaCoda.Abort();
              /* Acquisizione dati da utente */
              Console.Clear();
              testoApertura();
              Console.WriteLine("\t\t\t\t  * Manuale *\n");

              Console.WriteLine(" [Chattare]");
              Console.WriteLine(" Per chattare devi essere amico della persona con cui desideri farlo.");
              Console.WriteLine(" Per avere amici devi aggiungerli mediante - Aggiungi un Amico - .");
              Console.WriteLine(" Inviare messaggi e' semplice! Seleziona - Invia Messaggio - e potrai");
              Console.WriteLine(" subito scrivere ai tuoi amici. I messaggi vengono inviati anche a chi");
              Console.WriteLine(" e' offline. Gli verranno recapitati una volta online!\n");

              Console.WriteLine(" [Ricezione Messaggi]");
              Console.WriteLine(" I messaggi in arrivo vengono mostrati nella schermata Servizi. Se");
              Console.WriteLine(" usi un servizio non verra' mostrato alcun messaggio o richiesta\n\n");

              Console.WriteLine(" [Aggiungere un amico]");
              Console.WriteLine(" Per aggiungere un amico è sufficiente utilizzare - Aggiungi un Amico.");
              Console.WriteLine(" Non conosci nessuno? Utenti Online - ti mostrera' gli Utenti in rete.");
              Console.WriteLine(" Sappi che non e' necessario che l'utente sia Online per aggiungerlo. ");
              Console.WriteLine(" Se invece vieni aggiunto, per confermare l'amicizia e' sufficiente");
              Console.WriteLine(" che anche tu aggiungi l'Utente, cosi' l'amicizia diventerà reale.");
              Console.WriteLine(" Ricorda! Con - Lista Utenti - sai in qualsiasi momento chi sono");
              Console.WriteLine(" i tuoi amici.\n");

              /* Rimanda ai Servizi se l'utente preme invio */
              Console.WriteLine("\n <> Premi Invio per tornare ai Servizi\n");
              Console.ReadLine();
              Console.Clear();
              testoServizi(username);

              /* Reset della pressione */
              pressione = new ConsoleKeyInfo();

              /* Riavvio Thread chiamate asincrone */
              controllaCoda = new Thread(stampaChiamataAsincrona);
              controllaCoda.Start();
              break;

          /* 7. Chiudi Applicazione */
          case ConsoleKey.D7:
              break;

          /* Altra pressione */
          default:
              Console.Clear();
              testoServizi(username);
              Console.WriteLine(" (!) Errore nella scelta del Servizio\n");
              break;
        }
      }
    }

    /* Metodo chiamato in caso di ricevimento Real-Time di una amicizia */
    static void stampaChiamataAsincrona()
    {
      Convertitore converti = new Convertitore(); // Istanza di Convertitore
      Pacchetto pacchetto;    // Variabile per chimata asincrona
      bool condizione = true; // Condizione

      /* La condizione salta nel momento in cui viene interrotto il Thread */
      while (condizione)
      {
        try
        {
          try
          {
            /* Estraggo la prima chiamata asincrona memorizzata */
            pacchetto = Connessione.Istanza.chiamateAsincrone.Dequeue(); 
               
            /* Messaggio */
            if(pacchetto.Comando == "InoltroMsg")
            {
              /* Conversione della stringa in lista */
              List<string> valori = converti.StringaALista(pacchetto.Contenuto);
              /* Avviso a schermo */
              Console.WriteLine(" [{0}] {1}", valori[0], valori[1]);
            }

            /* Amicizia */
            if (pacchetto.Comando == "InoltroAmicizia")
              /* Avviso a schermo */
              Console.WriteLine(" -> Hai una richiesta di amicizia in sospeso in Segreteria\n");
          }
          catch (InvalidOperationException)
          {
            /* Metti in pausa il Thread per un secondo */
            Thread.Sleep(1000);
          } 
        }
        catch (ThreadInterruptedException)
        {
          condizione = false;
        }
      }
    }

    /* Metodo chiamato in caso di ricevimento di un Errore */
    public static void stampaErrore(string errore)
    {
      Console.WriteLine("\n <!> {0}", errore);

      /* Errori critici che necessitano della chiusura dell'applicazione */
      if (errore == "Il Server e' OFFLINE" ||
          errore == "Il Server ti ha inviato un pacchetto anomalo")
      {
        for (int i = 20; i > 0; i--)
        {
          /* Countdown a schermo */
          Console.Clear();
          testoApertura();
          Console.WriteLine("\n <!> {0}. CTRL-C per chiudere l'applicazione immediatamente\n", errore);
          Console.WriteLine(" <!> Spegnimento automatico in {0} secondi\n", i);
          Thread.Sleep(1000);
        }
        /* Chiusura */
        Environment.Exit(0);
      }
    }

    /* Funzione accessibile per garantire la corretta sincronizzazione dei flussi d'esecuzione */
    public static void liberaMutex()
    {
      mutex.Release();
    }
  }
}
