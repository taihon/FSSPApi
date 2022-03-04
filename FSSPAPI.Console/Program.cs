using FSSPAPI.Core;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FSSPAPI.UI
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = "input.xlsx";
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var persons = new List<Person>();
            using (var pack=new ExcelPackage(path)) {
                var sheet = pack.Workbook.Worksheets[0];
                var row = 2;
                while (!string.IsNullOrEmpty(sheet.Cells[row, 1].GetValue<string>()))
                {
                    var person = new Person();
                    person.FirstName = sheet.Cells[row, 1].GetValue<string>();
                    person.LastName = sheet.Cells[row, 2].GetValue<string>();
                    person.BirthDate = sheet.Cells[row, 3].GetValue<DateTime>().ToString("dd.MM.yyyy");
                    person.Region = sheet.Cells[row, 4].GetValue<int>();
                    persons.Add(person);
                    row++;
                }
            }
            var exchanger = new APIExchanger();
            var batchSize = 50;
            var batchCount = Math.Ceiling(persons.Count*1.0 / batchSize);
            var personsSorted = persons.OrderBy(t => t.Region).ThenBy(t => t.LastName).ToList();
            var personsGrouped = persons.GroupBy(p => p.Region).ToDictionary(grp => grp.Key, grp => grp.ToList());
            var processor = new QueueProcessor();
            var minKey = 2;
            foreach (var region in personsGrouped.Keys.OrderBy(t=>t).SkipWhile(k => k != minKey))
            {
                Console.WriteLine($"Processing region {region:D2}");
                var batch = personsGrouped[region];
                var batchResults = processor.Process(batch);
                var jsonBatch = JsonConvert.SerializeObject(batchResults);
                File.WriteAllText($"results_region_{region:D2}_{DateTime.Now:ddMMyyyyHHmmss}.json",jsonBatch);
            }
            Console.ReadKey();
        }
    }
}
