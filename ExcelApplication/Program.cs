﻿using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace ExcelApplication
{
    class Program
    {
        static async Task Main(string[] args)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var file = new FileInfo(@"C:\Users\AnandKrishnan\Downloads\CSharpProjects\ExcelApp\files\YouTubeDemo.xlsx");

            var people = GetSetUpData();

            await SaveExcelFile(people, file);

            List<PersonModel> peopleFromExcel = await LoadExcelFile(file);

            foreach(var p in peopleFromExcel)
            {
                Console.WriteLine($"{p.Id} {p.FirstName} {p.LastName} ");
            }
        }

        private static async Task<List<PersonModel>> LoadExcelFile(FileInfo file)
        {
            List<PersonModel> output = new();

            using var package = new ExcelPackage(file);
            await package.LoadAsync(file);

            var ws = package.Workbook.Worksheets[0];

            var row = 3;
            var col = 1;

            while (!string.IsNullOrWhiteSpace(ws.Cells[row, col].Value?.ToString()))
            {
                PersonModel p = new();
                p.Id = int.Parse(ws.Cells[row, col].Value.ToString());
                p.FirstName = ws.Cells[row, col + 1].Value.ToString();
                p.LastName = ws.Cells[row, col + 1].Value.ToString();

                output.Add(p);
                row += 1;
            }

            return output;

        }

        private static async Task SaveExcelFile (List<PersonModel> people,FileInfo file)
        {
            DeleteIfExists(file);

            using var package = new ExcelPackage(file);

            var ws = package.Workbook.Worksheets.Add("MainReport");

            var range = ws.Cells["A2"].LoadFromCollection(people, true);
            range.AutoFitColumns();

            //Format Header row
            ws.Cells["A1"].Value = "Our Cool Report";
            ws.Cells["A1:C1"].Merge = true;
            ws.Row(1).Style.Font.Size = 24;
            ws.Cells["A1:C1"].Style.Fill.SetBackground(Color.Green);
            ws.Row(1).Style.Font.Color.SetColor(Color.Blue);
            ws.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            ws.Row(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Row(2).Style.Font.Bold = true;
            ws.Column(3).Width = 20;


            await package.SaveAsync();
        }

        private static void DeleteIfExists(FileInfo file)
        {
            if (file.Exists)
            {
                file.Delete();
            }
        }
        

        private static List<PersonModel> GetSetUpData()
        {
            List<PersonModel> output = new()
            {
                new() { Id = 1, FirstName = "Tim", LastName = "Corey" },
                new() { Id = 2, FirstName = "Susan", LastName = "Sam" },
                new() { Id = 3, FirstName = "Oswald", LastName = "Gentle" }

            };

            return output;
        }
    }
}
