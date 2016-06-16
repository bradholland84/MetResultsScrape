using CsvHelper;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static List<Artifact> Artifacts = new List<Artifact>();
        static Queue<Artifact> ArtQueue = new Queue<Artifact>();
        static void Main(string[] args)
        {
            //File Setup
            string text  = File.ReadAllText("C:\\Users\\Brad\\Desktop\\mohammad_met.html");
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(text);

            //Grab all search-results containers
            var resultContainers = doc.DocumentNode.Descendants("div").Where(d =>
                 d.Attributes.Contains("class") && d.Attributes["class"].Value.Contains("search-results__content-wrapper"));
            Console.WriteLine(resultContainers.Count());
            foreach (HtmlNode result in resultContainers)
            {
                Artifact art = new Artifact();
                List<string> info = new List<string>();
                
                //The HTML Schema uses <dl> nodes for data fields
                var dataFields = result.Descendants("dl");
                var titleField = result.Descendants("span").Where(d =>
                    d.Attributes.Contains("ng-bind-html") && d.Attributes["ng-bind-html"].Value.Contains("search.stripBrTags(searchResult.title)"));
                art.Title = titleField.First().InnerText;

                //Loop through data nodes and populate a simple list with them
                foreach (HtmlNode dl in dataFields)
                {
                    foreach(HtmlNode data in dl.Descendants())
                    {

                        if (data.GetAttributeValue("class", "") == "ng-binding ng-scope")
                        {
                           info.Add(data.InnerText);
                        } 
                    }
                }

                //Assign data fields to artifact object
                for(int i = 0; i < info.Count(); i++)
                {
                    switch (i)
                    {
                        case 1:
                            art.Date = info[i];
                            break;
                        case 3:
                            art.Medium = info[i];
                            break;
                        case 5:
                            art.AccessionNum = info[i];
                            break;
                        case 7:
                            art.Location = info[i];
                            break;
                    }
                    
                }
                //FIFO Queue for ImageUrl grabbing
                ArtQueue.Enqueue(art);
            }

            //Grab all image-containers from serach results
            var imgContainers = doc.DocumentNode.Descendants("div").Where(d =>
                 d.Attributes.Contains("class") && d.Attributes["class"].Value.Contains("search-results__image-container"));
            Console.WriteLine(imgContainers.Count());
            foreach (HtmlNode result in imgContainers)
            {
                //Add ImageUrl to Artifact Objects
                Artifact art = ArtQueue.Dequeue();
                var imgNode = result.Descendants("img").First();
                art.Image = imgNode.Attributes["ng-src"].Value;
                ArtQueue.Enqueue(art);
            }

            //Output File Setup
            TextWriter writer = File.CreateText("output.csv");
            var csv = new CsvWriter(writer); 
            while(ArtQueue.Count > 0)
            {
                var a = ArtQueue.Dequeue();
                csv.WriteField(a.Title);
                csv.WriteField(a.Image);
                csv.WriteField(a.Date);
                csv.WriteField(a.AccessionNum);
                csv.WriteField(a.Medium);
                csv.WriteField(a.Location);
                csv.NextRecord();
            }

            //Wait for User input before close
            Console.Read();
        }
    }
}
