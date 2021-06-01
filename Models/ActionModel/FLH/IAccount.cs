using Queue.Models.IRepository.IFLH;
using System;
using Queue.Models.FLDModel;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using System.Security.Cryptography;

namespace Queue.Models.ActionModel.FLH
{
    public class IAccount
    {
        FLHEntities db = new FLHEntities();
        string con = System.Configuration.ConfigurationManager.ConnectionStrings["FLHConnection"].ConnectionString;


        public string Login(string mail, string password)
        {
            string result = "2";
            var user = db.Tb_RegistroInvitados.FirstOrDefault(x => x.Txt_Correo == mail && x.Txt_Contraseña == password);
            if (user != null)
            {
                result = user.Txt_Token;
                return result;
            }
            return result;
        }

        //acompañamte
        public string Insert(string token, string email, long number, string name, bool e1, bool e2, bool e3)
        {
            string s = "2";

            var selectacompañante = db.Tb_RegistroAcompañantes.FirstOrDefault(x => x.Txt_Correo == email);

            if (selectacompañante == null)
            {
                int evento1 = 0;
                int evento2 = 0;
                int evento3 = 0;

                if (e1 == true)
                {
                    evento1 = 1;
                }

                if (e2 == true)
                {
                    evento2 = 1;
                }

                if (e3 == true)
                {
                    evento3 = 1;
                }

                int length = 30;
                const string valid = "ABCDEFOPTUVWXYZ1234567890";

                using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
                {
                    while (s.Length != length)
                    {
                        byte[] oneByte = new byte[1];
                        rng.GetBytes(oneByte);
                        char character = (char)oneByte[0];
                        if (valid.Contains(character))
                        {
                            s += character;
                        }
                    }
                }

                var inv = db.Tb_RegistroInvitados.FirstOrDefault(x => x.Txt_Token == token);
                if (inv != null)
                {
                    using (SqlConnection sql = new SqlConnection(con))
                    {
                        try
                        {
                            string query = "insert into Tb_RegistroAcompañantes(Txt_Correo, Txt_Nombre,  Num_Telefono, Int_Status, Fec_Alta, Txt_QR )" +
                                                               "values('" + email + "', '" + name + "', " + number + ", 1, getdate(), '" + s + "' )";
                            SqlCommand cmd = new SqlCommand(query, sql);
                            sql.Open();
                            cmd.ExecuteNonQuery();

                            var acom = db.Tb_RegistroAcompañantes.FirstOrDefault(x => x.Txt_Correo == email);
                            string add = "insert into Tb_InvitadoAcompañante(Int_IdInvitado, Int_IdAcompañante)" +
                                           "values(" + inv.Int_IdRegistro + ", " + acom.Int_IdRegistro + ")";
                            SqlCommand cmt = new SqlCommand(add, sql);
                            cmt.ExecuteNonQuery();

                            string eventacomp = "insert into Tb_EventosAcompañante ( Int_IdAcompañante, Bol_Evento1, Bol_Evento2, Bol_Evento3, Fec_Alta, Bol_Validado) values( " + acom.Int_IdRegistro + ", " + evento1 + " , " + evento2 + " , " + evento3 + " , GETDATE(), 1) ";
                            SqlCommand acomp = new SqlCommand(eventacomp, sql);
                            acomp.ExecuteNonQuery();
                            sql.Close();

                            return s;

                        }
                        catch (Exception er)
                        {
                            er.Message.ToString();
                            return s;
                        }


                    }
                }
                return s;

            }
            else {
                return s;
            }

        }


        public bool Update(string qr)
        {
            var inv = db.Tb_RegistroInvitados.FirstOrDefault(x => x.Txt_QR == qr);
            if (inv == null)
            {
                var acom = db.Tb_RegistroAcompañantes.FirstOrDefault(x => x.Txt_QR == qr);
                if (acom != null)
                {
                    using (SqlConnection sql = new SqlConnection(con))
                    {
                        string query = "update ";
                        sql.Open();

                    }
                }
                return false;
            }
            return false;
        }


        public IEnumerable<InvitadosEvento> Exists(string token)
        {
            List<InvitadosEvento> list = new List<InvitadosEvento>();
            try
            {
                var user = db.Tb_RegistroInvitados.FirstOrDefault(x => x.Txt_Token == token);
                if (user != null)
                {
                    var exists = db.Tb_EventosInvitado.FirstOrDefault(x => x.Int_IdInvitado == user.Int_IdRegistro);
                    if (exists != null)
                    {
                        InvitadosEvento ia = new InvitadosEvento()
                        {
                            Int_IdRegistro = exists.Int_IdRegistro,
                            Int_IntIdInvitado = (int)exists.Int_IdInvitado,
                            Bol_Evento1 = (bool)exists.Bol_Evento1,
                            Bol_Evento2 = (bool)exists.Bol_Evento2,
                            Bol_Evento3 = (bool)exists.Bol_Evento3,
                            Fec_Alta = Convert.ToDateTime(exists.Fec_Alta),
                            Bol_Validado = (int)exists.Bol_Validado
                        };
                        list.Add(ia);

                        return list;
                    }
                }
            }
            catch (Exception er)
            {

            }
            return list;
        }


        //subir método
        public List<ListaInvitados> Invitados()
        {
            List<ListaInvitados> list = new List<ListaInvitados>();

            using (SqlConnection sql = new SqlConnection(con))
            {
                sql.Open();
                string query = "select Txt_Correo from Tb_ListadoInvitados";

                SqlDataAdapter da = new SqlDataAdapter(query, sql);
                DataSet a = new DataSet();
                da.Fill(a);
                ListaInvitados u = new ListaInvitados();
                for (int e = 0; e < a.Tables[0].Rows.Count; e++)
                {
                    if (a.Tables[0].Rows.Count > 0)
                    {
                        u.Txt_Correo = a.Tables[0].Rows[e]["Txt_Correo"].ToString();
                    }
                    list.Add(u);
                }
                sql.Close();
            }
            return list;
        }
        //subir método
        public List<Tb_RegistroInvitados> ListRegistroInvitados()
        {
            List<Tb_RegistroInvitados> lista = new List<Tb_RegistroInvitados>();
            // lista completa de invitado registrado
            lista = db.Tb_RegistroInvitados.Where(x => x.Txt_QR != null  && x.Int_Status == 1).ToList();

            //solo para un invitado especial  cambiar el correo
            // lista = db.Tb_RegistroInvitados.Where(x=>x.Txt_QR != "jcenteno@cecgruop.mx" && x.Int_Status == 1).ToList();

            return lista;
        }
        //subir método
        public List<Tb_RegistroAcompañantes> ListRegistroAcompanantes()
        {

            List<Tb_RegistroAcompañantes> lista = new List<Tb_RegistroAcompañantes>();
            lista = db.Tb_RegistroAcompañantes.Where(x=>x.Txt_Correo.Contains("@") && x.Int_Status == 1).ToList();
           /// lista = db.Tb_RegistroAcompañantes.Where(x => x.Txt_QR != null && x.Txt_Correo =="jcenteno@cecgruop.mx" && x.Int_Status == 1).ToList();
            return lista;
        }
        //subir método
        public Tb_EventosInvitado EventosInvitado(Tb_EventosInvitado eventosInvitado)
        {
            Tb_EventosInvitado tb_Eventos = new Tb_EventosInvitado();
            using (SqlConnection sql = new SqlConnection(con))
            {
                try
                {
                    sql.Open();
                    string query = "Update Tb_EventosInvitado set Bol_Evento1 = " + Convert.ToInt32(eventosInvitado.Bol_Evento1) + ", " +
                        "Bol_Evento2 = " + Convert.ToInt32(eventosInvitado.Bol_Evento2) + ", " +
                        "Bol_Evento3 = " + Convert.ToInt32(eventosInvitado.Bol_Evento3) + "," +
                         "Bol_Validado = " + Convert.ToInt32(eventosInvitado.Bol_Validado) + " " +
                        " where Int_IdInvitado =" + Convert.ToInt32(eventosInvitado.Int_IdInvitado) + "";
                    SqlCommand cmd = new SqlCommand(query, sql);
                    cmd.ExecuteNonQuery();
                    sql.Close();
                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message);
                }
                tb_Eventos = db.Tb_EventosInvitado.FirstOrDefault(x => x.Int_IdInvitado == eventosInvitado.Int_IdInvitado);
            }

            return tb_Eventos;
        }

        public Tb_EventosAcompañante EventosInvitadoAcompanante(Tb_EventosAcompañante eventosInvitado)
        {
            Tb_EventosAcompañante tb_Eventos = new Tb_EventosAcompañante();
            using (SqlConnection sql = new SqlConnection(con))
            {
                try
                {
                    sql.Open();
                    string query = "Update Tb_EventosAcompañante set Bol_Evento1 = " + Convert.ToInt32(eventosInvitado.Bol_Evento1) + ", " +
                        "Bol_Evento2 = " + Convert.ToInt32(eventosInvitado.Bol_Evento2) + ", " +
                        "Bol_Evento3 = " + Convert.ToInt32(eventosInvitado.Bol_Evento3) + "," +
                         "Bol_Validado = " + Convert.ToInt32(eventosInvitado.Bol_Validado) + " " +
                        " where Int_IdAcompañante =" + Convert.ToInt32(eventosInvitado.Int_IdAcompañante) + "";
                    SqlCommand cmd = new SqlCommand(query, sql);
                    cmd.ExecuteNonQuery();
                    sql.Close();
                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message);
                }
                tb_Eventos = db.Tb_EventosAcompañante.FirstOrDefault(x => x.Int_IdAcompañante == eventosInvitado.Int_IdAcompañante);
            }

            return tb_Eventos;
        }
        //subir método
        public string actualizarInvitado(int numEnviado, int idInvitado, string Correo )
        {
            using (SqlConnection sql = new SqlConnection(con))
            {
                try
                {
                    sql.Open();
                    string query = "Update Tb_ListadoInvitados set Num_Enviado = " + numEnviado + " " +                       
                        "where Int_IdInvitado =" + Convert.ToInt32(idInvitado) +"";

                   
                    SqlCommand cmd = new SqlCommand(query, sql);
                    cmd.ExecuteNonQuery();
                    sql.Close();
                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message);
                }
            }

            return "actualizado";
        }
        //subir método
        public string actualizarRegistroInvitado(int Int_Status, int Int_IdRegistro, string Correo)
        {
            using (SqlConnection sql = new SqlConnection(con))
            {
                try
                {
                    sql.Open();
                    string query = "Update Tb_RegistroInvitados set Int_Status = " + Int_Status + " " +
                        "where Int_IdRegistro =" + Convert.ToInt32(Int_IdRegistro) + "";


                    SqlCommand cmd = new SqlCommand(query, sql);
                    cmd.ExecuteNonQuery();
                    sql.Close();
                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message);
                }
            }

            return "actualizado";
        }
        //subir método
        public string actualizarRegistroAcompanante(int Int_Status, int Int_IdRegistro, string Correo)
        {
            using (SqlConnection sql = new SqlConnection(con))
            {
                try
                {
                    sql.Open();
                    string query = "Update Tb_RegistroAcompañantes set Int_Status = " + Int_Status + " " +
                        "where Int_IdRegistro =" + Convert.ToInt32(Int_IdRegistro) + "";


                    SqlCommand cmd = new SqlCommand(query, sql);
                    cmd.ExecuteNonQuery();
                    sql.Close();
                }
                catch (Exception ex)
                {

                    throw new Exception(ex.Message);
                }
            }

            return "actualizado";
        }
    }
}