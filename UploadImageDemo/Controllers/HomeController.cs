using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UploadImageDemo.Processor;

namespace UploadImageDemo.Controllers
{
    public class HomeController : Controller
    {
        ImageProcessor processor;

        public HomeController()
        {
            processor = new ImageProcessor();
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Upload()
        {
            try
            {
                var upload_dir = "uploads";

                var folderPath = Server.MapPath("~/") + upload_dir;
                if (!System.IO.Directory.Exists(folderPath))
                {
                    System.IO.Directory.CreateDirectory(folderPath);
                }

                var allow_ext = new string[] { "jpg", "jpeg", "png", "gif" };

                if (Request.Files != null && Request.Files.Count > 0)
                {
                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        var file = Request.Files[i];

                        //检查大小
                        //var x = stream.Length;

                        string path = Path.Combine(folderPath, file.FileName);

                        FileInfo info = new FileInfo(path);

                        string path100 = Path.Combine(folderPath, file.FileName.Replace(info.Extension, "_100_100" + info.Extension));

                        //保存原始图片
                        file.SaveAs(path);

                        using (var stream = processor.ResizeImage(file.InputStream, 900, 500))
                        {
                            var bytes = new byte[stream.Length];

                            stream.Read(bytes, 0, bytes.Length);

                            System.IO.File.WriteAllBytes(path100, bytes);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return Json(Request.Files.Count);
        }
    }
}
