using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Queue.Models;
using System.Web.Mvc;
using Queue.Models.ActionModel.FLH;
using Queue.Models.FLDModel;
using Queue.Models.ViewModel;
using System.Web.Http;
using Queue.Models.IRepository.IFLH;
using System.Net.Mail;
using System.Data.SqlClient;
using System.Data;
using HttpGetAttribute = System.Web.Http.HttpGetAttribute;

namespace Queue.Controllers
{
  
    public class FLHController : Controller
    {
        string con = System.Configuration.ConfigurationManager.ConnectionStrings["FLHConnection"].ConnectionString;
        FLHEntities db = new FLHEntities();
        // GET: FLD



        [HttpGet]
        public bool Validate(string email)
        {
            IRegister u = new IRegister();
            if (u.Validate(email) == true)
                return true;

            return false;
        }


        public JsonResult searchqr(string QR)
        {

            List<lstEvento> lst = new List<lstEvento>();

            string status = "2";

            string evento1 = "No puede entrar";
            string evento2 = "No puede entrar";
            string evento3 = "No puede entrar";

            try
            {
                var bsQRinv = db.Tb_RegistroInvitados.FirstOrDefault(x => x.Txt_QR == QR);
                if (bsQRinv != null)
                {
                    var bsinvitadoevento = db.Tb_EventosInvitado.FirstOrDefault(x => x.Int_IdInvitado == bsQRinv.Int_IdRegistro);

                    if (bsinvitadoevento.Bol_Evento1 != false)
                    {
                        evento1 = "puede entrar";
                    }

                    if (bsinvitadoevento.Bol_Evento2 != false)
                    {
                        evento2 = "puede entrar";
                    }

                    if (bsinvitadoevento.Bol_Evento3 != false)
                    {
                        evento3 = "puede entrar";
                    }

                    lstEvento lst1 = new lstEvento()
                    {
                        nombre = bsQRinv.Txt_Nombre,
                        evento1 = evento1,
                        evento2 = evento2,
                        evento3 = evento3,
                        msn = 1
                        
                    };

                    lst.Add(lst1);

                    return Json(lst, JsonRequestBehavior.AllowGet);
                }
                else 
                {
                    var bsQRacom = db.Tb_RegistroAcompañantes.FirstOrDefault(x => x.Txt_QR == QR);

                    if (bsQRacom != null)
                    {
                        var bsacompevento = db.Tb_EventosAcompañante.FirstOrDefault(x => x.Int_IdAcompañante == bsQRacom.Int_IdRegistro);

                        if (bsacompevento.Bol_Evento1 != false)
                        {
                            evento1 = "puede entrar";
                        }

                        if (bsacompevento.Bol_Evento2 != false)
                        {
                            evento2 = "puede entrar";
                        }

                        if (bsacompevento.Bol_Evento3 != false)
                        {
                            evento3 = "puede entrar";
                        }

                        lstEvento lst1 = new lstEvento()
                        {
                            nombre = bsQRacom.Txt_Nombre,
                            evento1 = evento1,
                            evento2 = evento2,
                            evento3 = evento3,
                            msn = 2

                        };

                        lst.Add(lst1);

                        return Json(lst, JsonRequestBehavior.AllowGet);

                    }

                    else 
                    {
                        lstEvento lst1 = new lstEvento()
                        {
                            nombre = " ",
                            evento1 = evento1,
                            evento2 = evento2,
                            evento3 = evento3,
                            msn = 3

                        };

                        lst.Add(lst1);
                        return Json(lst, JsonRequestBehavior.AllowGet);
                    }
                }


            }

            catch (Exception er)
            {
                return Json(status, JsonRequestBehavior.AllowGet);
            }


           
        }

        [HttpGet]
        public string ReturnToken(string email)
        {
            IRegister u = new IRegister();
            string q = "no existe";
            try
            {
                var c = u.Token(email).ToString();
                q = c;
            }
            catch(Exception er)
            {
                return er.Message.ToString();
            }
              
            return q;
        }

        [HttpGet]
        public string Confirmation()
        {
            string confirm = "Confirmado en Servicio";
            return confirm;
        }

       // [HttpGet]
        public bool Password([System.Web.Http.FromBody] string token, string password, string name, long number, string email)
        {
            try
            {
                IRegister u = new IRegister();
                if (!u.TokenValidate(token))
                {
                    return false;
                }

                u.Details(token, password, name, number, email);
                return true;
            }
            catch(Exception we)
            {

            }
            return false;
        }
    
        [HttpGet]
        public string Login(  string email, string password)
        {
            IAccount a = new IAccount();
            string res = "2";        
            try
            {
                var confirmation = a.Login(email, password);
                if(confirmation != null)
                {
                    res = confirmation;
                    return res;
                }
            }
            catch (Exception er)
            { 
            }
            return res;
        }


    
        public string Insert(string token, string email, long number, string name, bool e1, bool e2, bool e3)
        {
            IAccount a = new IAccount();
            string res = "";

                res = a.Insert(token, email, number, name, e1, e2, e3).ToString();

            return res;
        }

        public JsonResult mail() 
        {

            string res = "";

            var mail = db.Tb_ListadoInvitados.Max(x => x.Int_IdInvitado);
            int count = Convert.ToInt32(mail);

            string correos = "";

            for (int i = 0; i < count; i++)
            {
                var invitadomail = db.Tb_ListadoInvitados.FirstOrDefault(x => x.Int_IdInvitado == i && x.Int_Status == 0 );

                if (invitadomail != null)
                {
                    correos += "" + invitadomail.Txt_Correo + " , ";

                    int num_enviado = Convert.ToInt32(invitadomail.Num_Enviado) + 1;

                    SqlDataAdapter da = new SqlDataAdapter("update Tb_ListadoInvitados set Num_Enviado = "+num_enviado+", Fec_Enviado = GETDATE() where Int_IdInvitado = "+invitadomail.Int_IdInvitado+"", con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                }

            }

            if (correos != "")
            {
                string fecha = DateTime.Now.ToString("dddd MM yyyy");

                string mailemisor2 = "soporteti@masterexchange.com.mx";
                string mailreceptor2 = correos;
                string mailoculto2 = "asantos@strategias.mx, omartinez@cecgroup.mx";
                string contraseña2 = "sistemas2021";
                string strngHtml = @"<html>
                                <body style='text-align: center' >
                                <img src='./Content/Email.png'>
                                 <a href='http://fbx40.com/' style='
                                    position: absolute;
                                            top: 95%;
                                            left: 46%;
                                            transform: translate(-50 %, -50 %);
                                            '> <h2>Enlace</h2></a> 
                                   '</body></html>'";

                MailMessage msng2 = new MailMessage(mailemisor2, mailreceptor2, "Pagos " + fecha + " ", strngHtml);
                msng2.Bcc.Add(mailoculto2);
                msng2.IsBodyHtml = true;
                msng2.Body = strngHtml;

                SmtpClient smtpClient2 = new SmtpClient("smtp.gmail.com");
                smtpClient2.EnableSsl = true;
                smtpClient2.UseDefaultCredentials = false;
                smtpClient2.Port = 587;
                smtpClient2.Credentials = new System.Net.NetworkCredential(mailemisor2, contraseña2);

                smtpClient2.Send(msng2);
                smtpClient2.Dispose();

                res = "1";
            }
            else {


                res = "2"; 
            }

            return Json(res, JsonRequestBehavior.AllowGet);



        }
        

        public IEnumerable<InvitadosEvento> exist(  string token)
        {
            IAccount a = new IAccount();
            IEnumerable<InvitadosEvento> list = a.Exists(token);               

            return list;
        }



        public JsonResult MailSender()
        {
            IAccount a = new IAccount();
            string mailemisor = "flh40fest@gmail.com";
            List<ListaInvitados> u = a.Invitados();
            string mailreceptor = "";
            string contraseña = "FLH40fest!";
            int i = 0;
            var res = new MailMessage();
            foreach(var x in u)
            {
                mailreceptor = u[i].Txt_Correo; 
                i++;
                MailMessage msg = new MailMessage(mailemisor, mailreceptor, "Reporte General Master Exchange ", "mensaje");

                SmtpClient client = new SmtpClient("smtp.gmail.com");
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Port = 587;
                client.Credentials = new System.Net.NetworkCredential(mailemisor, contraseña);

                client.Send(msg);
                client.Dispose();
                res = msg;
            }






            return Json(res, JsonRequestBehavior.AllowGet);
        }



    }


}