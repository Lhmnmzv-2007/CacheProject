using CacheServer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace CacheServer.Services;

public class CacheService
{
    AppDbContext db;
    IMemoryCache cache;
    TimeSpan expire = TimeSpan.FromMinutes(10);

    public CacheService(AppDbContext d, IMemoryCache c)
    {
        db = d;
        cache = c;
    }

    public async Task<List<Product>> GetAll()
    {
        if (cache.TryGetValue("all", out List<Product> list))
        {
            Console.WriteLine("cache hit");
            return list;
        }

        Console.WriteLine("db-den gotururem...");
        list = await db.Products.ToListAsync();
        cache.Set("all", list, expire);
        return list;
    }

    public async Task<Product> Get(int id)
    {
        string key = "p" + id;
        if (cache.TryGetValue(key, out Product p))
        {
            Console.WriteLine("cache hit " + id);
            return p;
        }

        Console.WriteLine("db axtariram " + id);
        p = await db.Products.FindAsync(id);
        if (p != null) cache.Set(key, p, expire);
        return p;
    }

    public async Task<bool> Add(Product p)
    {
        db.Products.Add(p);
        if (await db.SaveChangesAsync() > 0)
        {
            cache.Remove("all");
            return true;
        }
        return false;
    }

    public async Task<bool> Update(Product p)
    {
        var old = await db.Products.FindAsync(p.Id);
        if (old == null) return false;

        old.Name = p.Name;
        old.Price = p.Price;
        old.Stock = p.Stock;

        if (await db.SaveChangesAsync() > 0)
        {
            cache.Remove("all");
            cache.Remove("p" + p.Id);
            return true;
        }
        return false;
    }

    public async Task<bool> Delete(int id)
    {
        var p = await db.Products.FindAsync(id);
        if (p == null) return false;

        db.Products.Remove(p);
        if (await db.SaveChangesAsync() > 0)
        {
            cache.Remove("all");
            cache.Remove("p" + id);
            return true;
        }
        return false;
    }
}