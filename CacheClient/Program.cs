using System.Net.Http.Json;

var http = new HttpClient { BaseAddress = new Uri("http://localhost:5000/") };

while (true)
{
    Console.Clear();
    Console.WriteLine("1 add");
    Console.WriteLine("2 update");
    Console.WriteLine("3 all");
    Console.WriteLine("4 get");
    Console.WriteLine("5 del");
    Console.WriteLine("0 exit");
    Console.Write("> ");
    var s = Console.ReadLine();

    try
    {
        if (s == "1")
        {
            var p = new Product();
            Console.Write("name: "); p.Name = Console.ReadLine();
            Console.Write("price: "); p.Price = decimal.Parse(Console.ReadLine());
            Console.Write("stock: "); p.Stock = int.Parse(Console.ReadLine());
            var r = await http.PostAsJsonAsync("products", p);
            Console.WriteLine(await r.Content.ReadAsStringAsync());
        }
        else if (s == "2")
        {
            var p = new Product();
            Console.Write("id: "); p.Id = int.Parse(Console.ReadLine());
            Console.Write("name: "); p.Name = Console.ReadLine();
            Console.Write("price: "); p.Price = decimal.Parse(Console.ReadLine());
            Console.Write("stock: "); p.Stock = int.Parse(Console.ReadLine());
            var r = await http.PutAsJsonAsync("products", p);
            Console.WriteLine(await r.Content.ReadAsStringAsync());
        }
        else if (s == "3")
        {
            var list = await http.GetFromJsonAsync<List<Product>>("products");
            if (list == null || list.Count == 0) Console.WriteLine("bos");
            else foreach (var x in list) Console.WriteLine($"{x.Id} {x.Name} {x.Price} {x.Stock}");
        }
        else if (s == "4")
        {
            Console.Write("id: ");
            var r = await http.GetAsync("products/" + Console.ReadLine());
            Console.WriteLine(await r.Content.ReadAsStringAsync());
        }
        else if (s == "5")
        {
            Console.Write("id: ");
            var r = await http.DeleteAsync("products/" + Console.ReadLine());
            Console.WriteLine(await r.Content.ReadAsStringAsync());
        }
        else if (s == "0") break;
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }

    Console.ReadLine();
}

class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
}