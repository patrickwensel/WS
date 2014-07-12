using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace WS.WinForm.ConvertEFSchema
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            
            InitializeComponent();
            txtEDMXPath.Text = @"C:\code\trunk\WS\WS.Framework\WSJDE\WSJDE.edmx";
        }

        private void btnConvertToDev_Click(object sender, EventArgs e)
        {
            XDocument doc = XDocument.Load(txtEDMXPath.Text);
            foreach (XElement cell in doc.Element("edmx").Elements())
            {
                if (cell.Element("ActionDate").Value == "PRODDTA")
                {
                    cell.Element("ActionDate").Value = "XXXX";
                }
            }

            doc.Save("c:\\tst.xml");



            //XmlDocument doc = new XmlDocument();
            //doc.Load(txtEDMXPath.Text);
            
            //string x = xmlDoc.ToString();

            //x = x.Replace("Schema='PRODDTA'", "Schema='XXXXXX'");

            //XmlDocument newXMLDoc = new XmlDocument();
            //newXMLDoc.LoadXml(x);

            //newXMLDoc.Save(txtEDMXPath.Text);
        }
    }
}
