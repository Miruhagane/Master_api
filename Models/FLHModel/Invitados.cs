using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Queue.Models.FLDModel
{
    public class Invitados
    {
        public int Int_IdInvitado { get; set; }
        public string Txt_Nombre { get; set; }
        public string Txt_Mail { get; set; }
        public string Txt_Code { get; set; }
        public int Int_IdStatus { get; set; }
        public DateTime Fec_In { get; set; }
        public DateTime Fec_Out { get; set; }
        public int Int_IdNivel { get; set; }
        public int Int_IdInvitadoRelacion { get; set; }
    }

    public class Niveles
    {
        public int Int_IdNivel { get; set; }
        public string Txt_Nivel { get; set; }
    }

    public class QR
    {
        public int Int_IdQR { get; set; }
        public string Txt_Code { get; set; }
        public int Int_IdInvitado { get; set; }
        public DateTime Fec_Enviado { get; set; }
        public DateTime Fec_Fin { get; set; }
    }

    public class ListaInvitados
    {
  
        public string Txt_Correo { get; set; }

    }

    public class Status
    {
        public int Int_IdStatus { get; set; }
        public string Txt_Status { get; set; }
    }

    public class InvitadosEvento
    {
        public int Int_IdRegistro { get; set; }
        public int Int_IntIdInvitado { get; set; }
        public bool Bol_Evento1 { get; set; }
        public bool Bol_Evento2 { get; set; }
        public bool Bol_Evento3 { get; set; }
        public DateTime Fec_Alta { get; set; }
        public int Bol_Validado { get; set; }
        
    }
    public class ListRegistroInvitados {
        public string  Txt_Correo { get; set; }
        public string Txt_QR { get; set; }
    }

    public class ListRegistroAcompanante
    {
        public string Txt_Correo { get; set; }
        public string Txt_QR { get; set; }
    }
}