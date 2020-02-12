using System;
using System.Net.Http;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Threading;
using Newtonsoft.Json.Linq;

namespace Forms
{
    class Program
    {
        public const string API_KEY                                     = "YOURKEY FROM PORTAL";
        public const string API_URL                                     = "YOURKEY URL PORTAL";
        static void Main(string[] args)
        {
            CallAPIAsync("YOUR FORM URL MAYBE FROM AZURE STORAGE ? :)");

            Console.ReadLine();
        }

        private static async System.Threading.Tasks.Task CallAPIAsync(string ImageURL)
        {
            try
            {
                string strURL                                           = @"{ 'url': '" + ImageURL + "'}";
                HttpClient ctx                                          = new HttpClient();
                ctx.DefaultRequestHeaders.Add("ocp-apim-subscription-key", API_KEY);

                var content                                             = new StringContent(strURL, Encoding.UTF8, "application/json");
                var result                                              = ctx.PostAsync( API_URL + "formrecognizer/v2.0-preview/prebuilt/receipt/analyze", content).Result;
                
                foreach (string URL in result.Headers.GetValues("Operation-Location"))
                {
                    bool bReady                                         = false;
                    do
                    {
                        Thread.Sleep(1000); // <--Might be helpful
                        var htResponse                                  = await ctx.GetAsync(URL);
                        string strResult                                = await htResponse.Content.ReadAsStringAsync();

                        JObject joParsed                                = JObject.Parse(strResult);
                        if (joParsed["status"].ToString() != "running")
                            bReady                                      = true;

                        foreach (var pair in joParsed)
                        {
                            Console.WriteLine("{0}: {1}", pair.Key, pair.Value);
                        }
                    } while (!bReady);
                }
            }
            catch(Exception exError)
            {
                string strMessage                                       = exError.Message;
            }
        }       
    }
}
