using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Queue.Models.IRepository.IFLH;
using Newtonsoft.Json;
using Queue.Models.ViewModel;
using System.Net.Http;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Web.Http;
using System.Net;
using System.Data;

namespace Queue.Models.ActionModel.FLH
{
   
    public class IRegister
    {
        FLHEntities db = new FLHEntities();
        string con = System.Configuration.ConfigurationManager.ConnectionStrings["FLHConnection"].ConnectionString;

     
       
        [HttpGet]//Valida que el usuario sea nuevo y haya sido registrado, si el status es igual a 0
        public  bool Validate(string email)//se generará el token y se insertará el correo en el Registro.
        {
            if (email != "")
            {
                //int status = 0;
                var validate = db.Tb_ListadoInvitados.FirstOrDefault(x => x.Txt_Correo == email && x.Int_Status == 0);
                //var validate = db.Tb_ListadoInvitados.Where(x => x.Txt_Correo == email && !x.Bol_Status.Equals(status));
                if (validate != null)
                {
                    int length = 20;
                    const string valid = "0aAbBcC1dDeEf2FgGh3HiIjJ4kKlLmM5nNoO6pPqQ7rRsSt8TuUvV9wWxXyYzZ";
                    string token = "";
                    using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
                    {
                        while (token.Length != length)
                        {
                            byte[] oneByte = new byte[1];
                            rng.GetBytes(oneByte);
                            char character = (char)oneByte[0];
                            if (valid.Contains(character))
                            {
                                token += character;
                            }
                        }
                    }
                    using(SqlConnection sql = new SqlConnection(con))
                    {
                        using (HttpClient client = new HttpClient())
                        {
                            string query = "insert into Tb_RegistroInvitados(Txt_Correo, Txt_Token, Int_Status, Fec_Alta) values('" + email + "', '" + token + "', 1, getdate())  update Tb_ListadoInvitados set Int_Status = 1 where Txt_Correo = '"+ email + "'";

                            try
                            {
                                sql.Open();
                                SqlCommand cmd = new SqlCommand(query, sql);
                                cmd.ExecuteNonQuery();
                                sql.Close();
                                return true;
                            }
                            catch(Exception er)
                            {

                            }
                        }
                    }
                   
                }
               
            }
            return false;

        }



        public string Token(string email)
        {
            string tok = "";
            using(SqlConnection de = new SqlConnection(con))
            {
                string query = "select Txt_Token from Tb_RegistroInvitados where Txt_Correo = '"+email+"'";
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataSet a = new DataSet();
                da.Fill(a);
                if(a.Tables[0].Rows.Count > 0)
                {
                    UserViewModel u = new UserViewModel();

                    u.Token = a.Tables[0].Rows[0]["Txt_Token"].ToString();

                    tok = u.Token;

                    return tok;
                }
            }

            return null;
        }




        public bool Details(string token, string password, string name, long number, string email)
        {
            using(SqlConnection sql = new SqlConnection(con))
            {
                using(HttpClient client = new HttpClient())
                {
                    string query = "Update Tb_RegistroInvitados set Txt_Contraseña = '" + password+"', Txt_Nombre = '"+name+"', Num_Telefono = '"+number+"' where Txt_Token = '" + token + "' update Tb_ListadoInvitados set Txt_Nombre = '"+name+"' where Txt_Correo = '"+email+"' ";

                    try
                    {
                        sql.Open();
                        SqlCommand cmd = new SqlCommand(query, sql);
                        cmd.ExecuteNonQuery();
                        sql.Close();
                        return true;
                    }
                    catch(Exception we)
                    {

                    }

                }
            }
            return false;
           
        }

        public bool TokenValidate(string token)
        {
            using (FLHEntities db = new FLHEntities())
            {
                var count = db.Tb_RegistroInvitados.FirstOrDefault(x => x.Txt_Token == token);
                if (count != null)
                    return true;
            }
            return false;
        }
    }
}

