using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using System.Net;
using HtmlAgilityPack;

namespace bloodHelperNew
{
    public partial class Form1 : Form
    {
        int loc = 2; int bg = 1; int page = 1; int totalPages;
        bool firstAttempt = true;
        string MAIN_URL = "http://bloodhelpers.com/search-blood-donor.php?location=";
        string url;
        string ConString = ConfigurationManager.AppSettings["connection"].ToString();
        SqlConnection DbCon;
        public Form1()
        {
            InitializeComponent();
            webBrowser1.ScriptErrorsSuppressed = true;
            webBrowser1.Navigate("http://bloodhelpers.com/search-blood-donor.php?location=2&bloodGroup=1&pageNo=1");
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (webBrowser1.ReadyState == WebBrowserReadyState.Complete && e.Url == webBrowser1.Url)
            {
                if (!firstAttempt)
                {
                    ExtractUsers(webBrowser1.DocumentText);
                }
                firstAttempt = false;
                PageHasData(webBrowser1.DocumentText);
                NavigateToNextUrl();
            }
        }

        Boolean PageHasData(string documentText)
        {
            try
            {
                HtmlAgilityPack.HtmlDocument htmlDocument = new HtmlAgilityPack.HtmlDocument();
                htmlDocument.LoadHtml(documentText);

                HtmlAgilityPack.HtmlNode bloodDonor = htmlDocument.DocumentNode.SelectSingleNode("//*[@id='regmain']");

                HtmlAgilityPack.HtmlNode userLink = bloodDonor.SelectSingleNode("//*[@id='regPage']/div/div/div");

                // Of Label
                // LabelElement.InnerHTML.Equals("of")
                // linkPageCount = aElement.InnHTML
                var n1 = userLink.Descendants().ToArray();

                var link = userLink.Descendants().Count();

                totalPages = Int32.Parse(n1[(link - 5)].InnerText);


                if (totalPages > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                totalPages = 0;
                return false;
            }

        }

        void NavigateToNextUrl()
        {
            if (loc <= 10)
            {
                if (bg <= 17)
                {
                    if (page <= totalPages)
                    {
                        url = MAIN_URL + loc + "&bloodGroup=" + bg + "&pageNo=" + page;
                        webBrowser1.Navigate(url);

                        page++;
                    }
                    else
                    {
                        bg++;
                        page = 1;

                        url = MAIN_URL + loc + "&bloodGroup=" + bg + "&pageNo=" + page;
                        webBrowser1.Navigate(url);

                        firstAttempt = true;
                    }
                }
                else
                {
                    loc++;
                    bg = 1;
                    page = 1;

                    url = MAIN_URL + loc + "&bloodGroup=" + bg + "&pageNo=" + page;
                    webBrowser1.Navigate(url);

                    firstAttempt = true;
                }
            }
            else
            {
                MessageBox.Show("Scrap Compelete");
                CloseDB();
                // Close Exe
            }
        }

        void ExtractUsers(string documentText)
        {
            try
            {
                HtmlAgilityPack.HtmlDocument htmlDocument = new HtmlAgilityPack.HtmlDocument();
                htmlDocument.LoadHtml(documentText);

                HtmlAgilityPack.HtmlNode bloodDonor = htmlDocument.DocumentNode.SelectSingleNode("//*[@id='regmain']");

                HtmlAgilityPack.HtmlNode linkPage = htmlDocument.DocumentNode.SelectSingleNode("//*[@id='regPage']/div/div/div/a[4]");

                //linkPageCount = linkPage.InnerLength;

                HtmlAgilityPack.HtmlNode userTable = htmlDocument.DocumentNode.SelectSingleNode("//*[@id='regPage']/div/table");

                HtmlAgilityPack.HtmlNodeCollection userTableCount = userTable.SelectNodes("./tr");

                for (int k = 2; k <= userTableCount.Count(); k++)
                {
                    HtmlAgilityPack.HtmlNode moreBtn = userTable.SelectSingleNode("./tr[" + k + "]/td[6]/a");

                    string linkBtn = moreBtn.GetAttributeValue("onclick", null);
                    string[] mainLink = linkBtn.Split('b');
                    string moreLink = mainLink[1];
                    using (WebClient client = new WebClient())
                    {
                        var link = client.DownloadString("http://bloodhelpers.com/b" + moreLink);
                        HtmlAgilityPack.HtmlDocument userDocument = new HtmlAgilityPack.HtmlDocument();
                        userDocument.LoadHtml(link);

                        HtmlAgilityPack.HtmlNode selectTable = userDocument.DocumentNode.SelectSingleNode("//*[@id='search']/form/table");

                        //User Name
                        HtmlAgilityPack.HtmlNode userName = selectTable.SelectSingleNode("./tr[1]/td[2]");
                        string Name = userName.InnerText;

                        //User Email
                        HtmlAgilityPack.HtmlNode userEmail = selectTable.SelectSingleNode("./tr[2]/td[2]");
                        string[] Mail = userEmail.InnerHtml.Split('=');
                        string addMail = Mail[1] + "=" + Mail[2];
                        string Email = addMail.Replace("border", "");
                        //fuction to convert png image into jpg


                        //User BloodGroup
                        HtmlAgilityPack.HtmlNode userBloodGroup = selectTable.SelectSingleNode("./tr[3]/td[2]");
                        string BloodGroup = userBloodGroup.InnerText;

                        //User Gender
                        HtmlAgilityPack.HtmlNode userGender = selectTable.SelectSingleNode("./tr[4]/td[2]");
                        string Gender = userGender.InnerText;

                        //User Age
                        HtmlAgilityPack.HtmlNode userAge = selectTable.SelectSingleNode("./tr[5]/td[2]");
                        int Age = Int32.Parse(userAge.InnerText.Replace("Years", ""));

                        //User City
                        HtmlAgilityPack.HtmlNode userCity = selectTable.SelectSingleNode("./tr[6]/td[2]");
                        string City = userCity.InnerText;

                        //User Mobile Number 
                        HtmlAgilityPack.HtmlNode userMobile = selectTable.SelectSingleNode("./tr[7]/td[2]");
                        string[] num = userMobile.InnerHtml.Split('=');
                        string addNum = num[1] + "=" + num[2];
                        string mobileNumber = addNum.Replace("border", "");
                        //function to convert png image into jpg

                        //User Land Line Number 
                        HtmlAgilityPack.HtmlNode userLandLine = selectTable.SelectSingleNode("./tr[8]/td[2]");
                        string[] landNum = userLandLine.InnerHtml.Split('=');
                        string addLandNum = landNum[1] + "=" + landNum[2];
                        string landLineNum = addLandNum.Replace("border", "");
                        //Function to convert png image into jpg

                        //User Last Donation Date
                        HtmlAgilityPack.HtmlNode userLastDonationDate = selectTable.SelectSingleNode("./tr[9]/td[2]");
                        string LastDonationDate = userLastDonationDate.InnerText;


                        //Store Data in DataBase
                        StoreUserData(Name, Email, BloodGroup, Gender, Age, City, mobileNumber, landLineNum, LastDonationDate);
                    }
                }
            }
            catch
            {

            }
        }


        private void StoreUserData(string name, string email, string bloodGroup, string gender, int age, string city, string mobileNumber, string landLineNumber, string lastDonationDate)
        {
            try
            {
                OpenDB();
                SqlCommand cmd = DbCon.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"INSERT INTO [dbo].[bloodHelperUsers] VALUES (@Name, @Email, @BloodGroup, @Gender, @Age, @City, @MobileNumber, @LandLineNumber, @LastDonationDate)";
                cmd.Parameters.Add(new SqlParameter { ParameterName = "@Name", Value = name.Trim() });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "@Email", Value = email.Trim() });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "@BloodGroup", Value = bloodGroup.Trim() });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "@Gender", Value = gender.Trim() });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "@Age", Value = age });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "@City", Value = city.Trim() });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "@MobileNumber", Value = mobileNumber.Trim() });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "@LandLineNumber", Value = landLineNumber.Trim() });
                cmd.Parameters.Add(new SqlParameter { ParameterName = "@LastDonationDate", Value = lastDonationDate.Trim() });
                cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {

            }
        }


        private void OpenDB()
        {
            if (DbCon == null)
            {
                DbCon = new SqlConnection();
                DbCon.ConnectionString = ConString;
                DbCon.Open();
            }
        }

        private void CloseDB()
        {
            if (DbCon != null && DbCon.State == System.Data.ConnectionState.Open)
            {
                DbCon.Close();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseDB();
        }
    }
}

