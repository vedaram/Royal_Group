<%@ WebHandler Language="C#" Class="FileUpload" %>

using System;
using System.Web;
using System.IO;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class FileUpload : IHttpHandler, System.Web.SessionState.IRequiresSessionState {
    
    public void ProcessRequest (HttpContext context) {

        ReturnObject return_object = new ReturnObject();

        string
            file_path = string.Empty;

        if (context.Request.Files.Count > 0)
        {
            HttpFileCollection uploaded_files = context.Request.Files;
            HttpPostedFile posted_file = uploaded_files[0];

            string file_name = context.Request["filename"];

            file_path = context.Server.MapPath("~/uploads/profiles/" + file_name);

            if (File.Exists(file_path))
                File.Delete(file_path);
            
            posted_file.SaveAs(file_path);

            string baseUrl = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.ApplicationPath.TrimEnd('/') + "/uploads/profiles/";
            
            context.Session["display_picture"] = baseUrl + file_name.ToString();
            
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