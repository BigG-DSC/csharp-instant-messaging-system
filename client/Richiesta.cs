using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
  /* La classe contiene tutte le richieste eseguibili dal Client */
  class Richiesta
  {
    /* Richiesta di aggiunta di un amico */
    public void aggiungiAmico(Connessione collegamento, string username, string amico)
    {
      /* Se siamo attualmente connessi al Server */
      if (collegamento.connessioneTCP.Connected)
      {
        /* Creazione del pacchetto */
        var msgPack = new Pacchetto("AggiungiAmico", string.Format("{0},{1}", username, amico));
        /* Invio del messaggio */
        collegamento.InviaPacchetto(msgPack);
      }
    }

    /* Richiesta di invio di un messaggio */
    public void inviaMessaggio(Connessione collegamento, string mittente, string destinatario, string messaggio)
    {
      /* Se siamo attualmente connessi al Server */
      if (collegamento.connessioneTCP.Connected)
      {
        /* Creazione del pacchetto */
        var msgPack = new Pacchetto("Messaggio", string.Format("{0},{1},{2}", mittente, destinatario, messaggio));
        /* Invio del messaggio */
        collegamento.InviaPacchetto(msgPack);
      }
    }

    /* Richiesta ottenimento lista amici */
    public void listaAmici(Connessione collegamento, string username)
    {
      /* Se siamo attualmente connessi al Server */
      if (collegamento.connessioneTCP.Connected)
      {
        /* Creazione del pacchetto */
        var msgPack = new Pacchetto("ListaAmici", string.Format("{0}", username));
        /* Invio del messaggio */
        collegamento.InviaPacchetto(msgPack);
      }
    }

    /* Richiesta ottenimento lista amici */
    public void listaUtentiOnline(Connessione collegamento, string username)
    {
      /* Se siamo attualmente connessi al Server */
      if (collegamento.connessioneTCP.Connected)
      {
        /* Creazione del pacchetto */
        var msgPack = new Pacchetto("ListaUtentiOnline", string.Format("{0}", username));
        /* Invio del messaggio */
        collegamento.InviaPacchetto(msgPack);
      }
    }

    /* Richiesta di accesso ai Servizi di messaggistica */
    public void login(Connessione collegamento,string username,string password)
    {
      /* Se siamo attualmente connessi al Server */
      if (collegamento.connessioneTCP.Connected)
      {
        /* Creazione del pacchetto */
        var msgPack = new Pacchetto("Login", string.Format("{0},{1}",username,password));
        /* Invio del messaggio */
        collegamento.InviaPacchetto(msgPack);
      }
    }

    /* Richiesta di Registrazione al servizio */
    public void registrazione(Connessione collegamento, string username, string password)
    {
      /* Se siamo attualmente connessi al Server */
      if (collegamento.connessioneTCP.Connected)
      {
        /* Creazione del pacchetto */
        var msgPack = new Pacchetto("Registrazione", string.Format("{0},{1}", username, password));
        /* Invio del messaggio */
        collegamento.InviaPacchetto(msgPack);
      }
    }

    /* Richiesta di richieste in Segreteria */
    public void segreteria(Connessione collegamento, string username)
    {
      /* Se siamo attualmente connessi al Server */
      if (collegamento.connessioneTCP.Connected)
      {
        /* Creazione del pacchetto */
        var msgPack = new Pacchetto("Segreteria", string.Format("{0}", username));
        /* Invio del messaggio */
        collegamento.InviaPacchetto(msgPack);
      }
    }
  }
}
