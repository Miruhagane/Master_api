using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web.Mvc;
using Queue.Models;
using Queue.Models.IRepository.IFLH;
using ZXing;
using System.Data.Sql;
using System.Data.SqlClient;


namespace Queue.Controllers
{
    public class QRController : Controller
    {
        FLHEntities db = new FLHEntities();
        string con = System.Configuration.ConfigurationManager.ConnectionStrings["FLHConnection"].ConnectionString;
        // GET: QR
       // [HttpPost]

        //PRINCIPAL
        public string QRCode(string token, bool e1, bool e2, bool e3)
        {
            string s = "";

            var detail = db.Tb_RegistroInvitados.FirstOrDefault(x => x.Txt_Token == token && x.Txt_QR == null);
            if(detail != null)
            {
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
                try
                {
                    
                    using (SqlConnection sql = new SqlConnection(con))
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
                            evento3 = 2;
                        }


                        sql.Open();
                        string query = "update Tb_RegistroInvitados set Txt_QR = '" + s + "' where Txt_Token = '"+ token + "' " +
                                       "insert into Tb_EventosInvitado(Int_IdInvitado, Bol_Evento1, Bol_Evento2,  Bol_Evento3, Fec_Alta, Bol_Validado) " +
                                       "values('"+detail.Int_IdRegistro+"', "+evento1+ ", " + evento2 + ", " + evento3 + ", getdate(), 1)";
                        SqlCommand cmd = new SqlCommand(query, sql);
                        cmd.ExecuteNonQuery();
                        sql.Close();
                    }
                }
                catch (Exception er)
                {

                }

                 

                 Generate(s);
            }
         
            return s;
        }

        public string Generate(string result)
        {
            try
            {

                QRCode u = new QRCode
                {
                    QRCodeText = result
                };
                u.QRCodeImagePath = GenerateQRCode(u.QRCodeText);
                ViewBag.Message = "QR Code Created successfully";


            }
            catch (Exception ex)
            {
                //catch exception if there is any
            }
            return result;
        }

        private string GenerateQRCode(string qrcodeText)
        {
            string fu = "";
            string folderPath = "~/Images/";
            string imagePath = "~/Images/" + qrcodeText + ".jpg";
            // If the directory doesn't exist then create it.
            if (!Directory.Exists(Server.MapPath(folderPath)))
            {
                Directory.CreateDirectory(Server.MapPath(folderPath));
            }
            var barcodeWriter = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE//,
                //Options = new ZXing.Common.EncodingOptions
                //{
                //    Height = 300,
                //    Width = 300,
                //    PureBarcode = true
                //}
            };

            var result = barcodeWriter.Write(qrcodeText);
           
            string barcodePath = Server.MapPath(imagePath);
            var barcodeBitmap = new Bitmap(result);
            
           
            //b = barcodeBitmap;
            using (MemoryStream memory = new MemoryStream())
            {
                using (FileStream fs = new FileStream(barcodePath, FileMode.Create, FileAccess.ReadWrite))
                {
                    int x, y;
                    for(x=0;x< barcodeBitmap.Width; x++)
                    {
                        for (y = 0; y < barcodeBitmap.Height; y++)
                        {
                            Color pixelColor = barcodeBitmap.GetPixel(x, y);
                            Color newColor = Color.FromArgb(pixelColor.R, x, y);
                            barcodeBitmap.SetPixel(x, y, newColor);
                        }
                    }
                    barcodeBitmap.Save(memory, ImageFormat.Jpeg);

                    byte[] bytes = memory.ToArray();
                    fs.Write(bytes, 0, bytes.Length);
                    Session["data"] = imagePath + " " + barcodeBitmap;
                }
            }
            
            return fu;
        }
    }
}