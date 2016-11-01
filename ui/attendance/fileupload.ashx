<%@ WebHandler Language="C#" Class="FileUpload" %>

using System;
using System.Web;
using System.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class FileUpload : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {

        ReturnObject return_object = new ReturnObject();

        string
            upload_path = ConfigurationManager.AppSettings["TEMP_FILE_UPLOAD"].ToString(),
            file_path = string.Empty;

        if (context.Request.Files.Count > 0)
        {
            HttpFileCollection uploaded_files = context.Request.Files;
            HttpPostedFile posted_file = uploaded_files[0];

            string file_name = context.Request["filename"];

            file_path = context.Server.MapPath("~/" + upload_path + "/" + file_name);
            posted_file.SaveAs(file_path);

            return_object.status = "success";
            return_object.return_data = file_name.ToString();
        }
        else {
            return_object.status = "error";
            return_object.return_data = "Please select a file to upload";
        }

        context.Response.ContentType = "text/html";
        context.Response.Write(JsonConvert.SerializeObject(return_object));
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}