﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class FLHEntities : DbContext
    {
        public FLHEntities()
            : base("name=FLHEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Ct_Eventos> Ct_Eventos { get; set; }
        public virtual DbSet<Tb_EventosAcompañante> Tb_EventosAcompañante { get; set; }
        public virtual DbSet<Tb_EventosInvitado> Tb_EventosInvitado { get; set; }
        public virtual DbSet<Tb_ListadoInvitados> Tb_ListadoInvitados { get; set; }
        public virtual DbSet<Tb_RegistroAcompañantes> Tb_RegistroAcompañantes { get; set; }
        public virtual DbSet<Tb_RegistroInvitados> Tb_RegistroInvitados { get; set; }
        public virtual DbSet<Tb_InvitadoAcompañante> Tb_InvitadoAcompañante { get; set; }
    }
}
