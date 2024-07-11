using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Diagnostics;
using Microsoft.Maui.Storage;
namespace DiplomnaPOS.Models
{
    public class PrinterManager
    {
        public PrinterManager() { }
        public void GenerateReceipt(Cart cart)
        {
            XDocument doc = XDocument.Load(Path.Combine(FileSystem.Current.AppDataDirectory, "ReceiptConfigurationBlank.xml"));

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.ContinuousSize(57, Unit.Millimetre);
                    page.MarginVertical(5);
                    page.MarginHorizontal(10);
                    page.DefaultTextStyle(x => x.FontFamily("Open Sans"));
                    //page.Size(new PageSize(226f, 300f));
                    page.Margin(0.5f, Unit.Centimetre);
                    page.PageColor(QuestPDF.Helpers.Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(7));

                    page.Header()
                        .AlignCenter()
                        .Text(doc.Root.Element("HeaderText").Value)
                        .SemiBold()
                        .FontSize(9);

                    page.Content()


                    .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });


                            foreach (var item in cart.ProductsCollection)
                            {
                                if (item.ProductCount != 1)
                                {


                                    table.Cell().Text(item.Product.Name);
                                    table.Cell().AlignRight().Text($"{item.ProductCount} x {item.Product.Price} = {item.Price}");

                                }
                                else
                                {
                                    //For a signle product
                                    table.Cell().Text(item.Product.Name);
                                    table.Cell().AlignRight().Text(item.Price);
                                }
                            }


                            table.Cell().ColumnSpan(2).PaddingVertical(5).LineHorizontal(1).LineColor(QuestPDF.Helpers.Colors.Grey.Medium);
                            table.Cell().Text("TOTAL:").Bold();
                            table.Cell().AlignRight().Text($"{cart.ProductsInCartPrice}").Bold();
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span(doc.Root.Element("FooterText").Value);
                        });
                });
            })
            .GeneratePdf(Path.Combine(FileSystem.Current.AppDataDirectory, "reciept.pdf"));

            Debug.WriteLine(FileSystem.Current.AppDataDirectory);
        }

        public void GenerateReceipt(Cart cart, double change)
        {
            XDocument doc = XDocument.Load(Path.Combine(FileSystem.Current.AppDataDirectory, "ReceiptConfigurationBlank.xml"));

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.ContinuousSize(57, Unit.Millimetre);
                    page.MarginVertical(5);
                    page.MarginHorizontal(10);
                    //page.Size(new PageSize(226f, 300f));
                    page.Margin(0.5f, Unit.Centimetre);
                    page.PageColor(QuestPDF.Helpers.Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(7));

                    page.Header()
                        .AlignCenter()
                        .Text(doc.Root.Element("HeaderText").Value)
                        .SemiBold()
                        .FontSize(9);

                    page.Content()


                    .Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });


                        foreach (var item in cart.ProductsCollection)
                        {
                            if (item.ProductCount != 1)
                            {


                                table.Cell().Text(item.Product.Name);
                                table.Cell().AlignRight().Text($"{item.ProductCount} x {item.Product.Price} = {item.Price}");

                            }
                            else
                            {
                                //For a signle product
                                table.Cell().Text(item.Product.Name);
                                table.Cell().AlignRight().Text(item.Price);
                            }
                        }


                        table.Cell().ColumnSpan(2).PaddingVertical(5).LineHorizontal(1).LineColor(QuestPDF.Helpers.Colors.Grey.Medium);
                        table.Cell().Text("TOTAL:").Bold();
                        table.Cell().AlignRight().Text($"{cart.ProductsInCartPrice}").Bold();
                        
                        table.Cell().Text("Cash:");
                        table.Cell().AlignRight().Text(Math.Round(cart.ProductsInCartPrice, 2) + change);

                        table.Cell().Text("Change:");
                        table.Cell().AlignRight().Text(change);
                    });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span(doc.Root.Element("FooterText").Value);
                        });
                });
            })
            .GeneratePdf(Path.Combine(FileSystem.Current.AppDataDirectory, "reciept.pdf"));

            Debug.WriteLine(FileSystem.Current.AppDataDirectory);
        }

        public void GenerateReprint(Cart cart)
        {
            XDocument doc = XDocument.Load(Path.Combine(FileSystem.Current.AppDataDirectory, "ReceiptConfigurationBlank.xml"));

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.ContinuousSize(57, Unit.Millimetre);
                    page.MarginVertical(5);
                    page.MarginHorizontal(10);
                    //page.Size(new PageSize(226f, 300f));
                    page.Margin(0.5f, Unit.Centimetre);
                    page.PageColor(QuestPDF.Helpers.Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(7));

                    page.Header()
                        .AlignCenter()
                        .Text(doc.Root.Element("HeaderText").Value)
                        .SemiBold()
                        .FontSize(9);

                    page.Content()

                    .Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });


                        foreach (var item in cart.ProductsCollection)
                        {
                            if (item.ProductCount != 1)
                            {
                                table.Cell().Text(item.Product.Name);
                                table.Cell().AlignRight().Text($"{item.ProductCount} x {item.Product.Price} = {item.Price}");
                            }
                            else
                            {
                                //For a signle product
                                table.Cell().Text(item.Product.Name);
                                table.Cell().AlignRight().Text(item.Price);
                            }
                        }


                        table.Cell().ColumnSpan(2).PaddingVertical(5).LineHorizontal(1).LineColor(QuestPDF.Helpers.Colors.Grey.Medium);
                        table.Cell().Text("TOTAL:").Bold();
                        table.Cell().AlignRight().Text($"{cart.ProductsInCartPrice}").Bold();
                    });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("THIS IS A REPRINT!");
                        });
                });
            })
            .GeneratePdf(Path.Combine(FileSystem.Current.AppDataDirectory, "reprint_reciept.pdf"));
        }


        #region ESC/POS command based methods
        public void EscPrintReceipt(Cart cart)
        {
            string ipAddress = "127.0.0.1"; // Replace with the printer's IP address
            int port = 1234; // The default port for many receipt printers

            try
            {
                // Create a TCP client to connect to the printer
                using (TcpClient client = new TcpClient(ipAddress, port))
                {
                    // Get the network stream
                    NetworkStream networkStream = client.GetStream();

                    // Send ESC/POS commands for print
                    string escPosCommands = "\x1B\x40"; // Initialize printer
                    int totalWidth = 42;


                    escPosCommands += "\x1B\x61\x1"; //Justification: Center
                    escPosCommands += "Waterview Restaurant\n" +
                                      "1015 Marina Drive\n" +
                                      "San Diego, CA 91945\n" +
                                      "800-532-1929\n";

                    escPosCommands += "\x1B\x61\x0"; //Justification: Left
                    escPosCommands += "\n\n";

                    foreach (var item in cart.ProductsCollection)
                    {
                        if (item.ProductCount != 1)
                        {
                            int productNameWidth = item.Product.Name.Length;
                            int productCountWidth = $"{item.ProductCount } x {item.Product.Price} = {item.Price}".Length;
                            int availableWidth = totalWidth - (productNameWidth + productCountWidth);

                            //escPosCommands += $"{item.Product.Name, -20}     {item.ProductCount} * {item.Price / item.ProductCount}\n"; // Text to print
                            escPosCommands += $"{item.Product.Name}{new string(' ', availableWidth)}{item.ProductCount} x {item.Product.Price} = {item.Price}\n"; // Text to print

                        }
                        else
                        {
                            //For a signle product

                            // Calculate the width for the the product and price
                            int productNameWidth = item.Product.Name.Length;
                            int pridceWidth = item.Price.ToString().Length;

                            // Calculate the width available for padding
                            int availableWidth = totalWidth - (productNameWidth + pridceWidth);

                            // Construct the formatted string with appropriate padding
                            escPosCommands += $"{item.Product.Name}{new string(' ', availableWidth)}{item.Price}\n"; 

                        }
                    }

                    escPosCommands += "\x1B\x2D\x1"; //Start underline
                    escPosCommands += "                                          \n";
                    escPosCommands += "\x1B\x2D\x0"; //End underline

                    int totalTextWidth = "TOTAL:".Length;
                    int totalPriceWidth = cart.ProductsInCartPrice.ToString().Length;
                    int totalTextPriceStringWidth = totalWidth - (totalTextWidth + totalPriceWidth);

                    escPosCommands += "\x1B\x45\x1"; //Bold text start
                    escPosCommands += $"TOTAL:{new string(' ', totalTextPriceStringWidth)}{cart.ProductsInCartPrice}" + "\n";
                    escPosCommands += "\x1B\x45\x0"; //Bold text end

                    escPosCommands += "\x1B\x61\x1"; //Justification: Center
                    escPosCommands += "Have a nice day!";
                    escPosCommands += "\x0A"; // Line feed

                    escPosCommands += "\x0A"; // Line feed
                    escPosCommands += "\x1D\x56\x01"; // Cut paper (if supported)

                    byte[] data = Encoding.ASCII.GetBytes(escPosCommands);
                    networkStream.Write(data, 0, data.Length);

                    // Close the network stream and the client
                    networkStream.Close();
                    client.Close();

                    Console.WriteLine("Test print sent successfully.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        public void EscPrintReceipt(Cart cart, double change)
        {
            string ipAddress = "127.0.0.1"; // Replace with the printer's IP address
            int port = 1234; // The default port for many receipt printers

            try
            {
                // Create a TCP client to connect to the printer
                using (TcpClient client = new TcpClient(ipAddress, port))
                {
                    // Get the network stream
                    NetworkStream networkStream = client.GetStream();

                    // Send ESC/POS commands for print
                    string escPosCommands = "\x1B\x40"; // Initialize printer
                    int totalWidth = 42;


                    escPosCommands += "\x1B\x61\x1"; //Justification: Center
                    escPosCommands += "Waterview Restaurant\n" +
                                      "1015 Marina Drive\n" +
                                      "San Diego, CA 91945\n" +
                                      "800-532-1929\n";

                    escPosCommands += "\x1B\x61\x0"; //Justification: Left
                    escPosCommands += "\n\n";

                    foreach (var item in cart.ProductsCollection)
                    {
                        if (item.ProductCount != 1)
                        {
                            int productNameWidth = item.Product.Name.Length;
                            int productCountWidth = $"{item.ProductCount} x {Math.Round((double)item.Product.Price, 2)} = {Math.Round(item.Price, 2)}".Length;
                            int availableWidth = totalWidth - (productNameWidth + productCountWidth);

                            //escPosCommands += $"{item.Product.Name, -20}     {item.ProductCount} * {item.Price / item.ProductCount}\n"; // Text to print
                            escPosCommands += $"{item.Product.Name}{new string(' ', availableWidth)}{item.ProductCount} x {Math.Round((double)item.Product.Price, 2)} = {Math.Round(item.Price, 2)}\n"; // Text to print

                        }
                        else
                        {
                            //For a signle product

                            // Calculate the width for the the product and price
                            int productNameWidth = item.Product.Name.Length;
                            int pridceWidth = Math.Round(item.Price, 2).ToString().Length;

                            // Calculate the width available for padding
                            int availableWidth = totalWidth - (productNameWidth + pridceWidth);

                            // Construct the formatted string with appropriate padding
                            escPosCommands += $"{item.Product.Name}{new string(' ', availableWidth)}{Math.Round(item.Price, 2)}\n";

                        }
                    }

                    escPosCommands += "\x1B\x2D\x1"; //Start underline
                    escPosCommands += "                                          \n";
                    escPosCommands += "\x1B\x2D\x0"; //End underline

                    int totalTextWidth = "TOTAL".Length;
                    int totalPriceWidth = Math.Round(cart.ProductsInCartPrice, 2).ToString().Length;
                    int totalTextPriceStringWidth = totalWidth - (totalTextWidth + totalPriceWidth);

                    escPosCommands += "\x1B\x45\x1"; //Bold text start
                    escPosCommands += $"TOTAL{new string(' ', totalTextPriceStringWidth)}{Math.Round(cart.ProductsInCartPrice, 2)}" + "\n";
                    escPosCommands += "\x1B\x45\x0"; //Bold text end

                    int givenCashTextWidth = "Cash".Length;
                    int givenCashValueWidth = $"{Math.Round(cart.ProductsInCartPrice, 2) + change}".Length;
                    escPosCommands += $"Cash{new string(' ', totalWidth - (givenCashTextWidth + givenCashValueWidth))}{Math.Round(cart.ProductsInCartPrice + change, 2)}" + "\n";

                    int changeTextWidth = "Change".Length;
                    int changeValueWidth = Math.Round(change, 2).ToString().Length;
                    escPosCommands += $"Change{new string(' ', totalWidth - (changeTextWidth + changeValueWidth))}{Math.Round(change, 2)}" + "\n";

                    escPosCommands += "\x1B\x61\x1"; //Justification: Center
                    escPosCommands += "Have a nice day!";
                    escPosCommands += "\x0A"; // Line feed

                    escPosCommands += "\x0A"; // Line feed
                    escPosCommands += "\x1D\x56\x01"; // Cut paper (if supported)

                    byte[] data = Encoding.ASCII.GetBytes(escPosCommands);
                    networkStream.Write(data, 0, data.Length);

                    // Close the network stream and the client
                    networkStream.Close();
                    client.Close();

                    Console.WriteLine("Test print sent successfully.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
        #endregion
    }
}
