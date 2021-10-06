using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
  class Convertitore
  {        
    /* METODI */
    /* Metodo per la conversione da lista a stringa */
    public string listaAStringa(List<string> lista)
    {
      string stringaOttenuta; // stringa di ritorno

      /* Controlla che la lista non sia vuota */
      if (lista.Count != 0)
      {
        /* utilizzo un boolean per creare un comportamento diverso sul *
          * primo elemento della lista */
        bool primoElemento = true;

        /* Creo una variabile di tipo StringBuilder */
        var stringa = new StringBuilder();

        /* Costruisco la stringa di questo tipo: *
          * Stringa1,Stringa2,Stringa3...         */
        foreach (var elemento in lista)
        {
          if (primoElemento)
          {
            stringa.Append(elemento);
            primoElemento = false;
          }
          else
          {
            stringa.Append(string.Format(",{0}", elemento));
          }
        }

        /* StringBuilder non è tipo string ma è facilmente rapportabile */
        stringaOttenuta = stringa.ToString();
      }
      else
      {
        stringaOttenuta = null; // Se vuota il valore è null
      }

      /* Ritorno della stringa */
      return stringaOttenuta;
    }

    /* Metodo per la conversione da Stringa a Lista */
    public List<string> stringaALista(string stringa)
    {
      /* Variabile che ospiterà la lista */
      List<string> lista = new List<string>();

      /* Se la stringa non è null o vuota */
      if (!string.IsNullOrEmpty(stringa))
      {
        /* Prova a comporre la stringa */
        try
        {
          foreach (string elemento in stringa.Split(','))
          {
            lista.Add(elemento);
          }
        }
        catch (Exception)
        {
          lista = null; // in caso di errore invia lista vuota
        }
      }
      else
      {
        lista = null; // La stringa è null, allora la lista è null
      }

      /* Ritorno della lista */
      return lista;
    }
  }
}
