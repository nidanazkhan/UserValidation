using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace UserValidation_first.Pages
{
    public class ConsumeXMLModel : PageModel
    {
        private IHostingEnvironment _environment;
        private string result;
        public ConsumeXMLModel(IHostingEnvironment environment)
        {
            _environment = environment;
        }

        [BindProperty]
        public IFormFile Upload { get; set; }

        public void OnGet()
        {

        }

        public void OnPost()
        {
            string fileName = Upload.FileName;
            string file = Path.Combine(_environment.ContentRootPath, "uploads", fileName);
            using (var fileStream = new FileStream(file, FileMode.Create))
            {
                Upload.CopyTo(fileStream);
            }
            XmlDocument doc = new XmlDocument();
            doc.Load(file);
            XmlNode node = doc.SelectSingleNode("/users/user[firstname='Nikita']");
           
            ValidateXML(file);
        }

        private void ValidateXML(string file)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.Schema;

            var xsdPath = Path.Combine(_environment.ContentRootPath, "uploads", "uservalidation(1).xsd");
            settings.Schemas.Add(null, xsdPath);

            settings.ValidationFlags |= System.Xml.Schema.XmlSchemaValidationFlags.ProcessSchemaLocation;
            settings.ValidationFlags |= System.Xml.Schema.XmlSchemaValidationFlags.ReportValidationWarnings;
            settings.ValidationEventHandler += new System.Xml.Schema.ValidationEventHandler(this.ValidationEventHandler);

            XmlReader xmlReader = XmlReader.Create(file, settings);
            try
            {
                while (xmlReader.Read())
                {

                }
                result = "validation passed!";
                ViewData["result"] = "validation Passed";
            } catch(Exception e)
            {
                ViewData["result"] = e.Message;
            }
        }

        public void ValidationEventHandler(object sender, ValidationEventArgs args)
        {
            result = "Validation failed. Message" + args.Message;
            throw new Exception("Validation failed. Message" + args.Message);
        }
    }
}