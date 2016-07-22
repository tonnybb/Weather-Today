using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
/*
 * To install the Newtonsoft.Json package, go to
 * TOOLS > NuGet Package Manager > Package Manager Console 
 * and paste "Install-Package Newtonsoft.Json" without quotes
 */
using Newtonsoft.Json;

namespace WeatherForecastREST
{
    public partial class FrmPrimary : Form
    {
        public static string m_Response { get; private set; }
        public static bool m_IsXml { get; private set; }
        public static Rootobject m_Root { get; private set; }

        public FrmPrimary()
        {
            InitializeComponent();

            lblCloudCover.Text = string.Empty;
            lblDescription.Text = string.Empty;
            lblFeelsLike.Text = string.Empty;
            lblHumidity.Text = string.Empty;
            txtLastUpdated.Text = string.Empty;
            lblPrecipitation.Text = string.Empty;
            lblTemperature.Text = string.Empty;

            rdoXml.Checked = true;
        }

        public async Task PlaceApiCall()
        {
            // Get location entered by user
            string location = txtLocation.Text;

            string apiKey = "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX";

            // used to append "xml" or "json" to end of api call URI.
            string xmlOrJson = string.Empty;

            if (rdoJson.Checked)
            {
                xmlOrJson = "json";
                m_IsXml = false;
            }
            else if (rdoXml.Checked)
            {
                xmlOrJson = "xml";
                m_IsXml = true;
            }

            string link = "http://api.worldweatheronline.com/premium/v1/weather.ashx?key=" + apiKey + "&q=" + location + "&num_of_days=1&showlocaltime=yes&fx=no&mca=no&date=today&format=" + xmlOrJson;

            // Get XML/JSON document as a string
            HttpClient http = new HttpClient();
            m_Response = await http.GetStringAsync(link);

            if (m_IsXml)
            {
                // XML
                updateTextFieldsFromXml(m_Response);
            }
            else
            {
                // JSON
                updateTextFieldsFromJson(m_Response);
            }
        }

        private void updateTextFieldsFromJson(string response)
        {
            // Deserialize JSON string into an object
            m_Root = JsonConvert.DeserializeObject<Rootobject>(response);

            // Get location
            string location = m_Root.data.request[0].query;
            txtLocation.Text = location;

            // Get timestamp
            string timeStamp = m_Root.data.time_zone[0].localtime;
            txtLastUpdated.Text = timeStamp;

            // Get temperature
            string tempInC = m_Root.data.current_condition[0].temp_C;
            lblTemperature.Text = tempInC + " C";

            // Get 'feels like' temperature
            string feelsLike = m_Root.data.current_condition[0].FeelsLikeC;
            lblFeelsLike.Text = feelsLike + " C";

            // Get weather description
            string description = m_Root.data.current_condition[0].weatherDesc[0].value;
            lblDescription.Text = description;

            // Get precipitation
            string precipitation = m_Root.data.current_condition[0].precipMM;
            lblPrecipitation.Text = precipitation + " mm";

            // Get cloudcover
            string cloudcover = m_Root.data.current_condition[0].cloudcover;
            lblCloudCover.Text = cloudcover + " %";

            // Get humidity
            string humidity = m_Root.data.current_condition[0].humidity;
            lblHumidity.Text = humidity + " %";

            // Get weather icon
            string icon = m_Root.data.current_condition[0].weatherIconUrl[0].value;

            // Set weather icon
            picWeatherIcon.Load(icon);
        }

        private void updateTextFieldsFromXml(string response)
        {
            // Build XML document using response string
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(response);

            XmlNodeList node;

            // Get location
            node = xmlDoc.GetElementsByTagName("query");
            string location = node[0].InnerText;
            txtLocation.Text = location;

            // Get timestamp
            node = xmlDoc.GetElementsByTagName("localtime");
            string timeStamp = node[0].InnerText;
            txtLastUpdated.Text = timeStamp;

            // Get temperature
            node = xmlDoc.GetElementsByTagName("temp_C");
            string tempInC = node[0].InnerText;
            lblTemperature.Text = tempInC + " C";

            // Get 'feels like' temperature
            node = xmlDoc.GetElementsByTagName("FeelsLikeC");
            string feelsLike = node[0].InnerText;
            lblFeelsLike.Text = feelsLike + " C";

            // Get weather description
            node = xmlDoc.GetElementsByTagName("weatherDesc");
            string description = node[0].InnerText;
            lblDescription.Text = description;

            // Get precipitation
            node = xmlDoc.GetElementsByTagName("precipMM");
            string precipitation = node[0].InnerText;
            lblPrecipitation.Text = precipitation + " mm";

            // Get cloudcover
            node = xmlDoc.GetElementsByTagName("cloudcover");
            string cloudcover = node[0].InnerText;
            lblCloudCover.Text = cloudcover + " %";

            // Get humidity
            node = xmlDoc.GetElementsByTagName("humidity");
            string humidity = node[0].InnerText;
            lblHumidity.Text = humidity + " %";

            // Get weather icon
            node = xmlDoc.GetElementsByTagName("weatherIconUrl");
            string icon = node[0].InnerText;
           
            // Set weather icon
            picWeatherIcon.Load(icon);
        }

        private void txtLocation_Click(object sender, EventArgs e)
        {
            // When the user clicks the text field, clear its contents
            txtLocation.Text = string.Empty;
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            PlaceApiCall();

            // Allow the user to click the 'Show Source' only after the 'Update' button has been clicked.
            btnShowSource.Enabled = true;
        }

        private void txtLocation_KeyPress(object sender, KeyPressEventArgs e)
        {
            // If the user presses the Enter key while the text field has focus then execute weather query
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnUpdate_Click(sender, e);
            }
        }

        private void btnShowSource_Click(object sender, EventArgs e)
        {
            // Open a new window to show the raw XML/JSON string
            FrmShowSources newWindow = new FrmShowSources();
            newWindow.Show();
        }

        private void rdoXml_CheckedChanged(object sender, EventArgs e)
        {
            // Do not allow the user to click the 'Show Source' button until after the 'Update' button has been clicked.
            btnShowSource.Enabled = false;
        }

        private void rdoJson_CheckedChanged(object sender, EventArgs e)
        {
            // Do not allow the user to click the 'Show Source' button until after the 'Update' button has been clicked.
            btnShowSource.Enabled = false;
        }
    }
}