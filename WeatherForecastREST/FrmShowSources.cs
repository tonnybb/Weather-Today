using Newtonsoft.Json;
using System;
using System.Windows.Forms;
using System.Xml.Linq;

namespace WeatherForecastREST
{
    public partial class FrmShowSources : Form
    {
        public FrmShowSources()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ShowSourceForm_Load(object sender, EventArgs e)
        {
            if (FrmPrimary.m_IsXml)
            {
                // Make XML string look nice and indented
                string formattedXml = FormatXml(FrmPrimary.m_Response);
                txtShowSource.Text = formattedXml;
            }
            else
            {
                // Make JSON string look nice and indented
                string formattedJson = JsonConvert.SerializeObject(FrmPrimary.m_Root, Formatting.Indented);
                txtShowSource.Text = formattedJson;
            }
        }

        private string FormatXml(string xml)
        {
            try
            {
                XDocument doc = XDocument.Parse(xml);
                return doc.ToString();
            }
            catch (Exception e)
            {
                return xml  + "\n\nERROR:" + e.Message;
            }
        }
    }
}