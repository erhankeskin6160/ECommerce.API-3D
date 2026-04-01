using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.API.Data
{
    public static class EtsyCsvSeeder
    {
        public static async Task SeedProductsAsync(IServiceProvider serviceProvider, string csvFilePath)
        {
            if (!File.Exists(csvFilePath))
            {
                Console.WriteLine($"Etsy CSV file not found at: {csvFilePath}");
                return;
            }

            var context = serviceProvider.GetRequiredService<ECommerceDbContext>();

            var newProductsCount = 0;

            try
            {
                var rows = ParseCsv(csvFilePath).ToList();

                if (rows.Count < 2) return; // Need header + at least 1 row

                var headers = rows[0];
                int titleIdx = Array.IndexOf(headers, "TITLE");
                int descIdx = Array.IndexOf(headers, "DESCRIPTION");
                int priceIdx = Array.IndexOf(headers, "PRICE");
                int qtyIdx = Array.IndexOf(headers, "QUANTITY");
                int tagsIdx = Array.IndexOf(headers, "TAGS");
                
                var imgIndices = new List<int>();
                for (int i = 1; i <= 10; i++)
                {
                    int idx = Array.IndexOf(headers, $"IMAGE{i}");
                    if (idx >= 0) imgIndices.Add(idx);
                }

                for (int r = 1; r < rows.Count; r++)
                {
                    var row = rows[r];
                    if (row.Length <= titleIdx || string.IsNullOrWhiteSpace(row[titleIdx])) continue;

                    string title = row[titleIdx];
                    string description = descIdx >= 0 && row.Length > descIdx ? row[descIdx] : "";
                    
                    decimal price = 0;
                    if (priceIdx >= 0 && row.Length > priceIdx)
                        decimal.TryParse(row[priceIdx], out price);

                    int qty = 10;
                    if (qtyIdx >= 0 && row.Length > qtyIdx)
                        int.TryParse(row[qtyIdx], out qty);

                    string tags = tagsIdx >= 0 && row.Length > tagsIdx ? row[tagsIdx] : "";
                    string category = tags.Split(',').FirstOrDefault()?.Trim() ?? "Standart";
                    if (string.IsNullOrEmpty(category)) category = "Standart";

                    string mainImage = "";
                    var additionalImages = new List<string>();

                    for (int i = 0; i < imgIndices.Count; i++)
                    {
                        int idx = imgIndices[i];
                        if (idx >= 0 && row.Length > idx && !string.IsNullOrWhiteSpace(row[idx]))
                        {
                            var urls = row[idx].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                               .Select(u => u.Trim())
                                               .Where(u => !string.IsNullOrEmpty(u))
                                               .ToList();

                            foreach (var url in urls)
                            {
                                if (string.IsNullOrEmpty(mainImage))
                                    mainImage = url;
                                else
                                    additionalImages.Add(url);
                            }
                        }
                    }

                    // Check if already exists by Name
                    var existingProduct = await context.Products.FirstOrDefaultAsync(p => p.Name == title);
                    
                    if (existingProduct == null)
                    {
                        Console.WriteLine($"Translating new product: {title}...");
                        string titleTr = await TranslateTextAsync(title);
                        string descTr = await TranslateTextAsync(description);

                        var product = new Product
                        {
                            Name = title,
                            Description = description,
                            NameTr = titleTr,
                            DescriptionTr = descTr,
                            Price = price,
                            StockQuantity = qty > 0 ? qty : 10,
                            ImageUrl = mainImage,
                            Category = category,
                            AdditionalImages = additionalImages
                        };

                        await context.Products.AddAsync(product);
                        newProductsCount++;
                    }
                    else
                    {
                        bool needsUpdate = false;
                        if (string.IsNullOrEmpty(existingProduct.NameTr) || existingProduct.NameTr == existingProduct.Name)
                        {
                            Console.WriteLine($"Translating missing NameTr for existing product: {title}...");
                            existingProduct.NameTr = await TranslateTextAsync(title);
                            needsUpdate = true;
                        }
                        if ((string.IsNullOrEmpty(existingProduct.DescriptionTr) || existingProduct.DescriptionTr == existingProduct.Description) && !string.IsNullOrWhiteSpace(description))
                        {
                            existingProduct.DescriptionTr = await TranslateTextAsync(description);
                            needsUpdate = true;
                        }

                        // Compare the combined strings to detect changes or fixes (like bad comma separated URLs)
                        if (existingProduct.AdditionalImages == null || 
                            string.Join(",", existingProduct.AdditionalImages) != string.Join(",", additionalImages))
                        {
                            existingProduct.AdditionalImages = additionalImages;
                            if (string.IsNullOrEmpty(existingProduct.ImageUrl) && !string.IsNullOrEmpty(mainImage))
                            {
                                existingProduct.ImageUrl = mainImage;
                            }
                            needsUpdate = true;
                        }

                        if (needsUpdate)
                        {
                            context.Products.Update(existingProduct);
                            newProductsCount++; // Just to trigger SaveChanges message
                        }
                    }
                }

                if (newProductsCount > 0)
                {
                    await context.SaveChangesAsync();
                    Console.WriteLine($"Successfully seeded {newProductsCount} products from Etsy CSV.");
                }
                else
                {
                    Console.WriteLine("Etsy CSV processed, but no new products were added (they might already exist).");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while seeding Etsy CSV: {ex.Message}");
            }
        }

        private static readonly HttpClient _httpClient = new HttpClient();

        private static async Task<string> TranslateTextAsync(string text, string from = "en", string to = "tr")
        {
            if (string.IsNullOrWhiteSpace(text)) return text;
            try
            {
                var url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl={from}&tl={to}&dt=t&q={Uri.EscapeDataString(text)}";
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    // Result format: [[["Merhaba dünya","Hello world",null,null,1]],null,"en",null,null,null,null,[]]
                    if (result.StartsWith("[[[\""))
                    {
                        var start = result.IndexOf('\"') + 1;
                        var end = result.IndexOf('\"', start);
                        if (start > 0 && end > start)
                        {
                            // This is a naive extraction. A full JSON parser traversing the arrays is better for long texts,
                            using var doc = System.Text.Json.JsonDocument.Parse(result);
                            var chunks = doc.RootElement[0];
                            var translatedStr = new StringBuilder();
                            foreach (var chunk in chunks.EnumerateArray())
                            {
                                translatedStr.Append(chunk[0].GetString());
                            }
                            return translatedStr.ToString();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Translation failed: {e.Message}");
            }
            return text; // Fallback to original text if failed
        }

        private static IEnumerable<string[]> ParseCsv(string path)
        {
            using var reader = new StreamReader(path);
            bool inQuotes = false;
            var currentToken = new StringBuilder();
            var currentRow = new List<string>();

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line == null) break;

                for (int i = 0; i < line.Length; i++)
                {
                    char c = line[i];
                    if (c == '"')
                    {
                        // Check for escaped quotes "" (optional, simple logic here treats it as toggle)
                        inQuotes = !inQuotes;
                    }
                    else if (c == ',' && !inQuotes)
                    {
                        currentRow.Add(currentToken.ToString());
                        currentToken.Clear();
                    }
                    else
                    {
                        currentToken.Append(c);
                    }
                }

                if (inQuotes)
                {
                    currentToken.AppendLine();
                }
                else
                {
                    currentRow.Add(currentToken.ToString());
                    yield return currentRow.ToArray();
                    currentRow.Clear();
                    currentToken.Clear();
                }
            }
        }
    }
}
