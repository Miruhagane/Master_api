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
using System.Text;
using System.Net.Mime;
using System.IO;
using ZXing;
using System.Drawing;
using System.Drawing.Imaging;
using ZXing.Common;

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

        /// <summary>
        /// metodo que muestra información del invitado o acompañante registrado
        /// </summary>
        /// <param name="QR"></param>
        /// <returns></returns>


        public JsonResult ListaConfirmado(string r)
        {
            try
            {
                var datoPersona = db.Tb_RegistroInvitados.Join(
                    db.Tb_EventosInvitado,
                    res => res.Int_IdRegistro,
                    eve => eve.Int_IdInvitado,
                    (res, eve) => new
                    {
                        res.Txt_Nombre,
                        res.Txt_NombreAcompañante,
                        eve.Bol_Validado,
                        res.Int_Status
                    }
                   ).Where(x=>x.Int_Status==2).OrderBy(x => x.Txt_Nombre).ToList();


                return Json(datoPersona, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;
            }

        }
         public JsonResult ListaConfirmadoAcomp(string r)
        {
            try
            {
                var datoPersona = db.Tb_RegistroAcompañantes.Join(
                    db.Tb_EventosAcompañante,
                    res => res.Int_IdRegistro,
                    eve => eve.Int_IdAcompañante,
                    (res, eve) => new
                    {
                        res.Txt_Nombre,
                        eve.Bol_Validado,
                        res.Int_Status
                    }
                   ).Where(x=>x.Int_Status==2).OrderBy(x=>x.Txt_Nombre).ToList();


                return Json(datoPersona, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;
            }

        }
        public JsonResult buscarPorNomTel(string nombre)
        {
            try
            {
                var datoPersona = db.Tb_RegistroInvitados.FirstOrDefault(x => x.Txt_Nombre.ToUpper().Contains(nombre));
                if (datoPersona != null)
                {
                    var invitado = db.Tb_EventosInvitado.FirstOrDefault(x => x.Int_IdInvitado == datoPersona.Int_IdRegistro);
                    var evento = db.Ct_Eventos.ToList();
                    EventoInfo lst = new EventoInfo();

                    int i = 1;
                    foreach (var item in evento)
                    {
                        if (invitado.Bol_Evento1 == true && i == 1)
                        {
                            lst.evento1 = item.Txt_Evento;
                        }
                        if (invitado.Bol_Evento2 == true && i == 2)
                        {
                            lst.evento2 = item.Txt_Evento;
                        }
                        if (invitado.Bol_Evento3 == true && i == 3)
                        {
                            lst.evento3 = item.Txt_Evento;
                        }
                        i++;
                    }

                    lst.nombreInvitado = datoPersona.Txt_Nombre;
                    lst.NombreAcompanante = datoPersona.Txt_NombreAcompañante;
                    lst.Txt_QR = datoPersona.Txt_QR;
                    lst.Bol_Validado = Convert.ToInt32(invitado.Bol_Validado);
                    lst.msn = "Por favor validar su entrada.";

                    return Json(lst, JsonRequestBehavior.AllowGet);
                }
                var datoAcompanante = db.Tb_RegistroAcompañantes.FirstOrDefault(x => x.Txt_Nombre.ToUpper().Contains(nombre));
                if (datoAcompanante != null)
                {
                    var invitado = db.Tb_EventosAcompañante.FirstOrDefault(x => x.Int_IdAcompañante == datoAcompanante.Int_IdRegistro);
                    var evento = db.Ct_Eventos.ToList();
                    EventoInfo lst = new EventoInfo();

                    int i = 1;
                    foreach (var item in evento)
                    {
                        if (invitado.Bol_Evento1 == true && i == 1)
                        {
                            lst.evento1 = item.Txt_Evento;
                        }
                        if (invitado.Bol_Evento2 == true && i == 2)
                        {
                            lst.evento2 = item.Txt_Evento;
                        }
                        if (invitado.Bol_Evento3 == true && i == 3)
                        {
                            lst.evento3 = item.Txt_Evento;
                        }
                        i++;
                    }

                    lst.nombreInvitado = datoAcompanante.Txt_Nombre;
                    lst.Txt_QR = datoAcompanante.Txt_QR;
                    lst.Bol_Validado = Convert.ToInt32(invitado.Bol_Validado);
                    lst.msn = "Por favor validar su entrada.";

                    return Json(lst, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {

                return Json(ex.Message, JsonRequestBehavior.AllowGet);

            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        //CAMBIOS A SUBIR
        public JsonResult asistencia(string QR, string nombre,string telefono)
        {
            try
            {
                var datoPersona = db.Tb_RegistroInvitados.FirstOrDefault(x => x.Txt_QR == QR);
                if (datoPersona != null)
                {
                    var invitado = db.Tb_EventosInvitado.FirstOrDefault(x => x.Int_IdInvitado == datoPersona.Int_IdRegistro);
                    var evento = db.Ct_Eventos.ToList();
                    EventoInfo lst = new EventoInfo();

                    int i = 1;
                    foreach (var item in evento)
                    {
                        if (invitado.Bol_Evento1 == true && i == 1)
                        {
                            lst.evento1 = item.Txt_Evento;
                        }
                        if (invitado.Bol_Evento2 == true && i == 2)
                        {
                            lst.evento2 = item.Txt_Evento;
                        }
                        if (invitado.Bol_Evento3 == true && i == 3)
                        {
                            lst.evento3 = item.Txt_Evento;
                        }
                        i++;
                    }

                    lst.nombreInvitado = datoPersona.Txt_Nombre;
                    lst.NombreAcompanante = datoPersona.Txt_NombreAcompañante;
                    lst.Txt_QR = datoPersona.Txt_QR;
                    lst.Bol_Validado = Convert.ToInt32(invitado.Bol_Validado);
                    lst.msn = "Por favor validar su entrada.";

                    return Json(lst, JsonRequestBehavior.AllowGet);
                }
                var datoAcompanante = db.Tb_RegistroAcompañantes.FirstOrDefault(x => x.Txt_QR == QR);
                if (datoAcompanante != null)
                {
                    var invitado = db.Tb_EventosAcompañante.FirstOrDefault(x => x.Int_IdAcompañante == datoAcompanante.Int_IdRegistro);
                    var evento = db.Ct_Eventos.ToList();
                    EventoInfo lst = new EventoInfo();

                    int i = 1;
                    foreach (var item in evento)
                    {
                        if (invitado.Bol_Evento1 == true && i == 1)
                        {
                            lst.evento1 = item.Txt_Evento;
                        }
                        if (invitado.Bol_Evento2 == true && i == 2)
                        {
                            lst.evento2 = item.Txt_Evento;
                        }
                        if (invitado.Bol_Evento3 == true && i == 3)
                        {
                            lst.evento3 = item.Txt_Evento;
                        }
                        i++;
                    }

                    lst.nombreInvitado = datoAcompanante.Txt_Nombre;
                    lst.Txt_QR = datoAcompanante.Txt_QR;
                    lst.Bol_Validado = Convert.ToInt32(invitado.Bol_Validado);
                    lst.msn = "Por favor validar su entrada.";

                    return Json(lst, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {

                return Json(ex.Message, JsonRequestBehavior.AllowGet);

            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

         //CAMBIOS A SUBIR 
        public JsonResult confirmarEntrada(string QR,int? dia )
        {
            try
            {
                var datoInvitado = db.Tb_RegistroInvitados.FirstOrDefault(x => x.Txt_QR == QR);
                IAccount ac = new IAccount();
                lstEvento ReponseEvento = new lstEvento();
                if (datoInvitado != null) 
                {
                    var diaInvitado = db.Tb_EventosInvitado.FirstOrDefault(x => x.Int_IdInvitado == datoInvitado.Int_IdRegistro);
                    //primer evento
                    if (diaInvitado.Bol_Evento1 == true && dia==1)
                    {
                        Tb_EventosInvitado eventosInvitado = new Tb_EventosInvitado
                        {
                            Int_IdRegistro = diaInvitado.Int_IdRegistro,
                            Int_IdInvitado = diaInvitado.Int_IdInvitado,
                            Bol_Evento1 = diaInvitado.Bol_Evento1,
                            Bol_Evento2 = diaInvitado.Bol_Evento2,
                            Bol_Evento3 = diaInvitado.Bol_Evento3,
                            Bol_Validado = 1,
                        };
                        // método para enviar información actualización 
                        var result = ac.EventosInvitado(eventosInvitado);

                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    //segundo dia del evento
                    if (diaInvitado.Bol_Evento2 == true && dia == 2)
                    {
                        Tb_EventosInvitado eventosInvitado = new Tb_EventosInvitado
                        {
                            Int_IdRegistro = diaInvitado.Int_IdRegistro,
                            Int_IdInvitado = diaInvitado.Int_IdInvitado,
                            Bol_Evento1 = diaInvitado.Bol_Evento1,
                            Bol_Evento2 = diaInvitado.Bol_Evento2,
                            Bol_Evento3 = diaInvitado.Bol_Evento3,
                            Bol_Validado = 2,
                        };
                        // método para enviar información actualización
                        var result = ac.EventosInvitado(eventosInvitado);

                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    // tercer dia del evento 
                    if (diaInvitado.Bol_Evento3 == true && dia == 3)
                    {
                        Tb_EventosInvitado eventosInvitado = new Tb_EventosInvitado
                        {
                            Int_IdRegistro = diaInvitado.Int_IdRegistro,
                            Int_IdInvitado = diaInvitado.Int_IdInvitado,
                            Bol_Evento1 = diaInvitado.Bol_Evento1,
                            Bol_Evento2 = diaInvitado.Bol_Evento2,
                            Bol_Evento3 = diaInvitado.Bol_Evento3,
                            Bol_Validado = 3,
                        };
                        // método para enviar información actualización 
                        var result = ac.EventosInvitado(eventosInvitado);

                        return Json(result, JsonRequestBehavior.AllowGet);
                    }


                }
                var datoAcompanante = db.Tb_RegistroAcompañantes.FirstOrDefault(x => x.Txt_QR == QR);
                if (datoAcompanante != null)
                {

                    var diaInvitado = db.Tb_EventosAcompañante.FirstOrDefault(x => x.Int_IdAcompañante == datoAcompanante.Int_IdRegistro);
                    //primer evento
                    if (diaInvitado.Bol_Evento1 == true && dia == 1)
                    {
                        Tb_EventosAcompañante eventosInvitado = new Tb_EventosAcompañante
                        {
                            Int_IdRegistro = diaInvitado.Int_IdRegistro,
                            Int_IdAcompañante = diaInvitado.Int_IdAcompañante,
                            Bol_Evento1 = diaInvitado.Bol_Evento1,
                            Bol_Evento2 = diaInvitado.Bol_Evento2,
                            Bol_Evento3 = diaInvitado.Bol_Evento3,
                            Bol_Validado = 1,
                        };
                        // método para enviar información actualización 
                        var result = ac.EventosInvitadoAcompanante(eventosInvitado);

                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    //segundo dia del evento
                    if (diaInvitado.Bol_Evento2 == true && dia == 2)
                    {
                        Tb_EventosAcompañante eventosInvitado = new Tb_EventosAcompañante
                        {
                            Int_IdRegistro = diaInvitado.Int_IdRegistro,
                            Int_IdAcompañante = diaInvitado.Int_IdAcompañante,
                            Bol_Evento1 = diaInvitado.Bol_Evento1,
                            Bol_Evento2 = diaInvitado.Bol_Evento2,
                            Bol_Evento3 = diaInvitado.Bol_Evento3,
                            Bol_Validado = 2,
                        };
                        // método para enviar información actualización
                        var result = ac.EventosInvitadoAcompanante(eventosInvitado);

                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    // tercer dia del evento 
                    if (diaInvitado.Bol_Evento3 == true && dia == 3)
                    {
                        Tb_EventosAcompañante eventosInvitado = new Tb_EventosAcompañante
                        {
                            Int_IdRegistro = diaInvitado.Int_IdRegistro,
                            Int_IdAcompañante = diaInvitado.Int_IdAcompañante,
                            Bol_Evento1 = diaInvitado.Bol_Evento1,
                            Bol_Evento2 = diaInvitado.Bol_Evento2,
                            Bol_Evento3 = diaInvitado.Bol_Evento3,
                            Bol_Validado = 3,
                        };
                        // método para enviar información actualización 
                        var result = ac.EventosInvitadoAcompanante(eventosInvitado);

                        return Json(result, JsonRequestBehavior.AllowGet);
                    }

                }


                }
            catch (Exception ex)
            {

                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }

            return Json("", JsonRequestBehavior.AllowGet);

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
            catch (Exception er)
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
            catch (Exception we)
            {

            }
            return false;
        }

        [HttpGet]
        public string Login(string email, string password)
        {
            IAccount a = new IAccount();
            string res = "2";
            try
            {
                var confirmation = a.Login(email, password);
                if (confirmation != null)
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
 


        //CAMBIOS A SUBIR 
        public JsonResult EmailInvitacion(string clave, string correo)
        {
            string respuesta = "";
            //CORREO ESPECIFICOS PARA UN INVITADO NUEVO 
            if (clave == "PERSONAL")
            {

                ///  var Invitado = db.Tb_ListadoInvitados.Where(x => x.Txt_Correo.ToUpper() == correo.ToUpper());
                var Invitado = db.Tb_RegistroInvitados.Where(x =>  x.Int_Status==1).ToList();
                if (Invitado != null)
                {
                    foreach (var invitado in Invitado)
                    {

                     
                            string fecha = DateTime.Now.ToString("dddd MM yyyy");

                            string mailemisor2 = "fbx40tulum@gmail.com";
                            string mailreceptor2 = invitado.Txt_Correo;
                            string contraseña2 = "fabricio21";
                            string text = "";
                            AlternateView plainView =
                                AlternateView.CreateAlternateViewFromString(text,
                                                        Encoding.UTF8,
                                                        MediaTypeNames.Text.Plain);
                            string html = "<body <body style='text-align:center;'>" +
                                "<img src='cid:imagen' style='text-align:center;width:60%; height: 60%;'/>" +
                                "<br>" +                                                         
                               "<h3 style ='text-align:center;'><strong>Para duda o información comunicarse al WhatsApp:  +52 998 242 1114 </strong></h3>" +
                                  "</body>";
                            AlternateView htmlView =
                                AlternateView.CreateAlternateViewFromString(html,
                                                        Encoding.UTF8,
                                                        MediaTypeNames.Text.Html);
                            string Path = "\\Content\\final.png";
                            string rutafondo = Server.MapPath(Path);
                            LinkedResource img =
                                new LinkedResource(rutafondo,
                                         MediaTypeNames.Image.Jpeg);
                            img.ContentId = "imagen";
                            htmlView.LinkedResources.Add(img);

                            MailMessage msng2 = new MailMessage(mailemisor2, mailreceptor2, "Evento FBX40 25,26 y 27 de junio ", "");
                            msng2.IsBodyHtml = true;
                            msng2.AlternateViews.Add(htmlView);


                            SmtpClient smtpClient2 = new SmtpClient("smtp.gmail.com");
                            smtpClient2.EnableSsl = true;
                            smtpClient2.UseDefaultCredentials = false;
                            smtpClient2.Port = 587;
                            smtpClient2.Credentials = new System.Net.NetworkCredential(mailemisor2, contraseña2);

                            smtpClient2.Send(msng2);
                            smtpClient2.Dispose();

                            //IAccount envio = new IAccount();
                            //int numIntento = 1;
                            //int numenviado = numIntento + Convert.ToInt32(invitado.Num_Enviado);
                           // envio.actualizarInvitado(numenviado, invitado.Int_IdInvitado, invitado.Txt_Correo);


                        //}
                        //if (invitado.Bol_Vip == true)
                        //{

                        //    string fecha = DateTime.Now.ToString("dddd MM yyyy");

                        //    string mailemisor2 = "fbx40tulum@gmail.com";
                        //    string mailreceptor2 = invitado.Txt_Correo;
                        //    string contraseña2 = "fabricio21";
                        //    string text = "";
                        //    AlternateView plainView =
                        //        AlternateView.CreateAlternateViewFromString(text,
                        //                                Encoding.UTF8,
                        //                                MediaTypeNames.Text.Plain);
                        //    string html = "<body style='text-align:center;'>" +
                        //            "<img src='cid:imagen' style='text-align:center;width:60%; height: 60%;' />" +
                        //             "<br>" +
                        //            " <a href='https://fbx40.azurewebsites.net' style='text-align: center; background:linear-gradient(to bottom, #dda60f 5%, #d36217 100%);background-color:#e6a313; border-radius:22px;border: 2px solid #cc8b11;display: inline-block;color:#ffffff;font-family:Arial;font-size:8px;padding: 10px 38px;text-decoration:none;text-shadow:0px 1px 0px #2f6627;'> <h2>Registro para el evento</h></a> " +
                        //            "<h3><strong> Cena: Circle Vip</strong></h3>" +
                        //           "<h3><strong> Lugar: Secret Location.</strong></h3>" +
                        //           "<h3><strong> Hora: 7PM </strong></h3>" +
                        //            "<br>" +
                        //           "<h3 style ='text-align:center;'><strong> Sigue las actualizaciones en el sitio web, para duda o información comunicarse al WhatsApp:  +52 998 242 1114 </strong></h3>" +

                        //                        "</body>";
                        //    AlternateView htmlView =
                        //        AlternateView.CreateAlternateViewFromString(html,
                        //                                Encoding.UTF8,
                        //                                MediaTypeNames.Text.Html);
                        //    string Path = "\\Content\\Email.png";
                        //    string rutafondo = Server.MapPath(Path);
                        //    LinkedResource img =
                        //        new LinkedResource(rutafondo,
                        //                 MediaTypeNames.Image.Jpeg);
                        //    img.ContentId = "imagen";
                        //    htmlView.LinkedResources.Add(img);

                        //    MailMessage msng2 = new MailMessage(mailemisor2, mailreceptor2, "Evento FBX40 25,26 y 27 de junio  ", "");
                        //    msng2.IsBodyHtml = true;
                        //    msng2.AlternateViews.Add(htmlView);


                        //    SmtpClient smtpClient2 = new SmtpClient("smtp.gmail.com");
                        //    smtpClient2.EnableSsl = true;
                        //    smtpClient2.UseDefaultCredentials = false;
                        //    smtpClient2.Port = 587;
                        //    smtpClient2.Credentials = new System.Net.NetworkCredential(mailemisor2, contraseña2);

                        //    smtpClient2.Send(msng2);
                        //    smtpClient2.Dispose();

                        //    IAccount envio = new IAccount();
                        //    int numIntento = 1;
                        //    int numenviado = numIntento + Convert.ToInt32(invitado.Num_Enviado);
                        //    ///envio.actualizarInvitado(numenviado, invitado.Int_IdInvitado, invitado.Txt_Correo);

                        //}


                        respuesta = "correos de invitación enviados";
                    }
                }
                else
                {
                    respuesta = "el correo ingresado no existe";
                }

                
            }

            // ENVIO DE CORREO MASIVO PARA TODO LOS USUARIO QUE TIENE STATUS IGUAL 0 IGNORANDO LOS QUE TIENEN STATUS 4
            //if (clave == "MASIVO")
            //{

            //    //  var listInvitados = db.Tb_ListadoInvitados.Where(x => x.Int_Status == 0 && x.Int_Status != 4).ToList();
      
            //    //var lsi = (from r in db.Tb_RegistroInvitados                   
            //    //            join i in db.Tb_ListadoInvitados
            //    //           on r.Txt_Correo equals i.Txt_Correo
            //    //           where r.Txt_QR == null && i.Fec_Enviado==null && i.Int_Status != 4
            //    //           select new { i.Txt_Correo, i.Txt_Nombre, i.Int_IdInvitado,i.Num_Enviado,i.Bol_Vip}
            //    //           ).ToList();
            //    IAccount a = new IAccount();
            //    List<Tb_ListadoInvitados> listInvitados = a.Invitadosincompleto();

            //    //foreach (var invitado in listInvitados)
            //    //{
            //    //    if (invitado.Txt_Correo != null)
            //    //    {
            //    //        if (invitado.Bol_Vip == false)
            //    //        {

            //    //            string fecha = DateTime.Now.ToString("dddd MM yyyy");

            //    //            string mailemisor2 = "fbx40tulum@gmail.com";
            //    //            string mailreceptor2 = invitado.Txt_Correo;
            //    //            string contraseña2 = "fabricio21";
            //    //            string text = "";
            //    //            AlternateView plainView =
            //    //                AlternateView.CreateAlternateViewFromString(text,
            //    //                                        Encoding.UTF8,
            //    //                                        MediaTypeNames.Text.Plain);
            //    //            string html = "<body <body style='text-align:center;'>" +
            //    //                "<img src='cid:imagen' style='text-align:center;width:60%; height: 60%;'/>" +
            //    //                "<br>" +
            //    //                " <a href='https://fbx40.azurewebsites.net' style='text-align: center; background:linear-gradient(to bottom, #dda60f 5%, #d36217 100%);background-color:#e6a313; border-radius:22px;border: 2px solid #cc8b11;display: inline-block;color:#ffffff;font-family:Arial;font-size:8px;padding: 10px 38px;text-decoration:none;text-shadow:0px 1px 0px #2f6627;'> <h2>Registro para el evento</h></a> " +
            //    //                "<br>" +
            //    //               "<h3 style ='text-align:center;'><strong>Sigue las actualizaciones en el sitio web, para duda o información comunicarse al WhatsApp:  +52 998 242 1114 </strong></h3>" +
            //    //                  "</body>";
            //    //            AlternateView htmlView =
            //    //                AlternateView.CreateAlternateViewFromString(html,
            //    //                                        Encoding.UTF8,
            //    //                                        MediaTypeNames.Text.Html);
            //    //            string Path = "\\Content\\Email.png";
            //    //            string rutafondo = Server.MapPath(Path);
            //    //            LinkedResource img =
            //    //                new LinkedResource(rutafondo,
            //    //                         MediaTypeNames.Image.Jpeg);
            //    //            img.ContentId = "imagen";
            //    //            htmlView.LinkedResources.Add(img);

            //    //            MailMessage msng2 = new MailMessage(mailemisor2, mailreceptor2, "Evento FBX40 25,26 y 27 de junio ", "");
            //    //            msng2.IsBodyHtml = true;
            //    //            msng2.AlternateViews.Add(htmlView);


            //    //            SmtpClient smtpClient2 = new SmtpClient("smtp.gmail.com");
            //    //            smtpClient2.EnableSsl = true;
            //    //            smtpClient2.UseDefaultCredentials = false;
            //    //            smtpClient2.Port = 587;
            //    //            smtpClient2.Credentials = new System.Net.NetworkCredential(mailemisor2, contraseña2);

            //    //            smtpClient2.Send(msng2);
            //    //            smtpClient2.Dispose();

            //    //            IAccount envio = new IAccount();
            //    //            int numIntento = 1;
            //    //            int numenviado = numIntento + Convert.ToInt32(invitado.Num_Enviado);
            //    //            envio.actualizarInvitado(numenviado, invitado.Int_IdInvitado, invitado.Txt_Correo);


            //    //        }
            //    //        if (invitado.Bol_Vip == true)
            //    //        {

            //    //            string fecha = DateTime.Now.ToString("dddd MM yyyy");

            //    //            string mailemisor2 = "fbx40tulum@gmail.com";
            //    //            string mailreceptor2 = invitado.Txt_Correo;
            //    //            string contraseña2 = "fabricio21";
            //    //            string text = "";
            //    //            AlternateView plainView =
            //    //                AlternateView.CreateAlternateViewFromString(text,
            //    //                                        Encoding.UTF8,
            //    //                                        MediaTypeNames.Text.Plain);
            //    //            string html = "<body style='text-align:center;'>" +
            //    //                    "<img src='cid:imagen' style='text-align:center;width:60%; height: 60%;' />" +
            //    //                     "<br>" +
            //    //                    " <a href='https://fbx40.azurewebsites.net' style='text-align: center; background:linear-gradient(to bottom, #dda60f 5%, #d36217 100%);background-color:#e6a313; border-radius:22px;border: 2px solid #cc8b11;display: inline-block;color:#ffffff;font-family:Arial;font-size:8px;padding: 10px 38px;text-decoration:none;text-shadow:0px 1px 0px #2f6627;'> <h2>Registro para el evento</h></a> " +
            //    //                    "<h3><strong> Cena: Circle Vip</strong></h3>" +
            //    //                   "<h3><strong> Lugar: Secret Location.</strong></h3>" +
            //    //                   "<h3><strong> Hora: 7PM </strong></h3>" +
            //    //                    "<br>" +
            //    //                   "<h3 style ='text-align:center;'><strong> Sigue las actualizaciones en el sitio web, para duda o información comunicarse al WhatsApp:  +52 998 242 1114 </strong></h3>" +

            //    //                                "</body>";
            //    //            AlternateView htmlView =
            //    //                AlternateView.CreateAlternateViewFromString(html,
            //    //                                        Encoding.UTF8,
            //    //                                        MediaTypeNames.Text.Html);
            //    //            string Path = "\\Content\\Email.png";
            //    //            string rutafondo = Server.MapPath(Path);
            //    //            LinkedResource img =
            //    //                new LinkedResource(rutafondo,
            //    //                         MediaTypeNames.Image.Jpeg);
            //    //            img.ContentId = "imagen";
            //    //            htmlView.LinkedResources.Add(img);

            //    //            MailMessage msng2 = new MailMessage(mailemisor2, mailreceptor2, "Evento FBX40 25,26 y 27 de junio  ", "");
            //    //            msng2.IsBodyHtml = true;
            //    //            msng2.AlternateViews.Add(htmlView);


            //    //            SmtpClient smtpClient2 = new SmtpClient("smtp.gmail.com");
            //    //            smtpClient2.EnableSsl = true;
            //    //            smtpClient2.UseDefaultCredentials = false;
            //    //            smtpClient2.Port = 587;
            //    //            smtpClient2.Credentials = new System.Net.NetworkCredential(mailemisor2, contraseña2);

            //    //            smtpClient2.Send(msng2);
            //    //            smtpClient2.Dispose();

            //    //            IAccount envio = new IAccount();
            //    //            int numIntento = 1;
            //    //            int numenviado = numIntento + Convert.ToInt32(invitado.Num_Enviado);
            //    //            envio.actualizarInvitado(numenviado, invitado.Int_IdInvitado, invitado.Txt_Correo);

            //    //        }
            //    //    }
            //    //    respuesta = "correo enviados  a todo los invitado que no se ha registrado";
                  
            //    //}
            //}

            return Json(respuesta, JsonRequestBehavior.AllowGet);


        }


        public IEnumerable<InvitadosEvento> exist(string token)
        {
            IAccount a = new IAccount();
            IEnumerable<InvitadosEvento> list = a.Exists(token);

            return list;
        }

        #region
        public JsonResult MailSender()
        {
            string fecha = DateTime.Now.ToString("dddd MM yyyy");
            IAccount a = new IAccount();
            List<ListaInvitados> u = a.Invitados();
            int i = 0;
            var res = new MailMessage();
            string mailemisor2 = "fbx40tulum@gmail.com";
            string mailreceptor2 = "jcenteno@cecgroup.mx";
            string mailoculto2 = "";//"asantos@strategias.mx, omartinez@cecgroup.mx";
            string contraseña2 = "fabricio21";
            string text = "";
            AlternateView plainView = AlternateView.CreateAlternateViewFromString(text, Encoding.UTF8, MediaTypeNames.Text.Plain);

            string platilla = "<!DOCTYPE html>" +
                "<html lang = 'en'>" +
                "<head><meta charset = 'UTF-8' ></head >" +
                "<style>" +
                ".bodycont{width: 64rem;max-height: fit-content;" +
                "height:52rem;align-items: flex-end; display: flex;}" +
                ".image{position: absolute;height: 15rem;width: 15rem;margin - bottom: 1rem;top: 18rem;left: 13rem;z-index: 1;}" +
                 ".image2{position:absolute;height:35rem; width:40rem;z-index:-1;}" +
                "</style>" +
                "<body style='text-align: center;'>" +
                  "<DIV STYLE = 'position:absolute; top:36px; left:240px; visibility:visible z-index:-1' >" +
                   "<IMG SRC='cid:imagenFondo' height='250px' width='600px'></div>" +
                    "<DIV STYLE ='text-align:center;position:absolute; top:287px; left:462px; visibility:visible z-index:1'>" +
                   "<IMG height='160px' width='160px' SRC='cid:imagen'></div>" +
                   //"</div>" +
                   "</body></html>";
            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(platilla, Encoding.UTF8, MediaTypeNames.Text.Html);
            string imagePath2 = "~\\Images\\emialQR2.png";
            string rutafondo = Server.MapPath(imagePath2);
            LinkedResource img2 = new LinkedResource(rutafondo, MediaTypeNames.Image.Jpeg);

            img2.ContentId = "imagenFondo";
            htmlView.LinkedResources.Add(img2);

            // para obtner el qr
            string cadenaqr = "OU5BT8OPU7C5Z1ZPZUWEZX8FWFYDV9";
            string imagePath = "~\\Images\\" + cadenaqr + ".jpg";
            string barcodePath = Server.MapPath(imagePath);

            LinkedResource img = new LinkedResource(barcodePath, MediaTypeNames.Image.Jpeg);
            img.ContentId = "imagen";
            htmlView.LinkedResources.Add(img);

            MailMessage msg = new MailMessage(mailemisor2, mailreceptor2, "Evento FBX40 25,26 y 27 de junio ", "");
            // msg.Bcc.Add(mailoculto2);
            msg.IsBodyHtml = true;
            msg.AlternateViews.Add(htmlView);

            SmtpClient client = new SmtpClient("smtp.gmail.com");
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            client.Port = 587;
            client.Credentials = new System.Net.NetworkCredential(mailemisor2, contraseña2);

            client.Send(msg);
            client.Dispose();
            res = msg;

            //foreach (var x in u)
            //{
            //    mailemisor2 = u[i].Txt_Correo;

            //    i++;
            //    MailMessage msg = new MailMessage(mailemisor2, mailreceptor2, "Evento FBX40"+ fecha + " ", "");

            //    SmtpClient client = new SmtpClient("smtp.gmail.com");
            //    client.EnableSsl = true;
            //    client.UseDefaultCredentials = false;
            //    client.Port = 587;
            //    client.Credentials = new System.Net.NetworkCredential(mailemisor2, contraseña2);

            //    client.Send(msg);
            //    client.Dispose();
            //    res = msg;
            //}


            return Json(res, JsonRequestBehavior.AllowGet);
        }
        #endregion

        //CAMBIOS A SUBIR  envio de Qr invitado el status dos que ya se ha enviado su qr
        public JsonResult MailQRInvitado()
        {
            try
            {

                string fecha = DateTime.Now.ToString("dddd MM yyyy");
                IAccount a = new IAccount();
                List<Tb_RegistroInvitados> u = a.ListRegistroInvitados();
                int i = 0;
                var res = new MailMessage();
                string mailemisor2 = "fbx40tulum@gmail.com";
                string mailreceptor2 = "";
                string contraseña2 = "fabricio21";
                string qrcodePath = "";
                foreach (var item in u)
                {
                    string cadenaQr = item.Txt_Nombre;
                    string imagePath = "~/Images/" + cadenaQr + ".jpg";
                    string rutaQr = Server.MapPath(imagePath);
                    if (System.IO.File.Exists(rutaQr))
                    {
                        try
                        {
                            System.IO.File.Delete(rutaQr);
                        }
                        catch (System.IO.IOException e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    }
                    // parte para convertir la cadena string a qr
                    var barcodeWriter = new BarcodeWriter
                    {
                        Format = BarcodeFormat.QR_CODE,
                        Options = new EncodingOptions
                        {
                            Height = 300,
                            Width = 300
                        }
                    };
                    var result = barcodeWriter.Write(item.Txt_QR);
                    string barcodePath = Server.MapPath(imagePath);
                    var barcodeBitmap = new Bitmap(result);

                    using (MemoryStream memory = new MemoryStream())
                    {
                        using (FileStream fs = new FileStream(barcodePath, FileMode.Create, FileAccess.ReadWrite))
                        {
                            barcodeBitmap.Save(memory, ImageFormat.Jpeg);
                            byte[] bytes = memory.ToArray();
                            fs.Write(bytes, 0, bytes.Length);
                            string rutfin = imagePath + " " + barcodeBitmap;
                        }
                    }

                    //string text = "";
                    //AlternateView plainView = AlternateView.CreateAlternateViewFromString(text, Encoding.UTF8, MediaTypeNames.Text.Plain);
                    //string platilla = "<!DOCTYPE html>" +
                    //    "<html lang = 'en'>" +
                    //    "<head><meta charset = 'UTF-8' ></head >" +
                    //    "<style>" +
                    //    ".bodycont{width: 64rem;max-height: fit-content;" +
                    //    "height:52rem;align-items: flex-end; display: flex;}" +
                    //    ".image{position: absolute;height: 15rem;width: 15rem;margin - bottom: 1rem;top: 18rem;left: 13rem;z-index: 1;}" +
                    //     ".image2{position:absolute;height:35rem; width:40rem;z-index:-1;}" +
                    //    "</style>" +
                    //    "<body style='text-align: center; '>" +
                    //    "<DIV STYLE = 'position:absolute; top:36px; left:240px; visibility:visible z-index:-1' >" +
                    //    "<IMG SRC='cid:imagenFondo' height='250px' width='600px'></div>" +
                    //    "<DIV STYLE ='text-align:center;position:absolute; top:287px; left:462px; visibility:visible z-index:1'>" +
                    //    "<IMG height='160px' width='160px' SRC='cid:imagen'></div>" +
                    //     "<br>" +
                    //      "<h3 style ='text-align:center;'><strong> Sigue las actualizaciones en el sitio web, para duda o información comunicarse al WhatsApp:  +52 998 242 1114 </strong></h3>" +
                    //    "</body></html>";
                    //AlternateView htmlView = AlternateView.CreateAlternateViewFromString(platilla, Encoding.UTF8, MediaTypeNames.Text.Html);
                    //string imagePath2 = "~\\Images\\emialQR2.png";
                    //string rutafondo = Server.MapPath(imagePath2);
                    //LinkedResource img2 = new LinkedResource(rutafondo, MediaTypeNames.Image.Jpeg);
                    //img2.ContentId = "imagenFondo";
                    //htmlView.LinkedResources.Add(img2);
                    //para obtener el qr
                    //string imageQr = "~\\Images\\" + cadenaQr + ".jpg";
                    //qrcodePath = Server.MapPath(imageQr);
                    //LinkedResource img = new LinkedResource(qrcodePath, MediaTypeNames.Image.Jpeg);
                    //img.ContentId = "imagen";
                    //htmlView.LinkedResources.Add(img);

                    //cambiar el seteo para enviar todo las variables
                    //mailreceptor2 = item.Txt_Correo; //"jcenteno@cecgroup.mx";

                    //MailMessage msg = new MailMessage(mailemisor2, mailreceptor2, "Evento FBX40 25,26 y 27 de junio", "");
                    //msg.IsBodyHtml = true;
                    //msg.AlternateViews.Add(htmlView);
                    //SmtpClient client = new SmtpClient("smtp.gmail.com");
                    //client.EnableSsl = true;
                    //client.UseDefaultCredentials = false;
                    //client.Port = 587;
                    //client.Credentials = new System.Net.NetworkCredential(mailemisor2, contraseña2);
                    //client.Send(msg);
                    //client.Dispose();
                    //res = msg;

                    a.actualizarRegistroInvitado(2, item.Int_IdRegistro, "Envio Whatsapp");

                }
                return Json("QR enviado a los invitados", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
           
        }
        //CAMBIOS A SUBIR envio de Qr acompañante
        public JsonResult MailQRAcompanante()
        {
            try
            {

           
            string fecha = DateTime.Now.ToString("dddd MM yyyy");
            IAccount a = new IAccount();
            List<Tb_RegistroAcompañantes> u = a.ListRegistroAcompanantes();
            int i = 0;
            var res = new MailMessage();
            string mailemisor2 = "fbx40tulum@gmail.com";
            string mailreceptor2 = "";
            string contraseña2 = "fabricio21";
            string qrcodePath = "";
            foreach (var item in u)
            {
                string cadenaQr = item.Txt_Nombre;
                string imagePath = "~/Images/" + cadenaQr + ".jpg";
                string rutaQr = Server.MapPath(imagePath);
                if (System.IO.File.Exists(rutaQr))
                {
                    try
                    {
                        System.IO.File.Delete(rutaQr);
                    }
                    catch (System.IO.IOException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                // parte para convertir la cadena string a qr
                var barcodeWriter = new BarcodeWriter
                {
                    Format = BarcodeFormat.QR_CODE,
                    Options = new EncodingOptions
                    {
                        Height = 300,
                        Width = 300
                    }
                };
                var result = barcodeWriter.Write(item.Txt_QR);
                string barcodePath = Server.MapPath(imagePath);
                var barcodeBitmap = new Bitmap(result);

                using (MemoryStream memory = new MemoryStream())
                {
                    using (FileStream fs = new FileStream(barcodePath, FileMode.Create, FileAccess.ReadWrite))
                    {
                        barcodeBitmap.Save(memory, ImageFormat.Jpeg);
                        byte[] bytes = memory.ToArray();
                        fs.Write(bytes, 0, bytes.Length);
                        string rutfin = imagePath + " " + barcodeBitmap;
                    }
                }

                    //string text = "";
                    //AlternateView plainView = AlternateView.CreateAlternateViewFromString(text, Encoding.UTF8, MediaTypeNames.Text.Plain);
                    //string platilla = "<!DOCTYPE html>" +
                    //    "<html lang = 'en'>" +
                    //    "<head><meta charset = 'UTF-8' ></head >" +
                    //    "<style>" +
                    //    ".bodycont{width: 64rem;max-height: fit-content;" +
                    //    "height:52rem;align-items: flex-end; display: flex;}" +
                    //    ".image{position: absolute;height: 15rem;width: 15rem;margin - bottom: 1rem;top: 18rem;left: 13rem;z-index: 1;}" +
                    //     ".image2{position:absolute;height:35rem; width:40rem;z-index:-1;}" +
                    //    "</style>" +
                    //    "<body style='text-align: center; '>" +
                    //    "<DIV STYLE = 'position:absolute; top:36px; left:240px; visibility:visible z-index:-1' >" +
                    //    "<IMG SRC='cid:imagenFondo' height='250px' width='600px'></div>" +
                    //    "<DIV STYLE ='text-align:center;position:absolute; top:287px; left:462px; visibility:visible z-index:1'>" +
                    //    "<IMG height='160px' width='160px' SRC='cid:imagen'></div>" +
                    //     "<br>" +
                    //      "<h3 style ='text-align:center;'><strong>Sigue las actualizaciones en el sitio web, para duda o información comunicarse al WhatsApp:  +52 998 242 1114 </strong></h3>" +
                    //    "</body></html>";
                    //AlternateView htmlView = AlternateView.CreateAlternateViewFromString(platilla, Encoding.UTF8, MediaTypeNames.Text.Html);
                    //string imagePath2 = "~\\Images\\emialQR2.png";
                    //string rutafondo = Server.MapPath(imagePath2);
                    //LinkedResource img2 = new LinkedResource(rutafondo, MediaTypeNames.Image.Jpeg);
                    //img2.ContentId = "imagenFondo";
                    //htmlView.LinkedResources.Add(img2);
                    //// para obtener el qr
                    //string imageQr = "~\\Images\\" + cadenaQr + ".jpg";
                    //qrcodePath = Server.MapPath(imageQr);
                    //LinkedResource img = new LinkedResource(qrcodePath, MediaTypeNames.Image.Jpeg);
                    //img.ContentId = "imagen";
                    //htmlView.LinkedResources.Add(img);

                    ////cambiar el seteo para enviar todo las variables
                    //mailreceptor2 = item.Txt_Correo; //"jcenteno@cecgroup.mx";

                    //MailMessage msg = new MailMessage(mailemisor2, mailreceptor2, "Evento FBX40 25,26 y 27 de junio", "");
                    //msg.IsBodyHtml = true;
                    //msg.AlternateViews.Add(htmlView);
                    //SmtpClient client = new SmtpClient("smtp.gmail.com");
                    //client.EnableSsl = true;
                    //client.UseDefaultCredentials = false;
                    //client.Port = 587;
                    //client.Credentials = new System.Net.NetworkCredential(mailemisor2, contraseña2);
                    //client.Send(msg);
                    //client.Dispose();
                    //res = msg;

                    a.actualizarRegistroAcompanante(2, item.Int_IdRegistro, item.Txt_Correo);

            }
                return Json("QR enviado a los acompañante", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet); throw;
            }
           
        }

    }


}