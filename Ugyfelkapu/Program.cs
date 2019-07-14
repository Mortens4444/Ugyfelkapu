#define FORM_URL_ENCODED_CONTENT

using Mtf.Utils.StringExtensions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Ugyfelkapu
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // Specify to use TLS 1.2 as default connection
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            if (args.Length < 0)
            {
                Console.WriteLine("Adja meg a keresett jármű rendszámát.");
                return;
            }

            try
            {
                var username = ConfigurationManager.AppSettings["Username"];
                var password = ConfigurationManager.AppSettings["Password"];
                var licensePlate = args[0];

                var getCarInfoTask = GetCarInfoAsync(username, password, licensePlate);
                getCarInfoTask.Wait();
                Console.WriteLine(getCarInfoTask.Result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
				if (ex.InnerException != null)
				{
					Console.WriteLine(ex.InnerException.Message);
				}
            }

            Console.ReadLine();
        }

        private static async Task<CarInfo> GetCarInfoAsync(string user, string pass, string licensePlate)
        {
			if (licensePlate.Length != 7)
			{
				throw new Exception("Érvénytelen rendszám-formátum. Helyesen: WWW-123. Egyedi rendszám esetén: WWWW-12 vagy WWWWW-1");
			}

            const string serviceLink = "https://kereses.magyarorszag.hu/gepjarmukereso";
            using (var httpClient = new HttpClient { BaseAddress = new Uri("https://gate.gov.hu") })
            {
                httpClient.DefaultRequestHeaders.Add("User-Agent", "Apache-HttpClient/4.1.1 (java 1.5)");
                var content = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
                {
                    new KeyValuePair<string, string>("partnerid", "mohu"),
                    new KeyValuePair<string, string>("felhasznaloNev", user),
                    new KeyValuePair<string, string>("jelszo", pass),
                    new KeyValuePair<string, string>("target", serviceLink),
                    new KeyValuePair<string, string>("artifact", null)
                });
                var result = await httpClient.PostAsync("/sso/ap/ApServlet", content);
                if (result.StatusCode == HttpStatusCode.OK)
                {
                    string resultContent = await result.Content.ReadAsStringAsync();
                    if (resultContent.IndexOf("sikeresen bejelentkezett") > 0)
                    {
                        result = await httpClient.GetAsync($"/sso/InterSiteTransfer?TARGET={serviceLink}/CarSearchWindow?id=frmCarSearch&PARTNER=mohu&struts.portlet.mode=view&dynamicAttributes=%7B%7D&struts.portlet.action=%2FcarSearchPortlet%2FcarSearchPortlet%2FshowCarSearchViewPortlet&templateDir=custom&theme=simple&action=d&windowstate=normal&mode=view");
                        resultContent = await result.Content.ReadAsStringAsync();
                        var jsessionid = resultContent.Substring("jsessionid=", "\"");

                        result = await httpClient.GetAsync($"{serviceLink}/CarSearchWindow?jsessionid={jsessionid}&PARTNER=mohu&id=frmCarSearch&struts.portlet.mode=view&dynamicAttributes=%7B%7D&struts.portlet.action=%2FcarSearchPortlet%2FcarSearchPortlet%2FshowCarSearchViewPortlet&templateDir=custom&theme=simple&action=d&windowstate=normal&mode=view&licensePlate={licensePlate}&registrationBook=&pretence=1");
                        resultContent = await result.Content.ReadAsStringAsync();
                        return new CarInfo(resultContent);
                    }
                    else
                    {
                        ResponseChecker.CreckForError(resultContent);
                    }
                }

                return null;
            }
        }
    }
}
