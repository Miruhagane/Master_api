//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Queue.Models.IRepository.IFLH
{
    using System;
    using System.Collections.Generic;
    
    public partial class Tb_ListadoInvitados
    {
        public int Int_IdInvitado { get; set; }
        public string Txt_Correo { get; set; }
        public string Txt_Nombre { get; set; }
        public Nullable<int> Int_Status { get; set; }
        public Nullable<int> Num_Enviado { get; set; }
        public Nullable<System.DateTime> Fec_Enviado { get; set; }
        public string Txt_Password { get; set; }
        public string Txt_Telefono { get; set; }
        public Nullable<bool> Bol_Vip { get; set; }
    }
}
