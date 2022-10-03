using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Data;
using Microsoft.Extensions.DependencyInjection;
using ImportLWIN.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using EFCore.BulkExtensions;

namespace ImportLWIN
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                IConfiguration config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .Build();

                var builder = new ServiceCollection()
                    .AddSingleton<Program>()
                    .AddSingleton<IConfiguration>(config)
                    .AddDbContext<LWINContext>(options => options.UseSqlServer(config.GetConnectionString("connectionstring.swampnet")))
                    .BuildServiceProvider();

                var app = builder.GetRequiredService<Program>();

                await app.RunAsync(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private readonly LWINContext _context;

        public Program(LWINContext context)
        {
            _context = context;
        }


        private async Task RunAsync(string[] args)
        {
            var raw = LoadSourceData()
                .Tables["LWINdatabase"]
                .ToRaw();

            await _context.BulkInsertAsync(raw);
        }


        private DataSet LoadSourceData()
        {
            // Load spreadsheet
            // data\\LWIN-20221002.xlsx
            var path = "data\\LWIN-20221002.xlsx";

            Console.WriteLine($"Loading source data: {path}");

            using (var xls = new XLWorkbook(path))
            {
                return xls.ToDataSet();
            }
        }
    }
}
